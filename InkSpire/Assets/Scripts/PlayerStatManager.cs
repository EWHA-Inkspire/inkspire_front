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
}
