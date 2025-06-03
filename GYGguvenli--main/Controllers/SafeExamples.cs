using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SecureEmployeeManagement.Data;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace SecureEmployeeManagement.Controllers
{
    public class SafeExamplesController : Controller
    {
        private readonly AppDbContext _context;

        public SafeExamplesController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
public IActionResult SafeLogin()
{
    return View("~/Views/Account/Login.cshtml");
}


        [HttpPost]
        public IActionResult SafeLogin(string username, string password, IFormFile xmlFile)
        {
            //  XML Bomb Güvenliği
            if (xmlFile != null && xmlFile.Length > 0)
            {
                try
                {
                    var settings = new XmlReaderSettings
                    {
                        DtdProcessing = DtdProcessing.Prohibit // DTD kapalı, bomba etkisiz
                    };

                    using (var reader = XmlReader.Create(xmlFile.OpenReadStream(), settings))
                    {
                        var doc = new XmlDocument();
                        doc.Load(reader);

                        var fileUsername = doc.SelectSingleNode("//username")?.InnerText;
                        var filePassword = doc.SelectSingleNode("//password")?.InnerText;

                        ViewBag.Result = $"XML ile gelen: {fileUsername} - {filePassword}";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = " XML dosyası güvenli değil: " + ex.Message;
                }

                return View("~/Views/Account/Login.cshtml");
            }

            //  SQL Injection'dan korunmuş LINQ sorgusu
            var user = _context.Users
                .FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetString("username", user.Username);
                HttpContext.Session.SetString("role", user.Role);
                return RedirectToAction("Index", "Employee");
            }

            ViewBag.Error = " Hatalı giriş!";
            return View("~/Views/Account/Login.cshtml");
        }

        public IActionResult BrokenSecret()
        {
            // Yetkisiz erişimi engeller
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username")))
                return Unauthorized();

            return Content(" Bu sayfa yalnızca giriş yapmış kullanıcılar içindir.");
        }
    }
}
