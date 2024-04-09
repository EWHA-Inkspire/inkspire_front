using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;
using UnityEngine.Networking;

// gpt api 코드 => https://github.com/srcnalt/OpenAI-Unity chatGPT 샘플 코드 변형

public class ScriptManager : MonoBehaviour
{
    // 스크립트 정보를 싱글톤으로 관리
    public static ScriptManager scriptinfo;

    public struct objective{
        public string type;
        public string title;
        public string detail;
        public string etc;
        public bool clear;
    }

    // 캐릭터 ID 추후 바꿀것
    public int character_id = 1;

    public objective final_obj;
    public objective[] chapter_obj = new objective[6];
    public string[] chapter_summary = new string[6];
    public int curr_chapter;
    public string time_background;
    public string space_background;
    public string genre;

    public string world_detail="placeholder";
    public string intro_string="placeholder";

    public NpcGPT.NPCinfo pNPC;
    public NpcGPT.NPCinfo aNPC;

    private OpenAIApi openai = new OpenAIApi();
    private List<ChatMessage> gpt_messages = new List<ChatMessage>();

    void Awake(){
        // 씬이 바뀔 때 파괴되지 않음
        DontDestroyOnLoad(this.gameObject);

        if(scriptinfo == null){
            scriptinfo=this;
        }
    }
    public void setBackground(string time, string space, string gen){
        time_background = time;
        space_background = space;
        genre = gen;
        WorldDetailGPT();
    }

    private async void WorldDetailGPT(){
        Debug.Log(">>Call World Detail GPT");
        gpt_messages.Clear();

        var newMessage = new ChatMessage()
        {
            Role = "system",
            Content = @"당신은 TRPG 게임 세계관을 기획하는 게임 기획자이다. 시대와 공간적 배경의 정확한 명칭이 없어 '가상배경' 등의 단어로 제시되는 경우, 해당 배경의 이름 또한 생성하여 매끄럽게 출력한다. 
            장르가 '판타지'가 아닌 경우 마법적 요소나 판타지적 요소는 일절 포함하지 않는다. 
            당신은 반드시 게임의 장르와 시간, 공간적 배경에 알맞는 세계관을 생성해야 한다. 
            
            게임의 장르는 "+genre+"이고, 배경은 "+time_background+" 시대"+space_background+"를 배경으로 한다."
        };

        gpt_messages.Add(newMessage);

        newMessage = new ChatMessage()
        {
            Role = "user",
            Content = "게임의 배경과 장르에 어울리는 세계관 생성"
        };

        gpt_messages.Add(newMessage);

        var response = await GptManager.gpt.CallGpt(gpt_messages);
        Debug.Log(response);

        if(response != "No text was generated from this prompt.") {
            world_detail = response;
            FinalObjectiveGPT();
            NpcGPT npc = new NpcGPT();
            pNPC = await npc.CreatePNPC();
            Debug.Log("pNPC: "+pNPC.name);
            aNPC = await npc.CreateANPC();
            Debug.Log("aNPC: "+aNPC.name);
            
            IntroGpt intro = new IntroGpt();
            intro.IntroGPT();
            MapManager.mapinfo.DrawMap();

            int i = 0;
            while(i/3 == 0){
                MapManager.mapinfo.CreatePlace(i+1);
                i++;
            }
        }
    }

    private async void FinalObjectiveGPT(){
        Debug.Log(">>Call Final Objective GPT");
        gpt_messages.Clear();

        var prompt_msg = new ChatMessage()
        {
            Role = "system",
            Content = @"당신은 진행될 게임의 최종 목표를 제시한다.
            최종 목표의 유형은 반드시 다음 세가지 중 하나이며 게임의 배경에 맞춰 하나만 생성된다.

            - 최종목표 유형 1: 적대 npc나 보스 몬스터를 처치
            비고: *{적의 이름,적의 공격력,적의 방어력,적의 체력} 과 같은 양식으로 정확한 수치를 출력*

            - 최종목표 유형 2: 스토리 설정상 중요한 아이템을 획득한다.
            비고: *아이템 이름만 출력*
            
            - 최종목표 유형3 : 어떤 큰 사건의 진상이나 마을에 숨겨진 비밀을 알아낸다
            비고:*얻어야 할 정보의 진상을 한문장으로 출력*

            다음은 게임의 배경인 
            "+time_background+"시대"+space_background+"를 배경으로 하는 세계관에 대한 설명이다."+world_detail+
            @"아래와 같은 양식으로 게임의 목표를 설정한다. 각 줄의 요소는 반드시 모두 포함되어야 하며, 답변할 때 줄바꿈을 절대 하지 않는다. ** 이 표시 안의 내용은 문맥에 맞게 채운다.

            최종목표유형: *위의 유형 1~3 중 하나를 숫자만 출력*
            최종목표: *최종 목표의 제목 출력*
            최종목표 설명: *최종 목표 설명*
            비고:*반드시 해당하는 최종목표 유형에 제시된 형식과 내용에 맞춰 출력*"
        };
        gpt_messages.Add(prompt_msg);
        
        var query_msg = new ChatMessage()
        {
            Role = "user",
            Content = "진행될 게임의 "+genre+"장르에 어울리는 최종 목표 생성"
        };
        gpt_messages.Add(query_msg);

        final_obj = chapter_obj[5] = StringToObjective(await GptManager.gpt.CallGpt(gpt_messages));

            curr_chapter = 0;
            ChapterObjectiveGPT(0);
            //StartCoroutine(PostChapterObjective());
        
    }

    private async void ChapterObjectiveGPT(int chapter_num){
        Debug.Log(">>Call Chapter Objective GPT");
        Debug.Log(">>curr chapter: "+(chapter_num+1));
        gpt_messages.Clear();

        var prompt_msg = new ChatMessage()
        {
            Role = "system",
            Content = @"당신은 진행되는 게임의 챕터 목표를 제시한다.
            게임은 총 5개의 챕터로 구성되어있으며 현재 생성할 목표의 챕터 번호는"+(chapter_num+1)+"이다."+
            @"챕터 목표의 유형은 반드시 다음 세가지 중 하나이며 게임의 배경에 맞춰 하나만 생성된다.

            - 최종목표 유형 1: 적대 npc나 보스 몬스터를 처치
            비고: *{적의 이름,적의 공격력,적의 방어력,적의 체력} 과 같은 양식으로 정확한 수치를 출력*

            - 챕터목표 유형 2: 이후 스토리 진행에 사용될 아이템, *아이템이름* 획득
            비고: *지금 챕터 번호부터 챕터5까지 숫자 중 랜덤 하나*

            - 챕터목표 유형 3: 스토리 진행에 중요한 정보 *얻어야 할 정보 제목* 습득
            비고:*얻어야 할 정보의 진상을 한문장으로 출력*

            다음은 게임의 배경인 
            "+time_background+"시대"+space_background+"를 배경으로 하는 세계관에 대한 설명이다."+world_detail+
            "게임의 최종 목표는 다음과 같다.\n"+final_obj.title+"\n"+final_obj.detail

        };
        if(chapter_num!=0){
            prompt_msg.Content+="다음은 이전 챕터의 진행 줄거리이다. 챕터 목표는 이전 챕터의 진행과 자연스럽게 이어져야 한다."+chapter_summary[chapter_num];
        }

        prompt_msg.Content+=@"아래와 같은 양식으로 게임의 목표를 설정한다. 각 줄의 요소는 반드시 모두 포함되어야 하며, 답변할 때 줄바꿈을 절대 하지 않는다. ** 이 표시 안의 내용은 문맥에 맞게 채운다.

            챕터목표유형: *위의 유형 1~3 중 하나를 숫자만 출력*
            챕터목표: *챕터 목표의 제목 출력*
            챕터목표 설명: *챕터 목표 설명*
            비고:*반드시 해당하는 챕터목표 유형에 제시된 형식과 내용에 맞춰 출력*";
        gpt_messages.Add(prompt_msg);

        var query_msg = new ChatMessage()
        {
            Role = "user",
            Content = "진행중인 게임의 "+genre+"장르에 어울리는 챕터"+curr_chapter.ToString()+"의 챕터 목표 생성"
        };
        gpt_messages.Add(query_msg);

        chapter_obj[chapter_num+1] = StringToObjective(await GptManager.gpt.CallGpt(gpt_messages));
            curr_chapter = chapter_num+1;
            if(curr_chapter!=1){
                //StartCoroutine(PostChapterObjective(curr_chapter));
            }

    }

    objective StringToObjective(string obj_string){
        objective obj = new objective();
        obj.clear = false;

        string[] obj_arr;
        obj_string = obj_string.Replace("\n",":");
        obj_arr = obj_string.Split(':');
        for(int i = 0; i<obj_arr.Length; i++ ){
            switch(i){
                case 1:
                    obj.type = obj_arr[1];
                    continue;
                case 3:
                    obj.title = obj_arr[3];
                    continue;
                case 5:
                    obj.detail = obj_arr[5];
                    continue;
                case 7:
                    obj.etc = obj_arr[7];
                    continue;
                default:
                    continue;
            }
        }

        return obj;
    }

    IEnumerator PostChapterObjective(int chap){
        string obj_type;
        switch(int.Parse(chapter_obj[chap].type)){
            case 1:
                obj_type = "MONSTER";
                break;
            case 2:
                obj_type = "ITEM";
                break;
            case 3:
            default:
                obj_type = "REPORT";
                break;
        }
        WWWForm form = new WWWForm();
        form.AddField("scriptId",character_id);
        if(chap == 0){
            form.AddField("chapter",5);
        }
        else{
            form.AddField("chapter",chap);
        }
        form.AddField("content",chapter_obj[chap].title);
        form.AddField("type",obj_type);
        form.AddField("require",chapter_obj[chap].detail);
        form.AddField("etc",chapter_obj[chap].etc);

        UnityWebRequest www = UnityWebRequest.Post("http://3.35.61.61:8080/scripts/goal",form);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(">>ChapterObjective upload complete.");
        }

    }

}
