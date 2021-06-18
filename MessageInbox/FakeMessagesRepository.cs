using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebChatPlay.MessageInbox
{
    public class FakeMessagesRepository : IMessagesRepository
    {
        public ConcurrentBag<InboxMessage> Messages { get; set; }

        public ConcurrentBag<ApplicationUser> ApplicationUsers { get; set; }

        public ConcurrentBag<UserOnlineStatus> UserStatuses { get; set; }

        public ConcurrentBag<UserFriend> UserFriends { get; set; }

        public FakeMessagesRepository()
        {
            Messages = new ConcurrentBag<InboxMessage>();
            ApplicationUsers = new ConcurrentBag<ApplicationUser>
                {
                    new ApplicationUser{UserName = "AMMI", FullName = "Ulfat Bibi"},
                    new ApplicationUser{UserName = "DAAGI", FullName = "Shah Jehan"},
                    new ApplicationUser{ UserName = "SAFIA", FullName = "Safia Shahid" },
                    new ApplicationUser{ UserName = "MONIBA", FullName = "Moniba Shahid"},
                    new ApplicationUser{ UserName = "SHAHID", FullName = "Shahid Ali"},
                };


            UserFriends = new ConcurrentBag<UserFriend>
               {

                new UserFriend{ UserName = "AMMI", FriendId = "SHAHID"},
                new UserFriend{ UserName = "AMMI", FriendId = "MONIBA"},
                new UserFriend{ UserName = "AMMI", FriendId = "DAAGI"},

                new UserFriend{ UserName = "DAAGI", FriendId = "SAFIA"},
                new UserFriend{ UserName = "DAAGI", FriendId = "MONIBA"},
                new UserFriend{ UserName = "DAAGI", FriendId = "AMMI"},


                new UserFriend{ UserName = "SAFIA", FriendId = "SHAHID"},
                new UserFriend{ UserName = "SAFIA", FriendId = "MONIBA"},
                new UserFriend{ UserName = "SAFIA", FriendId = "DAAGI"},


                new UserFriend{ UserName = "MONIBA", FriendId = "SHAHID"},
                new UserFriend{ UserName = "MONIBA", FriendId = "SAFIA"},
                new UserFriend{ UserName = "MONIBA", FriendId = "AMMI"},
                new UserFriend{ UserName = "MONIBA", FriendId = "DAAGI"},


                new UserFriend{ UserName = "SHAHID", FriendId = "SAFIA"},
                new UserFriend{ UserName = "SHAHID", FriendId = "MONIBA"},
                new UserFriend{ UserName = "SHAHID", FriendId = "AMMI"},
            };

            UserStatuses = new ConcurrentBag<UserOnlineStatus>();
        }

        public void AddMessage(InboxMessage message)
        {
            Messages.Add(message);
        }

        public async Task<IEnumerable<InboxMessage>> GetAllConversations(string userName, int page, int pageSize = 100)
        {
            var task = Task.Run(() =>
            {
                int take, skip;
                page = GetPageModel(page, pageSize, out take, out skip);

                return Messages
                .Where(m => StringEqual(m.Sender, userName) || StringEqual(m.Recepient, userName))
                .OrderByDescending(x => x.CreatedAt).Skip(skip).Take(take);
            });

            return await task;

        }

        public async Task<InboxMessage> UpdateMessage(IUpdateInboxMessage message)
        {
            var task = Task.Run(() =>
            {
                var storeMessage = Messages.FirstOrDefault(x => x.MessageId == message.MessageId);
                if (storeMessage == null)
                    throw new MessageNotFoundException($"Message with ID: {message.MessageId} cannot be found");

                storeMessage.Delivered = message.Delivered;
                storeMessage.Seen = message.Seen;
                storeMessage.RetryCount = message.RetryCount;
                storeMessage.ReceivedAt = message.ReceivedAt;
                System.Console.WriteLine($"Message - '{message.MessageId}' is updated. Seen: {message.Seen}");
                return storeMessage;
            });
            return await task;
        }

        public async Task<IEnumerable<InboxMessage>> GetConversationByFriendId(string userName, string friendsName, int page, int pageSize = 100)
        {
            var task = Task.Run(() =>
            {
                int take, skip;
                page = GetPageModel(page, pageSize, out take, out skip);

                return Messages.Where(m => ShouldSelect(userName, friendsName, m))
                    .OrderByDescending(x => x.CreatedAt).Skip(skip).Take(take);
            });

            return await task;
        }

        private static int GetPageModel(int page, int pageSize, out int take, out int skip)
        {
            take = pageSize;
            skip = 0;
            if (page <= 0)
            {
                page = 1;
            }

            if (page > 1)
            {
                skip = (page - 1) * pageSize;
            }

            return page;
        }

        private bool ShouldSelect(string userName, string friendsName, InboxMessage m)
        {
            //
            // two way conversation
            //

            return (StringEqual(m.Recepient, userName) && StringEqual(m.Sender, friendsName))
             || (StringEqual(m.Recepient, friendsName) && StringEqual(m.Sender, userName));
        }

        public async Task<IEnumerable<ApplicationUser>> GetFriendsList(string userName, int page, int pageSize = 100)
        {
            var task = Task.Run(() =>
            {
                int take, skip;
                page = GetPageModel(page, pageSize, out take, out skip);

                var friends = UserFriends.Where(x => StringEqual(x.UserName, userName));

                return ApplicationUsers
                .Join(friends, a => a.UserName, f => f.FriendId, (appUser, friend) => appUser).Distinct()
                .OrderBy(x => x.FullName).Skip(skip).Take(take);
            });

            return await task;
        }

        public async Task<bool> ValidFriends(string user1, string user2)
        {
            var task = Task.Run(() =>
            {
                var friends = UserFriends.Where(f => StringEqual(f.UserName, user1));
                return friends.Any(f => StringEqual(f.FriendId, user2));
            });

            return await task;
        }

        public ApplicationUser GetUser(string userName)
        {
            return ApplicationUsers.FirstOrDefault(x => StringEqual(x.UserName, userName));
        }

        private bool StringEqual(string a, string b)
        {
            return string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task UpdateUserStatus(UserOnlineStatus status)
        {
            var task = Task.Run(() =>
            {
                var model = UserStatuses.FirstOrDefault(x => StringEqual(x.UserName, status.UserName));
                if (model != null)
                {
                    model.IsOnline = status.IsOnline;
                    model.DoNotDisturb = status.DoNotDisturb;
                    model.LastUpdatedAt = status.LastUpdatedAt;
                }
                else
                {
                    UserStatuses.Add(status);
                }
            });

            await task;
        }

        public async Task<IEnumerable<UserOnlineStatus>> GetFriendsStatus(string userName)
        {

            if (!UserStatuses.Any()) return new List<UserOnlineStatus>();

            var task = Task.Run(() =>
            {
                var friends = UserFriends.Where(x => StringEqual(x.UserName, userName));
                var list = UserStatuses.Join(friends, s => s.UserName, f => f.FriendId, (status, friend) => status)
                .Distinct();
                return list;
            });

            return await task;
        }

        public async Task<InboxMessage> GetMessageById(Guid? messageId)
        {
            var task = Task.Run(() =>
            {
                return Messages.FirstOrDefault(x => x.MessageId == messageId);
            });

            return await task;

        }
    }
}