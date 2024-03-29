﻿using Graduation_Project_Dragons.Models;
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
        public static IHostingEnvironment Environment;

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
        public IActionResult Speech(string v_Name, string y, string z)
        {
            var start = y.Split(',');
            var end = z.Split(',');
            for (int i = 0; i < end.Length - 1; i++)
            {
                cut_Video(Convert.ToDouble(start[i]), Convert.ToDouble(end[i]), v_Name, i);
            }
            merge_Videos(v_Name);
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
            string wwwPath = Environment.WebRootPath;
            string contentPath = Environment.ContentRootPath;

            string path = Path.Combine(Environment.WebRootPath, "Video");
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
        static void cut_Video(double start, double end, string v_Name, int index)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(start);
            double ms = (end - start) * 1000;
            string time = timeSpan.ToString();
            string time2 = ms.ToString();
            string strCmdText = $"/c ffmpeg -i {v_Name}.mp4 -ss {time} -t {time2}ms -async 1  {v_Name}_{index.ToString()}.mp4 -y";
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.Arguments = strCmdText;
            process.Start();
            process.WaitForExit();
        }
        static void merge_Videos(string v_Name)
        {
            string strCmdText = $"/c (for %i in ({v_Name}_*.mp4) do @echo file '%i') > list.txt";
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.Arguments = strCmdText;
            process.Start();
            process.WaitForExit();
            strCmdText = $"/c ffmpeg -f concat -i list.txt -c copy {v_Name}_final.mp4";
            process.StartInfo.Arguments = strCmdText;
            process.Start();
            process.WaitForExit();
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
