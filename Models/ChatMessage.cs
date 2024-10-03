using Microsoft.EntityFrameworkCore;
public class ChatMessage
{
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty; // 初始化為空字串
    public string Message { get; set; } = string.Empty;  // 初始化為空字串

    public DateTime Timestamp { get; set; } = DateTime.Now;
}
