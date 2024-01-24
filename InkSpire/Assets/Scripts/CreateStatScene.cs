using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CreateStatScene : MonoBehaviour
{
    [SerializeField] TMP_InputField atk;
    [SerializeField] TMP_InputField def;
    [SerializeField] TMP_InputField luk;
    [SerializeField] TMP_InputField intl;
    [SerializeField] TMP_InputField dex;

    public void SetCharacterStat(){
        PlayerStatManager.playerstat.atk = int.Parse(atk.text);
        PlayerStatManager.playerstat.def = int.Parse(def.text);
        PlayerStatManager.playerstat.luk = int.Parse(luk.text);
        PlayerStatManager.playerstat.intl = int.Parse(intl.text);
        PlayerStatManager.playerstat.dex = int.Parse(dex.text);
        PlayerStatManager.playerstat.p_stats = new Stats(PlayerStatManager.playerstat.luk,PlayerStatManager.playerstat.def,PlayerStatManager.playerstat.intl,PlayerStatManager.playerstat.dex,PlayerStatManager.playerstat.atk);
        SceneManager.LoadScene("Play");
    }
}
