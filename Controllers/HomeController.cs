using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{

    private readonly ApplicationDbContext _context;
      public IActionResult Index()
    {
        return View();  // 返回 Index 視圖
    }


    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Chat()
    {
        // 加載最近的聊天記錄，按時間順序排序
        var messages = _context.ChatMessages.OrderBy(m => m.Timestamp).ToList();
        return View(messages); // 將訊息傳遞到視圖
    }
}
