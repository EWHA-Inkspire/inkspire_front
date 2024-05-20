using UnityEngine;
using UnityEngine.SceneManagement;

public class ScriptInfoLoad : MonoBehaviour
{
    private int script_id = -1;
    private int chapter_num = 0;
    public void LoadScriptInfo(int chapter_num) {
        PlayerPrefs.SetInt("Call API", 1);
        int character_id = PlayerPrefs.GetInt("character_id");
        this.chapter_num = chapter_num;
        // 스크립트 정보 조회 api 호출
        StartCoroutine(APIManager.api.GetRequest<GetScriptInfo>("/scripts/" + character_id, ProcessScriptInfo));
        // 캐릭터 스탯 조회 api 호출
        StartCoroutine(APIManager.api.GetRequest<CharacterStatInfo>("/characters/" + character_id + "/stat", ProcessCharacterStat));

        if (script_id == -1) {
            Invoke(nameof(LoadEtcInfo), 1f);
        }

        WaitForAPI();
    }

    private void WaitForAPI() {
        if(ScriptManager.script_manager.GetInitScript()){
            SceneManager.LoadScene("5_Play");
        } else {
            Invoke(nameof(WaitForAPI), 1f);
        }
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

        // 현재 챕터 정보 업데이트
        ScriptManager.script_manager.SetViewChap(chapter_num);
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
}
