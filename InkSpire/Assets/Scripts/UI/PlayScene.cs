using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayScene : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI header_name;
    [SerializeField] TextMeshProUGUI header_HP;
    [SerializeField] TextMeshProUGUI title_chapnum;
    [SerializeField] TextMeshProUGUI battle_stat;
    [SerializeField] TextMeshProUGUI statmodal_title;
    [SerializeField] GameObject place_list;
    [SerializeField] Slider slider_HP;

    private ScriptManager s_manager = ScriptManager.script_manager;

    void Start(){

        Changechap(s_manager.GetCurrChap());
        header_name.text = s_manager.GetCharName()+" HP";
        statmodal_title.text = s_manager.GetCharName()+"'s Stats";
        header_HP.text = PlayerStatManager.playerstat.p_stats.GetStatAmount(StatType.Hp).ToString()+" / "+PlayerStatManager.playerstat.p_stats.GetStatAmount(StatType.MaxHP).ToString();
        battle_stat.text = "공격: "+PlayerStatManager.playerstat.p_stats.GetStatAmount(StatType.Attack).ToString()+" | 방어: "+PlayerStatManager.playerstat.p_stats.GetStatAmount(StatType.Defence).ToString()+" | 민첩: "+PlayerStatManager.playerstat.p_stats.GetStatAmount(StatType.Dexterity).ToString()+" | 행운: "+PlayerStatManager.playerstat.p_stats.GetStatAmount(StatType.Luck).ToString();

        place_list.transform.GetChild(12).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Start Point\n"+s_manager.GetPlace(0).place_name;
        for (int i = 0; i<14; i++){
            if(i==12){
                continue;
            }
            if(i/3 == 0){
                place_list.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = s_manager.GetPlace(i+1).place_name;
            }
            else{
                place_list.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                place_list.transform.GetChild(i).GetComponent<Button>().interactable = false;
            }
            
        }
    }

    void Update(){
        slider_HP.value=(float)PlayerStatManager.playerstat.p_stats.GetStatAmountNormalized(StatType.Hp);
        header_HP.text = PlayerStatManager.playerstat.p_stats.GetStatAmount(StatType.Hp).ToString()+" / "+PlayerStatManager.playerstat.p_stats.GetStatAmount(StatType.MaxHP).ToString();
    }

    public void Changechap(int int_chapnum){
        string rome_chapnum;
        switch(int_chapnum){
            case 0:
                rome_chapnum = "I";
                break;
            case 1:
                rome_chapnum = "II";
                break;
            case 2:
                rome_chapnum = "III";
                break;
            case 3:
                rome_chapnum = "IV";
                break;
            case 4:
                rome_chapnum = "V";
                break;
            default:
                rome_chapnum = "NULL";
                break;
        }
        title_chapnum.text = "Chapter "+rome_chapnum;
    }
}
