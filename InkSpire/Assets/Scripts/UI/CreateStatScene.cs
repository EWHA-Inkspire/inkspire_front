using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

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
        //StartCoroutine(PostCharacterScriptInfo());
        LoadingText.text = "게임을 생성중입니다";
        WaitForGPT();

        //StartCoroutine(PostObjectiveInfo());
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
            Debug.Log(ScriptManager.script_manager.GetScript().GetIntro());
            Debug.Log(ScriptManager.script_manager.GetCurrChap());
            LoadingPannel.SetActive(false);
            SceneManager.LoadScene("5_Play");
        }
    }

    IEnumerator PostCharacterScriptInfo()
    {
        WWWForm char_form = new WWWForm();
        //char_form.AddField("userId", ScriptManager.scriptinfo.character_id);
        char_form.AddField("name", ScriptManager.script_manager.GetCharName());
        char_form.AddField("luck", stat_manager.p_stats.GetStatAmount(StatType.Luck));
        char_form.AddField("defense", stat_manager.p_stats.GetStatAmount(StatType.Defence));
        char_form.AddField("mental", stat_manager.p_stats.GetStatAmount(StatType.Mental));
        char_form.AddField("agility", stat_manager.p_stats.GetStatAmount(StatType.Dexterity));
        char_form.AddField("attack", stat_manager.p_stats.GetStatAmount(StatType.Attack));
        char_form.AddField("agility", stat_manager.p_stats.GetStatAmount(StatType.Hp));

        UnityWebRequest www = UnityWebRequest.Post("http://3.38.126.43:8080/characters", char_form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(">>CharacterInfo upload complete.");
        }

        WWWForm script_form = new WWWForm();
        // script_form.AddField("userId", ScriptManager.scriptinfo.character_id);
        // script_form.AddField("time", ScriptManager.scriptinfo.time_background);
        // script_form.AddField("place", ScriptManager.scriptinfo.space_background);
        // script_form.AddField("genre", ScriptManager.scriptinfo.genre);

        www = UnityWebRequest.Post("http://3.38.126.43:8080/scripts", script_form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(">>ScriptInfo upload complete.");
        }

    }

    // IEnumerator PostObjectiveInfo()
    // {
    //     // WWWForm obj_form = new WWWForm();
    //     // obj_form.AddField("scriptId", ScriptManager.scriptinfo.character_id);
    //     // obj_form.AddField("chapter", 5);
    //     // obj_form.AddField("content", ScriptManager.scriptinfo.chapter_obj[5].title);
    //     // obj_form.AddField("type", ScriptManager.scriptinfo.chapter_obj[5].type);
    //     // obj_form.AddField("require", ScriptManager.scriptinfo.chapter_obj[5].detail);
    //     // obj_form.AddField("etc", ScriptManager.scriptinfo.chapter_obj[5].etc);

    //     // UnityWebRequest www = UnityWebRequest.Post("http://3.38.126.43:8080/scripts/goal", obj_form);
    //     // yield return www.SendWebRequest();

    //     // WWWForm chap_form = new WWWForm();
    //     // chap_form.AddField("scriptId", ScriptManager.scriptinfo.character_id);
    //     // chap_form.AddField("chapter", 5);
    //     // chap_form.AddField("content", ScriptManager.scriptinfo.chapter_obj[5].title);
    //     // chap_form.AddField("type", ScriptManager.scriptinfo.chapter_obj[5].type);
    //     // chap_form.AddField("require", ScriptManager.scriptinfo.chapter_obj[5].detail);
    //     // chap_form.AddField("etc", ScriptManager.scriptinfo.chapter_obj[5].etc);

    //     // www = UnityWebRequest.Post("http://3.38.126.43:8080/scripts/goal", obj_form);
    //     // yield return www.SendWebRequest();
    // }
}
