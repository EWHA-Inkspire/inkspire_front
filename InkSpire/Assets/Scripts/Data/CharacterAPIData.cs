using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
// 캐릭터 정보 저장 요청 (POST)
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
// 캐릭터 스탯 업데이트 (PUT)
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
// 캐릭터 별 챕터 리스트 조회 응답 (GET)
class ChapterList {
    public List<int> chapters = new();
}
