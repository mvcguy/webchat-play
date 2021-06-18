using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebChatPlay.MessageInbox
{
    public interface IMessagesRepository
    {
        ConcurrentBag<InboxMessage> Messages { get; set; }

        void AddMessage(InboxMessage message);
        Task<IEnumerable<InboxMessage>> GetAllConversations(string userName, int page, int pageSize = 100);
        Task<InboxMessage> UpdateMessage(IUpdateInboxMessage message);

        /// <summary>
        /// Get the conversatation between the current user and her friend
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="friendName"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns>Returns the conversation between current user and his friend</returns>
        Task<IEnumerable<InboxMessage>> GetConversationByFriendId(string userName, string friendName, 
        int page, int pageSize = 100);

        Task<IEnumerable<ApplicationUser>> GetFriendsList(string userName, int page, int pageSize = 100);
        ApplicationUser GetUser(string userName);
        Task<IEnumerable<UserOnlineStatus>> GetFriendsStatus(string userName);

        Task UpdateUserStatus(UserOnlineStatus status);

        Task<bool> ValidFriends(string user1, string user2);
        Task<InboxMessage> GetMessageById(Guid? messageId);
    }
}