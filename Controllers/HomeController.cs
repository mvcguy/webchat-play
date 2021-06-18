using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebChatPlay.MessageInbox;
using WebChatPlay.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebChatPlay.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MessageSender messageSender;
        private readonly IMessagesRepository repo;

        public HomeController(ILogger<HomeController> logger,
            MessageSender messageSender,
            IMessagesRepository repo)
        {
            _logger = logger;
            this.messageSender = messageSender;
            this.repo = repo;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMessagesAllUsers(string userName, int page = 1)
        {
            if (!UserMatching(userName)) return BadRequest("Error: Invalid user");
            var result = await this.repo.GetAllConversations(userName, page);
            return Ok(result.OrderBy(x => x.CreatedAt));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMessagesByUserName(string userName, string friendsName, int page = 1)
        {
            if (!UserMatching(userName)) return BadRequest("Error: Invalid user");
            if (!await AreFriends(userName, friendsName)) return BadRequest("Error: Invalid request");

            var result = await this.repo.GetConversationByFriendId(userName, friendsName, page);
            return Ok(result.OrderBy(x => x.CreatedAt));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AckMessage(Guid? messageId, [FromBody] UpdateInboxMessageDto messageDto)
        {
            var message = await repo.GetMessageById(messageDto.MessageId);
            if (message == null) return NotFound("Message not found");
            if (!UserMatching(message.Recepient)) return BadRequest("Error: Invalid user");

            await this.messageSender.SendMessage(messageDto.Serialize());
            return Ok(new { messageDto.MessageId });

        }

        [HttpGet]
        public async Task<IActionResult> GetFriendsList(string userName, int page = 1)
        {
            if (!UserMatching(userName)) return BadRequest("Error: Invalid user");

            var list = await repo.GetFriendsList(userName, page);
            return Ok(list);
        }

        [HttpGet]
        public async Task<IActionResult> GetFriendsStatus(string userName)
        {
            if (!UserMatching(userName)) return BadRequest("Error: Invalid user");

            var friendsStatus = await repo.GetFriendsStatus(userName);
            return Ok(friendsStatus.Select(x => x.ToDto()));
        }

        private bool UserMatching(string userName)
        {
            var currentUser = User.Identity.Name;
            return string.Equals(currentUser, userName, StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<bool> AreFriends(string user1, string user2)
        {
            return await repo.ValidFriends(user1, user2);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
