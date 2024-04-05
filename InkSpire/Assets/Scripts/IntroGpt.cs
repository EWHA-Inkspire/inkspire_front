using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;

public class IntroGpt : MonoBehaviour
{
    private List<ChatMessage> messages = new List<ChatMessage>();

    private void Start() {
        IntroGPT();
    }

    private async void IntroGPT(){
        Debug.Log(">>Call Intro GPT");
        messages.Clear();
        
        var newMessage = new ChatMessage()
        {
            Role = "system",
            Content = @"당신은 게임 속 세계관을 전부 알고 있는 전능한 존재이자 스토리 게임을 진행하는 Narrator이다.
            게임의 배경과 세계관 설정을 참고하여 게임의 시작 멘트인 인트로를 아래의 양식대로 출력한다.
            ** 이 표시 안의 내용은 문맥에 맞게 채운다.
            ###
            여기는 *시간적 배경* *공간적 배경*. *플레이어 이름*님, 당신은 이 도시로 들어오면서 새로운 모험을 시작하게 됩니다.
            
            *게임 시작 멘트*
            ###"
        };
        
        messages.Add(newMessage);

        newMessage = new ChatMessage()
        {
            Role = "user",
            Content = "플레이어의 이름은 "+PlayerStatManager.playerstat.charname+"이고, 현재 플레이중인 게임은 "+ScriptManager.scriptinfo.time_background+", "
            +ScriptManager.scriptinfo.space_background+"(을)를 배경으로 하는 "
            +ScriptManager.scriptinfo.genre+" 장르의 게임이며 세계관은 다음과 같다."
            +ScriptManager.scriptinfo.world_detail+@"
            게임의 인트로 출력"
        };

        messages.Add(newMessage);

        var response = await GptManager.gpt.CallGpt(messages);
        Debug.Log(response);
    }
}
