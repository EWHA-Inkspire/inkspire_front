using System;
using System.Collections.Generic;

[Serializable]
public class GetMapList
{
    public List<GetMapInfo> maps;
}

[Serializable]
public class GetMapInfo
{
    public int mapId;
    public int idx;
    public int chapter;
    public string name;
    public string info;
    public bool visited;
    public bool lastVisited;
    public bool anpc;
}