using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class LikeData : WebSocketData
{
    public int userId;
    public string avatar;
    public string uniqueId;
    public string nickname;
    public int followerCount;
    public int followingCount;
    public bool isModerator;
    public int gifterLevel;
    public string teamMemberLevel;
    public int likeCount;
    public int totalLikeCount;
    public string followRole;
    public bool isSubscriber;
}
