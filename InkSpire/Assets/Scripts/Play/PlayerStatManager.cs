using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatManager : MonoBehaviour
{
    // 플레이어 스탯을 싱글톤으로 관리
    public static PlayerStatManager playerstat;
    public Stats p_stats;
    public int wheapone=0;  // 현재 전투시 아이템 사용에 의한 공격력 가중치

    void Awake(){
        // 씬이 바뀔 때 파괴되지 않음
        DontDestroyOnLoad(this.gameObject);

        if(playerstat == null){
            playerstat=this;
        }

        p_stats = new Stats(0,0,0,0,0);
    }
}
