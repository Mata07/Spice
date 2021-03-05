using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.IO;
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

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryID"].ToString());

            if (!ModelState.IsValid)
            {
                return View(MenuItemVM);
            }

            _db.MenuItem.Add(MenuItemVM.MenuItem);
            await _db.SaveChangesAsync();

            //Work on image saving section
            //Image name should be unique - rename image to Id

            // get web root of app
            string webRootPath = _hostingEnvironment.WebRootPath;
            // extract files user has submitted (from form)
            var files = HttpContext.Request.Form.Files;

            // get menu item from database from Id
            var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

            if (files.Count > 0)
            {
                //files has been uploaded
                // uploads - final path of uploaded image (wwwroot/images)
                var uploads = Path.Combine(webRootPath, "images");
                // get extension of first uploaded file (.jpg)
                var extension = Path.GetExtension(files[0].FileName);

                // move uploaded image and rename it to Id + extension (wwwroot/images/1.jpg)
                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                // add location of saved image to db - MenuItem.Image
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
            }
            else
            {
                // no file was uploaded, so use default image
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                // copy from uploads
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".png");
                // update db
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".png";
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        //GET - Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem
                .Include(m => m.Category)
                .Include(m => m.SubCategory)
                .SingleOrDefaultAsync(m => m.Id == id);
            MenuItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }
            return View(MenuItemVM);
        }

        //POST - Edit
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryID"].ToString());

            if (!ModelState.IsValid)
            {
                // again populate SubCategory
                MenuItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
                return View(MenuItemVM);
            }
            
            //Work on image saving section
            //Image name should be unique - rename image to Id

            // get web root of app
            string webRootPath = _hostingEnvironment.WebRootPath;
            // extract files user has submitted (from form)
            var files = HttpContext.Request.Form.Files;

            // get menu item from database from Id
            var menuItemFromDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

            if (files.Count > 0)
            {
                //new image has been uploaded
                // uploads - final path of uploaded image (wwwroot/images)
                var uploads = Path.Combine(webRootPath, "images");
                // get extension of first uploaded file (.jpg)
                var extension_new = Path.GetExtension(files[0].FileName);

                // delete the original file
                var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                // upload new image and rename it to Id + extension (wwwroot/images/1.jpg)
                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                // add location of saved image to db - MenuItem.Image
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension_new;
            }

            // if user changed only text or anything else than image
            // update properties in db
            menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
            menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
            menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
            menuItemFromDb.Spicyness = MenuItemVM.MenuItem.Spicyness;
            menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
            menuItemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;

            // save changes
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET - Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem
                .Include(m => m.Category)
                .Include(m => m.SubCategory)
                .SingleOrDefaultAsync(m=> m.Id == id);

            //MenuItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }


    }
}
