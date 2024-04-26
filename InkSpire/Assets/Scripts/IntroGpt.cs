using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;

public class IntroGpt
{
    private List<ChatMessage> messages = new List<ChatMessage>();
    public async void IntroGPT()
    {
        Debug.Log(">>Call Intro GPT");
        messages.Clear();

        var newMessage = new ChatMessage()
        {
            Role = "system",
            Content = @"당신은 게임 속 세계관을 전부 알고 있는 전능한 존재이자 스토리 게임을 진행하는 Narrator이다.
            게임의 배경과 세계관 설정을 참고하여 게임의 시작 멘트인 인트로를 아래의 양식대로 출력하되, 직접적으로 게임이라는 언급은 하지 않는다.
            또한, 조력자 npc인 " + ScriptManager.scriptinfo.pNPC.name + @"(" + ScriptManager.scriptinfo.pNPC.detail + @")와 적대자 NPC인 " + ScriptManager.scriptinfo.aNPC.name + @"(" + ScriptManager.scriptinfo.aNPC.detail + @")의 요약된 소개를 줄글 형태로 진행한다. 
            게임 시작 장소는" + MapManager.mapinfo.map[0].place_name + "으로, " + MapManager.mapinfo.map[0].place_info + @"이다.
            ** 이 표시 안의 내용은 문맥에 맞게 채운다.
            ###
            여기는 *시간적 배경* *공간적 배경*. *플레이어 이름*님, 당신은 이 도시로 들어오면서 새로운 모험을 시작하게 됩니다.
            
            *게임 시작 멘트*
            *조력자 NPC와 시작 장소에서 처음으로 만나는 내용이 출력되며 npc는 게임의 사건의 시작을 소개하며 게임의 시작을 알린다.*

            *조력자 NPC 이름*:
                *조력자 NPC의 대사*
            ###"
        };

        messages.Add(newMessage);

        newMessage = new ChatMessage()
        {
            Role = "user",
            Content = "플레이어의 이름은 다음과 같다. 플레이어 이름:" + PlayerStatManager.playerstat.charname + @" 
            현재 플레이중인 게임은 " + ScriptManager.scriptinfo.time_background + ", "
            + ScriptManager.scriptinfo.space_background + "(을)를 배경으로 하는 "
            + ScriptManager.scriptinfo.genre + " 장르의 게임이며 세계관은 다음과 같다."
            + ScriptManager.scriptinfo.world_detail + @"
            게임의 인트로 출력"
        };

        messages.Add(newMessage);

        var response = await GptManager.gpt.CallGpt(messages);
        response = response.Replace("###\n", "");
        ScriptManager.scriptinfo.intro_string = response.Replace("###", "");
        Debug.Log(response);
    }
}
