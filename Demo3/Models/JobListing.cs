using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;

namespace Demo3.Models
{
    public class JobListing
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Qualifications { get; set; }
        public DateTime DeadLine { get; set; }
        public decimal Salary { get; set; }
        public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
        public virtual ICollection<JobApplication>? JobApplication { get; set; }

        public string UserId { get; set; }
        public virtual IdentityUser? User { get; set; }
    }
}
