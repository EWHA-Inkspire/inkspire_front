using System.Collections.Generic;
using OpenAI;
using System.Threading.Tasks;

public class Goal
{
    private int id;
    private int type;
    private string title = "";
    private string detail;
    private string etc;
    private bool clear;

    private List<ChatMessage> gpt_messages = new();

    public async Task InitGoal(string time_background, string space_background, string world_detail, string genre)
    {
        // 최종 목표 생성자(챕터4)
        await FinalObjectiveGPT(time_background,space_background,world_detail,genre);
    }

    public async Task InitGoal(string time_background, string space_background, string world_detail, string genre, Goal final_obj)
    {
        // 챕터 목표 생성자(챕터0)
        Goal tmp = new();
        await ChapterObjectiveGPT(time_background,space_background,world_detail,genre,0,final_obj,tmp);
    }

    public async void InitGoal(string time_background, string space_background, string world_detail, string genre, Goal final_obj, Goal prev_obj){
        // 챕터 목표 생성자(챕터1~3)
        await ChapterObjectiveGPT(time_background,space_background,world_detail,genre,1,final_obj,prev_obj);
    }


// 최종목표 생성 함수
    private async Task FinalObjectiveGPT(string time_background, string space_background, string world_detail, string genre)
    {
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

다음은 게임의 배경인 "+time_background+"시대 "+space_background+"를 배경으로 하는 세계관에 대한 설명이다.\n"+world_detail+
@"아래와 같은 양식으로 게임의 목표를 설정한다. 각 줄의 요소는 반드시 모두 포함되어야 하며, 답변할 때 줄바꿈을 절대 하지 않는다. ** 이 표시 안의 내용은 문맥에 맞게 채운다.

최종목표유형: *위의 유형 1~3 중 하나를 숫자만 출력*
최종목표: *최종 목표의 제목 출력*
최종목표 설명: *최종 목표 설명*
비고:*반드시 해당하는 최종목표 유형에 제시된 형식과 내용에 맞춰 출력*

---
다음은 출력 예시이다. 

최종목표유형: 1
최종목표: 악마군주 말라카르 처치
최종목표 설명: 악마군주 말라카르는 에스텔리아 대륙을 저주하고 암흑의 힘을 휘두르는 강력한 악당입니다. 그의 힘을 막기 위해 그를 물리치고 대륙을 구원해야 합니다.
비고: 악마군주 말라카르

최종목표유형:1
최종목표: 다크프레스트 숲 깊은 곳에 서식하는 음산한 숲정령 처치
최종목표 설명: 다크프레스트 숲의 깊은 곳에는 음산한 숲정령이 사람들을 위협하고 있습니다. 그를 처치하여 숲을 안전하게 만들어야 합니다.
비고: 음산한 숲정령

최종목표유형:2
최종목표: 저주받은 검은 알파카의 뿔
최종목표 설명: 서부의 황야 속에 서식하는 악령 알파카의 뿔은 전설적인 힘을 지니고 있다. 이를 손에 넣어야만 마을을 구할 수 있다.
비고: 알파카의 뿔

최종목표유형: 2
최종목표: 신비로운 어둠의 홀로그램
최종목표 설명: 노바시티의 중심부에 위치한 과학 연구소에서 제작된 신비로운 어둠의 홀로그램은 현실과 가상이 융합된 기술력을 갖춘 미래 도시를 위협할 수 있는 위험한 물건입니다. 이 홀로그램을 획득하여 도시를 위험에서 구해야 합니다.
비고: 어둠의 홀로그램

최종목표유형: 3
최종목표: 패러모어 마을의 비밀을 밝힌다
최종목표 설명: 패러모어 마을에는 수많은 비밀이 숨어 있습니다. 이러한 비밀을 파헤쳐 마을의 진실을 밝히고 주변 사건들의 근원을 찾아내야 합니다.
비고: 사실 이 마을의 숨겨진 지하 공간에서는 사이비 종교 단체의 비밀스런 의식이 진행되고 있다. 빈민가의 사람들을 납치하여 제물로 바쳐 이루어지는 의식의 피해는 점점 넓어지고 있다. 범인은 이 지역의 의원으로 교주와 내통해 경제적 이득과 도시 청소를 이루려 한다.

최종목표유형: 3
최종목표: 사이버 시티 콘퓨전의 의심스러운 의원을 밝힌다
최종목표 설명: 사이버 시티 콘퓨전에는 각종 범죄와 의심스러운 활동이 빈번히 발생한다. 이를 조사하여 도시를 위협하는 의원의 음모를 밝혀내야 합니다.
비고: 시민들이 안전을 위협하는 범죄와 음모를 일으키는 사이버 갱단은 도시의 안전을 위협하고 있다. '도미스' 의원은 사이버 갱단과 손을 잡고 시민들을 위험에 빠뜨리고 있다. 권력을 가진 사람이기 때문에 조심해야 한다.

최종목표유형:3
최종목표: 바다교시에서의 금관 악기 사건 해결
최종목표 설명: 바다교시에서 오랜 역사를 가진 신비한 금관 악기가 갑자기 사라졌습니다. 이 사건을 해결하여 도시의 안전과 조화를 되찾아야 합니다.
비고: 바다교시의 금관 악기는 세계적으로 유명한 예술작품으로, 그 소리는 마을을 안위하고 품위 있게 해주는 존재입니다. 이 마을의 악기상은 비슷한 악기들 틈에 악기를 숨겨 자신만의 이득을 취하고 있었는데, 이 악기상은 지역의 의원과 내통하여 뒷돈을 챙기는 등 부정부패를 저지르고 있어 사람들의 삶이 궁핍해지고 있다."

        };
        gpt_messages.Add(prompt_msg);
        
        var query_msg = new ChatMessage()
        {
            Role = "user",
            Content = "진행될 게임의 "+genre+"장르에 어울리는 최종 목표 생성\n\n최종목표유형:"
        };
        gpt_messages.Add(query_msg);

        StringToObjective("최종목표유형:"+await GptManager.gpt.CallGpt(gpt_messages));
        
    }

// 챕터 목표 생성 함수
    private async Task ChapterObjectiveGPT(string time_background, string space_background, string world_detail, string genre, int chapter_num, Goal final_obj, Goal prev_obj)
    {
        gpt_messages.Clear();

        var prompt_msg = new ChatMessage()
        {
            Role = "system",
            Content = @"당신은 진행되는 게임의 챕터 목표를 제시한다.
챕터 목표의 유형은 반드시 다음 세가지 중 하나이며 게임의 배경에 맞춰 하나만 생성된다.

- 챕터목표 유형 1: 적대 npc나 보스 몬스터를 처치
비고: *적의 이름 출력*

- 챕터목표 유형 2: *아이템이름* 획득
비고: *아이템 이름 출력*

- 챕터목표 유형 3: 스토리 진행에 중요한 정보 *얻어야 할 정보 제목* 습득
비고:*얻어야 할 정보의 진상을 한문장으로 출력*

다음은 게임의 배경인 "+time_background+"시대 "+space_background+"를 배경으로 하는 세계관에 대한 설명이다.\n"+world_detail+
            "게임의 최종 목표는 다음과 같다.\n"+final_obj.GetTitle()+"\n"+final_obj.GetDetail()

        };
        if(chapter_num!=0){
            prompt_msg.Content+="다음은 이전 챕터의 챕터 목표이다. 생성되는 챕터 목표는 이전 챕터의 진행과 자연스럽게 이어져야 한다."+prev_obj.GetTitle()+"\n"+prev_obj.GetDetail();
        }

        prompt_msg.Content+=@"아래와 같은 양식으로 게임의 목표를 설정한다. 각 줄의 요소는 반드시 모두 포함되어야 하며, 답변할 때 줄바꿈을 절대 하지 않는다. ** 이 표시 안의 내용은 문맥에 맞게 채운다.

챕터목표유형: *위의 유형 1~3 중 하나를 숫자만 출력*
챕터목표: *챕터 목표의 제목 출력*
챕터목표 설명: *챕터 목표 설명*
비고:*반드시 해당하는 챕터목표 유형에 제시된 형식과 내용에 맞춰 출력*

--- 
다음은 출력 예시이다. 

챕터목표유형: 1
챕터목표: 악마군주 말라카르의 군단장 중 하나, '아카이어' 처치
챕터목표 설명: 이 도시에는 말라카르의 군단장 아카이어가 사람들을 착취하고 있습니다. 그를 처치하고 마을 사람들을 도와야 합니다.
비고: 군단장 아카이어

챕터목표유형: 1
챕터목표: 사이버 시티 콘퓨전의 갱단 졸개 '벤' 처치
챕터목표 설명: 도시의 사람들은 갱단에 의해 피해를 입고 있습니다. 선량한 시민을 괴롭히고 있던 갱단의 말단 멤버 '벤'을 쓰러트려 시민을 도와야 합니다.
비고: 갱단 벤

챕터목표유형: 2
챕터목표:황야 속에서 '신비로운 약초' 획득
챕터목표 설명: 황야 깊은 곳에 자라는 신비로운 약초를 찾아야 합니다. 이 약초는 어둠의 힘이 약화시키는 효능을 지니고 있어, 앞으로의 모험에서 큰 도움이 될 것입니다.
비고: 신비로운 약초

챕터목표유형: 2
챕터목표: 오래된 사원에서 '백조의 깃털' 획득
챕터목표 설명: 산호만 해안의 오래된 사원에서 전설 속 백조가 날아다닌다는 이야기를 듣게 되었습니다. 백조의 깃털을 획득하여 이야기의 진실을 밝혀야 합니다.
비고: 백조의 깃털

챕터목표유형: 3
챕터목표:패러모어 마을 주변의 '이상한 소리'에 대한 정보 습득
챕터목표 설명: 패러모어 마을 주변에서 최근 들려오는 이상한 소리에 대한 정보를 수집해야 합니다. 이 이상한 소리는 주변 주민들을 불안하게 만들고 있으며, 이 사건이 마을의 안전과 관련이 있을 수 있습니다.
비고: 이상한 소리는 주로 금요일 밤에만 들려오며, 어디에서 들려오는지 그 방향이 모호하지만 몇몇 사람들은 그 소리가 들릴 때 도시의 바닥에서 수상한 울림이 느껴진다고 하였다. 

챕터목표유형: 3
챕터목표: 신비로운 어둠의 홀로그램 정보 습득
챕터목표 설명: 과학 연구소에서 제작된 신비로운 어둠의 홀로그램에 대한 정보를 수집해야 합니다. 이 홀로그램은 도시를 위협할 수 있는 위험한 기술이며, 어떤 목적으로 만들어 졌는지 자세한 내용을 파악해야 합니다.
비고: 신비로운 어둠의 홀로그램은 노바시티의 가장 미래 지향적인 기술 중 하나로, 그 존재 자체가 도시를 위협할 수 있는 위험요인으로 작용할 수 있습니다. 이는 도시 사람들을 모두 세뇌하여 통제하기 위한 목적으로 제작되었으며, 곧 테스트 시행을 앞두고 있어 도시 사람들을 구하려면 이를 저지해야합니다.

챕터목표유형: 3
챕터목표: 왕국 내 미스터리한 인물 '은밀한 꽃파는 자'에 대한 정보 습득
챕터목표 설명: 왕국 내를 돌아다니며 소문으로만 듣던 '은밀한 꽃파는 자'에 대한 실체 정보를 알아내야 합니다. 그의 정체와 이유, 사람들이 그를 믿고 따르는 이유를 탐구하여 왕국 내 은밀한 이야기를 밝혀내야 합니다.
비고: 왕국 사람들은 꽃파는 자의 꽃 한 송이와 함께 받는 메시지가 좌절한 마음을 치유하고 소망을 심어주는 것으로 알고있다. 꽃을 파는 자는 이 메시지 카드에 마법을 걸어 사람들을 홀리고 있었다.";
        gpt_messages.Add(prompt_msg);

        var query_msg = new ChatMessage()
        {
            Role = "user",
            Content = "진행중인 게임의 "+genre+"장르에 어울리는 챕터 목표 생성"
                    +"\n챕터목표유형:"
        };
        gpt_messages.Add(query_msg);

        StringToObjective("챕터목표유형:"+await GptManager.gpt.CallGpt(gpt_messages));
    }
    private void StringToObjective(string obj_string)
    {
        
        clear = false;

        string[] obj_arr;
        obj_string = obj_string.Replace("\n",":");
        obj_arr = obj_string.Split(':');
        for(int i = 0; i<obj_arr.Length; i++ ){
            switch(i){
                case 1:
                    int.TryParse(obj_arr[1],out type);
                    continue;
                case 3:
                    title = obj_arr[3];
                    continue;
                case 5:
                    detail = obj_arr[5];
                    continue;
                case 7:
                    etc = obj_arr[7];
                    continue;
                default:
                    continue;
            }
        }
    }

    // 변수 호출 함수들
    public int GetGoalType()
    {
        return type;
    }

    public string GetTitle()
    {
        return title;
    }

    public string GetDetail()
    {
        return detail;
    }

    public string GetEtc()
    {
        return etc;
    }

    public bool GetClear()
    {
        return clear;
    }

    // Setter
    public void SetGoalInfo(GetGoalInfo goal_info)
    {
        id = goal_info.goalId;
        type = ConvertToType(goal_info.type);
        title = goal_info.title;
        detail = goal_info.detail;
        etc = goal_info.etc;
    }

    public void SetGoalId(int id)
    {
        this.id = id;
    }

    private int ConvertToType(string type)
    {
        return type switch
        {
            "Monster" => 1,
            "Item" => 2,
            "Report" => 3,
            _ => 0,
        };
    }
}
