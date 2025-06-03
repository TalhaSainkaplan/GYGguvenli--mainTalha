using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using SecureEmployeeManagement.Data;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace SecureEmployeeManagement.Controllers
{
    public class UnsafeExamplesController : Controller
    {
        private readonly AppDbContext _context;

        public UnsafeExamplesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult UnsafeLogin(string username, string password, IFormFile xmlFile)
        {
            if (xmlFile != null && xmlFile.Length > 0)
            {
                var doc = new XmlDocument();
                using (var stream = xmlFile.OpenReadStream())
                {
                    doc.Load(stream);
                }

                var fileUsername = doc.SelectSingleNode("//username")?.InnerText ?? string.Empty;
                var filePassword = doc.SelectSingleNode("//password")?.InnerText ?? string.Empty;

                ViewBag.Result = $"XML ile gelen: {fileUsername} - {filePassword}";
                return View("~/Views/Account/Login.cshtml");
            }

            // SQL Injection'a karşı korumalı sorgu
            var user = _context.Users
                .FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("username", user.Username ?? string.Empty);
                HttpContext.Session.SetString("role", user.Role ?? string.Empty);
                return RedirectToAction("Index", "Employee");
            }

            ViewBag.Error = "Hatalı giriş!";
            return View("~/Views/Account/Login.cshtml");
        }

        [Authorize] // Oturum kontrolü ekledik
        public IActionResult BrokenSecret()
        {
            return Content("Bu sayfa sadece giriş yapmış kişilerce görülebilir.");
        }

        [HttpGet]
        public IActionResult XmlLogin()
        {
            return View("~/Views/UnsafeExamples/XmlLogin.cshtml");
        }

        [HttpPost]
        public IActionResult XmlLogin(IFormFile xmlFile)
        {
            if (xmlFile != null && xmlFile.Length > 0)
            {
                var doc = new XmlDocument();
                using (var stream = xmlFile.OpenReadStream())
                {
                    doc.Load(stream);
                }

                var username = doc.SelectSingleNode("//username")?.InnerText ?? string.Empty;
                var password = doc.SelectSingleNode("//password")?.InnerText ?? string.Empty;

                ViewBag.Result = $"Giriş bilgisi: {username} - {password}";
            }

            return View("~/Views/UnsafeExamples/XmlLogin.cshtml");
        }
    }
}
