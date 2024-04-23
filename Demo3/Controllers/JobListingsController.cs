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
    public class JobListingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public JobListingsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: JobListings
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.JobListing.Include(j => j.Company).Include(j => j.User);
            //return View(await applicationDbContext.ToListAsync());
            var userId = _userManager.GetUserId(User); // Lấy UserId của người dùng hiện tại
            var applicationDbContext = _context.JobListing
                                              .Include(j => j.Company)
                                              .Include(j => j.User)
                                              .Where(j => j.UserId == userId); 
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: JobListings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.JobListing == null)
            {
                return NotFound();
            }

            var jobListing = await _context.JobListing
                .Include(j => j.Company)
                //Thêm CV người dùng đã apply
                .Include(j => j.JobApplication)
                    .ThenInclude(ja => ja.JobSeeker)
                .Include(j => j.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobListing == null)
            {
                return NotFound();
            }

            return View(jobListing);
        }

        // GET: JobListings/Create
        [Authorize(Roles = "Company")]
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: JobListings/Create
      
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Qualifications,DeadLine,Salary,CompanyId,UserId")] JobListing jobListing)
        {
            if (ModelState.IsValid)
            {
                jobListing.UserId = _userManager.GetUserId(User);
                _context.Add(jobListing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Name", jobListing.CompanyId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", jobListing.UserId);
            return View(jobListing);
        }

        // GET: JobListings/Edit/5
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.JobListing == null)
            {
                return NotFound();
            }

            var jobListing = await _context.JobListing.FindAsync(id);
            if (jobListing == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Name", jobListing.CompanyId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", jobListing.UserId);
            return View(jobListing);
        }

        // POST: JobListings/Edit/5
        [Authorize(Roles = "Company")]
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Qualifications,DeadLine,Salary,CompanyId,UserId")] JobListing jobListing)
        {
            if (id != jobListing.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobListing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobListingExists(jobListing.Id))
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
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Name", jobListing.CompanyId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", jobListing.UserId);
            return View(jobListing);
        }

        // GET: JobListings/Delete/5
        [Authorize(Roles = "Company")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.JobListing == null)
            {
                return NotFound();
            }

            var jobListing = await _context.JobListing
                .Include(j => j.Company)
                .Include(j => j.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobListing == null)
            {
                return NotFound();
            }

            return View(jobListing);
        }

        // POST: JobListings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.JobListing == null)
            {
                return Problem("Entity set 'ApplicationDbContext.JobListing'  is null.");
            }
            var jobListing = await _context.JobListing.FindAsync(id);
            if (jobListing != null)
            {
                _context.JobListing.Remove(jobListing);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobListingExists(int id)
        {
          return (_context.JobListing?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
