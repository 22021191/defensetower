using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEngine;

public class WebSocketHandler : MonoBehaviour
{
    public event System.Action<ChatData> OnChatReceived;
    public event System.Action<GiftData> OnGiftReceived;
    public event System.Action<LikeData> OnLikeReceived;
    public event System.Action<ShareData> OnShareReceived;
    public void HandleWebSocketMessage(string jsonData)
    {

        // Chuyển đổi JSON sang lớp WebSocketData cơ bản để kiểm tra type.
        WebSocketData baseData = JsonUtility.FromJson<WebSocketData>(jsonData);

        if (baseData != null)
        {
            switch (baseData.type)
            {
                case "chat":
                    ChatData chat = JsonUtility.FromJson<ChatData>(jsonData);
                    OnChatReceived?.Invoke(chat);
                    break;

                case "gift":
                    GiftData gift = JsonUtility.FromJson<GiftData>(jsonData);
                    OnGiftReceived?.Invoke(gift);
                    Debug.LogWarning(jsonData);
                    break;

                case "join":
                    JoinData join = JsonUtility.FromJson<JoinData>(jsonData);
                    break;
                case "like":
                    LikeData likeData = JsonUtility.FromJson<LikeData>(jsonData);
                    OnLikeReceived?.Invoke(likeData);
                    break;
                case "viewercount":
                    ViewerCountData viewercount = JsonUtility.FromJson<ViewerCountData>(jsonData);
                    break;
                case "share":
                    ShareData share = JsonUtility.FromJson<ShareData>(jsonData);
                    OnShareReceived?.Invoke(share);
                    break;
                case "follow":
                    FollowData follow = JsonUtility.FromJson<FollowData>(jsonData);
                    break;
                case "subscribe":
                    SubscribeData subscribeData = JsonUtility.FromJson<SubscribeData>(jsonData);
                    break;
                default:
                    Debug.LogWarning("Unknown data type: " + baseData.type);
                    break;
            }
        }
        else
        {
            Debug.LogError("Failed to parse JSON data.");
        }
    }
}
