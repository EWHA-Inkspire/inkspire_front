using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class ScriptManager : MonoBehaviour
{
    public static ScriptManager script_manager;
    private int character_id = 0; // 추후 API 호출 결과 값으로 변경
    private string char_name;
    private int curr_chapter = 0;
    
    // 플레이어의 입력값
    [SerializeField] TMP_InputField input_name;
    [SerializeField] TMP_InputField time_background;
    [SerializeField] TMP_InputField space_background;
    [SerializeField] ToggleGroup genregroup;

    private Script script;
    private Goals[] chapter_obj = new Goals[5];
    private Place[] map = new Place[14];
    private Npc pro_npc;
    private Npc anta_npc;

    void Awake()
    {
        // 씬이 바뀔 때 파괴되지 않음
        DontDestroyOnLoad(this.gameObject);

        if (script_manager == null) {
            script_manager = this;
        }
    }

    // 기본 틀 생성
    public void SetScriptInfo()
    {
        char_name = input_name.text;
        // 스크립트 기본 틀 생성 (세계관, 인트로)
        script = new Script(char_name, GetGenre(), time_background.text, space_background.text);
        // 목표 생성

        // npc 정보 생성
        pro_npc = new Npc("P", script.getWorldDetail(), script.getGenre());
        anta_npc = new Npc("A", script.getWorldDetail(), script.getGenre());

        // 맵 정보 생성 -> 아이템 & 이벤트 트리거

        script.IntroGpt(pro_npc, anta_npc, map, char_name);
    }


    // 장르 그룹에서 장르 텍스트 뽑아오기
    private string GetGenre()
    {
        string[] strlist = genregroup.ActiveToggles().FirstOrDefault().GetComponentInChildren<Text>().text.Split("#");
        return (strlist[1]);
    }

    // map의 이벤트 타입 설정 (3개 장소마다 목표 이벤트 출현 장소 정하는 로직)

}