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
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Demo3.Controllers
{
    public class JobSeekersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public JobSeekersController(ApplicationDbContext context, 
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: JobSeekers
        [Authorize(Roles = "Seeker")]
        public async Task<IActionResult> Index()
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = _userManager.GetUserId(User); 

            var jobSeekers = await _context.JobSeeker
                                .Where(j => j.UserId == userId)
                                .ToListAsync();

            return View(jobSeekers);
        }

        // GET: JobSeekers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.JobSeeker == null)
            {
                return NotFound();
            }

            var jobSeeker = await _context.JobSeeker
                .Include(j => j.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobSeeker == null)
            {
                return NotFound();
            }

            return View(jobSeeker);
        }

        // GET: JobSeekers/Create
        [Authorize(Roles = "Seeker")]
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: JobSeekers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name,Email,PhoneNumber,Gender,Address,Education,Experience,UserId")] JobSeeker jobSeeker)
        public async Task<IActionResult> Create([Bind("Id,Name,Email,PhoneNumber,Gender,Address,Education,Experience,UserId")] JobSeeker jobSeeker)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User); // Lấy thông tin người dùng hiện tại
                jobSeeker.UserId = user.Id; // Gán UserId cho JobSeeker mới

                _context.Add(jobSeeker);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", jobSeeker.UserId);
            return View(jobSeeker);
        }

        // GET: JobSeekers/Edit/5
        [Authorize(Roles = "Seeker")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.JobSeeker == null)
            {
                return NotFound();
            }

            var jobSeeker = await _context.JobSeeker.FindAsync(id);
            if (jobSeeker == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", jobSeeker.UserId);
            return View(jobSeeker);
        }

        // POST: JobSeekers/Edit/5
        [Authorize(Roles = "Seeker")]
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,PhoneNumber,Gender," +
            "Address,Education,Experience,UserId")] JobSeeker jobSeeker)
        {
            if (id != jobSeeker.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobSeeker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobSeekerExists(jobSeeker.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", jobSeeker.UserId);
            return View(jobSeeker);
        }

        // GET: JobSeekers/Delete/5
        [Authorize(Roles = "Seeker")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.JobSeeker == null)
            {
                return NotFound();
            }

            var jobSeeker = await _context.JobSeeker
                .Include(j => j.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobSeeker == null)
            {
                return NotFound();
            }

            return View(jobSeeker);
        }

        // POST: JobSeekers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.JobSeeker == null)
            {
                return Problem("Entity set 'ApplicationDbContext.JobSeeker'  is null.");
            }
            var jobSeeker = await _context.JobSeeker.FindAsync(id);
            if (jobSeeker != null)
            {
                _context.JobSeeker.Remove(jobSeeker);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobSeekerExists(int id)
        {
          return (_context.JobSeeker?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
