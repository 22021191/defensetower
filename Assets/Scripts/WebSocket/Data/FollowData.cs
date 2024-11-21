using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class FollowData : WebSocketData
{
    public string avatar;
    public int userId;
    public string uniqueId;
    public string nickname;
    public int followerCount;
    public int followingCount;
    public bool isModerator;
    public UserBadge userBadges;
    public int createTime;
    public int msgId;
    public string displayType;
    public string label;
    public string followRole;
    public bool isSubscriber;
}
