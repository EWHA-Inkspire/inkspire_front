using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using System.Threading.Tasks;
using System;

public class NpcGPT
{
    private enum NPCType{
        A,
        P,
        etc
    }
    public class NPCinfo{
        public string name;
        NPCType type;
        public Stats stat;
        public string detail;
        public NPCinfo(string name, string type, int atk, int def, int dex, int luk, int mtl, string detail){
            stat = new Stats(luk, def, mtl, dex, atk);
            this.name = name;
            this.type = (NPCType)Enum.Parse(typeof(NPCType),type);
            this.detail = detail;
        }

        string getType(){
            return NPCtypeToString(type);
        }

        string NPCtypeToString(NPCType type){
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

    }

    string NPCtypeToString(NPCType type){
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

    

    async Task<string> NameGPT(NPCType type){
        string name;
        string type_s = NPCtypeToString(type);

        List<ChatMessage> messages = new List<ChatMessage>();
        ChatMessage prompt = new ChatMessage(){
            Role = "system",
            Content = "당신은 "+type_s+"NPC의 이름을 한 단어로 출력해주는 생성기이다. NPC의 이름은 주인공인 "+PlayerStatManager.playerstat.charname+"의 이름과 같아서는 안된다. 게임의 배경과 장르 분위기에 알맞은 NPC이름을 출력해야하며 게임의 배경과 장르는 다음과 같다."
                        +"\n게임 배경: "+ScriptManager.scriptinfo.world_detail
                        +"\n게임 장르: "+ScriptManager.scriptinfo.genre
        };
        ChatMessage query = new ChatMessage(){
            Role = "user",
            Content = "NPC 이름 생성"
        };
        messages.Add(prompt);
        messages.Add(query);

        name = await GptManager.gpt.CallGpt(messages);
        return name;
    }

    async Task<string> NpcDetailGPT(NPCType type, string name){
        string detail;
        string type_s = NPCtypeToString(type);

        List<ChatMessage> messages = new List<ChatMessage>();
        ChatMessage prompt = new ChatMessage(){
            Role = "system",
            Content = "당신은 "+type_s+"NPC의 설정을 생성하는 생성기이다. NPC는 게임 배경과 장르에 어울리는 직업을 가져야 하며, 성격에 맞게 존댓말 혹은 반말 중 하나의 말투만을 일관되게 사용한다. NPC의 이름은 사용자가 제시한다."
                        +"게임 배경: "+ScriptManager.scriptinfo.world_detail
                        +"게임 장르: "+ScriptManager.scriptinfo.genre
        };
        ChatMessage query = new ChatMessage(){
            Role = "user",
            Content = "NPC"+name+"의 직업, 성격, 말투설정 생성"
        };
        messages.Add(prompt);
        messages.Add(query);

        detail = await GptManager.gpt.CallGpt(messages);
        return detail;

    }

    public async Task<NPCinfo> CreatePNPC(){
        string name = await NameGPT(NPCType.P);
        string detail = await NpcDetailGPT(NPCType.P,name);

        return new NPCinfo(name, "P", 50,50,50,50,50, detail);
    }

    public async Task<NPCinfo> CreateANPC(){
        string name = await NameGPT(NPCType.A);
        string detail = await NpcDetailGPT(NPCType.A,name);

        return new NPCinfo(name, "A", 50,50,50,50,50, detail);
    }
}
