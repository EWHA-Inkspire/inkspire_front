using System;
using System.Collections;
using System.Collections.Generic;
using OpenAI;
using UnityEngine;
using UnityEngine.Networking;

public class MapManager : MonoBehaviour
{
    public static MapManager mapinfo;
    public struct place
    {
        public string place_name; //장소 이름
        public string place_info; //장소 설명
        public string item_name; //아이템 이름 
        public string item_info; //아이템 설명
        public string item_type; //일반 이벤트일 경우 아이템, 목표 이벤트일 경우 목표
        public string event_trigger; //이벤트 트리거
        public int event_type; //일반 이벤트 == 0, 목표 이벤트 == 1;
        public int ANPC_exist; //ANPC 등장 여부
        public bool clear; //파싱용 클리어 속성
    }

    public enum ItemType
    {
        Recover,
        Mob,
        Weapon
    }

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
    public int chapter_idx;
    private OpenAIApi openai = new OpenAIApi();
    private List<ChatMessage> gpt_messages = new List<ChatMessage>();

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
        CreatePlace(place_idx);
        //chapter index 설정
        if (place_idx > 0 && place_idx < 4)
            chapter_idx = 1;
        else if (place_idx > 3 && place_idx < 7)
            chapter_idx = 2;
        else if (place_idx > 6 && place_idx < 10)
            chapter_idx = 3;
        else if (place_idx > 9 && place_idx < 13)
            chapter_idx = 4;
        else if (place_idx == 13)
            chapter_idx = 5;
        place_idx++;
        // 모든 챕터의 장소 설정
        ChooseEventType();
        IsANPCexists();
        ChooseItemType();
    }
    // public void setBackground(string time, string space, string gen)
    // {
    //     time_background = time;
    //     space_background = space;
    //     genre = gen;
    //     CreatePlace();
    // }

    //목표 or 일반 이벤트 여부 정하기
    public void ChooseEventType()
    {
        //Random random = new Random();
        int i = 1;
        int flag = 0;
        while (i < 13)
        {
            flag = UnityEngine.Random.Range(0, 3);
            if (flag == 0)
                map[i].event_type = 1;
            else
                map[i].event_type = 0;
            if (map[i].event_type == 1) //100
            {
                map[i + 1].event_type = 0;
                map[i + 2].event_type = 0;
                i += 3;
                continue;
            }
            else
            {
                i++;
                map[i].event_type = UnityEngine.Random.Range(0, 2);
                if (map[i].event_type == 1) //010
                    map[i + 1].event_type = 0;
                else
                    map[i + 1].event_type = 1; //001
                i += 2;
            }
        }
        if (i == 13) //최종 에필로그 보스방
        {
            map[i].item_type = "NULL";
            map[i].event_type = 1;
            map[i].ANPC_exist = 0;
        }
    }
    private async void CreateItem(int place_idx)
    {
        string timeBackground = ScriptManager.scriptinfo.time_background;
        string spaceBackground = ScriptManager.scriptinfo.space_background;
        string worldDetail = ScriptManager.scriptinfo.world_detail;

        Debug.Log(">>Call Create ITEM GPT");
        Debug.Log(">>현재 장소 인덱스: " + place_idx);
        gpt_messages.Clear();
        string about_item = "";
        if (map[place_idx].item_type == "Recover")
            about_item = "플레이어의 HP를 회복시켜주는 아이템";
        else if (map[place_idx].item_type == "Mob")
            about_item = "잡몹을 처치했을 때 얻는 보상 아이템";
        else if (map[place_idx].item_type == "Weapon")
            about_item = "플레이어의 공격력을 높여주는 무기 아이템";
        map[place_idx].item_info = about_item;

        var prompt_msg = new ChatMessage()
        {
            Role = "system",
            Content = @"당신은 게임 진행에 필요한 아이템의 이름을 한 단어로 제시한다.
            다음은 게임의 배경인 
            " + timeBackground + "시대" + spaceBackground + "를 배경으로 하는 세계관에 대한 설명이다." + worldDetail +
            @"당신이 생성해야할 아이템은" + about_item + "이다."
        };
        gpt_messages.Add(prompt_msg);

        var query_msg = new ChatMessage()
        {
            Role = "user",
            Content = "진행중인 게임에 필요한" + about_item + "생성"
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
            map[place_idx].item_name = message.Content;
            curr_place = place_idx;
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
    private async void CreateEventTrigger(int place_idx)
    {
        string worldDetail = ScriptManager.scriptinfo.world_detail;

        Debug.Log(">>Call Create EventTrigger GPT");
        Debug.Log(">>현재 장소 인덱스: " + place_idx);
        gpt_messages.Clear();
        if (map[place_idx].item_type == "Mob")
            return;
        var prompt_msg = new ChatMessage()
        {
            Role = "system",
            Content = @"당신은 챕터 목표에 맞는 게임 아이템의 위치를 생성한다. 챕터 목표는" + ScriptManager.scriptinfo.chapter_obj + "이며 게임의 세계관 배경은 다음과 같다." + worldDetail
            + "플레이어가 현재 위치한 장소 이름은" + map[place_idx].place_name + "이며 이 장소에서 게임 아이템인" + map[place_idx].item_name + @"가 존재하는 위치를 생성한다. 
            위치의 이름은 장소 이름 및 게임 아이템과 자연스럽게 어울려야 하며 반드시 한 단어로 출력한다." // 장소 이름, 아이템 이름, 월드디테일 전달, 챕터목표 -> 이 물건이 있을만한 위치를 생성  
        };
        gpt_messages.Add(prompt_msg);

        var query_msg = new ChatMessage()
        {
            Role = "user",
            Content = "아이템이 존재하는 위치를 한 단어로 생성"
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
            map[place_idx].event_trigger = message.Content; //이거 파싱 어케할지 고민
            curr_place = place_idx;
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

    //아이템 타입 결정 함수
    // public static void ChooseItemType()
    // {
    //     int i = 0;
    //     Random random = new Random();
    //     while (i < 13)
    //     {
    //         if (map[i].event_type == 0) //일반 이벤트일 경우
    //         {
    //             //enum의 모든 값 리스트로 가져오기 : Recover, Mob, Weapon
    //             List<string> values = new List<string>(Enum.GetNames(typeof(ItemType)));
    //             //null 항목 4개 추가
    //             for (int idx = 0; idx < 4; idx++)
    //                 values.Add("NULL");
    //             //돌려돌려돌림판
    //             int randomIdx = random.Next(values.Count);

    //             map[i].item_type = values[randomIdx];
    //         }
    //         else //목표 이벤트일 경우
    //         {
    //             //enum의 모든 값 리스트로 가져오기 : Item, Report, Monster
    //             List<string> values = new List<string>(Enum.GetNames(typeof(GoalType)));
    //             //돌려돌려돌림판
    //             int randomIdx = random.Next(values.Count);

    //             map[i].item_type = values[randomIdx];
    //         }
    //         i++;
    //     }
    // }

    public void ChooseItemType()
    {
        int i = 0;
        while (i < 13)
        {
            if (map[i].event_type == 0) //일반 이벤트일 경우
            {
                // 일반 이벤트일 때의 항목 정의
                ItemType?[] normalEventItems = { ItemType.Recover, ItemType.Mob, ItemType.Weapon, null, null, null, null };

                //돌려돌려돌림판
                int randomIdx = UnityEngine.Random.Range(0, normalEventItems.Length);

                map[i].item_type = normalEventItems[randomIdx]?.ToString(); // 열거형을 문자열로 변환하여 할당
            }
            else //목표 이벤트일 경우
            {
                // 목표 이벤트일 때의 항목 정의
                GoalType[] goalEventItems = { GoalType.Item, GoalType.Report, GoalType.Monster };

                //돌려돌려돌림판
                int randomIdx = UnityEngine.Random.Range(0, goalEventItems.Length);

                map[i].item_type = goalEventItems[randomIdx].ToString(); // 열거형을 문자열로 변환하여 할당
            }
            i++;
        }
    }


    //ANPC 미등장 == 0, 등장 == 1 (목표이벤트일 경우 무조건 0)
    public void IsANPCexists()
    {
        // Random random = new Random();
        int i = 1;
        while (i < 13)
        {
            if (map[i].event_type == 1)
                map[i].ANPC_exist = 0;
            else
                map[i].ANPC_exist = UnityEngine.Random.Range(0, 2);
            i++;
        }
    }
    private async void CreatePlace(int place_idx)
    {
        string timeBackground = ScriptManager.scriptinfo.time_background;
        string spaceBackground = ScriptManager.scriptinfo.space_background;
        string worldDetail = ScriptManager.scriptinfo.world_detail;
        string genre = ScriptManager.scriptinfo.genre;

        Debug.Log(">>Call Create Place GPT");
        Debug.Log(">>현재 장소 인덱스: " + place_idx);
        gpt_messages.Clear();

        ChatMessage prompt_msg;
        if (place_idx == 0) //장소 인덱스가 0일 경우 PNPC 장소 생성
        {
            prompt_msg = new ChatMessage()
            {
                Role = "system",
                Content = @"당신은 조력자 NPC가 머무는 장소를 제시한다.
            다음은 게임의 배경인 
            " + timeBackground + "시대" + spaceBackground + "를 배경으로 하는 세계관에 대한 설명이다." + worldDetail +
                @"장소는 해당 게임의 조력자 NPC의 집 혹은 직장으로 생성되며 조력자 NPC의 정보는 다음과 같다." + "PNPC_info" +
                @"장소 생성 양식은 아래와 같다. 각 줄의 요소는 반드시 모두 포함되어야 하며, 답변할 때 줄바꿈을 절대 하지 않는다. ** 이 표시 안의 내용은 문맥에 맞게 채운다.


            장소명: *장소 이름을 한 단어로 출력*
            장소설명: *장소에 대한 설명을 50자 내외로 설명 * "
            };
        }
        else //장소 인덱스가 0이 아닐 경우 일반 장소 생성
        {
            prompt_msg = new ChatMessage()
            {
                Role = "system",
                Content = @"당신은 게임 진행에 필요한 장소를 제시한다.
            다음은 게임의 배경인 
            " + timeBackground + "시대" + spaceBackground + "를 배경으로 하는 세계관에 대한 설명이다." + worldDetail +
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
