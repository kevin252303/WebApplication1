using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.DTOs;
using WebApplication1.Extentions;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;

        public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper,
            IHubContext<PresenceHub> presenceHub)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _presenceHub = presenceHub;
        }

        public override async Task OnConnectedAsync()
        {
            var httpcontext = Context.GetHttpContext();
            var otherUser = httpcontext.Request.Query["user"];
            var groupName = GetGroupName(Context.User.GetUserName(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);

            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = await _messageRepository.GetMessageThread(Context.User.GetUserName(), otherUser);

            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task Typing(bool typing, CreateMessageDTO createMessageDTO)
        {
            var senderUsername = Context.User.GetUserName();
            var sender = await _userRepository.GetUserByNameAsync(senderUsername);
            var recipient = await _userRepository.GetUserByNameAsync(createMessageDTO.RecipientUserName);
            var groupname= GetGroupName(sender.UserName, recipient.UserName);
            var group= await _messageRepository.GetMessageGroup(groupname);
            var connections = group.Connections;

            if(group.Connections.Any(x=>x.Username == recipient.UserName))
            {
                if (typing)
                {
                    await Clients.OthersInGroup(groupname).SendAsync("ReceiveTypingStatus", typing);
                }
                else
                {
                    await Clients.OthersInGroup(groupname).SendAsync("ReceiveTypingStatus", typing);
                }
            }
            
        }

        public async Task SendMessage(CreateMessageDTO createMessageDTO)
        {
            var username = Context.User.GetUserName();

            if (username == createMessageDTO.RecipientUserName.ToLower()) throw new HubException("You cannot send message to yourself");

            var sender = await _userRepository.GetUserByNameAsync(username);
            var recipient = await _userRepository.GetUserByNameAsync(createMessageDTO.RecipientUserName);

            if (recipient == null) throw new HubException("User not found");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDTO.Content
            };

            var groupname = GetGroupName(sender.UserName, recipient.UserName);

            var group = await _messageRepository.GetMessageGroup(groupname);

            if (group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connection = await PresenceTracker.GetConnectionForUsers(recipient.UserName);
                if (connection != null)
                {
                    await _presenceHub.Clients.Clients(connection).SendAsync("NewMessageReceived",
                        new { username = sender.UserName, knownAs = sender.KnownAs });
                }
            }

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync())
            {
                await Clients.Group(groupname).SendAsync("NewMessage", _mapper.Map<MessageDTO>(message));
            }

        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";

        }

        private async Task<Group> AddToGroup(string groupname)
        {
            var group = await _messageRepository.GetMessageGroup(groupname);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());

            if (group == null)
            {
                group = new Group(groupname);
                _messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            if (await _messageRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to add to group");
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            _messageRepository.RemoveConnection(connection);

            if (await _messageRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to remove form group");
        }

        public async Task CallUser(string recipientUsername)
        {
            var senderUsername = Context.User.GetUserName();
            var sender = await _userRepository.GetUserByNameAsync(senderUsername);
            var recipient = await _userRepository.GetUserByNameAsync(recipientUsername);
            var groupname = GetGroupName(sender.UserName, recipient.UserName);
            var group = await _messageRepository.GetMessageGroup(groupname);
            var connections = group.Connections;

            if (group.Connections.Any(x => x.Username == recipient.UserName))
            {
                await Clients.OthersInGroup(groupname).SendAsync("IncomingCall", senderUsername);
            }

            else
            {
                await Clients.Caller.SendAsync("UserOffline");
            }

        }

        public async Task AnswerCall(string callerUsername)
        {
            var recipientUsername = Context.User.GetUserName();
            var groupname = GetGroupName(callerUsername, recipientUsername);
            var group = await _messageRepository.GetMessageGroup(groupname);
            var connections = group.Connections;

            if (connections != null)
            {
                await Clients.All.SendAsync("CallAccepted", callerUsername);
            }
        }

        public async Task RejectCall(string callerUsername)
        {
            var recipientUsername = Context.User.GetUserName();
            var groupname = GetGroupName(callerUsername, recipientUsername);
            var group = await _messageRepository.GetMessageGroup(groupname);
            var connections = group.Connections;

            if (connections != null)
            {
                await Clients.OthersInGroup(groupname).SendAsync("CallRejected");
            }
        }

        public async Task EndCall(string otherUsername)
        {
            var callerUsername = Context.User.GetUserName();
            var recipientUsername = Context.User.GetUserName();
            var groupname = GetGroupName(callerUsername, otherUsername);
            var group = await _messageRepository.GetMessageGroup(groupname);
            var connections = group.Connections;
            var otherConnection = await PresenceTracker.GetConnectionForUsers(otherUsername);

            if (connections != null)
            {
                await Clients.All.SendAsync("CallEnded");
            }
        }
    }
}
