using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            //return list of all users, except logged user
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return View(await _db.ApplicationUser.Where(u=>u.Id != claim.Value).ToListAsync());
        }

        //Lock user account
        public async Task<IActionResult> Lock(string id)
        {
            if (id==null)
            {
                return NotFound();
            }

            var applicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            //user lockout time after 5 failed password attempts
            applicationUser.LockoutEnd = DateTime.Now.AddYears(1000);

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //UnLock user account
        public async Task<IActionResult> UnLock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            //unlock user after lockout
            applicationUser.LockoutEnd = DateTime.Now;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
