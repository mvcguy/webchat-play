namespace WebChatPlay.MessageInbox
{
    public class ApplicationUser
    {
        public string UserName { get; set; }

        public string FullName { get; set; }
    }

    public class UserFriend
    {
        public string UserName { get; set; }

        public string FriendId { get; set; }
    }

}