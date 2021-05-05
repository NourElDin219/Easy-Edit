using Graduation_Project_Dragons.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NAudio.Wave;
using Microsoft.CodeAnalysis;
using DeepSpeechClient.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;


namespace Graduation_Project_Dragons.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _Ihosting;
        [Obsolete]
        private IHostingEnvironment Environment;

        /// <summary>

        [Obsolete]
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment Ihosting, IHostingEnvironment _Environment)
        {
            _logger = logger;
            _Ihosting = Ihosting;
            Environment = _Environment;
        }
        public IActionResult Index()
        {
            //// ViewBag.d= deepspeech.Main1();
            return View();
        }

        public IActionResult Speech(string w)
        {
            ViewBag.d = w;
            return View();
        }
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
        [Obsolete]
        public IActionResult Index(Video v)
        {

            string Words = deepspeech.Main1(Upload_Video(v.video));

            return RedirectToAction("Speech", "Home", new { w = Words });
        }

        [Obsolete]
        public string Upload_Video(IFormFile photo)
        {
            string wwwPath = this.Environment.WebRootPath;
            string contentPath = this.Environment.ContentRootPath;

            string path = Path.Combine(this.Environment.WebRootPath, "Video");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileName = Path.GetFileName(photo.FileName);
            using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                // ViewBag.Message += string.Format("<b>{0}</b> uploaded.<br />", fileName);
                photo.CopyTo(stream);
            }

            return photo.FileName;

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
