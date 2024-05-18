using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;

public class CreateStatScene : MonoBehaviour
{
    [SerializeField] GameObject LoadingPannel;
    [SerializeField] TextMeshProUGUI LoadingText;
    [SerializeField] TextMeshProUGUI warning;
    [SerializeField] StatModalButton testObj;
    [SerializeField] TMP_InputField atk;
    [SerializeField] TMP_InputField def;
    [SerializeField] TMP_InputField luk;
    [SerializeField] TMP_InputField intl;
    [SerializeField] TMP_InputField dex;

    private PlayerStatManager stat_manager = PlayerStatManager.playerstat;

    public void SetAttack()
    {
        stat_manager.SetStatAmount(StatType.Attack, int.Parse(atk.text));
        testObj.ModalActivate();
    }

    public void SetDefence()
    {
        stat_manager.SetStatAmount(StatType.Defence, int.Parse(def.text));
        testObj.ModalActivate();
    }

    public void SetLuck()
    {
        stat_manager.SetStatAmount(StatType.Luck, int.Parse(luk.text));
        testObj.ModalActivate();
    }

    public void SetMental()
    {
        stat_manager.SetStatAmount(StatType.Mental, int.Parse(intl.text));
        testObj.ModalActivate();

    }

    public void SetDexterity()
    {
        stat_manager.SetStatAmount(StatType.Dexterity, int.Parse(dex.text));
        testObj.ModalActivate();
    }
    public void SetCharacterStat()
    {
        stat_manager.SetStats(int.Parse(luk.text), int.Parse(def.text), int.Parse(intl.text), int.Parse(dex.text), int.Parse(atk.text));
    }

    public void SetRandomStat(){
        int[] tmp_stat = new int [5];
        int stat_sum = 400;
        while(stat_sum>300){
            stat_sum = 0;
            for(int i = 0; i<5; i++){
                tmp_stat[i] =  Random.Range(1, 100);
                stat_sum += tmp_stat[i];
            }
        }
        Debug.Log("stat sum:"+stat_sum);

        atk.text = tmp_stat[0].ToString();
        luk.text = tmp_stat[1].ToString();
        def.text = tmp_stat[2].ToString();
        dex.text = tmp_stat[3].ToString();
        intl.text = tmp_stat[4].ToString();
        SetAttack();
        SetLuck();
        SetDefence();
        SetDexterity();
        SetMental();
    }
    public void GameStartButton()
    {
        if (int.Parse(luk.text) + int.Parse(def.text) + int.Parse(intl.text) + int.Parse(dex.text) + int.Parse(atk.text) > 300)
        {
            warning.text = "Stat의 총합은 300을 넘을 수 없습니다!";
        }
        else
        {
            warning.text = "";
            SetCharacterStat();
            LoadingText.text = "게임을 생성중입니다";
            StartCoroutine(WaitForGPT());
        }
    }

    IEnumerator WaitForGPT()
    {
        if (!LoadingPannel.activeSelf)
        {
            LoadingPannel.SetActive(true);
        }

        while (!ScriptManager.script_manager.GetInitScript())
        {
            if (LoadingText.text == "게임을 생성중입니다 . . .")
            {
                LoadingText.text = "게임을 생성중입니다";
            }
            else
            {
                LoadingText.text += " .";
            }
            yield return new WaitForSeconds(1f);
        }

        PlayAPI.play_api.UpdateCharacterStat(stat_manager.GetStats());

        // API 호출 - PNPC, ANPC 정보 저장
        List<Place> map = ScriptManager.script_manager.GetMap();
        Npc pro_npc = ScriptManager.script_manager.GetPnpc();
        Npc anta_npc = ScriptManager.script_manager.GetAnpc();
        ScriptAPI.script_api.PostNpcInfo(map, pro_npc, anta_npc);

        LoadingPannel.SetActive(false);
        SceneManager.LoadScene("5_Play");
    }
}
