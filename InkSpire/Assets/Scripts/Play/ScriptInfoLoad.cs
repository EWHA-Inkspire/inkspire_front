using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScriptInfoLoad : MonoBehaviour
{
    [SerializeField] GameObject LoadingPannel;
    [SerializeField] TextMeshProUGUI LoadingText;
    private int script_id = -1;
    private int view_chapter = 0;
    private int curr_chapter = 0;

    public void LoadScriptInfo(int chapter_num) {
        PlayerPrefs.SetInt("Call API", 1);
        int character_id = PlayerPrefs.GetInt("character_id");
        this.view_chapter = chapter_num;
        // 스크립트 정보 조회 api 호출
        StartCoroutine(APIManager.api.GetRequest<GetScriptInfo>("/scripts/" + character_id, ProcessScriptInfo));
        // 캐릭터 스탯 조회 api 호출
        StartCoroutine(APIManager.api.GetRequest<CharacterStatInfo>("/characters/" + character_id + "/stat", ProcessCharacterStat));

        if (script_id == -1) {
            Invoke(nameof(LoadEtcInfo), 1f);
        }
        LoadingText.text = "게임을 불러오는 중입니다";
        StartCoroutine(WaitForAPI());
    }

    IEnumerator WaitForAPI()
    {
        if (!LoadingPannel.activeSelf)
        {
            LoadingPannel.SetActive(true);
        }

        while (!ScriptManager.script_manager.GetInitScript() || ScriptManager.script_manager.GetScript().GetIntroImage() == null)
        {
            if (LoadingText.text == "게임을 불러오는 중입니다 . . .")
            {
                LoadingText.text = "게임을 불러오는 중입니다";
            }
            else
            {
                LoadingText.text += " .";
            }
            yield return new WaitForSeconds(1f);
        }

        SceneManager.LoadScene("5_Play");
    }

    private void LoadEtcInfo() {
        // 전체 목표 정보 조회 api 호출
        StartCoroutine(APIManager.api.GetRequest<GetGoalList>("/goals/" + script_id, ProcessGoalList));
        // 전체 npc 정보 조회 api 호출
        StartCoroutine(APIManager.api.GetRequest<GetNpcList>("/npcs/" + script_id, ProcessNpcList));
        // 전체 맵 정보 조회 api 호출
        StartCoroutine(APIManager.api.GetRequest<GetMapList>("/maps/" + script_id, ProcessMapList));
        // 전체 아이템 정보 조회 api 호출
        StartCoroutine(APIManager.api.GetRequest<GetItemList>("/inventory/" + script_id + "/items", ProcessItemList));
        // 전체 이벤트 정보 조회 api 호출
        StartCoroutine(APIManager.api.GetRequest<GetEventList>("/events/" + script_id, ProcessEventList));
        // 챕터 인트로 이미지 조회 api 호출
        StartCoroutine(APIManager.api.GetRequest<string>("/images/" + script_id + "/" + (view_chapter + 1), ProcessChapterIntroImage));

        // 현재 챕터 정보 업데이트
        Debug.Log("view_chapter: " + view_chapter);
        Debug.Log("curr_chapter: " + curr_chapter);
        ScriptManager.script_manager.SetViewChap(view_chapter);
        ScriptManager.script_manager.SetCurrChap(curr_chapter);
        ScriptManager.script_manager.SetCharName(PlayerPrefs.GetString("character_name"));
    }

    private void ProcessScriptInfo(Response<GetScriptInfo> response) {
        if (!response.success) {
            return;
        }
        ScriptManager.script_manager.SetScriptInfo(response.data);
        script_id = response.data.scriptId;
        PlayerPrefs.SetInt("script_id", response.data.scriptId);
    }

    private void ProcessCharacterStat(Response<CharacterStatInfo> response) {
        if (!response.success) {
            return;
        }
        PlayerStatManager.playerstat.SetCharacterStat(response.data);
    }

    private void ProcessGoalList(Response<GetGoalList> response) {
        if (!response.success) {
            return;
        }
        ScriptManager.script_manager.SetGoalList(response.data);

        // 챕터 순으로 정렬
        response.data.goals.Sort((a, b) => a.chapter.CompareTo(b.chapter));

        // 목표들 중 성공하지 못한 목표가 있는 챕터를 현재 챕터로 설정
        foreach (var goal in response.data.goals) {
            if (!goal.success) {
                curr_chapter = goal.chapter - 1;
                break;
            }
        }
    }

    private void ProcessNpcList(Response<GetNpcList> response) {
        if (!response.success) {
            return;
        }
        ScriptManager.script_manager.SetNpcList(response.data);
    }

    private void ProcessMapList(Response<GetMapList> response) {
        if (!response.success) {
            return;
        }
        ScriptManager.script_manager.SetMapList(response.data);
    }

    private void ProcessItemList(Response<GetItemList> response) {
        if (!response.success) {
            return;
        }
        ScriptManager.script_manager.SetItemList(response.data);
    }

    private void ProcessEventList(Response<GetEventList> response) {
        if (!response.success) {
            return;
        }
        ScriptManager.script_manager.SetEventList(response.data);
        ScriptManager.script_manager.SetInitScript(true);
    }

    private void ProcessChapterIntroImage(Response<string> response)
    {
        if (!response.success)
        {
            return;
        }

        ScriptManager.script_manager.GetScript().SetIntroImage(response.data);
    }
}
