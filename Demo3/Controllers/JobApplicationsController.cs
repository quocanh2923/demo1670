using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Demo3.Data;
using Demo3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Demo3.Controllers
{
    public class JobApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public JobApplicationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: JobApplications
        [Authorize(Roles = "Seeker")]
        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.JobApplication.Include(j => j.JobListing).Include(j => j.JobSeeker).Include(j => j.User);
            //return View(await applicationDbContext.ToListAsync());
            var userId = _userManager.GetUserId(User); // Lấy UserId của người dùng hiện tại
            var applicationDbContext = _context.JobApplication
                                              .Include(j => j.JobListing)
                                              .Include(j => j.JobSeeker)
                                              .Include(j => j.User)
                                              .Where(j => j.UserId == userId); // Lọc theo UserId
            return View(await applicationDbContext.ToListAsync());
        }
        //// Su li SubmitCV
        public async Task<IActionResult> CreateFromHome(int jobListingId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var jobSeeker = await _context.JobSeeker.FirstOrDefaultAsync(js => js.UserId == user.Id);
                if (user != null && jobSeeker != null)
                {
                    var jobApplication = new JobApplication
                    {
                        JobListingId = jobListingId,
                        JobSeekerId = jobSeeker.Id, 
                        UserId = user.Id,
                        ApplicationTime = DateTime.Now
                    };

                    _context.Add(jobApplication);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "JobApplications");
                }
            }

            return Redirect("/Identity/Account/Login");
        }
        ///

        // GET: JobApplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.JobApplication == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplication
                .Include(j => j.JobListing)
                .Include(j => j.JobSeeker)
                .Include(j => j.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobApplication == null)
            {
                return NotFound();
            }

            return View(jobApplication);
        }

        // GET: JobApplications/Create
        [Authorize(Roles = "Seeker")]
        public IActionResult Create()
        {
            ViewData["JobListingId"] = new SelectList(_context.JobListing, "Id", "Title");
            ViewData["JobSeekerId"] = new SelectList(_context.JobSeeker, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: JobApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,JobListingId,JobSeekerId,ApplicationTime,UserId")] JobApplication jobApplication)
        {
            if (ModelState.IsValid)
            {
                jobApplication.UserId = _userManager.GetUserId(User);
                _context.Add(jobApplication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["JobListingId"] = new SelectList(_context.JobListing, "Id", "Id", jobApplication.JobListingId);
            ViewData["JobSeekerId"] = new SelectList(_context.JobSeeker, "Id", "Name", jobApplication.JobSeekerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", jobApplication.UserId);
            return View(jobApplication);
        }

        // GET: JobApplications/Edit/5
        [Authorize(Roles = "Seeker")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.JobApplication == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplication.FindAsync(id);
            if (jobApplication == null)
            {
                return NotFound();
            }
            ViewData["JobListingId"] = new SelectList(_context.JobListing, "Id", "Id", jobApplication.JobListingId);
            ViewData["JobSeekerId"] = new SelectList(_context.JobSeeker, "Id", "Id", jobApplication.JobSeekerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", jobApplication.UserId);
            return View(jobApplication);
        }

        // POST: JobApplications/Edit/5
        [Authorize(Roles = "Seeker")]
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,JobListingId,JobSeekerId,ApplicationTime,UserId")] JobApplication jobApplication)
        {
            if (id != jobApplication.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobApplication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobApplicationExists(jobApplication.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["JobListingId"] = new SelectList(_context.JobListing, "Id", "Id", jobApplication.JobListingId);
            ViewData["JobSeekerId"] = new SelectList(_context.JobSeeker, "Id", "Id", jobApplication.JobSeekerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", jobApplication.UserId);
            return View(jobApplication);
        }

        // GET: JobApplications/Delete/5
        [Authorize(Roles = "Seeker")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.JobApplication == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplication
                .Include(j => j.JobListing)
                .Include(j => j.JobSeeker)
                .Include(j => j.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobApplication == null)
            {
                return NotFound();
            }

            return View(jobApplication);
        }

        // POST: JobApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.JobApplication == null)
            {
                return Problem("Entity set 'ApplicationDbContext.JobApplication'  is null.");
            }
            var jobApplication = await _context.JobApplication.FindAsync(id);
            if (jobApplication != null)
            {
                _context.JobApplication.Remove(jobApplication);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobApplicationExists(int id)
        {
          return (_context.JobApplication?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
