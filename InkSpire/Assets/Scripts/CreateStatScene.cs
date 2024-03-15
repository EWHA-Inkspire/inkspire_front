using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Security.Cryptography;

public class CreateStatScene : MonoBehaviour
{
    [SerializeField] StatGraphTest testObj;
    [SerializeField] TMP_InputField atk;
    [SerializeField] TMP_InputField def;
    [SerializeField] TMP_InputField luk;
    [SerializeField] TMP_InputField intl;
    [SerializeField] TMP_InputField dex;

    public void SetAttack(){
        PlayerStatManager.playerstat.SetSingleStat("Attack",int.Parse(atk.text));
    }

    public void SetDefence(){
        PlayerStatManager.playerstat.SetSingleStat("Defence",int.Parse(def.text));
        testObj.ModalActivate();
    }

    public void SetLuck(){
        PlayerStatManager.playerstat.SetSingleStat("Luck",int.Parse(luk.text));
        testObj.ModalActivate();
    }

    public void SetMental(){
        PlayerStatManager.playerstat.SetSingleStat("Mental",int.Parse(intl.text));
        testObj.ModalActivate();

    }

    public void SetDexterity(){
        PlayerStatManager.playerstat.SetSingleStat("Dexterity",int.Parse(dex.text));
        testObj.ModalActivate();
    }
    public void SetCharacterStat(){
        
        PlayerStatManager.playerstat.atk = int.Parse(atk.text);
        PlayerStatManager.playerstat.def = int.Parse(def.text);
        PlayerStatManager.playerstat.luk = int.Parse(luk.text);
        PlayerStatManager.playerstat.mental = int.Parse(intl.text);
        PlayerStatManager.playerstat.dex = int.Parse(dex.text);
        PlayerStatManager.playerstat.p_stats = new Stats(PlayerStatManager.playerstat.luk,PlayerStatManager.playerstat.def,PlayerStatManager.playerstat.mental,PlayerStatManager.playerstat.dex,PlayerStatManager.playerstat.atk);
        
    }

    public void GameStartButton(){
        SetCharacterStat();
        // while(ScriptManager.scriptinfo.curr_chapter!=1){
        //     Debug.Log(">>Wating for GPT Response");
        // }
        //StartCoroutine(PostCharacterScriptInfo());
        SceneManager.LoadScene("Play");
    }

    IEnumerator PostCharacterScriptInfo(){
        WWWForm char_form = new WWWForm();
        char_form.AddField("userId",ScriptManager.scriptinfo.character_id);
        char_form.AddField("name",PlayerStatManager.playerstat.charname);
        char_form.AddField("luck",PlayerStatManager.playerstat.GetStatAmount("Luck"));
        char_form.AddField("defense",PlayerStatManager.playerstat.GetStatAmount("Defense"));
        char_form.AddField("mental",PlayerStatManager.playerstat.GetStatAmount("Mental"));
        char_form.AddField("agility",PlayerStatManager.playerstat.GetStatAmount("Dexterity"));
        char_form.AddField("attack",PlayerStatManager.playerstat.GetStatAmount("Attack"));
        char_form.AddField("agility",PlayerStatManager.playerstat.GetStatAmount("CurrHP"));

        UnityWebRequest www = UnityWebRequest.Post("http://3.35.61.61:8080/characters",char_form);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(">>CharacterInfo upload complete.");
        }

        WWWForm script_form = new WWWForm();
        script_form.AddField("userId",ScriptManager.scriptinfo.character_id);
        script_form.AddField("time",ScriptManager.scriptinfo.time_background);
        script_form.AddField("place",ScriptManager.scriptinfo.space_background);
        script_form.AddField("genre",ScriptManager.scriptinfo.genre);

        www = UnityWebRequest.Post("http://3.35.61.61:8080/scripts",script_form);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(">>ScriptInfo upload complete.");
        }
        
    }

}
