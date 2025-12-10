using test.Models;

namespace test.ViewModels
{
    public class RequestIndexViewModel
    {
        // Incoming requests (others want to adopt MY animals) - Pending
        public List<Request> IncomingPending { get; set; } = new();
        
        // Incoming requests (others want to adopt MY animals) - Approved
        public List<Request> IncomingApproved { get; set; } = new();
        
        // Outgoing requests (I want to adopt others' animals) - Pending
        public List<Request> OutgoingPending { get; set; } = new();
        
        // Outgoing requests (I want to adopt others' animals) - Approved
        public List<Request> OutgoingApproved { get; set; } = new();
    }
}

