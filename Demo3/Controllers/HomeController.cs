

using Demo3.Data;
using Demo3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Demo3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            //return View();
            var jobListingsQuery = _context.JobListing.Include(j => j.Company).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                jobListingsQuery = jobListingsQuery.Where(j => j.Title.Contains(searchString));
            }

            var jobListings = await jobListingsQuery.ToListAsync();
            return View(jobListings);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}