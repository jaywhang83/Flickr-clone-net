using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using FlikrClone.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using System.IO;
using Microsoft.Net.Http.Headers;

namespace FlikrClone.Controllers
{
    [Authorize]
    public class ImageController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private IHostingEnvironment _environment;

        public ImageController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext db,
            IHostingEnvironment environment
        )
        {
            _userManager = userManager;
            _db = db;
            _environment = environment;
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ICollection<IFormFile> files)
        {
            Image image = new Image();
            image.Description = Request.Form["Description"];
            image.User = await _userManager.FindByIdAsync(User.GetUserId());
            //var currentUser = await _userManager.FindByIdAsync(User.GetUserId());
            //image.User = currentUser;
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            string fileName;
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    await file.SaveAsAsync(Path.Combine(uploads, fileName));
                    image.Url = "/uploads/" + fileName;
                    break; 
                }
            }

            _db.Images.Add(image);
            _db.SaveChanges();
            return RedirectToAction("Index");
            //return View();
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.FindByIdAsync(User.GetUserId());
            return View(_db.Images.Where(x => x.User.Id == currentUser.Id));
        }

        public IActionResult Details(int id)
        {
            var thisImage = _db.Images.FirstOrDefault(images => images.ImageId == id);
            thisImage.Comments = _db.Comments.Where(x => x.ImageId == id).ToList();
            return View(thisImage);
        }

        public IActionResult Edit(int id)
        {
            var thisImage = _db.Images.FirstOrDefault(images => images.ImageId == id);
            return View(thisImage);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Image image)
        {
            var currentUser = await _userManager.FindByIdAsync(User.GetUserId());
            image.User = currentUser;
            _db.Entry(image).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var thisImage = _db.Images.FirstOrDefault(images => images.ImageId == id);
            return View(thisImage);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            //var currentUser = await _userManager.FindByIdAsync(User.GetUserId());
            //image.User = currentUser;
            var thisImage = _db.Images.FirstOrDefault(images => images.ImageId == id);
            _db.Images.Remove(thisImage);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
