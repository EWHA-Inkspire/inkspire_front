using System.Collections.Generic;
using OpenAI;
using System;
using System.Threading.Tasks;

public class Npc
{
    private enum NPCType
    {
        A,
        P,
        etc
    }

    private int id;
    private string name;
    private NPCType type;
    private Stats stat;
    private string detail = "";

    private List<ChatMessage> gpt_messages = new();
    private readonly string GPT_ERROR = "No text was generated from this prompt.";

    public async Task InitNpc(string type, string world_detail, string genre, string char_name)
    {
        this.type = (NPCType)Enum.Parse(typeof(NPCType),type);
        await NameGPT(this.type, world_detail, genre, char_name);
        await NpcDetailGPT(this.type, world_detail, genre);
        stat = new Stats(50, 50, 50, 50, 50);
    }

    private async Task<string> NameGPT(NPCType type, string world_detail, string genre, string char_name){
        string type_s = NPCtypeToString(type);
        gpt_messages.Clear();

        var newMessage = new ChatMessage(){
            Role = "system",
            Content = "당신은 "+type_s+"NPC의 이름을 한 단어로 출력해주는 생성기이다. NPC의 이름은 주인공인 "+char_name+"의 이름과 같아서는 안된다. 게임의 배경과 장르 분위기에 알맞은 NPC이름을 출력해야하며 게임의 배경과 장르는 다음과 같다."
                        +"또한, 출력의 영어표기를 생략하고 한글표기만 나타낸다. 출력은 반드시 한글로만 이루어진다."
                        +"\n게임 배경: "+world_detail
                        +"\n게임 장르: "+genre
        };
        gpt_messages.Add(newMessage);

        newMessage = new ChatMessage(){
            Role = "user",
            Content = "NPC 이름 생성"
        };
        gpt_messages.Add(newMessage);

        string response = await GptManager.gpt.CallGpt(gpt_messages);
        if(response == GPT_ERROR) {
            // 출력 안 될 경우 처리
        }

        this.name = response;
        return response;
    }

    private async Task<string> NpcDetailGPT(NPCType type, string world_detail, string genre){
        string type_s = NPCtypeToString(type);
        gpt_messages.Clear();

        var newMessage = new ChatMessage(){
            Role = "system",
            Content = "당신은 "+type_s+"NPC의 설정을 생성하는 생성기이다. NPC는 게임 배경과 장르에 어울리는 직업을 가져야 하며, 성격에 맞게 존댓말 혹은 반말 중 하나의 말투만을 일관되게 사용한다. NPC의 이름은 사용자가 제시한다."
                        +"게임 배경: "+world_detail
                        +"게임 장르: "+genre
        };
        gpt_messages.Add(newMessage);

        newMessage = new ChatMessage(){
            Role = "user",
            Content = "NPC\""+name+"\"의 직업, 성격, 말투설정 생성"
        };
        gpt_messages.Add(newMessage);

        string response = await GptManager.gpt.CallGpt(gpt_messages);
        if(response == GPT_ERROR) {
            // 출력 안 될 경우 처리
        }

        detail = response;
        return response;
    }

    private string NPCtypeToString(NPCType type){
        string type_s;
        if(type == NPCType.P){
            type_s = "조력자";
        }
        else if(type == NPCType.A){
            type_s = "적대자";
        }
        else{
            type_s = "조연";
        }
        return type_s;
    }

    public string GetName()
    {
        return name;
    }

    public string GetDetail()
    {
        return detail;
    }

    public Stats GetStat()
    {
        return stat;
    }

    public void SetNpcInfo(GetNpcInfo npc) {
        id = npc.npcId;
        name = npc.name;
        detail = npc.detail;
        stat = new Stats(npc.luck, npc.defence, npc.mental, npc.dexterity, npc.attack);
        stat.SetStatAmount(StatType.Hp, npc.hp);
    }

    public void SetNpcId(int id) {
        this.id = id;
    }
}