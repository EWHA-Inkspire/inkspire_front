using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptInfoLoad : MonoBehaviour
{
    public void LoadScriptInfo(int chapter_num) {
        int character_id = PlayerPrefs.GetInt("character_id");
        // 스크립트 정보 조회 api 호출
        StartCoroutine(APIManager.api.GetRequest<GetScriptInfo>("/scripts/" + character_id, ProcessScriptInfo));
        // 전체 목표 정보 조회 api 호출
        int script_id = PlayerPrefs.GetInt("script_id");
        StartCoroutine(APIManager.api.GetRequest<GetGoalList>("/goals/" + script_id, ProcessGoalList));
        // 전체 npc 정보 조회 api 호출
        StartCoroutine(APIManager.api.GetRequest<GetNpcList>("/npcs/" + script_id, ProcessNpcList));
        // 전체 맵 정보 조회 api 호출
        StartCoroutine(APIManager.api.GetRequest<GetMapList>("/maps/" + script_id, ProcessMapList));
        // 전체 아이템 정보 조회 api 호출
        StartCoroutine(APIManager.api.GetRequest<GetItemList>("/inventory/" + script_id + "/items", ProcessItemList));
        // 전체 이벤트 정보 조회 api 호출
        StartCoroutine(APIManager.api.GetRequest<GetEventList>("/events/" + script_id, ProcessEventList));
    }

    private void ProcessScriptInfo(Response<GetScriptInfo> response) {
        if (!response.success) {
            Debug.Log("스크립트 정보 조회 실패: " + response.message);
            return;
        }

        Debug.Log(response.data);
        ScriptManager.script_manager.SetScriptInfo(response.data);
        PlayerPrefs.SetInt("script_id", response.data.scriptId);
    }

    private void ProcessGoalList(Response<GetGoalList> response) {
        if (!response.success) {
            Debug.Log("목표 정보 조회 실패: " + response.message);
            return;
        }

        Debug.Log(response.data);
        ScriptManager.script_manager.SetGoalList(response.data);
    }

    private void ProcessNpcList(Response<GetNpcList> response) {
        if (!response.success) {
            Debug.Log("NPC 정보 조회 실패: " + response.message);
            return;
        }

        Debug.Log(response.data);
        ScriptManager.script_manager.SetNpcList(response.data);
    }

    private void ProcessMapList(Response<GetMapList> response) {
        if (!response.success) {
            Debug.Log("맵 정보 조회 실패: " + response.message);
            return;
        }

        Debug.Log(response.data);
        ScriptManager.script_manager.SetMapList(response.data);
    }

    private void ProcessItemList(Response<GetItemList> response) {
        if (!response.success) {
            Debug.Log("아이템 정보 조회 실패: " + response.message);
            return;
        }

        Debug.Log(response.data);
        ScriptManager.script_manager.SetItemList(response.data);
    }

    private void ProcessEventList(Response<GetEventList> response) {
        if (!response.success) {
            Debug.Log("이벤트 정보 조회 실패: " + response.message);
            return;
        }

        Debug.Log(response.data);
        ScriptManager.script_manager.SetEventList(response.data);
    }
}
