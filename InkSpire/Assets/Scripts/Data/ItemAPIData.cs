using System;
using System.Collections.Generic;

[Serializable]
public class GetItemList
{
    public List<GetItemInfo> items;
}

[Serializable]
public class GetItemInfo
{
    public int mapId;
    public int itemId;
    public string name;
    public string info;
    public string type;
    public int stat;
}