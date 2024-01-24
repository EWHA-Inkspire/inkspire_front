using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats
{
    public event EventHandler OnStatsChanged;
    public static int STAT_MIN = 0;
    public static int STAT_MAX = 20;

    public enum Type {
        Attack,
        Defence, 
        Luck, 
        Intelligence,
        Dexterity,
        MaxHP,
        CurrHP
    }

    private SingleStat stat_luk;
    private SingleStat stat_def;
    private SingleStat stat_atk;
    private SingleStat stat_int;
    private SingleStat stat_dex;
    private SingleStat stat_maxhp;
    private SingleStat stat_currhp;

    public Stats (int luck, int defence, int intelligence, int dexterity, int attack){
        stat_atk = new SingleStat(attack);
        stat_def = new SingleStat(defence);
        stat_luk = new SingleStat(luck);
        stat_int = new SingleStat(intelligence);
        stat_dex = new SingleStat(dexterity);
        stat_maxhp = new SingleStat(1000);
        stat_currhp = new SingleStat(1000);
    }

    private SingleStat GetSingleStat(Type stat_type){
     switch(stat_type)  {
        default:
        case Type.Attack:       return stat_atk;
        case Type.Defence:      return stat_def;
        case Type.Luck:         return stat_luk;
        case Type.Intelligence: return stat_int;
        case Type.Dexterity:    return stat_dex;
        case Type.MaxHP:        return stat_maxhp;
        case Type.CurrHP:       return stat_currhp;
     }
    }

    public void SetStatAmount(Type stat_type, int stat_amount){
        GetSingleStat(stat_type).SetStatAmount(stat_amount);
        if(OnStatsChanged != null) OnStatsChanged(this, EventArgs.Empty);
    }

    public int GetStatAmount(Type stat_type){
        return GetSingleStat(stat_type).GetStatAmount();
    }

    public float GetStatAmountNormalized(Type stat_type){
        return GetSingleStat(stat_type).GetStatAmountNormalized();
    }

    public Type parseEnumType(string orgstring){
        if(Enum.IsDefined(typeof(Type),orgstring)){
            return (Type)Enum.Parse(typeof(Type),orgstring);
        }
        else
        {
            return Type.Attack;
        }
    }


    private class SingleStat{
        private int stat;

        public SingleStat(int stat_amount){
            SetStatAmount(stat_amount);
        }

        public void SetStatAmount(int stat_amount){
            stat = Mathf.Clamp(stat_amount,STAT_MIN,STAT_MAX);
        }

        public int GetStatAmount(){
            return stat;
        }

        public float GetStatAmountNormalized(){
            return (float)stat/STAT_MAX;
        }
        
    
    }

}
