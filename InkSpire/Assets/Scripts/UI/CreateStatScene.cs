using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CreateStatScene : MonoBehaviour
{
    [SerializeField] GameObject LoadingPannel;
    [SerializeField] TextMeshProUGUI LoadingText;
    [SerializeField] StatModalButton testObj;
    [SerializeField] TMP_InputField atk;
    [SerializeField] TMP_InputField def;
    [SerializeField] TMP_InputField luk;
    [SerializeField] TMP_InputField intl;
    [SerializeField] TMP_InputField dex;

    private PlayerStatManager stat_manager = PlayerStatManager.playerstat;

    public void SetAttack()
    {
        stat_manager.p_stats.SetStatAmount(StatType.Attack, int.Parse(atk.text));
        testObj.ModalActivate();
    }

    public void SetDefence()
    {
        stat_manager.p_stats.SetStatAmount(StatType.Defence, int.Parse(def.text));
        testObj.ModalActivate();
    }

    public void SetLuck()
    {
        stat_manager.p_stats.SetStatAmount(StatType.Luck, int.Parse(luk.text));
        testObj.ModalActivate();
    }

    public void SetMental()
    {
        stat_manager.p_stats.SetStatAmount(StatType.Mental, int.Parse(intl.text));
        testObj.ModalActivate();

    }

    public void SetDexterity()
    {
        stat_manager.p_stats.SetStatAmount(StatType.Dexterity, int.Parse(dex.text));
        testObj.ModalActivate();
    }
    public void SetCharacterStat()
    {
        stat_manager.p_stats = new Stats(int.Parse(luk.text), int.Parse(def.text), int.Parse(intl.text), int.Parse(dex.text), int.Parse(atk.text));
    }

    public void GameStartButton()
    {
        SetCharacterStat();
        LoadingText.text = "게임을 생성중입니다";
        WaitForGPT();
    }

    void WaitForGPT()
    {
        if (!LoadingPannel.activeSelf)
        {
            LoadingPannel.SetActive(true);
        }
        if (LoadingText.text == "게임을 생성중입니다 . . .")
        {
            LoadingText.text = "게임을 생성중입니다";
        }
        else
        {
            LoadingText.text += " .";
        }
        if (!ScriptManager.script_manager.GetInitScript())
        {
            Invoke(nameof(WaitForGPT), 1f);
        }
        else
        {
            int character_id = PlayerPrefs.GetInt("character_id");
            CharacterStatInfo stat_info = new(stat_manager.p_stats.GetStatAmount(StatType.Attack), stat_manager.p_stats.GetStatAmount(StatType.Defence), stat_manager.p_stats.GetStatAmount(StatType.Luck), stat_manager.p_stats.GetStatAmount(StatType.Mental), stat_manager.p_stats.GetStatAmount(StatType.Dexterity), stat_manager.p_stats.GetStatAmount(StatType.Hp));
            string json = JsonUtility.ToJson(stat_info);
            StartCoroutine(APIManager.api.PutRequest<Null>("/characters/"+character_id+"/stat", json, (response) => {
                Debug.Log(response.success);
            }));

            // API 호출 - PNPC, ANPC 정보 저장
            List<Place> map = ScriptManager.script_manager.GetMap();
            Npc pro_npc = ScriptManager.script_manager.GetPnpc();
            Npc anta_npc = ScriptManager.script_manager.GetAnpc();
            ScriptAPI.script_api.PostNpcInfo(map, pro_npc, anta_npc);

            LoadingPannel.SetActive(false);
            SceneManager.LoadScene("5_Play");
        }
    }
}
