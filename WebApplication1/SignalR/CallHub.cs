using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApplication1.DTOs;
using WebApplication1.Interfaces;
using WebApplication1.Extentions;
using AutoMapper;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.SignalR
{
    public class CallHub : Hub
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;

        public CallHub( IUserRepository userRepository, IMessageRepository messageRepository)
        {
            
            _userRepository = userRepository;
            _messageRepository = messageRepository;


        }
        [Authorize]

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
        public async Task SendIncomingCallNotification(string userId, string callerName)
        {
            await Clients.User(userId).SendAsync("ReceiveIncomingCall", callerName);
        }

        // Method to notify the caller that the call has been accepted by the recipient
        public async Task SendCallAcceptedNotification(string callerId)
        {
            await Clients.User(callerId).SendAsync("ReceiveCallAccepted");
        }

        // Method to notify the caller that the call has ended
        public async Task SendCallEndedNotification(string callerId)
        {
            await Clients.User(callerId).SendAsync("ReceiveCallEnded");
        }

        public async Task ReceiveCall(string username)
        {
            var senderUsername = Context.User.GetUserName();
            var sender = await _userRepository.GetUserByNameAsync(senderUsername);
            var recipient = await _userRepository.GetUserByNameAsync(username);
            var groupname = GetGroupName(sender.UserName, recipient.UserName);
            var group = await _messageRepository.GetMessageGroup(groupname);
            var connections = group.Connections;

            if(group.Connections.Any(x=>x.Username == recipient.UserName))
            {
                await Clients.OthersInGroup(groupname).SendAsync("ReceiveIncomingCall", username);
            }
            

            
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

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";

        }
    }
}
