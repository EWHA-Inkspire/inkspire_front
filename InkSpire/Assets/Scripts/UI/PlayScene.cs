using System.Collections.Generic;
using OpenAI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayScene : MonoBehaviour
{
    public static PlayScene play_scene;
    [SerializeField] private Play play;
    [SerializeField] TextMeshProUGUI header_name;
    [SerializeField] TextMeshProUGUI header_HP;
    [SerializeField] TextMeshProUGUI title_chapnum;
    [SerializeField] TextMeshProUGUI battle_stat;
    [SerializeField] TextMeshProUGUI statmodal_title;
    [SerializeField] GameObject place_list;
    [SerializeField] Slider slider_HP;
    [SerializeField] GameObject loading_win;
    [SerializeField] TextMeshProUGUI loading_text;

    [SerializeField] GameObject[]   disable_set;

    private bool is_loading = false;
    private int view_idx;

    private ScriptManager s_manager = ScriptManager.script_manager;

    void Awake(){
        play_scene = this;
    }

    void Start(){
        view_idx = ScriptManager.script_manager.GetViewChap();
        LoadPlayScene();
    }

    public void LoadNextChapUI(){
        is_loading = true;
        loading_win.gameObject.SetActive(true);
        loading_text.text = "챕터를 생성중입니다";
        WaitForGPT();
    }

    void WaitForGPT()
    {
        if (loading_text.text == "챕터를 생성중입니다 . . .")
        {
            loading_text.text = "챕터를 생성중입니다";
        }
        else
        {
            loading_text.text += " .";
        }
        if (is_loading)
        {
            Invoke(nameof(WaitForGPT), 1f);
        }
        else{
            loading_win.gameObject.SetActive(false);
        }
    }

    public void LoadPlayScene(){
        s_manager = ScriptManager.script_manager;
        Changechap(view_idx);
        header_name.text = s_manager.GetCharName()+" HP";
        statmodal_title.text = s_manager.GetCharName()+"'s Stats";
        header_HP.text = PlayerStatManager.playerstat.GetStatAmount(StatType.Hp).ToString()+" / "+PlayerStatManager.playerstat.GetStatAmount(StatType.MaxHP).ToString();
        battle_stat.text = "공격: "+PlayerStatManager.playerstat.GetStatAmount(StatType.Attack).ToString()+" | 방어: "+PlayerStatManager.playerstat.GetStatAmount(StatType.Defence).ToString()+" | 민첩: "+PlayerStatManager.playerstat.GetStatAmount(StatType.Dexterity).ToString()+" | 행운: "+PlayerStatManager.playerstat.GetStatAmount(StatType.Luck).ToString();   
        LoadChapter(view_idx, true);
    }

    public void LoadChapter(int idx, bool is_new){
        view_idx = idx;
        ScriptManager.script_manager.SetViewChap(view_idx);
        Changechap(idx);
        // 장소 모달 버튼 셋팅
        place_list.transform.GetChild(12).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Start Point\n"+s_manager.GetPlace(0).place_name;
        for (int i = 0; i<14; i++){
            if(ScriptManager.script_manager.GetCurrChap() == 2 && i == 13)
            {
                place_list.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = s_manager.GetPlace(7).place_name;
            }
            else if(i == 12)
            {
                continue;
            }
            else if(i >= 8)
            {
                place_list.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                place_list.transform.GetChild(i).GetComponent<Button>().interactable = false;
            }
            else if(i/3 == idx)
            {
                place_list.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = s_manager.GetPlace(i+1).place_name;
                if(idx!=ScriptManager.script_manager.GetCurrChap()){
                    place_list.transform.GetChild(i).GetComponent<Button>().interactable = false;
                }
            }
            else
            {
                place_list.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                place_list.transform.GetChild(i).GetComponent<Button>().interactable = false;
            }
        }
        // 텍스트박스 및 버튼 비활성화
        if(idx != ScriptManager.script_manager.GetCurrChap()){
            for(int i = 0; i<disable_set.Length;i++){
                if(disable_set[i].GetComponent<Button>()!=null){
                    disable_set[i].GetComponent<Button>().interactable = false;
                }
                else if(disable_set[i].GetComponent<TMP_InputField>()!=null){
                    disable_set[i].GetComponent<TMP_InputField>().interactable = false;
                }
            }
        }
        else{
            for(int i = 0; i<disable_set.Length;i++){
                if(disable_set[i].GetComponent<Button>()!=null){
                    disable_set[i].GetComponent<Button>().interactable = true;
                }
                else if(disable_set[i].GetComponent<TMP_InputField>()!=null){
                    disable_set[i].GetComponent<TMP_InputField>().interactable = true;
                }
            }
        }
        if(!is_new)
        {
            play.InitMessages(new List<ChatMessage>());
            play.text_scroll.InitStoryObj(new List<ChatMessage>());
            PlayAPI.play_api.GetChatList(play);
        }
        is_loading = false;
    }

    public void ViewPrevChapButton(){
        if(view_idx<=0){
            return;
        }
        LoadNextChapUI();
        LoadChapter(view_idx-1, false);
    }

    public void ViewNextChapButton(){
        if(view_idx>=ScriptManager.script_manager.GetCurrChap()){
            return;
        }
        LoadNextChapUI();
        LoadChapter(view_idx+1, false);
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