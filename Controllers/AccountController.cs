using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    // GET: Register view
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // POST: Handle register form submission
    [HttpPost]
    public async Task<IActionResult> Register(string email, string password)
    {
        // Create new user
        var user = new IdentityUser { UserName = email, Email = email };
        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            // Sign in the user after registration
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        // If registration failed, show error
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        return View();
    }

    // GET: Login view
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: Handle login form submission
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        if (ModelState.IsValid)
        {
            // 找到用戶
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                // 驗證憑據並登入
                var result = await _signInManager.PasswordSignInAsync(user.UserName, password, false, false);
                if (result.Succeeded)
                {
                    // 登入成功後重定向到聊天室頁面
                    return RedirectToAction("Chat", "Home");
                }
            }

            // 如果登入失敗，顯示錯誤
            ModelState.AddModelError("", "Invalid login attempt");
        }

        return View();
    }

    // Logout action
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}