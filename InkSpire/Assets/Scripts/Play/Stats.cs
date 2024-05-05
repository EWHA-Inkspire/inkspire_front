using System;
using UnityEngine;

public enum StatType
{
    Luck,
    Defence,
    Mental,
    Dexterity,
    Attack,
    Hp,
    MaxHP
}

public class Stats
{
    public event EventHandler OnStatsChanged;
    private int luk;
    private int def;
    private int mntl;
    private int dex;
    private int atk;
    private int hp;

    private readonly int STAT_MIN = 0;
    private readonly int STAT_MAX = 100;
    private readonly int HP_MAX = 1000;

    public Stats(int luk, int def, int mntl, int dex, int atk)
    {
        this.luk = Mathf.Clamp(luk, STAT_MIN, STAT_MAX);
        this.def = Mathf.Clamp(def, STAT_MIN, STAT_MAX);
        this.mntl = Mathf.Clamp(mntl, STAT_MIN, STAT_MAX);
        this.dex = Mathf.Clamp(dex, STAT_MIN, STAT_MAX);
        this.atk = Mathf.Clamp(atk, STAT_MIN, STAT_MAX);
        this.hp = HP_MAX;
    }

    public void SetCharacterStat(CharacterStatInfo statInfo)
    {
        SetStatAmount(StatType.Luck, statInfo.luck);
        SetStatAmount(StatType.Defence, statInfo.defence);
        SetStatAmount(StatType.Mental, statInfo.mental);
        SetStatAmount(StatType.Dexterity, statInfo.dexterity);
        SetStatAmount(StatType.Attack, statInfo.attack);
        SetStatAmount(StatType.Hp, statInfo.hp);
    }

    public void SetStatAmount(StatType type, int amount)
    {
        switch(type) {
            case StatType.Luck:
                luk = Mathf.Clamp(amount, STAT_MIN, STAT_MAX);
                break;
            case StatType.Defence:
                def = Mathf.Clamp(amount, STAT_MIN, STAT_MAX);
                break;
            case StatType.Mental:
                mntl = Mathf.Clamp(amount, STAT_MIN, STAT_MAX);
                break;
            case StatType.Dexterity:
                dex = Mathf.Clamp(amount, STAT_MIN, STAT_MAX);
                break;
            case StatType.Attack:
                atk = Mathf.Clamp(amount, STAT_MIN, STAT_MAX);
                break;
            case StatType.Hp:
                hp = Mathf.Clamp(amount, STAT_MIN, STAT_MAX);
                break;
        }

        OnStatsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetStatAmount(StatType type)
    {
        return type switch
        {
            StatType.Luck => luk,
            StatType.Defence => def,
            StatType.Mental => mntl,
            StatType.Dexterity => dex,
            StatType.Attack => atk,
            StatType.Hp => hp,
            StatType.MaxHP => HP_MAX,
            _ => 0,
        };
    }

    public float GetStatAmountNormalized(StatType type)
    {
        return type switch
        {
            StatType.Luck => (float)luk / STAT_MAX,
            StatType.Defence => (float)def / STAT_MAX,
            StatType.Mental => (float)mntl / STAT_MAX,
            StatType.Dexterity => (float)dex / STAT_MAX,
            StatType.Attack => (float)atk / STAT_MAX,
            StatType.Hp => (float)hp / HP_MAX,
            _ => 0f,
        };
    }
    
}