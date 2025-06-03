using Microsoft.AspNetCore.Mvc;
using System.Xml;

namespace SecureEmployeeManagement.Controllers
{
    public class XmlExampleController : Controller
    {
        [HttpGet]
        public IActionResult UploadXml()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadXml(IFormFile xmlFile)
        {
            if (xmlFile != null && xmlFile.Length > 0)
            {
                using (var stream = xmlFile.OpenReadStream())
                {
                    var doc = new XmlDocument(); // GÜVENSİZ YAPI
                    doc.Load(stream); // XML Bomb'a karşı korumasız
                    var name = doc.SelectSingleNode("//name")?.InnerText;
                    ViewBag.Result = $"İsim: {name}";
                }
            }
            return View();
        }
    }
}
