using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Unity.VisualScripting;

public class CreateStatScene : MonoBehaviour
{
    [SerializeField] GameObject LoadingPannel;
    [SerializeField] TextMeshProUGUI LoadingText;
    [SerializeField] StatGraphTest testObj;
    [SerializeField] TMP_InputField atk;
    [SerializeField] TMP_InputField def;
    [SerializeField] TMP_InputField luk;
    [SerializeField] TMP_InputField intl;
    [SerializeField] TMP_InputField dex;

    public void SetAttack()
    {
        PlayerStatManager.playerstat.p_stats.SetStatAmount(Stats.Type.Attack, int.Parse(atk.text));
    }

    public void SetDefence()
    {
        PlayerStatManager.playerstat.p_stats.SetStatAmount(Stats.Type.Defence, int.Parse(def.text));
        testObj.ModalActivate();
    }

    public void SetLuck()
    {
        PlayerStatManager.playerstat.p_stats.SetStatAmount(Stats.Type.Luck, int.Parse(luk.text));
        testObj.ModalActivate();
    }

    public void SetMental()
    {
        PlayerStatManager.playerstat.p_stats.SetStatAmount(Stats.Type.Mental, int.Parse(intl.text));
        testObj.ModalActivate();

    }

    public void SetDexterity()
    {
        PlayerStatManager.playerstat.p_stats.SetStatAmount(Stats.Type.Dexterity, int.Parse(dex.text));
        testObj.ModalActivate();
    }
    public void SetCharacterStat()
    {

        PlayerStatManager.playerstat.atk = int.Parse(atk.text);
        PlayerStatManager.playerstat.def = int.Parse(def.text);
        PlayerStatManager.playerstat.luk = int.Parse(luk.text);
        PlayerStatManager.playerstat.mental = int.Parse(intl.text);
        PlayerStatManager.playerstat.dex = int.Parse(dex.text);
        PlayerStatManager.playerstat.p_stats = new Stats(PlayerStatManager.playerstat.luk, PlayerStatManager.playerstat.def, PlayerStatManager.playerstat.mental, PlayerStatManager.playerstat.dex, PlayerStatManager.playerstat.atk);

    }

    public void GameStartButton()
    {
        SetCharacterStat();
        //StartCoroutine(PostCharacterScriptInfo());
        LoadingText.text = "게임을 생성중입니다";
        SceneManager.LoadScene("5_Play");
        WaitForGPT();

        //StartCoroutine(PostObjectiveInfo());
    }

    void WaitForGPT()
    {
        if (!LoadingPannel.gameObject.activeSelf)
        {
            LoadingPannel.gameObject.SetActive(true);
        }
        if (LoadingText.text == "게임을 생성중입니다 . . .")
        {
            LoadingText.text = "게임을 생성중입니다";
        }
        else
        {
            LoadingText.text += " .";
        }
        if (ScriptManager.scriptinfo.intro_string == "placeholder" || ScriptManager.scriptinfo.intro_string == "" || ScriptManager.scriptinfo.curr_chapter != 1 || !MapManager.mapinfo.is_drawmap)
        {
            Debug.Log(">>Wating for GPT Response");
            //Debug.Log(ScriptManager.scriptinfo.intro_string);
            //Debug.Log(ScriptManager.scriptinfo.curr_chapter);
            Invoke("WaitForGPT", 1f);
        }
        else
        {
            Debug.Log(ScriptManager.scriptinfo.intro_string);
            Debug.Log(ScriptManager.scriptinfo.curr_chapter);
            LoadingPannel.gameObject.SetActive(false);
            SceneManager.LoadScene("5_Play");
        }
    }

    IEnumerator PostCharacterScriptInfo()
    {
        WWWForm char_form = new WWWForm();
        char_form.AddField("userId", ScriptManager.scriptinfo.character_id);
        char_form.AddField("name", PlayerStatManager.playerstat.charname);
        char_form.AddField("luck", PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.Luck));
        char_form.AddField("defense", PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.Defence));
        char_form.AddField("mental", PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.Mental));
        char_form.AddField("agility", PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.Dexterity));
        char_form.AddField("attack", PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.Attack));
        char_form.AddField("agility", PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.CurrHP));

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
        script_form.AddField("userId", ScriptManager.scriptinfo.character_id);
        script_form.AddField("time", ScriptManager.scriptinfo.time_background);
        script_form.AddField("place", ScriptManager.scriptinfo.space_background);
        script_form.AddField("genre", ScriptManager.scriptinfo.genre);

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

    IEnumerator PostObjectiveInfo()
    {
        WWWForm obj_form = new WWWForm();
        obj_form.AddField("scriptId", ScriptManager.scriptinfo.character_id);
        obj_form.AddField("chapter", 5);
        obj_form.AddField("content", ScriptManager.scriptinfo.chapter_obj[5].title);
        obj_form.AddField("type", ScriptManager.scriptinfo.chapter_obj[5].type);
        obj_form.AddField("require", ScriptManager.scriptinfo.chapter_obj[5].detail);
        obj_form.AddField("etc", ScriptManager.scriptinfo.chapter_obj[5].etc);

        UnityWebRequest www = UnityWebRequest.Post("http://3.38.126.43:8080/scripts/goal", obj_form);
        yield return www.SendWebRequest();

        WWWForm chap_form = new WWWForm();
        chap_form.AddField("scriptId", ScriptManager.scriptinfo.character_id);
        chap_form.AddField("chapter", 5);
        chap_form.AddField("content", ScriptManager.scriptinfo.chapter_obj[5].title);
        chap_form.AddField("type", ScriptManager.scriptinfo.chapter_obj[5].type);
        chap_form.AddField("require", ScriptManager.scriptinfo.chapter_obj[5].detail);
        chap_form.AddField("etc", ScriptManager.scriptinfo.chapter_obj[5].etc);

        www = UnityWebRequest.Post("http://3.38.126.43:8080/scripts/goal", obj_form);
        yield return www.SendWebRequest();
    }
}
