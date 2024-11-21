using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventData
{
    public int giftType;
    public string uniqueId;
    public string userName;
    public GameObject path;
    public GameObject positon_spawn;
    public string url;
    public EventData(int giftType, string userName, GameObject path, string uniqueId,GameObject gameObject,string avatar)
    {
        this.giftType = giftType;
        this.userName = userName;
        this.path = path;
        this.uniqueId = uniqueId;
        positon_spawn = gameObject;
        this.url = avatar;
    }
}
