using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using System.Threading.Tasks;

public class Epilogue
{
    [SerializeField] public TextScrollUI text_scroll;
    private List<ChatMessage> gpt_messages = new();
    private readonly string GPT_ERROR = "No text was generated from this prompt.";

    public async Task SuccessOutroGPT(Npc pro_npc, Npc anta_npc, Script script)
    {
        gpt_messages.Clear();

        var newMessage = new ChatMessage()
        {
            Role = "system",
            Content = @"당신은 게임 속 세계관과 지금까지 진행된 게임의 스토리를 전부 알고 있는 전능한 존재이자 스토리 게임을 진행하는 Narrator이다.
게임의 배경과 세계관 설정을 참고하여 게임의 스토리를 마무리하는 에필로그를 아래의 양식대로 출력하되, 직접적으로 게임이라는 언급은 하지 않는다.
게임의 주인공인 플레이어는 현재까지 주어진 모든 게임 목표를 성공시키고 마지막 최종 목표였던" + ScriptManager.script_manager.GetFinalGoal() + @"또한 성공시켜 전체 스토리를 성공적으로 마무리하게 되었다.
조력자 npc의 축하 멘트를 포함하여 아래에 주어지는 게임 정보를 바탕으로 모든 사건이 완벽하게 해결되고, 세계관이 안정을 되찾았다는 사실을 약 500자 내외의 소설체로 묘사한다.
이 게임의 정보는 아래와 같다. 
조력자 npc : " + pro_npc.GetName() + "(" + pro_npc.GetDetail() + @")
적대자 NPC : " + anta_npc.GetName() + "(" + anta_npc.GetDetail() + @") 
게임 장르 : " + script.GetGenre() + @" 게임 배경 : " + script.GetTimeBackground() + "/" + script.GetSpaceBackground() +
@"** 이 표시 안의 내용은 문맥에 맞게 채운다.
### 

*조력자 NPC의 성공 축하 멘트*    
*게임 마무리 에필로그 (500자 내외의 소설체)*

###"
        };

        gpt_messages.Add(newMessage);

        newMessage = new ChatMessage()
        {
            Role = "user",
            Content = "플레이어의 이름은 다음과 같다. 플레이어 이름:" + ScriptManager.script_manager.GetCharName()
            + "\n현재 플레이중인 게임은 " + script.GetTimeBackground() + ", "
            + script.GetSpaceBackground() + "(을)를 배경으로 하는 "
            + script.GetGenre() + " 장르의 게임이며 세계관은 다음과 같다.\n"
            + script.GetWorldDetail()
            + "\n 또한 이 게임의 최종 목표는 다음과 같다."
            + ScriptManager.script_manager.GetFinalGoal()
            + "\n플레이어는 게임의 마지막 관문인 이 목표 통과하였다. 이에 따른 게임의 에필로그를 출력하라."
        };

        gpt_messages.Add(newMessage);

        var response = await GptManager.gpt.CallGpt(gpt_messages);
        response = response.Replace("###\n", "");
        response = response.Replace("*", "");
        text_scroll.AppendMsg(newMessage);
    }

    public async Task FailOutroGPT(Npc pro_npc, Npc anta_npc, Script script)
    {
        gpt_messages.Clear();

        var newMessage = new ChatMessage()
        {
            Role = "system",
            Content = @"당신은 게임 속 세계관과 지금까지 진행된 게임의 스토리를 전부 알고 있는 전능한 존재이자 스토리 게임을 진행하는 Narrator이다.
게임의 배경과 세계관 설정을 참고하여 게임의 스토리를 마무리하는 에필로그를 아래의 양식대로 출력하되, 직접적으로 게임이라는 언급은 하지 않는다.
게임의 주인공인 플레이어는 앞선 게임에서 주어진 게임 목표 중 하나 이상을 성공시키지 못해 마지막 최종 목표였던" + ScriptManager.script_manager.GetFinalGoal() + @"를 수행할 기회조차 얻지 못했다.
따라서 해당 스크립트의 스토리는 실패로 돌아가게 되었으며 세계관의 안정은 되찾지 못했다. 세계는 여전히 문제 속에 있으며 아무것도 해결된 것이 없다. 
조력자 npc의 성격에 맞는 격려 멘트를 포함하여 아래에 주어지는 게임 정보를 바탕으로 어딘가 어긋난 부분이 있어 주어진 문제를 해결하지 못했다는 사실을 약 500자 내외의 소설체로 묘사한다.
이 게임의 정보는 아래와 같다. 
조력자 npc : " + pro_npc.GetName() + "(" + pro_npc.GetDetail() + @")
적대자 NPC : " + anta_npc.GetName() + "(" + anta_npc.GetDetail() + @") 
게임 장르 : " + script.GetGenre() + @" 게임 배경 : " + script.GetTimeBackground() + "/" + script.GetSpaceBackground() +
@"** 이 표시 안의 내용은 문맥에 맞게 채운다.
###

*조력자 NPC의 실패 격려 멘트*
*게임 마무리 에필로그 (500자 내외의 소설체)*

*플레이어 이름*은 마치 오래 전부터 알고 있었던 누군가의 아주 친숙한 목소리를 듣는다.
어딘가 잘못된 부분이 있는 것 같아. 다시 돌아가서 해결해볼까...?

###"
        };

        gpt_messages.Add(newMessage);

        newMessage = new ChatMessage()
        {
            Role = "user",
            Content = "플레이어의 이름은 다음과 같다. 플레이어 이름:" + ScriptManager.script_manager.GetCharName()
            + "\n현재 플레이중인 게임은 " + script.GetTimeBackground() + ", "
            + script.GetSpaceBackground() + "(을)를 배경으로 하는 "
            + script.GetGenre() + " 장르의 게임이며 세계관은 다음과 같다.\n"
            + script.GetWorldDetail()
            + "\n 또한 이 게임의 최종 목표는 다음과 같다."
            + ScriptManager.script_manager.GetFinalGoal()
            + "\n플레이어는 게임의 마지막 관문인 이 목표 통과하지 못했다. 이에 따른 게임의 에필로그를 출력하라."
        };

        gpt_messages.Add(newMessage);

        var response = await GptManager.gpt.CallGpt(gpt_messages);
        response = response.Replace("###\n", "");
        response = response.Replace("*", "");
        text_scroll.AppendMsg(newMessage);
    }
}