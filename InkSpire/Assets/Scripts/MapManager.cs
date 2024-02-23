using System;
using System.Collections;
using System.Collections.Generic;
using OpenAI;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private ScriptManager scriptManager;
    public static MapManager mapinfo;
    public struct place
    {
        public string place_name; //장소 이름
        public string place_info; //장소 설명
        public string item_name; //아이템 이름, 아이템 설명? 
        public string item_type; //일반 이벤트일 경우 아이템, 목표 이벤트일 경우 목표
        public int event_type; //일반 이벤트 == 0, 목표 이벤트 == 1;
        public int ANPC_exist; //ANPC 등장 여부
    }

    //이거 나중에 아이템 스크립트에서 가져오는걸로 수정해야 함
    public enum ItemType
    {
        Recover,
        Mob,
        Weapon
    }

    //이것도 목표 스크립트에서 가져오는걸로
    public enum GoalType
    {
        Item,
        Report,
        Monster
    }
    public place[] map = new place[14];
    public string PNPC_place;
    public int curr_place;
    public int place_idx;
    private OpenAIApi openai = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();
    private ChatMessage input_msg = new ChatMessage();

    void Awake()
    {
        // 씬이 바뀔 때 파괴되지 않음
        DontDestroyOnLoad(this.gameObject);

        if (mapinfo == null)
        {
            mapinfo = this;
        }
    }

    //맵 생성 함수 (GPT 사용하지 않는 기능)
    public void DrawMap()
    {
        //PNPC 장소 생성
        place_idx = 0;
        CreatePlace();
        place_idx++;
        // 모든 챕터의 장소 설정
        ChooseEventType();
        IsANPCexists();
        ChooseItemType();
    }
    public void setBackground(string time, string space, string gen)
    {
        time_background = time;
        space_background = space;
        genre = gen;
        CreatePlace();
    }

    //목표 or 일반 이벤트 여부 정하기
    public void ChooseEventType()
    {
        Random random = new Random();
        int flag = 0;
        while (place_idx < 13)
        {
            flag = random.Next(3);
            if (flag == 0)
                map[place_idx].event_type = 1;
            else
                map[place_idx].event_type = 0;
            if (map[place_idx].event_type == 1) //100
            {
                map[place_idx + 1].event_type = 0;
                map[place_idx + 2].event_type = 0;
                place_idx += 3;
                continue;
            }
            else
            {
                place_idx++;
                map[place_idx].event_type = random.Next(2);
                if (map[place_idx].event_type == 1) //010
                    map[place_idx + 1].event_type = 0;
                else
                    map[place_idx + 1].event_type = 1; //001
                place_idx += 2;
            }
        }
        if (place_idx == 13) //최종 에필로그 보스방
        {
            map[place_idx].item_type = "NULL";
            map[place_idx].event_type = 1;
            map[place_idx].ANPC_exist = 0;
        }
    }
    private async void CreateItem()
    {
        Debug.Log(">>Call Create ITEM GPT");
        Debug.Log(">>현재 장소 인덱스: " + place_idx);
        gpt_messages.Clear();

        if (place_idx == 0) //장소 인덱스가 0일 경우 PNPC 장소 생성
        {
            var prompt_msg = new ChatMessage()
            {
                Role = "system",
                Content = @"당신은 조력자 NPC가 머무는 장소를 제시한다.
            다음은 게임의 배경인 
            " + time_background + "시대" + space_background + "를 배경으로 하는 세계관에 대한 설명이다." + world_detail +
                @"장소는 해당 게임의 조력자 NPC의 집 혹은 직장으로 생성되며 조력자 NPC의 정보는 다음과 같다." + PNPC_info + //이거 변수 어디서 가져오는지 확인
                @"장소 생성 양식은 아래와 같다. 각 줄의 요소는 반드시 모두 포함되어야 하며, 답변할 때 줄바꿈을 절대 하지 않는다. ** 이 표시 안의 내용은 문맥에 맞게 채운다.


            장소명: *장소 이름을 한 단어로 출력*
            장소설명: *장소에 대한 설명을 50자 내외로 설명 * "
            };
        }
        else //장소 인덱스가 0이 아닐 경우 일반 장소 생성
        {
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
        }
        gpt_messages.Add(prompt_msg);

        var query_msg = new ChatMessage()
        {
            Role = "user",
            Content = "진행중인 게임의 " + genre + "장르와 세계관에 어울리는 장소 생성"
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
            map[place_idx] = StringToPlace(message.Content);
            curr_place = place_idx; //curr_chapter가 어디에서 i++되는 변수인지 확인하기
            if (curr_place != 1)
            {
                //StartCoroutine(PostChapterObjective(curr_chapter));
            }

        }
        else
        {
            Debug.LogWarning("No text was generated from this prompt.");
        }

    }
    private async void CreateEventTrigger()
    {

    }

    //아이템 타입 결정 함수
    public static void ChooseItemType()
    {
        int i = 0;
        Random random = new Random();
        while (i < 13)
        {
            if (map[i].event_type == 0) //일반 이벤트일 경우
            {
                //enum의 모든 값 리스트로 가져오기 : Recover, Mob, Weapon
                List<string> values = new List<string>(Enum.GetNames(typeof(ItemType)));
                //null 항목 4개 추가
                for (int idx = 0; idx < 4; idx++)
                    values.Add("NULL");
                //돌려돌려돌림판
                int randomIdx = random.Next(values.Count);

                map[i].item_type = values[randomIdx];
            }
            else //목표 이벤트일 경우
            {
                //enum의 모든 값 리스트로 가져오기 : Item, Report, Monster
                List<string> values = new List<string>(Enum.GetNames(typeof(GoalType)));
                //돌려돌려돌림판
                int randomIdx = random.Next(values.Count);

                map[i].item_type = values[randomIdx];
            }
            i++;
        }
    }

    //ANPC 미등장 == 0, 등장 == 1 (목표이벤트일 경우 무조건 0)
    public void IsANPCexists()
    {
        Random random = new Random();
        int i = 1;
        while (i < 13)
        {
            if (map[i].event_type == 1)
                map[i].ANPC_exist = 0;
            else
                map[i].ANPC_exist = random.Next(2);
            i++;
        }
    }
    private async void CreatePlace(int place_idx)
    {
        Debug.Log(">>Call Create Place GPT");
        Debug.Log(">>현재 장소 인덱스: " + place_idx);
        gpt_messages.Clear();

        if (place_idx == 0) //장소 인덱스가 0일 경우 PNPC 장소 생성
        {
            var prompt_msg = new ChatMessage()
            {
                Role = "system",
                Content = @"당신은 조력자 NPC가 머무는 장소를 제시한다.
            다음은 게임의 배경인 
            " + time_background + "시대" + space_background + "를 배경으로 하는 세계관에 대한 설명이다." + world_detail +
                @"장소는 해당 게임의 조력자 NPC의 집 혹은 직장으로 생성되며 조력자 NPC의 정보는 다음과 같다." + PNPC_info + //이거 변수 어디서 가져오는지 확인
                @"장소 생성 양식은 아래와 같다. 각 줄의 요소는 반드시 모두 포함되어야 하며, 답변할 때 줄바꿈을 절대 하지 않는다. ** 이 표시 안의 내용은 문맥에 맞게 채운다.


            장소명: *장소 이름을 한 단어로 출력*
            장소설명: *장소에 대한 설명을 50자 내외로 설명 * "
            };
        }
        else //장소 인덱스가 0이 아닐 경우 일반 장소 생성
        {
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
        }
        gpt_messages.Add(prompt_msg);

        var query_msg = new ChatMessage()
        {
            Role = "user",
            Content = "진행중인 게임의 " + genre + "장르와 세계관에 어울리는 장소 생성"
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
            map[place_idx] = StringToPlace(message.Content);
            curr_place = place_idx; //curr_chapter가 어디에서 i++되는 변수인지 확인하기
            if (curr_place != 1)
            {
                //StartCoroutine(PostChapterObjective(curr_chapter));
            }

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
