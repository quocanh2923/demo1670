//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Demo3.Models;
using Microsoft.AspNetCore.Identity;

namespace Demo3.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Demo3.Models.Company> Company { get; set; } = default!;
        public DbSet<Demo3.Models.JobListing> JobListing { get; set; } = default!;
        public DbSet<Demo3.Models.JobSeeker> JobSeeker { get; set; } = default!;
        public DbSet<Demo3.Models.JobApplication> JobApplication { get; set; } = default!;
        //public DbSet<Demo3.Models.Company> Company { get; set; } = default!;
        //public DbSet<Demo3.Models.JobListing> JobListing { get; set; } = default!;
        //public DbSet<Demo3.Models.JobSeeker> JobSeeker { get; set; } = default!;
        //public DbSet<Demo3.Models.JobApplication> JobApplication { get; set; } = default!;

       
    }
}