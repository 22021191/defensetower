using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SubscribeData : WebSocketData
{
    public int userId;
    public string avatar;
    public string uniqueId;
    public string nickname;
}
