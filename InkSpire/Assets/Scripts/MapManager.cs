using System.Collections;
using System.Collections.Generic;
using OpenAI;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private ScriptManager scriptManager;
    public struct place
    {
        public string place_name;
        public string place_info; //장소 설명
        public string item_name;
        public string item_type; //recover, weapon, mob, null (목표이벤트일 경우 report 추가)
        public bool event_type; //일반 이벤트 == 0, 목표 이벤트 == 1 
        public bool ANPC_exist; //ANPC 미등장 == 0, 등장 == 1 (목표이벤트일 경우 무조건 0)
    }
    private place[] map = new place[14];
    public string PNPC_place;
    private OpenAIApi openai = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();
    private ChatMessage input_msg = new ChatMessage();
    private string system_prompt = "너는 플레이어가 탐색할 수 있는 장소를 한 단어로 출력해야 해.";
    //background + " 배경의 " + genre + "분위기에 어울리는 장소명 1개를 출력해줘";

    // def getProtaNPCName(background, genre):
    // npc_setting = "너는 조력자 NPC 캐릭터의 이름을 한 단어로 출력해야 해."
    // query = background + " 배경의 " + genre + "분위기에 어울리는 조력자 NPC 이름 1개를 출력해줘"

    // messages = [
    //     {"role": "system", "content": npc_setting},
    //     {"role": "user", "content": query}
    // ]

    // PNPC_name = callGPT(messages=messages, stream=False)

    // return PNPC_name

    public void setBackground(string time, string space, string gen)
    {
        time_background = time;
        space_background = space;
        genre = gen;
        FinalObjectiveGPT(); //장소 생성에 맞게 수정
    }

    private async void CreateItem()
    {

    }
    private async void CreatePlace(int place_idx)
    {
        Debug.Log(">>Call Chapter Objective GPT");
        Debug.Log(">>현재 장소 인덱스: " + place_idx);
        gpt_messages.Clear();

        var prompt_msg = new ChatMessage()
        {
            Role = "system",
            Content = @"당신은 게임 진행에 필요한 장소를 제시한다.
            다음은 게임의 배경인 
            " + time_background + "시대" + space_background + "를 배경으로 하는 세계관에 대한 설명이다." + world_detail +
            @"장소는 게임의 배경에 맞추어 플레이어가 흥미롭게 탐색할 수 있는 곳으로 생성된다. 장소 생성 양식은 아래와 같다. 각 줄의 요소는 반드시 모두 포함되어야 하며, 답변할 때 줄바꿈을 절대 하지 않는다. ** 이 표시 안의 내용은 문맥에 맞게 채운다.
            
            장소명: *장소 이름을 한 단어로 출력*
            장소설명: *장소에 대한 설명을 50자 내외로 설명*"
        };
        gpt_messages.Add(prompt_msg);

        var query_msg = new ChatMessage()
        {
            Role = "user",
            Content = "진행중인 게임의 " + genre + "장르에 어울리는 챕터" + curr_chapter.ToString() + "의 챕터 목표 생성"
        };
        gpt_messages.Add(query_msg);
    }
    private async void ChapterObjectiveGPT(int chapter_num)
    {
        Debug.Log(">>Call Chapter Objective GPT");
        Debug.Log(">>curr chapter: " + (chapter_num + 1));
        gpt_messages.Clear();

        var prompt_msg = new ChatMessage()
        {
            Role = "system",
            Content = @"당신은 진행되는 게임의 챕터 목표를 제시한다.
            게임은 총 5개의 챕터로 구성되어있으며 현재 생성할 목표의 챕터 번호는" + (chapter_num + 1) + "이다." +
            @"챕터 목표의 유형은 반드시 다음 세가지 중 하나이며 게임의 배경에 맞춰 하나만 생성된다.

            - 최종목표 유형 1: 적대 npc나 보스 몬스터를 처치
            비고: *{적의 이름,적의 공격력,적의 방어력,적의 체력} 과 같은 양식으로 정확한 수치를 출력*

            - 챕터목표 유형 2: 이후 스토리 진행에 사용될 아이템, *아이템이름* 획득
            비고: *지금 챕터 번호부터 챕터5까지 숫자 중 랜덤 하나*

            - 챕터목표 유형 3: 스토리 진행에 중요한 정보 *얻어야 할 정보 제목* 습득
            비고:*얻어야 할 정보의 진상을 한문장으로 출력*

            다음은 게임의 배경인 
            " + time_background + "시대" + space_background + "를 배경으로 하는 세계관에 대한 설명이다." + world_detail +
            "게임의 최종 목표는 다음과 같다.\n" + final_obj.title + "\n" + final_obj.detail

        };
        if (chapter_num != 0)
        {
            prompt_msg.Content += "다음은 이전 챕터의 진행 줄거리이다. 챕터 목표는 이전 챕터의 진행과 자연스럽게 이어져야 한다." + chapter_summary[chapter_num];
        }

        prompt_msg.Content += @"아래와 같은 양식으로 게임의 목표를 설정한다. 각 줄의 요소는 반드시 모두 포함되어야 하며, 답변할 때 줄바꿈을 절대 하지 않는다. ** 이 표시 안의 내용은 문맥에 맞게 채운다.

            최종목표유형: *위의 유형 1~3 중 하나를 숫자만 출력*
            최종목표: *최종 목표*
            최종목표 설명: *최종 목표 설명*
            비고:*반드시 해당하는 최종목표 유형에 제시된 형식과 내용에 맞춰 출력*";
        gpt_messages.Add(prompt_msg);

        var query_msg = new ChatMessage()
        {
            Role = "user",
            Content = "진행중인 게임의 " + genre + "장르에 어울리는 챕터" + curr_chapter.ToString() + "의 챕터 목표 생성"
        };
        gpt_messages.Add(query_msg);


        // Complete the instruction
        var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
        {
            Model = "gpt-3.5-turbo",
            Messages = gpt_messages
        });

        if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
        {
            var message = completionResponse.Choices[0].Message;
            message.Content = message.Content.Trim();
            Debug.Log(message.Content);
            chapter_obj[chapter_num + 1] = StringToObjective(message.Content);
            curr_chapter = chapter_num + 1;
            if (curr_chapter != 1)
            {
                //StartCoroutine(PostChapterObjective(curr_chapter));
            }

        }
        else
        {
            Debug.LogWarning("No text was generated from this prompt.");
        }


    }
    private async void CreatePlaceInfo()
    {
        Debug.Log(">>Call Create Place GPT");
        gpt_messages.Clear();

        var prompt_msg = new ChatMessage()
        {
            Role = "system",
            Content = @"당신은 게임 진행에 필요한 장소에 대한 설명을 50자 이내로 제시한다.
            다음은 게임의 배경인 
            " + time_background + "시대" + space_background + "를 배경으로 하는 세계관에 대한 설명이다." + world_detail +
            @"장소는 게임의 배경에 맞추어 플레이어가 흥미롭게 탐색할 수 있는 곳으로 생성된다."
        };
        gpt_messages.Add(prompt_msg);

        var query_msg = new ChatMessage()
        {
            Role = "user",
            Content = "진행될 게임의 세계관에 어울리는 새로운 장소를 생성하라."
        };
        gpt_messages.Add(query_msg);

        // Complete the instruction
        var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
        {
            Model = "gpt-3.5-turbo",
            Messages = gpt_messages
        });

        if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
        {
            var message = completionResponse.Choices[0].Message;
            message.Content = message.Content.Trim();
            Debug.Log(message.Content);
            final_obj = chapter_obj[5] = StringToObjective(message.Content); //최종목표 변수 수정

            curr_chapter = 0; //여기도
            ChapterObjectiveGPT(0); //여기도 
            //StartCoroutine(PostChapterObjective());
        }
        else
        {
            Debug.LogWarning("No text was generated from this prompt.");
        }
    }

    //장소 이름 및 장소 설명 파싱 함수
    place StringToPlace(string plc_string)
    {
        place plc = new place();
        plc.clear = false;

        string[] plc_arr;
        plc_string = plc_string.Replace("\n", ":");
        plc_arr = plc_string.Split(':');

        plc.place_name = plc_arr[1];
        plc.place_info = plc_arr[3];

        return plc;
    }
}
