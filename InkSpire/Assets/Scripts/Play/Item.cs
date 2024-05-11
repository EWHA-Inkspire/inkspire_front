using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OpenAI;
using System.Threading.Tasks;
using UnityEngine;

public enum ItemType
{
    // 일반 아이템 유형
    Recover,
    Mob,
    Weapon,
    // 목표 아이템 유형
    Item,
    Report,
    Monster,
    Null
}

public class Item
{
    private List<ChatMessage> gpt_messages = new();

    // 특수문자, 괄호, 점 제거를 위한 정규 표현식
    readonly Regex regex = new("[`~!@#$%^&*()_|+\\-=?;:'\",.<>{}[\\]\\\\/]", RegexOptions.IgnoreCase);
    public int id; // 아이템 아이디
    public string name; // 아이템 이름
    public string info; // 아이템 설명
    public ItemType type = ItemType.Null; // 아이템 타입
    public int stat; // 아이템 기능치

    public async Task InitItem(Script script, Goal goal, int event_type)
    {
        // 아이템 타입 설정
        ChooseItemType(goal.GetGoalType(), event_type);
        // 아이템 스탯 설정
        if (type != ItemType.Null) {
            stat = UnityEngine.Random.Range(1, 6);
        }
        // 아이템 이름 설정
        if (type == ItemType.Item) 
        {
            name = goal.GetEtc();
            info = goal.GetDetail();
        }
        else if (type == ItemType.Report)
        {
            await CreateReportName(goal.GetEtc());
        }
        else if (type == ItemType.Recover || type == ItemType.Mob && type == ItemType.Weapon)
        {
            await CreateItemName(script);
        }
    }

    public void ChooseItemType(int goal_type, int event_type)
    {
        if (event_type == 0) //일반 이벤트일 경우
        {
            // 일반 이벤트일 때의 항목 정의
            ItemType[] normalEventItems = { ItemType.Recover, ItemType.Weapon, ItemType.Mob, ItemType.Null, ItemType.Null, ItemType.Null, ItemType.Null };
            int randomIdx = UnityEngine.Random.Range(0, normalEventItems.Length);

            type = normalEventItems[randomIdx]; // 열거형을 문자열로 변환하여 할당
        }
        else //목표 이벤트일 경우
        {
            // 목표 이벤트일 때의 항목 정의
            ItemType[] goalEventItems = { ItemType.Monster, ItemType.Item, ItemType.Report  };

            type = goalEventItems[goal_type-1]; // 열거형을 문자열로 변환하여 할당
        }
    }

    private async Task CreateReportName(string goal_etc)
    {
        gpt_messages.Clear();

        var prompt_msg = new ChatMessage()
        {
            Role = "system",
            Content = @"당신은 사건의 진상 내용을 요약하여 아이템 이름에 걸맞는 한 단어로 제시한다. 또한, 출력의 영어표기를 생략하고 한글표기만 나타낸다.
            사건의 진상: "+goal_etc
        };
        gpt_messages.Add(prompt_msg);

        var query_msg = new ChatMessage()
        {
            Role = "user",
            Content = "사건의 진상 내용을 요약하여 한 단어로 생성"
        };
        gpt_messages.Add(query_msg);

        string response = await GptManager.gpt.CallGpt(gpt_messages);

        name = regex.Replace(response, "");
    }

    private async Task CreateItemName(Script script)
    {
        string time_background = script.GetTimeBackground();
        string space_background = script.GetSpaceBackground();
        string world_detail = script.GetWorldDetail();

        gpt_messages.Clear();
        string about_item = "";
        if (type == ItemType.Recover)
            info = "플레이어의 HP를 회복시켜주는 아이템";
        else if (type == ItemType.Mob)
            info = "잡몹을 처치했을 때 얻는 보상 아이템";
        else if (type == ItemType.Weapon)
            info = "플레이어의 공격력을 높여주는 무기 아이템";

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

        name = regex.Replace(response, "");
    }

    // Setter
    public void SetItemInfo(GetItemInfo itemInfo)
    {
        id = itemInfo.itemId;
        name = itemInfo.name;
        info = itemInfo.info;
        type = (ItemType)Enum.Parse(typeof(ItemType), itemInfo.type);
        stat = itemInfo.stat;
    }
}
