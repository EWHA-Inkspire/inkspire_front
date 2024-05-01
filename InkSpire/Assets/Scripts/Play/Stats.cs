using System;
using System.Collections;
using System.Collections.Generic;
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

    private static int STAT_MIN = 0;
    private static int STAT_MAX = 100;
    private static int HP_MAX = 1000;

    public Stats(int luk, int def, int mntl, int dex, int atk)
    {
        this.luk = Mathf.Clamp(luk, STAT_MIN, STAT_MAX);
        this.def = Mathf.Clamp(def, STAT_MIN, STAT_MAX);
        this.mntl = Mathf.Clamp(mntl, STAT_MIN, STAT_MAX);
        this.dex = Mathf.Clamp(dex, STAT_MIN, STAT_MAX);
        this.atk = Mathf.Clamp(atk, STAT_MIN, STAT_MAX);
        this.hp = HP_MAX;
    }

    public void SetStatAmount(StatType type, int amount)
    {
        switch(type) {
            case StatType.Luck:
                luk = amount;
                break;
            case StatType.Defence:
                def = amount;
                break;
            case StatType.Mental:
                mntl = amount;
                break;
            case StatType.Dexterity:
                dex = amount;
                break;
            case StatType.Attack:
                atk = amount;
                break;
            case StatType.Hp:
                hp = amount;
                break;
        }

        if (OnStatsChanged != null) {
            OnStatsChanged(this, EventArgs.Empty);
        }
    }

    public int GetStatAmount(StatType type)
    {
        switch(type) {
            case StatType.Luck:          return luk;
            case StatType.Defence:          return def;
            case StatType.Mental:       return mntl;
            case StatType.Dexterity:    return dex;
            case StatType.Attack:       return atk;
            case StatType.Hp:           return hp;
            case StatType.MaxHP:        return HP_MAX;
            default: return 0;
        }
    }

    public float GetStatAmountNormalized(StatType type)
    {
        switch(type) {
            case StatType.Luck:          return (float) luk/STAT_MAX;
            case StatType.Defence:          return (float) def/STAT_MAX;
            case StatType.Mental:       return (float) mntl/STAT_MAX;
            case StatType.Dexterity:    return (float) dex/STAT_MAX;
            case StatType.Attack:       return (float) atk/STAT_MAX;
            case StatType.Hp:           return (float) hp/HP_MAX;
            default: return 0f;
        }
    }
    
}