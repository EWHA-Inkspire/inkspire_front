using System;
using System.Collections.Generic;

[Serializable]
public class GetEventList
{
    public List<GetEventInfo> events;
}

[Serializable]
public class GetEventInfo
{
    public int eventId;
    public int mapId;
    public string eventTrigger;
    public string title;
    public string intro;
    public string success;
    public string failure;
    public bool goal;
}

[Serializable]
public class PostEventInfo
{
    public int mapId;
    public bool isGoal;
    public string eventTrigger;
    public string title;
    public string intro;
    public string success;
    public string failure;
}