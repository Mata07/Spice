﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.Coupon.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupons)
        {
            if (ModelState.IsValid)
            {
                //get picture
                var files = HttpContext.Request.Form.Files;
                if (files.Count>0)
                {
                    //convert picture to byte stream to save in db
                    byte[] p1 = null;

                    //start reading file
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        //convert
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    coupons.Picture = p1;
                }
                // save picture to database
                _db.Coupon.Add(coupons);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(coupons);
        }

    }
}
