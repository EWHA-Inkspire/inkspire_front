using System.Collections;
using System.Collections.Generic;
using OpenAI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] GameObject generating_win;
    [SerializeField] TextMeshProUGUI generating_text;

    [SerializeField] GameObject[]   disable_set;

    private bool is_loading = false;
    private int view_idx;

    private ScriptManager s_manager = ScriptManager.script_manager;

    void Awake(){
        play_scene = this;
    }

    void Start(){
        view_idx = s_manager.GetViewChap();
        LoadPlayScene();
    }

    public void LoadNextChapUI(){
        is_loading = true;
        loading_win.gameObject.SetActive(true);
        play.InitMessages(new List<ChatMessage>());
        StartCoroutine(WaitForGPT(loading_win, loading_text, "챕터를 생성중입니다"));
    }

    public void GenerateGPT(){
        is_loading = true;
        generating_win.gameObject.SetActive(true);
        StartCoroutine(WaitForGPT(generating_win, generating_text, "대화를 생성중입니다"));
    }

    IEnumerator WaitForGPT(GameObject win, TextMeshProUGUI win_text, string origin_text)
    {
        win_text.text = origin_text;
        while (is_loading)
        {
            if (win_text.text == origin_text + " . . .")
            {
                win_text.text = origin_text;
            }
            else
            {
                win_text.text += " .";
            }
            yield return new WaitForSeconds(1f);
        }
        win.gameObject.SetActive(false);
    }

    public void LoadPlayScene(){
        Changechap(view_idx);
        header_name.text = s_manager.GetCharName()+" HP";
        statmodal_title.text = s_manager.GetCharName()+"'s Stats";
        header_HP.text = PlayerStatManager.playerstat.GetStatAmount(StatType.Hp).ToString()+" / "+PlayerStatManager.playerstat.GetStatAmount(StatType.MaxHP).ToString();
        battle_stat.text = "공격: "+PlayerStatManager.playerstat.GetStatAmount(StatType.Attack).ToString()+" | 방어: "+PlayerStatManager.playerstat.GetStatAmount(StatType.Defence).ToString()+" | 민첩: "+PlayerStatManager.playerstat.GetStatAmount(StatType.Dexterity).ToString()+" | 행운: "+PlayerStatManager.playerstat.GetStatAmount(StatType.Luck).ToString();   
        LoadChapter(view_idx, true);
    }

    public void LoadChapter(int idx, bool is_new){
        view_idx = idx;
        s_manager.SetViewChap(view_idx);
        Changechap(idx);
    
        // 장소 모달 버튼 셋팅
        for (int i = 0; i < Const.PLACE_COUNT; i++){
            if(s_manager.GetCurrChap() == Const.CHAPTER-1 && i == 13)
            {
                place_list.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = s_manager.GetPlace(7).place_name;
            }
            else if(i == 12)
            {
                place_list.transform.GetChild(12).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Start Point\n"+s_manager.GetPlace(0).place_name;
            }
            else if(i >= Const.PLACE_COUNT-2)
            {
                place_list.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                place_list.transform.GetChild(i).GetComponent<Button>().interactable = false;
            }
            else if(i/3 == idx)
            {
                place_list.transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = s_manager.GetPlace(i+1).place_name;
                if(idx!=s_manager.GetCurrChap()){
                    Debug.Log(place_list.transform.GetChild(i).GetComponent<Button>()==null);
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
        if(idx != s_manager.GetCurrChap()){
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
                if(disable_set[i].GetComponent<Button>()!=null&&(i/3==idx||i>13)){
                    disable_set[i].GetComponent<Button>().interactable = true;
                }
                else if(disable_set[i].GetComponent<TMP_InputField>()!=null){
                    disable_set[i].GetComponent<TMP_InputField>().interactable = true;
                }
            }
        }
        if(!is_new)
        {
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
        if(view_idx>=s_manager.GetCurrChap()){
            return;
        }
        LoadNextChapUI();
        LoadChapter(view_idx+1, false);
    }

    void Update(){
        slider_HP.value=(float)PlayerStatManager.playerstat.GetStatAmountNormalized(StatType.Hp);
        header_HP.text = PlayerStatManager.playerstat.GetStatAmount(StatType.Hp).ToString()+" / "+PlayerStatManager.playerstat.GetStatAmount(StatType.MaxHP).ToString();
        
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene("2_CharacterList");
            }
        }
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

    public void SetIsLoading(bool is_loading){
        this.is_loading = is_loading;
    }

}