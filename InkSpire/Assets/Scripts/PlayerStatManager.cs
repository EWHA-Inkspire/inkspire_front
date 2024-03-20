using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatManager : MonoBehaviour
{

    public int atk;
    public int def;
    public int luk;
    public int mental;
    public int dex;
    public string charname;

    public int wheapone=0;
    

    // 플레이어 스탯을 싱글톤으로 관리
    public static PlayerStatManager playerstat;

    public Stats p_stats;

    void Awake(){
        // 씬이 바뀔 때 파괴되지 않음
        DontDestroyOnLoad(this.gameObject);

        if(playerstat == null){
            playerstat=this;
        }

        p_stats = new Stats(luk,def,mental,dex,atk);
        p_stats.SetStatAmount(Stats.Type.CurrHP,5);

    }
    
    public int GetStatAmount(string stat_type){
        return p_stats.GetStatAmount(p_stats.parseEnumType(stat_type));
    }
    public void SetSingleStat(string stat_type, int stat_amount){
        p_stats.SetStatAmount(p_stats.parseEnumType(stat_type),stat_amount);
        p_stats.OnStatsChanged += OnStatsChanged;
    }

    public void ChangeSingleStat(string stat_type, int delta_amount){
        p_stats.SetStatAmount(p_stats.parseEnumType(stat_type),p_stats.GetStatAmount(p_stats.parseEnumType(stat_type))+delta_amount);
        p_stats.OnStatsChanged += OnStatsChanged;
    }

    private void OnStatsChanged(object sender, System.EventArgs e){
        //Debug.Log("Stat Changed: ",p_stats.GetStatAmount(CurrHP),"/",p_stats.GetStatAmount(MaxHP),"\t",p_stats.GetStatAmount(Luck),"\t",p_stats.GetStatAmount(Defence),"\t",p_stats.GetStatAmount(Intelligence),"\t",p_stats.GetStatAmount(Dexterity),"\t",p_stats.GetStatAmount(Attack));
    }
}
