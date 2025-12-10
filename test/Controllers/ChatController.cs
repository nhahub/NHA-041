using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using test.Data;
using test.Hubs;
using test.Models;

namespace test.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DepiContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(UserManager<ApplicationUser> userManager, DepiContext context, IHubContext<ChatHub> hubContext)
        {
            _userManager = userManager;
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index(string? receiverId, int? animalid)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (string.IsNullOrEmpty(receiverId))
            {
                return RedirectToAction("index", "Home");
            }

            var receiverUser = await _userManager.FindByIdAsync(receiverId);
            if (receiverUser == null) return NotFound();
            var isShelterUser = await _userManager.IsInRoleAsync(currentUser, "Shelter");
            var isShelter = await _userManager.IsInRoleAsync(receiverUser, "Shelter");

            if (!isShelter&&!isShelterUser)
            {
                var hasApprovedRequest = await _context.Requests.AnyAsync(r =>
                    r.Status == "approved" &&
                    ((r.Userid == currentUser.Id && r.Useridreq == receiverId) ||
                     (r.Userid == receiverId && r.Useridreq == currentUser.Id)));

                if (!hasApprovedRequest)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            var messages = await _context.ChatMessages
                .Include(m => m.Animal)
                .Where(m => (m.SenderId == currentUser.Id && m.ReceiverId == receiverId) ||
                            (m.SenderId == receiverId && m.ReceiverId == currentUser.Id))
                .OrderBy(m => m.Time)
                .ToListAsync();

            var unreadMessageIds = messages
                .Where(m => m.ReceiverId == currentUser.Id && m.read == false)
                .Select(m => m.id)
                .ToList();
            
            if (unreadMessageIds.Any())
            {
                await _context.ChatMessages
                    .Where(m => unreadMessageIds.Contains(m.id))
                    .ExecuteUpdateAsync(s => s.SetProperty(m => m.read, true));

                // Update Notification Count in Session
                var notificationCount = await _context.ChatMessages
                    .Where(m => m.ReceiverId == currentUser.Id && m.read == false)
                    .Select(m => m.SenderId)
                    .Distinct()
                    .CountAsync();
                HttpContext.Session.SetInt32("NotificationCount", notificationCount);

                // Notify sender that messages have been read
                await _hubContext.Clients.User(receiverId).SendAsync("messagesRead", currentUser.Id);
            }

            var receiver = await _userManager.FindByIdAsync(receiverId);
            ViewBag.ReceiverName = receiver?.UserName ?? "Unknown User";

            ViewBag.ReceiverId = receiverId;
            ViewBag.CurrentUserId = currentUser.Id;

            // Fetch animal info if animalid is provided
            if (animalid.HasValue)
            {
                var animal = await _context.Animals.FindAsync(animalid.Value);
                if (animal != null)
                {
                    ViewBag.AnimalId = animal.AnimalId;
                    ViewBag.AnimalName = animal.Name;
                    ViewBag.AnimalType = animal.Type;
                    ViewBag.AnimalAge = animal.Age;
                    ViewBag.AnimalPhoto = animal.Photo;

                    // Check if a quote message already exists for this sender/receiver/animal
                    var quoteExists = await _context.ChatMessages.AnyAsync(m =>
                        m.SenderId == currentUser.Id &&
                        m.ReceiverId == receiverId &&
                        m.AnimalId == animalid.Value);
                    
                    ViewBag.QuoteAlreadySent = quoteExists;
                }
            }

            return View(messages);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificationCount()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Content("");

            var notificationCount = await _context.ChatMessages
                .Where(m => m.ReceiverId == currentUser.Id && m.read == false)
                .Select(m => m.SenderId)
                .Distinct()
                .CountAsync();

            HttpContext.Session.SetInt32("NotificationCount", notificationCount);

            return ViewComponent("Notifications");
        }
        
        [HttpGet]
        public async Task<IActionResult> GetUserNotifications()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Json(new List<object>());

            var unreadSenders = await _context.ChatMessages
                .Where(m => m.ReceiverId == currentUser.Id && m.read == false)
                .GroupBy(m => m.SenderId)
                .Select(g => new { SenderId = g.Key, Count = g.Count() })
                .ToListAsync();

            var notifications = new List<object>();

            foreach (var item in unreadSenders)
            {
                var sender = await _userManager.FindByIdAsync(item.SenderId);
                if (sender != null)
                {
                    notifications.Add(new
                    {
                        userId = item.SenderId,
                        userName = sender.UserName,
                        unreadCount = item.Count
                    });
                }
            }

            return Json(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int messageId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var message = await _context.ChatMessages.FindAsync(messageId);
            if (message != null && message.ReceiverId == currentUser.Id && message.read == false)
            {
                message.read = true;
                await _context.SaveChangesAsync();

                // Notify the sender that this message was read
                await _hubContext.Clients.User(message.SenderId).SendAsync("messagesRead", currentUser.Id);

                return Ok();
            }

            return BadRequest();
        }
    }
}
