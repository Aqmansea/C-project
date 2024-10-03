using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using System.Collections.Concurrent;

public class ChatHub : Hub
{
    private static ConcurrentDictionary<string, string> _connectedUsers = new ConcurrentDictionary<string, string>(); // 保存用戶和連接 ID 的對應關係
    private readonly ApplicationDbContext _context;

    public ChatHub(ApplicationDbContext context)
    {
        _context = context;
    }

    public override Task OnConnectedAsync()
    {
        // 保存用戶的連接 ID
        string userName = Context.User.Identity.Name ?? "Anonymous"; // 獲取用戶名或默認匿名
        _connectedUsers.TryAdd(Context.ConnectionId, userName);

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        // 移除用戶的連接 ID
        _connectedUsers.TryRemove(Context.ConnectionId, out _);

        return base.OnDisconnectedAsync(exception);
    }

    // 群聊與私聊的訊息處理
    public async Task SendMessage(string user, string message, string chatType)
    {
        var chatMessage = new ChatMessage
        {
            UserName = user,
            Message = message,
            Timestamp = DateTime.Now
        };

        // 將訊息存儲到資料庫
        _context.ChatMessages.Add(chatMessage);
        await _context.SaveChangesAsync();

        if (chatType == "group")
        {
            // 廣播訊息給所有用戶 (群聊)
            await Clients.All.SendAsync("ReceiveMessage", user, message, "group");
        }
        else if (chatType == "private")
        {
            // 查找目標用戶的 ConnectionId
            var targetConnectionId = _connectedUsers.FirstOrDefault(u => u.Value == user).Key;

            if (targetConnectionId != null)
            {
                // 發送私聊訊息給目標用戶
                await Clients.Client(targetConnectionId).SendAsync("ReceiveMessage", user, message, "private");
            }
        }
    }

    // 加入群聊房間
    public async Task JoinRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
    }

    // 離開群聊房間
    public async Task LeaveRoom(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
    }

    // 群聊房間發送訊息
    public async Task SendMessageToRoom(string roomName, string message)
    {
        var userName = Context.User.Identity.Name ?? "Anonymous";
        await Clients.Group(roomName).SendAsync("ReceiveMessage", userName, message);
    }
}