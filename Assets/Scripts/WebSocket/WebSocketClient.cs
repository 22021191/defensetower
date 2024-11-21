using UnityEngine;
using WebSocketSharp;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket ws;
    public WebSocketHandler WebSocketHandler;

    void Start()
    {
        // Khởi tạo WebSocket với URL của bạn
        ws = new WebSocket("wss://vliveapp.com/websocket?wsToken=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyIjpudWxsLCJpYXQiOjE3MzE1OTk2MzZ9.vWHX6vVPdKgooXW2uzGBg9RNkweOdWC6xpnJlwxgCVM&type=game&uid=quplMyoiiobJojxzd3l4db0DyIr1");

        // Đăng ký sự kiện mở kết nối
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket đã kết nối thành công.");
        };

        // Đăng ký sự kiện nhận tin nhắn
        ws.OnMessage += (sender, e) =>
        {
            WebSocketHandler.HandleWebSocketMessage(e.Data);
        };

        // Đăng ký sự kiện lỗi
        ws.OnError += (sender, e) =>
        {
            Debug.LogError("Lỗi WebSocket: " + e.Message);
        };

        // Đăng ký sự kiện đóng kết nối
        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket đã đóng kết nối.");
        };

        // Mở kết nối WebSocket
        ws.Connect();
    }

    void OnDestroy()
    {
        // Đóng kết nối khi script bị hủy
        if (ws != null && ws.IsAlive)
        {
            ws.Close();
        }
    }

    // Gửi dữ liệu qua WebSocket
    public void SendMessage(string message)
    {
        if (ws != null && ws.IsAlive)
        {
            ws.Send(message);
        }
        else
        {
            Debug.LogWarning("WebSocket không kết nối.");
        }
    }
}