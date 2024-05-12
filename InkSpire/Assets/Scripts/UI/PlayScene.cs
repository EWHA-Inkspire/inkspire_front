using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayScene : MonoBehaviour
{
    public static PlayScene play_scene;
    [SerializeField] TextMeshProUGUI header_name;
    [SerializeField] TextMeshProUGUI header_HP;
    [SerializeField] TextMeshProUGUI title_chapnum;
    [SerializeField] TextMeshProUGUI battle_stat;
    [SerializeField] TextMeshProUGUI statmodal_title;
    [SerializeField] GameObject place_list;
    [SerializeField] Slider slider_HP;

    private readonly ScriptManager s_manager = ScriptManager.script_manager;

    public void Start(){

        Changechap(s_manager.GetCurrChap());
        header_name.text = s_manager.GetCharName()+" HP";
        statmodal_title.text = s_manager.GetCharName()+"'s Stats";
        header_HP.text = PlayerStatManager.playerstat.GetStatAmount(StatType.Hp).ToString()+" / "+PlayerStatManager.playerstat.GetStatAmount(StatType.MaxHP).ToString();
        battle_stat.text = "공격: "+PlayerStatManager.playerstat.GetStatAmount(StatType.Attack).ToString()+" | 방어: "+PlayerStatManager.playerstat.GetStatAmount(StatType.Defence).ToString()+" | 민첩: "+PlayerStatManager.playerstat.GetStatAmount(StatType.Dexterity).ToString()+" | 행운: "+PlayerStatManager.playerstat.GetStatAmount(StatType.Luck).ToString();

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
        slider_HP.value=(float)PlayerStatManager.playerstat.GetStatAmountNormalized(StatType.Hp);
        header_HP.text = PlayerStatManager.playerstat.GetStatAmount(StatType.Hp).ToString()+" / "+PlayerStatManager.playerstat.GetStatAmount(StatType.MaxHP).ToString();
    }

    public void Changechap(int int_chapnum){
        string rome_chapnum = int_chapnum switch
        {
            0 => "I",
            1 => "II",
            2 => "III",
            3 => "IV",
            4 => "V",
            _ => "NULL",
        };
        title_chapnum.text = "Chapter "+rome_chapnum;
    }
}
