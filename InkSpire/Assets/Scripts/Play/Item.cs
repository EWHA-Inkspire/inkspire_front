using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OpenAI;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class Item
{
    private List<ChatMessage> gpt_messages = new List<ChatMessage>();

    // 특수문자, 괄호, 점 제거를 위한 정규 표현식
    Regex regex = new Regex("[`~!@#$%^&*()_|+\\-=?;:'\",.<>{}[\\]\\\\/]", RegexOptions.IgnoreCase);

    public string item_name; // 아이템 이름
    public string item_info; // 아이템 설명
    public string item_type; // 아이템 타입
    public int item_stat; // 아이템 기능치
    public enum ItemType
    {
        Recover,
        Mob,
        Weapon,
        Item,
        Report,
        Monster
    }

    public async Task InitItem(string time_background, string space_background, string world_detail, int event_type)
    {
        ChooseItemType(event_type);
        ItemStat();
        // 전투 이벤트(잡몹, 적 처치) 혹은 item_type이 null일 경우에는 이벤트 트리거 생성하지 않음
        if (item_type != "Mob" && item_type != "Monster" && item_type != null)
        {
            await CreateItem(time_background, space_background, world_detail);
        }
    }

    public void ChooseItemType(int event_type)
    {
        if (event_type == 0) //일반 이벤트일 경우
        {
            // 일반 이벤트일 때의 항목 정의
            ItemType?[] normalEventItems = { ItemType.Recover, ItemType.Weapon, ItemType.Mob, null, null, null, null };
            int randomIdx = UnityEngine.Random.Range(0, normalEventItems.Length);

            item_type = normalEventItems[randomIdx]?.ToString(); // 열거형을 문자열로 변환하여 할당
        }
        else //목표 이벤트일 경우
        {
            // 목표 이벤트일 때의 항목 정의
            ItemType[] goalEventItems = { ItemType.Item, ItemType.Report, ItemType.Monster };

            //돌려돌려돌림판 -> TODO: 챕터 목표의 유형을 받아와야 함
            int randomIdx = UnityEngine.Random.Range(0, goalEventItems.Length);

            item_type = goalEventItems[randomIdx].ToString(); // 열거형을 문자열로 변환하여 할당
        }
    }

    //아이템 기능치: 1~5 사이의 정수 랜덤 생성
    public void ItemStat()
    {
        int i = 1;
        while (i < 13)
        {
            if (item_type != "NULL")
            {
                item_stat = UnityEngine.Random.Range(1, 6);
            }
            i++;
        }
    }
    private async Task CreateItem(string time_background, string space_background, string world_detail)
    {
        gpt_messages.Clear();
        string about_item = "";
        if (item_type == "Recover")
            about_item = "플레이어의 HP를 회복시켜주는 아이템";
        else if (item_type == "Mob")
            about_item = "잡몹을 처치했을 때 얻는 보상 아이템";
        else if (item_type == "Weapon")
            about_item = "플레이어의 공격력을 높여주는 무기 아이템";
        item_info = about_item;

        var prompt_msg = new ChatMessage()
        {
            Role = "system",
            Content = @"당신은 게임 진행에 필요한 아이템의 이름을 한 단어로 제시한다. 또한, 출력의 영어표기를 생략하고 한글표기만 나타낸다.
            다음은 게임의 배경인 
            " + time_background + " 시대 " + space_background + "를 배경으로 하는 세계관에 대한 설명이다. " + world_detail +
            @" 당신이 생성해야할 아이템은 " + about_item + "이다."
        };
        gpt_messages.Add(prompt_msg);

        var query_msg = new ChatMessage()
        {
            Role = "user",
            Content = "진행중인 게임에 필요한 " + about_item + " 이름을 한 단어로 생성"
        };
        gpt_messages.Add(query_msg);

        string response = await GptManager.gpt.CallGpt(gpt_messages);

        item_name = regex.Replace(response, "");
    }
}
