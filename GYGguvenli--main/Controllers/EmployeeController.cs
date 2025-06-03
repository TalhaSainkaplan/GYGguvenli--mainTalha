using Microsoft.AspNetCore.Mvc;
using SecureEmployeeManagement.Data;
using SecureEmployeeManagement.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

public class EmployeeController : Controller
{
    private readonly AppDbContext _context;

    public EmployeeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("username") == null)
            return RedirectToAction("Login", "Account");

        var employees = _context.Employees.ToList();
        return View(employees);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Employee employee)
    {
        _context.Employees.Add(employee);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var employee = _context.Employees.FirstOrDefault(e => e.Id == id);
        return View(employee);
    }

    [HttpPost]
    public IActionResult Edit(Employee employee)
    {
        _context.Employees.Update(employee);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
{
    var employee = _context.Employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        TempData["Error"] = "Silinmek istenen kayıt bulunamadı.";
        return RedirectToAction("Index");
    }

    _context.Employees.Remove(employee);
    _context.SaveChanges();
    return RedirectToAction("Index");
}

}