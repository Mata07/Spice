using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        // for images
        private readonly IWebHostEnvironment _hostingEnvironment;

        // property is bind to this controller
        // so we do not need to pass it in Action where we use it
        [BindProperty] 
        public MenuItemViewModel MenuItemVM { get; set; }

        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;

            MenuItemVM = new MenuItemViewModel()
            {
                //SubCategory will depend on which category is selected, so it is not defined here
                Category = _db.Category,
                MenuItem = new MenuItem()
            };
        }

        public async Task<IActionResult> Index()
        {
            var menuItems = await _db.MenuItem
                .Include(m => m.Category)
                .Include(m => m.SubCategory)
                .ToListAsync();

            return View(menuItems);
        }


        //GET - Create
        public IActionResult Create()
        {
            // no need to pass it in because is binded -MenuItemVM
            return View(MenuItemVM);
        }


    }
}
