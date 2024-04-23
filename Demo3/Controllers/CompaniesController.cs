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
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CompaniesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Companies
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> Index()
        {
            //var applicationDbContext = _context.Company.Include(c => c.User);
            //return View(await applicationDbContext.ToListAsync());
            var userId = _userManager.GetUserId(User); 
            var applicationDbContext = _context.Company.Include(c => c.User).Where(c => c.UserId == userId);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Company == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        [Authorize(Roles = "Company")]
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Companies/Create
        [Authorize(Roles = "Company")]
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,HotLine,UserId")] Company company)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User); // Lấy UserId của người dùng hiện tại
                company.UserId = userId; // Gán UserId cho Company mới

                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", company.UserId);
            return View(company);
        }

        // GET: Companies/Edit/5
        [Authorize(Roles = "Company")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Company == null)
            {
                return NotFound();
            }

            var company = await _context.Company.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", company.UserId);
            return View(company);
        }

        // POST: Companies/Edit/5
        [Authorize(Roles = "Company")]
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,HotLine,UserId")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", company.UserId);
            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Company == null)
            {
                return NotFound();
            }

            var company = await _context.Company
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [Authorize(Roles = "Company")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Company == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Company'  is null.");
            }
            var company = await _context.Company.FindAsync(id);
            if (company != null)
            {
                _context.Company.Remove(company);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
          return (_context.Company?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
