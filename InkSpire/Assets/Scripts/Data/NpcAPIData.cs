using System;
using System.Collections.Generic;

[Serializable]
public class GetNpcList
{
    public List<GetNpcInfo> npcs;
}

[Serializable]
public class GetNpcInfo
{
    public int npcId;
    public string name;
    public string detail;
    public int luck;
    public int defence;
    public int mental;
    public int dexterity;
    public int attack;
    public int hp;
    public bool pnpc;
}

[Serializable]
public class PostNpcInfo
{
    public int mapId;
    public string name;
    public bool isPnpc;
    public string detail;
    public int luck;
    public int defence;
    public int mental;
    public int dexterity;
    public int attack;
    public int hp;
}