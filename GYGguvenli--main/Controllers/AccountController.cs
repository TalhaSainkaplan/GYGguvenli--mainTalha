using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using SecureEmployeeManagement.Data;
using SecureEmployeeManagement.Models;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        if (user != null)
        {
            HttpContext.Session.SetString("username", user.Username ?? string.Empty);
            HttpContext.Session.SetString("role", user.Role ?? string.Empty);
            return RedirectToAction("Index", "Employee");
        }
        ViewBag.Error = "Geçersiz kullanıcı adı veya şifre.";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}