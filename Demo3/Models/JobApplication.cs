using Microsoft.AspNetCore.Identity;

namespace Demo3.Models
{
    public class JobApplication
    {
        public int Id { get; set; }
        public int JobListingId { get; set; }
        public virtual JobListing? JobListing { get; set; }
        public int JobSeekerId { get; set; }
        public virtual JobSeeker? JobSeeker { get; set; }
        public DateTime? ApplicationTime { get; set; }
        public string UserId { get; set; }
        public virtual IdentityUser? User { get; set; }
    }
}
