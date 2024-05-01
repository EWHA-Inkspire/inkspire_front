using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OpenAI;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class Place
{
    // 특수문자, 괄호, 점 제거를 위한 정규 표현식
    Regex regex = new Regex("[`~!@#$%^&*()_|+\\-=?;:'\",.<>{}[\\]\\\\/]", RegexOptions.IgnoreCase);

    private List<ChatMessage> gpt_messages = new List<ChatMessage>();
    public string place_name = ""; //장소 이름
    public string place_info = ""; //장소 설명
    public Item item; //아이템 
    public Event game_event; //이벤트
    public int ANPC_exist; //ANPC 등장 여부
    public bool clear; //파싱용 클리어 속성

    public Place()
    {
        game_event = new Event();
        item = new Item();
    }

    public async Task InitPlace(int idx, Script script, Npc pro_npc, string chapter_obj, string[] place_names)
    {
        string time_background = script.GetTimeBackground();
        string space_background = script.GetSpaceBackground();
        string world_detail = script.GetWorldDetail();
        string genre = script.GetGenre();
        string pnpc_name = pro_npc.GetName();
        string pnpc_detail = pro_npc.GetDetail();

        await item.InitItem(time_background, space_background, world_detail, game_event.event_type);
        IsANPCexists();
        await CreatePlace(idx, time_background, space_background, world_detail, genre, pnpc_name, pnpc_detail,place_names);

        // 전투 이벤트(잡몹, 적 처치) 혹은 item_type이 null일 경우에는 이벤트 트리거 생성하지 않음
        if (item.item_type != ItemType.Mob && item.item_type != ItemType.Monster && item.item_type != ItemType.Null)
        {
            await game_event.CreateEventTrigger(idx, world_detail, chapter_obj, place_name, item.item_name);
        }
    }

    //ANPC 미등장 == 0, 등장 == 1 (목표이벤트일 경우 무조건 0)
    public void IsANPCexists()
    {
        if (game_event.event_type == 1)
        {
            ANPC_exist = 0;
        }
        else
        {
            ANPC_exist = UnityEngine.Random.Range(0, 2);
        }
    }
    public async Task CreatePlace(int idx, string time_background, string space_background, string world_detail, string genre, string pnpc_name, string pnpc_detail,string[] place_names)
    {
        gpt_messages.Clear();

        ChatMessage prompt_msg;
        if (idx == 0) //장소 인덱스가 0일 경우 PNPC 장소 생성
        {
            prompt_msg = new ChatMessage()
            {
                Role = "system",
                Content = @"당신은 조력자 NPC가 머무는 장소를 제시한다.
            다음은 게임의 배경인 
            " + time_background + "시대 " + space_background + "를 배경으로 하는 세계관에 대한 설명이다. " + world_detail +
                @" 장소는 해당 게임의 조력자 NPC의 집 혹은 직장으로 생성되며 조력자 NPC의 정보는 다음과 같다. " +
                "이름은 " + pnpc_name + "이며, " + pnpc_detail +
                @" 장소 생성 양식은 아래와 같다. 각 줄의 요소는 반드시 모두 포함되어야 하며, 답변할 때 줄바꿈을 절대 하지 않는다. 또한, 출력의 영어표기를 생략하고 한글표기만 나타낸다. ** 이 표시 안의 내용은 문맥에 맞게 채운다.


            장소명: *장소 이름을 한 단어로 출력*
            장소설명: *장소에 대한 설명을 50자 내외로 설명, 어미는 입니다 체로 통일합니다.* "
            };
        }
        else //장소 인덱스가 0이 아닐 경우 일반 장소 생성
        {
            prompt_msg = new ChatMessage()
            {
                Role = "system",
                Content = @"당신은 게임 진행에 필요한 장소를 제시한다.
            다음은 게임의 배경인 
            " + time_background + "시대" + space_background + "를 배경으로 하는 세계관에 대한 설명이다." + world_detail +
                @"장소는 게임의 배경에 맞추어 플레이어가 흥미롭게 탐색할 수 있는 곳으로 생성된다. 장소 생성 양식은 아래와 같다. 각 줄의 요소는 반드시 모두 포함되어야 하며, 답변할 때 줄바꿈을 절대 하지 않는다. ** 이 표시 안의 내용은 문맥에 맞게 채운다.
            
            장소명: *장소 이름을 한 단어로 출력*
            장소설명: *장소에 대한 설명을 50자 내외로 설명, 어미는 입니다 체로 통일합니다.*"
            };
        }
        gpt_messages.Add(prompt_msg);

        var query_msg = new ChatMessage()
        {
            Role = "user",
            Content = "와 장소 이름이 중복되지 않도록 진행중인 게임의 " + genre + " 장르와 세계관에 어울리는 장소 생성. 장소 이름은 절대 중복되어서는 안된다."
        };
        for (int i = 0; i < idx; i++)
        {
            if (i != 0)
            {
                query_msg.Content = place_names[i] + ", " + query_msg.Content;
            }
            else
            {
                query_msg.Content = place_names[i] + query_msg.Content;
            }
        }

        gpt_messages.Add(query_msg);
        StringToPlace(await GptManager.gpt.CallGpt(gpt_messages));
    }

    //장소 이름 및 장소 설명 파싱 함수
    public void StringToPlace(string place_string)
    {
        clear = false;

        string[] place_arr;
        place_string = place_string.Replace("\n\n", ":");
        place_string = place_string.Replace(":\n", ":");
        place_string = place_string.Replace("\n", ":");
        place_string = place_string.Replace(": ", ":");

        place_arr = place_string.Split(':');
        place_name = regex.Replace(place_arr[1], "");
        place_name = place_name.Trim();
        place_info = place_arr[3];
    }

    public void SetClear(bool clr){
        clear = clr;
    }
}
