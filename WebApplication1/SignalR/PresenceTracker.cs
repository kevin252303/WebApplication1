﻿namespace WebApplication1.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string,List<string>> OnlineUsers = new Dictionary<string,List<string>>();

        public Task<bool> UserConnected(string username,string connectionId)
        {
            bool isOnline = false;
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    OnlineUsers.Add(username, new List<string> { connectionId});
                    isOnline = true;
                }
            }
            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnected(string username,string connectionId)
        {
            bool isOffline = false;
            lock (OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);
                OnlineUsers[username].Remove(connectionId);

                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                    isOffline = true;
                }
            }
            return Task.FromResult(isOffline);
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock (OnlineUsers)
            {
                onlineUsers = OnlineUsers.OrderBy(k=>k.Key).Select(k=>k.Key).ToArray();
            }
            return Task.FromResult(onlineUsers);
        }

        public static Task<List<string>> GetConnectionForUsers(string username)
        {
            List<string> connectionIds;
            lock (OnlineUsers)
            {
                if (OnlineUsers.TryGetValue(username, out connectionIds))
                {
                    return Task.FromResult(connectionIds);
                }
                else
                {
                    return Task.FromResult(new List<string>());
                }
            }
        }
    }
}
