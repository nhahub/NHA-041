using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using test.Models;

namespace test.Interfaces
{
    public interface IRequests
    {
        public  List<ApplicationUser> RequestSent(string userid, List<Request> requests);
        public List<ApplicationUser> RequestGot(string userid, List<Request> requests);
        public List<Animal> AnimalsNeeded(string  userid,List<Request> requests);
        public Task<List<Models.Request>> LoadRequests(string userid);
        public Task<bool> addRequest(Request request);
        public Task<bool> approverequest(int id);
        public Task<bool> rejectRequest(int id);
        public Task<Request> GetRequestById(int id);
        public Task<bool> DeleteRequest(Request request);
        public Task<bool> HasAcceptedRequest(string userId1, string userId2);
        public Task<List<Models.Request>> LoadRequestsForAnimal(int id);
        public Task<bool> HasPendingRequestForAnimal(string userId, int animalId);


        public bool savechanges();



    }
}
