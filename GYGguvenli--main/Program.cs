using SecureEmployeeManagement.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC Controller'ları tanımlanıyor
builder.Services.AddControllersWithViews();

// Veritabanı bağlantısı (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Tekrar yazmana gerek yoktu ama zarar vermez:
builder.Services.AddControllersWithViews();

// Oturum yönetimi (Session)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum süresi
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

//  Ortam kontrolü: Geliştirme değilse özel hata sayfası
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//  Uygulama middleware'ları
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();       // Oturum devreye alınıyor
app.UseAuthorization(); // Yetkilendirme kontrolü

//  Açılış rotası: Güvenli login sayfası
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=SafeExamples}/{action=SafeLogin}/{id?}");

app.Run();
