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
    [SerializeField] GameObject inventory_back;     // 전투 뒤로 버튼

    [SerializeField] GameObject bdice_window;   // 전투용 주사위 UI
    [SerializeField] TextMeshProUGUI tens_dice; // 십의 자리 텍스트
    [SerializeField] TextMeshProUGUI ones_dice; // 일의 자리 텍스트
    [SerializeField] TextMeshProUGUI bonus_dice; // 행운 보정치 텍스트

    [SerializeField] Button scene_inven_button; // 씬 상단 인벤토리 버튼

    [SerializeField] ToggleGroup mobgroup;  // 공격 타겟 선택용 토글그룹

    [SerializeField] private TextScrollUI text_scroll;  // 게임 플레이 텍스트 영역
    [SerializeField] private GameObject EndChapterModal;

    BType bType;    // 전투 타입 구분 -> 보스, 일반
    Turn curr_turn; //현재 공격 턴인 쪽을 의미
    PlayerStatManager p_manager = PlayerStatManager.playerstat; // 플레이어 스탯
    Stats[] enm_stat = new Stats[5]; // 적 스탯
    bool[] is_dead = new bool[5]; // 적 생존 여부
    int m_num = 1;
    int target = 0;
    int pl_action;
    string boss_name;

    // *****열거형, 상수 선언 및 정의*****
    public enum BType
    {
        BOSS,
        MOB
    }
    public enum Turn
    {
        PL,
        ENM
    }
    const int MOB_BASE = 40;
    const int BOSS_BASE = 70;
    const int DAMAGE_BASE = 50;
    const int DAMAGE_COEF = 200;
    // *****************************************


    public void SetBattle(BType battle_type, int mob_num)
    {
        battle_window.SetActive(true);
        scene_inven_button.interactable = false;
        bType = battle_type;
        m_num = mob_num;
        int[] tmp_stat = new int[5];
        int enm_dex = 0;
        if (bType == BType.BOSS)
        {
            m_num = 1;
        }

        // 타겟몬스터 토글 개수 셋팅
        bdice_window.SetActive(true);
        mobgroup.gameObject.SetActive(true);
        for (int k = mob_num; k < 5; k++)
        {
            mobgroup.gameObject.transform.GetChild(k).gameObject.SetActive(false);
        }
        mobgroup.gameObject.SetActive(false);
        bdice_window.SetActive(false);

        if (bType == BType.BOSS)
        {
            boss_name = ScriptManager.script_manager.GetCurrGoal().GetEtc();
            mobgroup.gameObject.transform.GetChild(0).gameObject.transform.GetChild(1).GetComponent<Text>().text = boss_name;
            //보스몬스터 스탯 셋팅
            for (int j = 0; j < 5; j++)
            {
                tmp_stat[j] = BOSS_BASE + (int)Mathf.Pow(-1, Random.Range(0, 2)) * Random.Range(0, 11);
            }
            Stats tmp = new(tmp_stat[0], tmp_stat[1], tmp_stat[2], tmp_stat[3], tmp_stat[4]);
            Debug.Log(tmp.GetStatAmount(StatType.Defence));
            enm_stat[0] = tmp;
            enm_dex = enm_stat[0].GetStatAmount(StatType.Dexterity);
            is_dead[0] = false;
        }
        else
        {
            //일반몬스터 스탯 셋팅
            for (int i = 0; i < mob_num; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    tmp_stat[j] = MOB_BASE + (int)Mathf.Pow(-1, Random.Range(0, 2)) * Random.Range(0, 11);
                    Debug.Log(">>tmpstat" + j + ": " + tmp_stat[j]);
                }
                Stats tmp = new(tmp_stat[0], tmp_stat[1], tmp_stat[2], tmp_stat[3], tmp_stat[4]);
                Debug.Log(tmp.GetStatAmount(StatType.Defence));
                enm_stat[i] = tmp;
                enm_stat[i].SetStatAmount(StatType.MaxHP, 500);
                enm_stat[i].SetStatAmount(StatType.Hp, 500);
                enm_dex = Mathf.Max(enm_dex, enm_stat[i].GetStatAmount(StatType.Dexterity));
                is_dead[i] = false;
            }

        }

        InventoryManager.i_manager.is_battle = true;

        //민첩스탯이 높은 쪽이 선공
        if (p_manager.GetStatAmount(StatType.Dexterity) > enm_dex)
        {
            curr_turn = Turn.PL;

            // 선공 메시지 append
            AppendMsg("\n<b>:: 전투 시작 ::</b>\n>> 선공: " + ScriptManager.script_manager.GetCharName());

            SetAttackTurn();
        }
        else
        {
            curr_turn = Turn.ENM;

            // 선공 메시지 append
            if(bType == BType.BOSS){
                AppendMsg("\n<b>:: 전투 시작 ::</b>\n>> 선공: "+GetMobName(0));
            }
            else{
                AppendMsg("\n<b>:: 전투 시작 ::</b>\n>> 선공: Enemy");
            }

            SetDefenceTurn();
        }

    }

    void SetAttackTurn()
    {
        // "공격 턴 시작" 메시지 append
        AppendMsg("\n<i>- 공격 턴 시작 -</i>");
        // 플레이어 체력 로그, 적 스탯 로그 append
        AppendMsg("\n>> " + ScriptManager.script_manager.GetCharName() + "\tHP: " + p_manager.GetStatAmount(StatType.Hp) + "/" + p_manager.GetStatAmount(StatType.MaxHP));
        AppendMsg("공격: " + p_manager.GetStatAmount(StatType.Attack).ToString() + " | 방어: " + p_manager.GetStatAmount(StatType.Defence).ToString() + " | 민첩: " + p_manager.GetStatAmount(StatType.Dexterity).ToString() + " | 행운: " + p_manager.GetStatAmount(StatType.Luck).ToString());

        for (int i = 0; i < m_num; i++)
        {
            AppendMsg("\n>> "+GetMobName(i) + "\tHP: " + enm_stat[i].GetStatAmount(StatType.Hp) + "/" + enm_stat[i].GetStatAmount(StatType.MaxHP));
            AppendMsg("공격: " + enm_stat[i].GetStatAmount(StatType.Attack).ToString() + " | 방어: " + enm_stat[i].GetStatAmount(StatType.Defence).ToString() + " | 민첩: " + enm_stat[i].GetStatAmount(StatType.Dexterity).ToString() + " | 행운: " + enm_stat[i].GetStatAmount(StatType.Luck).ToString());
        }
        AppendMsg(" ");
        inventory_window.SetActive(false);
        bdice_window.SetActive(false);
        def_window.SetActive(false);
        atk_window.SetActive(true);
    }

    void SetDefenceTurn()
    {
        // "방어 턴 시작" 메시지 append
        AppendMsg("\n<i>- 방어 턴 시작 -</i>");
        // 플레이어 체력 로그, 적 스탯 로그 append
        AppendMsg("\n>> " + ScriptManager.script_manager.GetCharName() + "\tHP: " + p_manager.GetStatAmount(StatType.Hp) + "/" + p_manager.GetStatAmount(StatType.MaxHP));
        AppendMsg("공격: " + p_manager.GetStatAmount(StatType.Attack).ToString() + " | 방어: " + p_manager.GetStatAmount(StatType.Defence).ToString() + " | 민첩: " + p_manager.GetStatAmount(StatType.Dexterity).ToString() + " | 행운: " + p_manager.GetStatAmount(StatType.Luck).ToString());

        for (int i = 0; i < m_num; i++)
        {
            AppendMsg("\n>> " + GetMobName(i) + "\tHP: " + enm_stat[i].GetStatAmount(StatType.Hp) + "/" + enm_stat[i].GetStatAmount(StatType.MaxHP));
            AppendMsg("공격: " + enm_stat[i].GetStatAmount(StatType.Attack).ToString() + " | 방어: " + enm_stat[i].GetStatAmount(StatType.Defence).ToString() + " | 민첩: " + enm_stat[i].GetStatAmount(StatType.Dexterity).ToString() + " | 행운: " + enm_stat[i].GetStatAmount(StatType.Luck).ToString());
        }
        AppendMsg(" ");
        inventory_window.SetActive(false);
        bdice_window.SetActive(false);
        atk_window.SetActive(false);
        def_window.SetActive(true);
    }

    public void AtkTurnAction(int action)
    {
        // 공격 행동 버튼에 붙여놓기
        pl_action = action;
        switch (action)
        {
            case 1: // 공격
            case 3: // 도망
                atk_window.SetActive(false);
                SetBDiceWindow();
                break;
            case 2: // 아이템
                inventory_window.SetActive(true);
                inventory_back.SetActive(true);
                atk_window.SetActive(false);
                break;
            default:
                Debug.Log(">> AtkTurn: Wrong Action");
                break;
        }
    }

    public void DefTurnAction(int action)
    {
        // 방어 행동 버튼에 붙여놓기
        pl_action = action;
        switch (action)
        {
            case 1: // 방어
            case 2: // 회피
            case 4: // 도망
                SetBDiceWindow();
                def_window.SetActive(false);
                break;
            case 3: // 아이템
                inventory_window.SetActive(true);
                inventory_back.SetActive(true);
                def_window.SetActive(false);
                break;
            default:
                Debug.Log(">> DefTurn: Wrong Action");
                break;
        }
    }

    void SetBDiceWindow()
    {
        bdice_window.SetActive(true);
        if (curr_turn == Turn.PL && pl_action == 1)
        {
            mobgroup.gameObject.SetActive(true);
        }
        else
        {
            mobgroup.gameObject.SetActive(false);
        }
        tens_dice.text = "00";
        ones_dice.text = "00";
        bonus_dice.text = "00";
    }

    public void BattleRollButton()
    {
        // 전투용 주사위 창 Roll 버튼에 붙여놓기
        if (curr_turn == Turn.PL && pl_action == 1)
        {
            Debug.Log(">> target setting");
            target = int.Parse(mobgroup.ActiveToggles().FirstOrDefault().transform.name);
            Debug.Log("target:" + target);
        }
        int ones = Random.Range(0, 10);
        int tens = Random.Range(0, 10);
        int pl_value = tens * 10 + ones;
        //행운 스탯은 모든 판정에 int(rand(0,행운)*0.5)만큼의 보정치를 더해준다.
        int luk_value = Random.Range(0, p_manager.GetStatAmount(StatType.Luck) / 2);

        ones_dice.text = ones.ToString();
        tens_dice.text = tens.ToString();
        bonus_dice.text = luk_value.ToString();

        // 텍스트박스에 결과값(결괏값 행운값 나눠서) 메시지 append
        AppendMsg(ScriptManager.script_manager.GetCharName() + " DICE>> " + pl_value.ToString() + " + " + luk_value.ToString() + "(Bonus)");
        EnemyAI(pl_value + luk_value);
    }

    void EnemyAI(int pl_dice)
    {
        int hp_sum = 0;
        int enm_dice = Random.Range(0, 100);
        int damage;
        int d_damage = 0;

        if ((curr_turn == Turn.ENM && pl_action == 4) || (curr_turn == Turn.PL && pl_action == 3))
        {
            // 도망
            AppendMsg("RESULT>> 도망 시도!!");
            int enm_dex = 0;
            for (int j = 0; j < m_num; j++)
            {  // 몬스터 민첩스탯 중 가장 큰 값 기준
                enm_dex = Mathf.Max(enm_dex, enm_stat[j].GetStatAmount(StatType.Dexterity));
            }

            int dex_diff = enm_dex - p_manager.GetStatAmount(StatType.Dexterity);
            if (dex_diff < 0 || pl_dice > dex_diff)
            {
                //도망 성공
                AppendMsg("RESULT>> 도망 성공!!");
                EndBattle(2);
                return;
            }
            else
            {
                // 도망 실패 메시지 append
                AppendMsg("RESULT>> 도망 실패!!");
                // 데미지 까임
                pl_dice = 0;
            }

        }

        if (curr_turn == Turn.ENM)
        {
            // 적 공격 턴
            for (int j = 0; j < m_num; j++)
            {
                enm_dice = Random.Range(0, 100);
                // 적 주사위 결과 메시지 append
                if (is_dead[j])
                {
                    continue;
                }
                AppendMsg("Enemy" + (j + 1).ToString() + " DICE>> " + enm_dice.ToString());
                if (pl_action == 1 && enm_dice >= 90 && pl_dice >= 90)
                {
                    // 방어 공격 크리 상쇄 메시지 append
                    AppendMsg("!! 크리티컬 상쇄 !!");
                    enm_dice = pl_dice = 50;
                }
                damage = CalcATKDamage("Enemy" + (j + 1).ToString(), enm_dice, enm_stat[j].GetStatAmount(StatType.Luck), enm_stat[j].GetStatAmount(StatType.Attack));
                switch (pl_action)
                {
                    case 1: // 방어
                        d_damage = CalcDEFDamage(ScriptManager.script_manager.GetCharName(), pl_dice, p_manager.GetStatAmount(StatType.Defence), damage);
                        break;
                    case 2: // 회피
                        d_damage = CalcDodgeDamage(ScriptManager.script_manager.GetCharName(), pl_dice, p_manager.GetStatAmount(StatType.Dexterity), damage);
                        break;
                    default:
                        break;

                }
                damage -= d_damage;
                // 데미지 로그 append
                AppendMsg("DAMAGE>> " + ScriptManager.script_manager.GetCharName() + " HP" + p_manager.GetStatAmount(StatType.Hp).ToString() + " -> " + (p_manager.GetStatAmount(StatType.Hp) - damage).ToString() + "\n");
                p_manager.SetStatAmount(StatType.Hp, p_manager.GetStatAmount(StatType.Hp) - damage);
            }
        }

        else
        {
            // 플레이어 공격 턴 - 타겟만 데미지 계산
            int enm_action = Random.Range(1, 3);

            if (enm_action == 1)
            {
                // 방어
                // 적 주사위 결과 메시지 append
                AppendMsg("Enemy" + (target + 1).ToString() + " DICE>> " + enm_dice.ToString());
                if (pl_action == 1 && enm_dice >= 90 && pl_dice >= 90)
                {
                    // 방어 공격 크리 상쇄 메시지 append
                    AppendMsg("!! 크리티컬 상쇄 !!");
                    enm_dice = pl_dice = 50;
                }
                damage = CalcATKDamage(ScriptManager.script_manager.GetCharName(), pl_dice, p_manager.GetStatAmount(StatType.Luck), p_manager.GetStatAmount(StatType.Attack) + PlayerStatManager.playerstat.wheapone);
                d_damage = CalcDEFDamage("Enemy" + (target + 1).ToString(), enm_dice, enm_stat[target].GetStatAmount(StatType.Defence), damage);

            }
            else
            {
                // 회피
                // 적 주사위 결과 메시지 append
                AppendMsg("Enemy" + (target + 1).ToString() + " DICE>> " + enm_dice.ToString());
                damage = CalcATKDamage(ScriptManager.script_manager.GetCharName(), pl_dice, p_manager.GetStatAmount(StatType.Luck), p_manager.GetStatAmount(StatType.Attack) + PlayerStatManager.playerstat.wheapone);
                d_damage = CalcDodgeDamage("Enemy" + (target + 1).ToString(), enm_dice, enm_stat[target].GetStatAmount(StatType.Dexterity), damage);

            }
            damage -= d_damage;
            AppendMsg("DAMAGE>> " + GetMobName(target) + " HP" + enm_stat[target].GetStatAmount(StatType.Hp).ToString() + " -> " + (enm_stat[target].GetStatAmount(StatType.Hp) - damage).ToString() + "\n");
            enm_stat[target].SetStatAmount(StatType.Hp, enm_stat[target].GetStatAmount(StatType.Hp) - damage);
        }

        for (int i = 0; i < m_num; i++)
        {
            int mob_HP = enm_stat[i].GetStatAmount(StatType.Hp);
            hp_sum += mob_HP;
            if (enm_stat[i].GetStatAmount(StatType.Hp) == 0 && !is_dead[i])
            {
                // 적 처치 메시지 출력
                AppendMsg("DEFEAT>> " + GetMobName(i) + " 처치!!");
                is_dead[i] = true;
            }
        }

        if (p_manager.GetStatAmount(StatType.Hp) == 0)
        {
            // 패배
            EndBattle(1);
            return;
        }
        else if (hp_sum == 0)
        {
            // 승리
            EndBattle(0);
            return;
        }
        else
        {
            SetNextTurn();
        }

    }

    public void SetNextTurn()
    {
        if (curr_turn == Turn.PL)
        {
            curr_turn = Turn.ENM;
            Invoke(nameof(SetDefenceTurn), 0.5f);
        }
        else
        {
            curr_turn = Turn.PL;
            Invoke(nameof(SetAttackTurn), 0.5f);
        }
    }

    public void BackButton(){
        inventory_window.gameObject.SetActive(false);
        bdice_window.gameObject.SetActive(false);

        if(curr_turn == Turn.ENM){
            def_window.gameObject.SetActive(true);
        }
        else{
            atk_window.gameObject.SetActive(true);
        }
    }


    int CalcATKDamage(string name, int dice, int luk_stat, int atk_stat){
        // 공격 데미지 계산 함수
        string result = "실패!";

        int damage = DAMAGE_BASE;   // 기본데미지
        int luk_dice = Random.Range(1, 100);
        if (dice <= 5)
        {    // 펌블
            result = "대실패!!";
            damage = 0;
        }
        if (dice >= 10)
        {   // 성공
            result = "성공!";
            damage += DAMAGE_COEF * atk_stat / 100;
            if (luk_dice < luk_stat)
            {  // 행운 보너스
                // 추가타 발생 메시지 append
                AppendMsg("!! 추가타 발생 !!");
                damage += DAMAGE_BASE;
            }
        }
        if (dice >= 90)
        {   // 크리티컬
            result = "대성공!!";
            damage *= 2;
        }

        // 공격 메시지 append
        AppendMsg(name + " | 공격>> " + result + "\t  공격량:" + damage.ToString());
        return damage;
    }

    int CalcDEFDamage(string name, int dice, int def_stat, int damage)
    {
        // 방어 데미지 계산 함수
        string result = "실패!\t방어량: 0";
        int d_damage = 0;   // 실패 데미지
        if (dice <= 5)
        {    // 펌블
            result = "대실패!!\t*받는 데미지 1.5배";
            d_damage -= damage / 2;
        }
        if (dice >= 10)
        {   // 성공
            d_damage += DAMAGE_COEF * def_stat / 100;
            result = "성공!\t방어량: " + d_damage.ToString();
        }
        if (dice >= 90 || d_damage > damage)
        {    // 크리
            result = "대성공!!\t*고정데미지 제외 무효화";
            d_damage = Mathf.Clamp(d_damage, 0, damage - DAMAGE_BASE);
        }
        // 방어 메시지 append
        AppendMsg(name + " | 방어>> " + result);
        return d_damage;
    }

    int CalcDodgeDamage(string name, int dice, int dex_stat, int damage)
    {
        // 회피 데미지 계산 함수
        int d_damage = 0;
        if (dice > (100 - dex_stat) * 2)
        {  // 회피 성공
            // 회피 성공 메시지 append
            AppendMsg(name + " | 회피>> 회피 성공!!\t*모든 데미지 무효화");
            d_damage = damage;
        }
        else
        {   // 회피 실패(방어펌블과 동일)
            // 회피 실패 메시지 append
            AppendMsg(name + " | 회피>> 회피 실패!!");
            d_damage = CalcDEFDamage(name, 0, 0, damage);
        }
        return d_damage;
    }

    void EndBattle(int result)
    {
        InventoryManager.i_manager.is_battle = false;
        string result_str;
        bool flag = false; // 목표 관련 이벤트인지 체크

        if(result==0){
            result_str = "WIN\n";
            Item map_item = ScriptManager.script_manager.GetCurrItem();
            Debug.Log("맵 아이템 이름:" + map_item.name);
            InventoryManager.i_manager.AddItem(map_item);
            ScriptManager.script_manager.SetPlaceClear(true);

            if (bType == BType.BOSS)
            {
                ScriptManager.script_manager.SetCurrGoalClear(true);
                flag = true;
            }

        }
        else if (result == 1)
        {
            result_str = "LOSE\n";
            flag = bType == BType.BOSS;
        }
        else
        {
            result_str = "RUN\n";
        }

        Debug.Log(">>Battle " + result_str);

        // 텍스트박스쪽에 전투 결과 메시지 append
        AppendMsg("\n<b>:: 전투 종료 ::</b>\nRESULT>> " + result_str);

        scene_inven_button.interactable = true;
        bdice_window.SetActive(true);
        mobgroup.gameObject.SetActive(true);
        if(bType  == BType.BOSS){
            mobgroup.gameObject.transform.GetChild(0).GetComponentInChildren<Text>().text = "Enemy1";
        }
        for (int k = 0; k < 5; k++)
        {
            mobgroup.gameObject.transform.GetChild(k).gameObject.SetActive(true);
        }

        mobgroup.gameObject.SetActive(false);
        bdice_window.SetActive(false);

        PlayerStatManager.playerstat.wheapone = 0;

        m_num = 0;
        atk_window.SetActive(false);
        def_window.SetActive(false);
        inventory_window.SetActive(false);
        battle_window.SetActive(false);

        if(flag){
            EndChapterModal.SetActive(true);
        }
    }

    string GetMobName(int idx){
        string mobname = "Enemy";
        if(idx == 0 && bType == BType.BOSS){
            mobname = boss_name;
        }
        else{
            mobname += (idx+1).ToString();
        }
        return mobname;
    }

    public void AppendMsg(string msg)
    {
        text_scroll.AppendMsg(msg,false);
    }
}
