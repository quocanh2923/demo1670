using Microsoft.AspNetCore.Identity;

namespace Demo3.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int HotLine { get; set; }
        public virtual ICollection<JobListing>? JobListing { get; set; }
        public string UserId { get; set; }
        public virtual IdentityUser? User { get; set; }

    }
}
