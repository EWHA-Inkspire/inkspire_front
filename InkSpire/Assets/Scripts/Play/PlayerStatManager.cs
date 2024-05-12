using UnityEngine;

public class PlayerStatManager : MonoBehaviour
{
    // 플레이어 스탯을 싱글톤으로 관리
    public static PlayerStatManager playerstat;
    private Stats p_stats;
    public int wheapone=0;  // 현재 전투시 아이템 사용에 의한 공격력 가중치

    void Awake(){
        if(playerstat == null){
            playerstat = this;
            DontDestroyOnLoad(playerstat);
        }
        else if(playerstat != this){
            Destroy(this);
        }

        p_stats = new Stats(0,0,0,0,0);
    }

    public void SetStats(int atk, int def, int luk, int intl, int dex){
        p_stats = new(atk, def, luk, intl, dex);

        // 플레이어 스탯 변경 API 호출
        PlayAPI.play_api.UpdateCharacterStat(p_stats);
    }

    public void SetStatAmount(StatType type, int amount){
        p_stats.SetStatAmount(type, amount);

        // 플레이어 스탯 변경 API 호출
        PlayAPI.play_api.UpdateCharacterStat(p_stats);
    }

    public int GetStatAmount(StatType type){
        return p_stats.GetStatAmount(type);
    }

    public void SetCharacterStat(CharacterStatInfo statInfo){
        p_stats.SetCharacterStat(statInfo);
    }

    public float GetStatAmountNormalized(StatType type){
        return p_stats.GetStatAmountNormalized(type);
    }

    public Stats GetStats(){
        return p_stats;
    }
}
