using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenAI;

public class Script
{
    private string genre;
    private string time_background;
    private string space_background;
    private string world_detail;
    private string intro;

    private List<ChatMessage> gpt_messages = new List<ChatMessage>();
    private string GPT_ERROR = "No text was generated from this prompt.";

    public Script(string genre, string time_background, string space_background)
    {
        this.genre = genre;
        this.time_background = time_background;
        this.space_background = space_background;
        WorldDetailGPT();
    }

    private async void WorldDetailGPT()
    {
        Debug.Log(">>Call World Detail GPT");
        gpt_messages.Clear();

        var newMessage = new ChatMessage()
        {
            Role = "system",
            Content = @"당신은 TRPG 게임 세계관을 기획하는 게임 기획자이다. 시대와 공간적 배경의 정확한 명칭이 없어 '가상배경' 등의 단어로 제시되는 경우, 해당 배경의 이름 또한 생성하여 매끄럽게 출력한다. 
            장르가 '판타지'가 아닌 경우 마법적 요소나 판타지적 요소는 일절 포함하지 않는다. 
            당신은 반드시 게임의 장르와 시간, 공간적 배경에 알맞는 세계관을 생성해야 한다. 
            
            게임의 장르는 "+genre+"이고, 배경은 "+time_background+" 시대"+space_background+@"를 배경으로 한다.
            
            ---
            다음은 출력 형식 예시이다. 

            세계관: 이 게임의 배경은 '섀도우폴'이라는 고요하고 신비로운 영국 시골 마을입니다. 이 마을은 영국의 어딘가에 위치해 있으며, 주변을 울창한 숲과 신비한 분위기가 가득한 산맥으로 둘러싸여 있습니다.
            주변 지역에서는 이 마을을 '저주받은 마을'로 부르며, 역사적으로 끔찍한 사건들이 자주 일어났다는 소문이 있습니다. 마을을 둘러싼 울창한 숲은 누구도 정확히 언급하지 않는 기이한 소문들과 전설로 가득 차 있습니다.
            이 마을은 '섀도우폴'이라 불리는 어두운 분위기와 고요한 공포를 품고 있습니다. 마을 주변의 형편없는 공기와 마을 주민들의 초연한 차가운 태도는 이 마을이 어떠한 극악의 힘에 의해 영향을 받고 있음을 짐작케 합니다.
            이 곳에서 인물들은 과거의 어두운 사건과 마주하여 자신의 운명과 질문을 마주해야 할 것입니다. 절망과 희망의 난처한 미로를 탐험하게 합니다. 주변 환경물들과 미로를 탐험하고 사건의 정체를 파헤치며 생존에 도전하는 환희와 공포의 호러 게임이 펼쳐집니다.

            세계관: 이 게임의 배경은 '낙천도시 청도'입니다. '낙천도시 청도'는 15세기 동아시아를 배경으로 한 고요하고 아름다운 마을로, 중국과 한국의 조선 시대를 혼합한 가상의 세계입니다. 
            이 마을은 창백한 달빛 아래 번뜻 밝히는 등잔불이 유독 많아, 마을 전체가 우아하게 빛나는 모습을 보여줍니다. 오랜 옛날부터 이 마을은 도태될 수 없는 풍요로움과 고귀한 문화가 전해져 온 곳으로, 사람들은 정신적 안정과 평화로움을 찾아 찾아 이곳을 찾아옵니다.
            낙천도시 청도는 넓은 황홀한 정자와 현란한 능구렁이, 그리고 천연산수의 귀호한 울림으로 유명합니다. 이 마을은 동양 전통 건축물이 우아하게 자리 잡은 공간을 가지고 있으며, 그 안에는 문화와 예술이 고스란히 담겨져 있습니다.
            주변 지역에서는 '노고지리의 정원'으로 불리며, 이 마을을 찾은 이들은 자아를 쉽게 찾게 되어 인생의 진정한 가치를 깨우치게 되는 곳으로 소문이 자자합니다.
            이 곳에서 인물들은 신중한 사령과 올곧은 분위기 안에서 자신의 운명을 탐험해 나갈 것입니다. 독특한 동양풍의 세계관이 플레이어들을 매혹할 것입니다.

            세계관: 이 게임의 배경은 '드래곤피크'라는 고대 요새가 위치한 황량하고 산업적인 도시입니다. 드래곤피크는 잿빛 구름이 끊임없이 떠다니며, 황량한 산맥과 쇠락한 건물들로 둘러싸여 있습니다. 이곳은 마법적인 요소가 쓸모가 없는, 기술과 산업이 주를 이루는 도시로 알려져 있습니다.
            주변 지역에서는 드래곤피크를 '마법의 황무지'라고 불리며, 옛 전설에 따르면 고대 드래곤이 이곳을 거처했다고 전해집니다. 신비한 기운이 가득한 이곳에서는 전설적인 보물이 잠들어 있다고 믿어지며, 다수의 모험가들이 이곳을 탐험하고자 합니다.
            드래곤피크는 고대 요새인 '드래곤킵'이 그 중심에 위치하고 있습니다. 드래곤킵은 강력한 방어 시스템과 공격적인 드래곤들이 지키고 있어서 그 누구도 쉽게 접근할 수 없는 요새로 알려져 있습니다. 이곳에서 플레이어들은 고대 유물을 찾기 위해 도전을 겪게 되며, 거대 드래곤과 그들의 비밀을 파헤치는 여정이 펼쳐질 것입니다. 신비로운 고대 도구와 기술, 그리고 거센 전투가 펼쳐지는 모험 어드벤처 게임이 펼쳐집니다.
            "
        };
        gpt_messages.Add(newMessage);

        newMessage = new ChatMessage()
        {
            Role = "user",
            Content = "게임의 배경과 장르에 어울리는 세계관 생성\n세계관: "
        };
        gpt_messages.Add(newMessage);

        string response = await GptManager.gpt.CallGpt(gpt_messages);
        Debug.Log("세계관 출력:" + response);

        if(response == GPT_ERROR) {
            // 출력 안 될 경우 처리
        }

        this.world_detail = response;
    }

    public async void IntroGPT(Npc pro_npc, Npc anta_npc, Place[] map, string char_name)
    {
        Debug.Log(">>Call Intro GPT");
        gpt_messages.Clear();

        var newMessage = new ChatMessage()
        {
            Role = "system",
            Content = @"당신은 게임 속 세계관을 전부 알고 있는 전능한 존재이자 스토리 게임을 진행하는 Narrator이다.
            게임의 배경과 세계관 설정을 참고하여 게임의 시작 멘트인 인트로를 아래의 양식대로 출력하되, 직접적으로 게임이라는 언급은 하지 않는다.
            또한, 조력자 npc인 " + pro_npc.GetDetail() + "(" + pro_npc.GetDetail() + ")와 적대자 NPC인 " + anta_npc.GetName() + "(" + anta_npc.GetDetail() + @")의 요약된 소개를 줄글 형태로 진행한다. 
            게임 시작 장소는" + map[0].place_name + "으로, " + map[0].place_info + @"이다.
            ** 이 표시 안의 내용은 문맥에 맞게 채운다.
            ###
            여기는 *시간적 배경* *공간적 배경*. *플레이어 이름*님, 당신은 이 도시로 들어오면서 새로운 모험을 시작하게 됩니다.
            
            *게임 시작 멘트*
            *조력자 NPC와 시작 장소에서 처음으로 만나는 내용이 출력되며 npc는 게임의 사건의 시작을 소개하며 게임의 시작을 알린다.*

            *조력자 NPC 이름*:
                *조력자 NPC의 대사*
            ###"
        };

        gpt_messages.Add(newMessage);

        newMessage = new ChatMessage()
        {
            Role = "user",
            Content = "플레이어의 이름은 다음과 같다. 플레이어 이름:" + char_name + @" 
            현재 플레이중인 게임은 " + time_background + ", "
            + space_background + "(을)를 배경으로 하는 "
            + genre + " 장르의 게임이며 세계관은 다음과 같다.\n"
            + world_detail + @"
            게임의 인트로 출력"
        };

        gpt_messages.Add(newMessage);

        var response = await GptManager.gpt.CallGpt(gpt_messages);
        response = response.Replace("###\n", "");
        response = response.Replace("*", "");
        this.intro = response.Replace("###", "");
        Debug.Log(response);
    }

    public string getGenre()
    {
        return this.genre;
    }

    public string getWorldDetail()
    {
        return this.world_detail;
    }
}