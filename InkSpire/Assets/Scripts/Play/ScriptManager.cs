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
    private int char_id = 0; // 추후 API 호출 결과 값으로 변경
    private string char_name;
    private int curr_chapter = 0;
    private int curr_place = 0;

    private Script script;
    private Goals[] chapter_obj = new Goals[5];
    private Place[] map = new Place[14];
    private string[] place_names = new string[14];
    private Npc pro_npc;
    private Npc anta_npc;



// 일반함수
    void Awake()
    {
        // 씬이 바뀔 때 파괴되지 않음
        DontDestroyOnLoad(this.gameObject);

        if (script_manager == null) {
            script_manager = this;
        }
    }

    // 초기 스크립트 틀 생성 (장소 4개)
    public void SetScriptInfo(string char_name, string genre, string time_background, string space_background)
    {
        this.char_name = char_name;
        // 세계관, 인트로 생성
        script = new Script(genre, time_background, space_background);

        // 목표 생성

        // npc 정보 생성
        pro_npc = new Npc("P", script.GetWorldDetail(), genre);
        anta_npc = new Npc("A", script.GetWorldDetail(), genre);

        // 맵 정보 생성
        ChooseEventType(); // 14개의 장소 별 이벤트 타입 생성
        for (int i = 0; i < 4; i++) {
            map[i].InitPlace(i, script, pro_npc, chapter_obj[curr_chapter].GetDetail(), place_names);
            place_names[i] = map[i].place_name;
        }

        script.IntroGPT(pro_npc, anta_npc, map[0].place_name, map[0].place_info, this.char_name);
    }

    // 각 장소별 목표 or 일반 이벤트 여부 정하기
    private void ChooseEventType()
    {
        int i = 1;
        int flag = 0;
        while (i < 13) {
            flag = UnityEngine.Random.Range(0, 3);
            if (flag == 0) {
                map[i].game_event.event_type = 1;
            }
            else {
                map[i].game_event.event_type = 0;
            }

            if (map[i].game_event.event_type == 1) {
                //100
                map[i + 1].game_event.event_type = 0;
                map[i + 2].game_event.event_type = 0;
                i += 3;
                continue;
            }
            else {
                i++;
                map[i].game_event.event_type = UnityEngine.Random.Range(0, 2);
                if (map[i].game_event.event_type == 1){
                    map[i + 1].game_event.event_type = 0; //010
                }
                else {
                    map[i + 1].game_event.event_type = 1; //001
                }
                i += 2;
            }
        }
        //최종 에필로그
        if (i == 13) {
            map[i].game_event.event_type = 1;
            map[i].ANPC_exist = 0;
        }
    // map의 이벤트 타입 설정 (3개 장소마다 목표 이벤트 출현 장소 정하는 로직)
    }
// Set 함수
    public void SetCurrPlace(int idx){
        curr_place = idx;
    }


// 필드 호출 함수
    public Goals GetGoal(int chap_num){
        return chapter_obj[chap_num];
    }

    public Script GetScript(){
        return script;
    }

    public int GetCurrChap(){
        return curr_chapter;
    }

    public Place GetPlace(int idx){
        return map[idx];
    }

    public string GetCharName(){
        return char_name;
    }

    public Npc GetPnpc(){
        return pro_npc;
    }

}