using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Web.WebPages.Html;
using test.Data;
using test.Models;

namespace test.Hubs
{
    public class ChatHub : Hub
    {
        private readonly DepiContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ChatHub(DepiContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public  override Task OnConnectedAsync()
        {
            var userId = _userManager.GetUserId(Context.User);
            if (userId == null)
            {
                return base.OnConnectedAsync();
            }
            var con = new UserConnections
            {
                ConnectionId = Context.ConnectionId,
                UserId = userId
            };
            _context.UserConnections.Add(con);
            _context.SaveChanges();

            return  base.OnConnectedAsync();
        }

        public async Task sendmessage(ChatMessage chatMessage,int? animalid)
        {
            if (chatMessage != null)
            {
                chatMessage.read = false;
                chatMessage.Time = DateTime.Now;
                chatMessage.AnimalId = animalid;
                _context.ChatMessages.Add(chatMessage);
                await _context.SaveChangesAsync();
            }
            var connectionsid = _context.UserConnections
    .Where(c => c.UserId == chatMessage.SenderId)
    .Select(p => p.ConnectionId)
    .ToList();
    
var connectionsid2 = _context.UserConnections
    .Where(c => c.UserId == chatMessage.ReceiverId)
    .Select(p => p.ConnectionId)
    .ToList(); 

            // Fetch animal data if animalid exists
            object? animalData = null;
            if (animalid != null)
            {
                var animal = await _context.Animals.FindAsync(animalid);
                if (animal != null)
                {
                    animalData = new
                    {
                        animalId = animal.AnimalId,
                        name = animal.Name,
                        type = animal.Type,
                        age = animal.Age,
                        photo = animal.Photo
                    };
                }
            }

            if (connectionsid != null) {
                foreach (var connection in connectionsid) {
                    await Clients.Client(connection).SendAsync("sendermessage", chatMessage);
                    
                    // Send animal quote message to sender if animalid exists
                    if (animalid != null && animalData != null)
                    {
                        await Clients.Client(connection).SendAsync("AnimalQuoteMessage", chatMessage, animalData);
                    }
                }
                foreach (var connection in connectionsid2)
                {
                    await Clients.Client(connection).SendAsync("recievermessage", chatMessage);
                    await Clients.Client(connection).SendAsync("updateNotifications", chatMessage.SenderId);
                    
                    // Send animal quote message to receiver if animalid exists
                    if (animalid != null && animalData != null)
                    {
                        await Clients.Client(connection).SendAsync("AnimalQuoteMessageReciever", chatMessage, animalData);
                    }
                }
            }
            
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var connection = _context.UserConnections.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
            if (connection != null)
            {
                _context.UserConnections.Remove(connection);
                _context.SaveChanges();
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
