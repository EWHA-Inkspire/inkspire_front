using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class CreateCharacterScene : MonoBehaviour
{
    [SerializeField] TMP_InputField charname;
    [SerializeField] TMP_InputField timebackground;
    [SerializeField] TMP_InputField spacebackground;

    [SerializeField] ToggleGroup genregroup;

    void Awake()
    {
        PlayerPrefs.DeleteKey("script_id");
        PlayerPrefs.DeleteKey("character_id");
    }

    public string GetGenre()
    {
        string[] strlist = genregroup.ActiveToggles().FirstOrDefault().GetComponentInChildren<Text>().text.Split("#");
        return strlist[1];
    }

    public void SetCharacterInfo()
    {
        ScriptManager.script_manager.SetScriptInfo(charname.text, GetGenre(), timebackground.text, spacebackground.text);
        ScriptManager.script_manager.SetCurrChap(0);
        SceneManager.LoadScene("4_CreateStat");
    }
}
