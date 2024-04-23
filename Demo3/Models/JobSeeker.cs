using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;

namespace Demo3.Models
{
    public class JobSeeker
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Education { get; set; }
        public string Experience { get; set; }
        public virtual ICollection<JobApplication>? JobApplication { get; set; }
        public string UserId { get; set; }
        public virtual IdentityUser? User { get; set; }

    }
}
