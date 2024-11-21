using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ChatData : WebSocketData
{
    public string avatar;
    public int userId;
    public string uniqueId;
    public string nickname;
    public int followerCount;
    public int followingCount;
    public bool isModerator;
    public int gifterLevel;
    public string teamMemberLevel;
    public string comment;
    public string followRole;
    public bool isSubscriber;
}
