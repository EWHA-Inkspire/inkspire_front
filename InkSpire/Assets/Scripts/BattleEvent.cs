using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;


// 죽은 적 토글 비활성화, 보너스 주사위 텍스트 초기화!!!!!!!!!!!!!!!!!!!!!!!!
public class BattleEvent : MonoBehaviour
{
    [SerializeField] GameObject battle_window;  // 전체 전투창
    [SerializeField] GameObject atk_window;     // 공격 행동 버튼 그룹
    [SerializeField] GameObject def_window;     // 방어 행동 버튼 그룹
    [SerializeField] GameObject inventory_window;   // 전투용 인벤토리창

    [SerializeField] GameObject bdice_window;   // 전투용 주사위 UI
    [SerializeField] TextMeshProUGUI tens_dice; // 십의 자리 텍스트
    [SerializeField] TextMeshProUGUI ones_dice; // 일의 자리 텍스트
    [SerializeField] TextMeshProUGUI bonus_dice; // 행운 보정치 텍스트

    [SerializeField] ToggleGroup mobgroup;  // 공격 타겟 선택용 토글그룹

    [SerializeField] private TextMeshProUGUI story_object;  // 게임 플레이 텍스트 영역
    [SerializeField] private ScrollRect scroll; // 스크롤 영역
    
    BType bType;    // 전투 타입 구분 -> 보스, 일반
    Turn curr_turn; //현재 공격 턴인 쪽을 의미
    Stats pl_stat = PlayerStatManager.playerstat.p_stats;   //플레이어 스탯
    Stats[] enm_stat = new Stats[5]; // 적 스탯
    bool[] is_dead = new bool[5]; // 적 생존 여부
    int m_num = 1;
    int target = 0;
    int pl_action;

    // *****열거형, 상수 선언 및 정의*****
    public enum BType{
        BOSS,
        MOB
    }
    public enum Turn{
        PL,
        ENM
    }
    const int MOB_BASE = 40;
    const int BOSS_BASE = 70;
    const int DAMAGE_BASE = 50;
    const int DAMAGE_COEF = 200;
    // *****************************************


    public void SetBattle(BType battle_type, int mob_num){
        battle_window.gameObject.SetActive(true);
        bType = battle_type;
        m_num = mob_num;
        int[] tmp_stat = new int[5];
        int enm_dex = 0;
        if(bType == BType.BOSS){
            m_num = 1;
        }

        // 타겟몬스터 토글 개수 셋팅
        bdice_window.gameObject.SetActive(true);
        mobgroup.gameObject.SetActive(true);
        for(int k = mob_num; k<5; k++){
            mobgroup.gameObject.transform.GetChild(k).gameObject.SetActive(false);
        }
        mobgroup.gameObject.SetActive(false);
        bdice_window.gameObject.SetActive(false);

        if(bType == BType.BOSS){
            //보스몬스터 스탯 셋팅
            for(int j = 0; j<5; j++){
                tmp_stat[j] = BOSS_BASE+(int)Mathf.Pow(-1,Random.Range(0,2))*Random.Range(0,11);
            }
            Stats tmp = new Stats(tmp_stat[0],tmp_stat[1],tmp_stat[2],tmp_stat[3],tmp_stat[4]);
            Debug.Log(tmp.GetStatAmount(Stats.Type.Defence));
            enm_stat[0] = tmp;
            enm_dex = enm_stat[0].GetStatAmount(Stats.Type.Dexterity);
            is_dead[0] = false;
        }
        else{
            //일반몬스터 스탯 셋팅
            for(int i = 0; i<mob_num; i++){
                for(int j = 0; j<5; j++){
                    tmp_stat[j] = MOB_BASE+(int)Mathf.Pow(-1,Random.Range(0,2))*Random.Range(0,11);
                    Debug.Log(">>tmpstat"+j+": "+tmp_stat[j]);
                }
                Stats tmp = new Stats(tmp_stat[0],tmp_stat[1],tmp_stat[2],tmp_stat[3],tmp_stat[4]);
                Debug.Log(tmp.GetStatAmount(Stats.Type.Defence));
                enm_stat[i]=tmp;
                enm_stat[i].SetStatAmount(Stats.Type.MaxHP,500);
                enm_stat[i].SetStatAmount(Stats.Type.CurrHP,500);
                enm_dex = Mathf.Max(enm_dex,enm_stat[i].GetStatAmount(Stats.Type.Dexterity));
                is_dead[i] = false;
            }
            
        }

        InventoryManager.inventory.is_battle = true;
        
        //민첩스탯이 높은 쪽이 선공
        if(pl_stat.GetStatAmount(Stats.Type.Dexterity)>enm_dex){
            curr_turn = Turn.PL;
            
            // 선공 메시지 append
            AppendMsg("\n<b>:: 전투 시작 ::</b>\n>> 선공: "+PlayerStatManager.playerstat.charname);

            SetAttackTurn();
        }
        else{
            curr_turn = Turn.ENM;
            
            // 선공 메시지 append
            AppendMsg("\n<b>:: 전투 시작 ::</b>\n>> 선공: Enemy");
            
            SetDefenceTurn();
        }

    }

    void SetAttackTurn(){
        // "공격 턴 시작" 메시지 append
        AppendMsg("\n<i>- 공격 턴 시작 -</i>");
        // 플레이어 체력 로그, 적 스탯 로그 append
        AppendMsg("\n>> "+PlayerStatManager.playerstat.charname+"\tHP: "+PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.CurrHP)+"/"+PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.MaxHP));
        AppendMsg("공격: "+pl_stat.GetStatAmount(Stats.Type.Attack).ToString()+" | 방어: "+pl_stat.GetStatAmount(Stats.Type.Defence).ToString()+" | 민첩: "+pl_stat.GetStatAmount(Stats.Type.Dexterity).ToString()+" | 행운: "+pl_stat.GetStatAmount(Stats.Type.Luck).ToString());

        for(int i = 0; i<m_num; i++){
            AppendMsg("\n>> Enemy"+(i+1)+"\tHP: "+enm_stat[i].GetStatAmount(Stats.Type.CurrHP)+"/"+enm_stat[i].GetStatAmount(Stats.Type.MaxHP));
            AppendMsg("공격: "+enm_stat[i].GetStatAmount(Stats.Type.Attack).ToString()+" | 방어: "+enm_stat[i].GetStatAmount(Stats.Type.Defence).ToString()+" | 민첩: "+enm_stat[i].GetStatAmount(Stats.Type.Dexterity).ToString()+" | 행운: "+enm_stat[i].GetStatAmount(Stats.Type.Luck).ToString());
        }
        AppendMsg(" ");
        inventory_window.gameObject.SetActive(false);
        bdice_window.gameObject.SetActive(false);
        def_window.gameObject.SetActive(false);
        atk_window.gameObject.SetActive(true);
    }

    void SetDefenceTurn(){
        // "방어 턴 시작" 메시지 append
        AppendMsg("\n<i>- 방어 턴 시작 -</i>");
        // 플레이어 체력 로그, 적 스탯 로그 append
        AppendMsg("\n>> "+PlayerStatManager.playerstat.charname+"\tHP: "+PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.CurrHP)+"/"+PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.MaxHP));
        AppendMsg("공격: "+pl_stat.GetStatAmount(Stats.Type.Attack).ToString()+" | 방어: "+pl_stat.GetStatAmount(Stats.Type.Defence).ToString()+" | 민첩: "+pl_stat.GetStatAmount(Stats.Type.Dexterity).ToString()+" | 행운: "+pl_stat.GetStatAmount(Stats.Type.Luck).ToString());
        
        for(int i = 0; i<m_num; i++){
            AppendMsg("\n>> Enemy"+(i+1)+"\tHP: "+enm_stat[i].GetStatAmount(Stats.Type.CurrHP)+"/"+enm_stat[i].GetStatAmount(Stats.Type.MaxHP));
            AppendMsg("공격: "+enm_stat[i].GetStatAmount(Stats.Type.Attack).ToString()+" | 방어: "+enm_stat[i].GetStatAmount(Stats.Type.Defence).ToString()+" | 민첩: "+enm_stat[i].GetStatAmount(Stats.Type.Dexterity).ToString()+" | 행운: "+enm_stat[i].GetStatAmount(Stats.Type.Luck).ToString());
        }
        AppendMsg(" ");
        inventory_window.gameObject.SetActive(false);
        bdice_window.gameObject.SetActive(false);
        atk_window.gameObject.SetActive(false);
        def_window.gameObject.SetActive(true);
    }

    public void AtkTurnAction(int action){
        // 공격 행동 버튼에 붙여놓기
        pl_action = action;
        switch(action){
            case 1: // 공격
            case 3: // 도망
                atk_window.gameObject.SetActive(false);
                SetBDiceWindow();
                break;
            case 2: // 아이템
                inventory_window.gameObject.SetActive(true);
                atk_window.gameObject.SetActive(false);
                break;
            default:
                Debug.Log(">> AtkTurn: Wrong Action");
                break;
        }
    }

    public void DefTurnAction(int action){
        // 방어 행동 버튼에 붙여놓기
        pl_action = action;
        switch(action){
            case 1: // 방어
            case 2: // 회피
            case 4: // 도망
                SetBDiceWindow();
                def_window.gameObject.SetActive(false);
                break;
            case 3: // 아이템
                inventory_window.gameObject.SetActive(true);
                def_window.gameObject.SetActive(false);
                break;
            default:
                Debug.Log(">> DefTurn: Wrong Action");
                break;
        }
    }

    void SetBDiceWindow(){
        bdice_window.gameObject.SetActive(true);
        if(curr_turn == Turn.PL && pl_action == 1){
            mobgroup.gameObject.SetActive(true);
        }
        else{
            mobgroup.gameObject.SetActive(false);
        }
        tens_dice.text = "00";
        ones_dice.text = "00";
        bonus_dice.text = "00";
    }

    public void BattleRollButton(){
        // 전투용 주사위 창 Roll 버튼에 붙여놓기
        if(curr_turn == Turn.PL && pl_action == 1){
            Debug.Log(">> target setting");
            target = int.Parse(mobgroup.ActiveToggles().FirstOrDefault().transform.name);
            Debug.Log("target:"+target);
        }
        int ones = Random.Range(0,10);
        int tens = Random.Range(0,10);
        int pl_value = tens*10+ones;
        //행운 스탯은 모든 판정에 int(rand(0,행운)*0.5)만큼의 보정치를 더해준다.
        int luk_value = Random.Range(0,PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.Luck)/2);

        ones_dice.text = ones.ToString();
        tens_dice.text = tens.ToString();
        bonus_dice.text = luk_value.ToString();

        // 텍스트박스에 결과값(결괏값 행운값 나눠서) 메시지 append
        AppendMsg(PlayerStatManager.playerstat.charname+" DICE>> "+pl_value.ToString()+" + "+luk_value.ToString()+"(Bonus)");
        EnemyAI(pl_value+luk_value);
    }

    void EnemyAI(int pl_dice){
        int hp_sum = 0;
        int enm_dice = Random.Range(0,100);
        int damage;
        int d_damage = 0;

        if((curr_turn == Turn.ENM && pl_action == 4) || (curr_turn == Turn.PL && pl_action == 3)){
            // 도망
            AppendMsg("RESULT>> 도망 시도!!");
            int enm_dex = 0;
            for(int j = 0; j<m_num; j++){  // 몬스터 민첩스탯 중 가장 큰 값 기준
                enm_dex = Mathf.Max(enm_dex,enm_stat[j].GetStatAmount(Stats.Type.Dexterity));
            }

            int dex_diff = enm_dex-PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.Dexterity);
            if(dex_diff<0 || pl_dice>dex_diff){
                //도망 성공
                EndBattle(2);
                return;
            }
            else{
                // 도망 실패 메시지 append
                AppendMsg("RESULT>> 도망 실패!!");
                // 데미지 까임
                pl_dice = 0;
            }

        }

        if(curr_turn == Turn.ENM){
            // 적 공격 턴
            for(int j = 0; j<m_num; j++){
                enm_dice = Random.Range(0,100);
                // 적 주사위 결과 메시지 append
                if(is_dead[j]){
                    continue;
                }
                AppendMsg("Enemy"+(j+1).ToString()+" DICE>> "+enm_dice.ToString());
                if(pl_action == 1 && enm_dice>=90 && pl_dice>=90){
                    // 방어 공격 크리 상쇄 메시지 append
                    AppendMsg("!! 크리티컬 상쇄 !!");
                    enm_dice = pl_dice = 50;
                }
                damage = CalcATKDamage("Enemy"+(j+1).ToString(),enm_dice,enm_stat[j].GetStatAmount(Stats.Type.Luck),enm_stat[j].GetStatAmount(Stats.Type.Attack));
                switch(pl_action){
                    case 1: // 방어
                        d_damage = CalcDEFDamage(PlayerStatManager.playerstat.charname,pl_dice,pl_stat.GetStatAmount(Stats.Type.Defence),damage);
                        break;
                    case 2: // 회피
                        d_damage = CalcDodgeDamage(PlayerStatManager.playerstat.charname,pl_dice,pl_stat.GetStatAmount(Stats.Type.Dexterity),damage);
                        break;
                    default:
                        break;

                }
                damage -= d_damage;
                // 데미지 로그 append
                AppendMsg("DAMAGE>> "+PlayerStatManager.playerstat.charname+" HP"+PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.CurrHP).ToString()+" -> "+(PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.CurrHP)-damage).ToString()+"\n");
                PlayerStatManager.playerstat.p_stats.SetStatAmount(Stats.Type.CurrHP,PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.CurrHP)-damage);
                pl_stat = PlayerStatManager.playerstat.p_stats;
            }
        }

        else{
            // 플레이어 공격 턴 - 타겟만 데미지 계산
            int enm_action = Random.Range(1,3);

            if(enm_action == 1){
                // 방어
                // 적 주사위 결과 메시지 append
                AppendMsg("Enemy"+(target+1).ToString()+" DICE>> "+enm_dice.ToString());
                if(pl_action == 1 && enm_dice>=90 && pl_dice>=90){
                    // 방어 공격 크리 상쇄 메시지 append
                    AppendMsg("!! 크리티컬 상쇄 !!");
                    enm_dice = pl_dice = 50;
                }
                damage = CalcATKDamage(PlayerStatManager.playerstat.charname,pl_dice,pl_stat.GetStatAmount(Stats.Type.Luck),pl_stat.GetStatAmount(Stats.Type.Attack)+PlayerStatManager.playerstat.wheapone);
                d_damage = CalcDEFDamage("Enemy"+(target+1).ToString(),enm_dice,enm_stat[target].GetStatAmount(Stats.Type.Defence),damage);
                
            }
            else{
                // 회피
                // 적 주사위 결과 메시지 append
                AppendMsg("Enemy"+(target+1).ToString()+" DICE>> "+enm_dice.ToString());
                damage = CalcATKDamage(PlayerStatManager.playerstat.charname,pl_dice,pl_stat.GetStatAmount(Stats.Type.Luck),pl_stat.GetStatAmount(Stats.Type.Attack)+PlayerStatManager.playerstat.wheapone);
                d_damage = CalcDodgeDamage("Enemy"+(target+1).ToString(),enm_dice,enm_stat[target].GetStatAmount(Stats.Type.Dexterity),damage);

            }
            damage -= d_damage;
            AppendMsg("DAMAGE>> Enemy"+(target+1).ToString()+" HP"+enm_stat[target].GetStatAmount(Stats.Type.CurrHP).ToString()+" -> "+(enm_stat[target].GetStatAmount(Stats.Type.CurrHP)-damage).ToString()+"\n");
            enm_stat[target].SetStatAmount(Stats.Type.CurrHP,enm_stat[target].GetStatAmount(Stats.Type.CurrHP)-damage);
        }

        for(int i = 0; i<m_num; i++){
            int mob_HP = enm_stat[i].GetStatAmount(Stats.Type.CurrHP);
            hp_sum += mob_HP;
            if(enm_stat[i].GetStatAmount(Stats.Type.CurrHP)==0 && !is_dead[i]){
                // 적 처치 메시지 출력
                AppendMsg("DEFEAT>> Enemy"+(i+1).ToString()+" 처치!!");
                is_dead[i] = true;
            }
        }

        if(PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.CurrHP)==0){
            // 패배
            EndBattle(1);
            return;
        }
        else if(hp_sum == 0){
            // 승리
            EndBattle(0);
            return;
        }
        else{
            SetNextTurn();
        }

    }

    public void SetNextTurn(){
        if(curr_turn == Turn.PL){
            curr_turn = Turn.ENM;
            Invoke("SetDefenceTurn",0.5f);
        }
        else{
            curr_turn = Turn.PL;
            Invoke("SetAttackTurn", 0.5f);   
        }
    }

    int CalcATKDamage(string name, int dice, int luk_stat, int atk_stat){
        // 공격 데미지 계산 함수
        string result="실패!";

        int damage = DAMAGE_BASE;   // 기본데미지
        int luk_dice = Random.Range(1,100);
        if(dice<=5){    // 펌블
            result = "대실패!!";
            damage=0;
        }
        if(dice>=10){   // 성공
            result = "성공!";
            damage+=DAMAGE_COEF*atk_stat/100;
            if(luk_dice<luk_stat){  // 행운 보너스
                // 추가타 발생 메시지 append
                AppendMsg("!! 추가타 발생 !!");
                damage+=DAMAGE_BASE;
            }
        }
        if(dice>=90){   // 크리티컬
            result = "대성공!!";
            damage*=2;
        }

        // 공격 메시지 append
        AppendMsg(name+" | 공격>> "+result+"\t  공격량:"+damage.ToString());
        return damage;
    }

    int CalcDEFDamage(string name, int dice, int def_stat, int damage){
        // 방어 데미지 계산 함수
        string result = "실패!\t방어량: 0";
        int d_damage = 0;   // 실패 데미지
        if(dice<=5){    // 펌블
            result = "대실패!!\t*받는 데미지 1.5배";
            d_damage-=damage/2;
        }
        if(dice>=10){   // 성공
            d_damage+=DAMAGE_COEF*def_stat/100;
            result = "성공!\t방어량: "+d_damage.ToString();
        }
        if(dice>=90 || d_damage>damage){    // 크리
            result = "대성공!!\t*고정데미지 제외 무효화";
            d_damage = Mathf.Clamp(d_damage,0,damage-DAMAGE_BASE);
        }
        // 방어 메시지 append
        AppendMsg(name+" | 방어>> "+result);
        return d_damage;
    }

    int CalcDodgeDamage(string name, int dice, int dex_stat, int damage){
        // 회피 데미지 계산 함수
        int d_damage = 0;
        if(dice>(100-dex_stat)*2){  // 회피 성공
            // 회피 성공 메시지 append
            AppendMsg(name+" | 회피>> 회피 성공!!\t*모든 데미지 무효화");
            d_damage = damage;
        }
        else{   // 회피 실패(방어펌블과 동일)
            // 회피 실패 메시지 append
            AppendMsg(name+" | 회피>> 회피 실패!!");
            d_damage = CalcDEFDamage(name, 0,0,damage);
        }
        return d_damage;
    }

    void EndBattle(int result){
        InventoryManager.inventory.is_battle = false;
        string result_str;
        if(result==0){
            result_str = "WIN";
            // 인벤토리에 보상 아이템 추가
            
        }
        else if(result==1){
            result_str = "LOSE";
        }
        else{
            result_str = "RUN";
            // 도망 전용 스크립트 출력
        }

        Debug.Log(">>Battle "+result_str);

        // 텍스트박스쪽에 전투 결과 메시지 append
        AppendMsg("\n<b>:: 전투 종료 ::</b>\nRESULT>> "+result_str);
        // gpt에게 전투 결과 전달


        bdice_window.gameObject.SetActive(true);
        mobgroup.gameObject.SetActive(true);
        for(int k = 0; k<5; k++){
            mobgroup.gameObject.transform.GetChild(k).gameObject.SetActive(true);
        }
        mobgroup.gameObject.SetActive(false);
        bdice_window.gameObject.SetActive(false);

        PlayerStatManager.playerstat.wheapone = 0;

        //enm_stat.Clear();
        //is_dead.Clear();
        m_num = 0;
        atk_window.gameObject.SetActive(false);
        def_window.gameObject.SetActive(false);
        inventory_window.gameObject.SetActive(false);
        battle_window.gameObject.SetActive(false);

    }

    public void AppendMsg(string msg)
    {
        story_object.text += "\n"+msg;
        LayoutRebuilder.ForceRebuildLayoutImmediate(scroll.content);
        scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scroll.content.sizeDelta.y);
        scroll.verticalNormalizedPosition = 0f;
    }
}
