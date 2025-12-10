using test.Models;

namespace test.ViewModels
{
    public class AdminDashboardViewModel
    {
        // Statistics
        public int TotalUsers { get; set; }
        public int AnimalsAvailable { get; set; }
        public int PendingRequests { get; set; }
        public int TotalMessages { get; set; }
        
        // Percentage changes (for display)
        public string UsersChange { get; set; } = "+0%";
        public string AnimalsChange { get; set; } = "0";
        public string RequestsChange { get; set; } = "+0";
        public string MessagesChange { get; set; } = "+0";
        
        // Recent data
        public List<ChatMessage> RecentMessages { get; set; } = new();
        public List<Request> PendingRequestsList { get; set; } = new();
        
        // For display purposes
        public List<ApplicationUser> RequestUsers { get; set; } = new();
        public List<Animal> RequestAnimals { get; set; } = new();
    }
}

