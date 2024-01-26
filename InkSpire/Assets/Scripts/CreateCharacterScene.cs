using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreateCharacterScene : MonoBehaviour
{
    [SerializeField] TMP_InputField charname;
    [SerializeField] TMP_InputField timebackground;
    [SerializeField] TMP_InputField spacebackground;

    public void SetCharacterInfo(){
        PlayerStatManager.playerstat.charname = charname.text;
        PlayerStatManager.playerstat.time_background = timebackground.text;
        PlayerStatManager.playerstat.space_background = spacebackground.text;
        SceneManager.LoadScene("CreateStat");
    }
}
