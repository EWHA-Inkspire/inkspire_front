using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class CharacterInfo
{
    public int userId = PlayerPrefs.GetInt("user_id");
    public string name;

    public CharacterInfo(string name)
    {
        this.name = name;
    }
}

[Serializable]
class CharacterStatInfo
{
    public int luck;
    public int defence;
    public int mental;
    public int dexterity;
    public int attack;
    public int hp;

    public CharacterStatInfo(int attack, int defence, int luck, int mental, int dexterity, int hp)
    {
        this.attack = attack;
        this.defence = defence;
        this.luck = luck;
        this.mental = mental;
        this.dexterity = dexterity;
        this.hp = hp;
    }
}


[Serializable]
public class ChapterList {
    public List<int> chapters = new();
}
