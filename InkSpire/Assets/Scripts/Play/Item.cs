using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OpenAI;
using System.Threading.Tasks;

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
            return;
        }
        
        if (type == ItemType.Report)
        {
            await CreateReportName(goal.GetTitle(), goal.GetDetail(), goal.GetEtc());
            info = goal.GetEtc();
            return;
        }
        
        if (type == ItemType.Recover || type == ItemType.Mob || type == ItemType.Weapon)
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

    private async Task CreateReportName(string goal_title, string goal_detail, string goal_etc)
    {
        gpt_messages.Clear();

        var prompt_msg = new ChatMessage()
        {
            Role = "system",
            Content = @"당신은 목표, 목표 설명, 비고 텍스트를 분석하여 보고서 이름을 한 단어로 제시한다. 출력은 반드시 한글표기만 나타낸다.    
ex 1)
목표:패러모어 마을 주변의 '이상한 소리'에 대한 정보 습득
목표 설명: 패러모어 마을 주변에서 최근 들려오는 이상한 소리에 대한 정보를 수집해야 합니다. 이 이상한 소리는 주변 주민들을 불안하게 만들고 있으며, 이 사건이 마을의 안전과 관련이 있을 수 있습니다.
비고: 이상한 소리는 주로 금요일 밤에만 들려오며, 어디에서 들려오는지 그 방향이 모호하지만 몇몇 사람들은 그 소리가 들릴 때 도시의 바닥에서 수상한 울림이 느껴진다고 하였다.
보고서 이름: 이상한 소리 보고서 

ex 2)
목표: 신비로운 어둠의 홀로그램 정보 습득
목표 설명: 과학 연구소에서 제작된 신비로운 어둠의 홀로그램에 대한 정보를 수집해야 합니다. 이 홀로그램은 도시를 위협할 수 있는 위험한 기술이며, 어떤 목적으로 만들어 졌는지 자세한 내용을 파악해야 합니다.
비고: 신비로운 어둠의 홀로그램은 노바시티의 가장 미래 지향적인 기술 중 하나로, 그 존재 자체가 도시를 위협할 수 있는 위험요인으로 작용할 수 있습니다. 이는 도시 사람들을 모두 세뇌하여 통제하기 위한 목적으로 제작되었으며, 곧 테스트 시행을 앞두고 있어 도시 사람들을 구하려면 이를 저지해야합니다.
보고서 이름: 어둠의 홀로그램 보고서

ex 3)
목표: 왕국 내 미스터리한 인물 '은밀한 꽃파는 자'에 대한 정보 습득
목표 설명: 왕국 내를 돌아다니며 소문으로만 듣던 '은밀한 꽃파는 자'에 대한 실체 정보를 알아내야 합니다. 그의 정체와 이유, 사람들이 그를 믿고 따르는 이유를 탐구하여 왕국 내 은밀한 이야기를 밝혀내야 합니다.
비고: 왕국 사람들은 꽃파는 자의 꽃 한 송이와 함께 받는 메시지가 좌절한 마음을 치유하고 소망을 심어주는 것으로 알고있다. 꽃을 파는 자는 이 메시지 카드에 마법을 걸어 사람들을 홀리고 있었다.
보고서 이름: 꽃파는 자 보고서"
        };
        gpt_messages.Add(prompt_msg);

        var query_msg = new ChatMessage()
        {
            Role = "user",
            Content = "목표: " + goal_title + "\n목표 설명: " + goal_detail + "\n비고: " + goal_etc + "\n보고서 이름: "
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
다음은 게임의 배경인 " + time_background + " 시대 " + space_background + "를 배경으로 하는 세계관에 대한 설명이다.\n" + world_detail +
            @"\n당신이 생성해야할 아이템 정보는 " + info + "이다."
        };
        gpt_messages.Add(prompt_msg);

        var query_msg = new ChatMessage()
        {
            Role = "user",
            Content = "진행중인 게임에 필요한 " + info + " 이름을 한 단어로 생성"
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
