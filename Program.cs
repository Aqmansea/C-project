using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();  // 註冊 MVC 服務
builder.Services.AddSignalR();  // 註冊 SignalR 服務

// 註冊資料庫上下文和 Identity 服務
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false; // 如果你不希望強制使用特殊字元
    options.Password.RequiredLength = 6; // 密碼至少要 6 位數
}).AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 注意中間件順序
app.UseAuthentication();  // 必須先調用身份驗證
app.UseAuthorization();   // 然後調用授權

// 註冊 MVC 路由
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 添加 SignalR Hub 路由
app.MapHub<ChatHub>("/chatHub");

app.Run();