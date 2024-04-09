using System.Collections;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using OpenAI;
using UnityEngine.UI;

// gpt api 코드 => https://github.com/srcnalt/OpenAI-Unity chatGPT 샘플 코드 변형

public class PlayGPT : MonoBehaviour
{
    [SerializeField] DiceEvent dice_event;
    [SerializeField] BattleEvent battle_event;
    [SerializeField] private TMP_InputField player_input;
    [SerializeField] private TextMeshProUGUI story_object;
    [SerializeField] private Button button;
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private GameObject map_modal;

    private OpenAIApi openai = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();
    private ChatMessage input_msg = new ChatMessage();
    private string system_prompt = @"당신은 게임 속 세계관을 전부 알고 있는 전능한 존재이자 스토리 게임을 진행하는 Narrator이다.
플레이어가 선택해야 하는 모든 선택지들은 플레이어의 선택을 기다려야 한다.
TRPG 진행을 하듯 진행하되, TRPG라는 단어는 언급하면 안된다.

아래와 같은 양식으로 사용자가 입력한 배경과 분위기에 맞는 다른 내용의 게임 시나리오를 출력한다.
게임의 최종 목표나 챕터 목표를 절대로 직접언급해서는 안 되며, npc 정보 또한 절대로 직접언급해서는 안 된다.
게임 배경에 대한 정보는 출력을 위한 참고사항이며, 해당 정보들을 바탕으로 다음 시나리오 진행한다.
npc 정보들을 토대로 적절한 시점에 npc를 등장시킨다.

현재 플레이중인 게임은"+ScriptManager.scriptinfo.time_background+"시대 "+ScriptManager.scriptinfo.space_background+"를 배경으로 하는 "+ScriptManager.scriptinfo.genre+"장르의 게임이며 세계관은 다음과 같다."
+ScriptManager.scriptinfo.world_detail+"\n\n"
+"게임의 최종 목표는 "+ScriptManager.scriptinfo.final_obj.title+"\n"+ScriptManager.scriptinfo.final_obj.detail+"이며"
+"현재 챕터의 목표는 다음과 같다."+ScriptManager.scriptinfo.chapter_obj[ScriptManager.scriptinfo.curr_chapter].title+"\n"+ScriptManager.scriptinfo.chapter_obj[ScriptManager.scriptinfo.curr_chapter].detail
+" 현재 플레이어가 있는 장소는 "+MapManager.mapinfo.map[MapManager.mapinfo.curr_place].place_name+"로, "+MapManager.mapinfo.map[MapManager.mapinfo.curr_place].place_info
+@"이 아래로 게임 진행 양식이 이어진다. ** 이 표시 안의 내용은 문맥에 맞게 채운다.
###
Narrator (내레이터):
*게임 스토리 진행 멘트 혹은 플레이어의 선택지 생성*

*필요할 경우 현재 상황에 대한 설명*

*NPC 이름*:
*npc 대사 내용*
###";

    void Awake()
    {
        story_object.text = "";
        
        if (messages.Count == 0)
        {
            var newMessage = new ChatMessage()
            {
                Role = "system",
                Content = system_prompt
            };
            messages.Add(newMessage);

            newMessage.Role = "user";
            newMessage.Content = "게임을 시작하고 게임의 인트로를 보여줘";
            messages.Add(newMessage);

            var introMessage = new ChatMessage(){
                Role = "assistant",
                Content = ScriptManager.scriptinfo.intro_string+"\n\nNarrator:\n"+PlayerStatManager.playerstat.charname+"님, 처음으로 조사할 장소를 선택해주십시오. \n Map 창의 "+MapManager.mapinfo.map[0].place_name+"를 선택할 시, "+ScriptManager.scriptinfo.pNPC.name+"가 당신을 반겨줄 것입니다."
            };
            messages.Add(introMessage);
            AppendMsg(introMessage);
        }
    }

    public async void SendButton()
    {
        // 이벤트 체커 메시지 설정 (가장 마지막 gpt 대화 추가)
        var checkerMessage = new List<ChatMessage>();
        checkerMessage.Add(messages.Last());

        input_msg.Role = "user";
        input_msg.Content = player_input.text;

        AppendMsg(input_msg);

        // 이벤트 체커 메시지 설정 (플레이어 입력값 추가)
        checkerMessage.Add(input_msg);

        if(item_type == "Recover" || item_type == "Weapon" ||
        item_type == "Item" || item_type == "Report") {
            Debug.Log(">>이벤트 트리거");
            var event_trigger = MapManager.mapinfo.map[MapManager.mapinfo.curr_place].event_trigger;
            Debug.Log(event_trigger);

            if(await EventChecker.eventChecker.EventCheckerGPT(checkerMessage, event_trigger)) {
                // TODO: 주사위 이벤트 진행
            }
        }
        SendReply();
        
        //dice_event.SetDiceEvent(50);
        //battle_event.SetBattle(BattleEvent.BType.MOB,3);
        
    }
    void AppendMsg(ChatMessage msg)
    {
        string add_text = "";
        if (msg.Role == "user")
        {
            add_text += PlayerStatManager.playerstat.charname + "> ";
        }

        add_text += msg.Content;

        story_object.text += add_text + "\n\n";
        LayoutRebuilder.ForceRebuildLayoutImmediate(scroll.content);
        scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scroll.content.sizeDelta.y);
        scroll.verticalNormalizedPosition = 0f;
    }

    private async void SendReply()
    {
        var newMessage = new ChatMessage()
        {
            Role = "user",
            Content = player_input.text
        };

        messages.Add(newMessage);

        button.enabled = false;
        player_input.text = "";
        player_input.enabled = false;

        var message = await GptManager.gpt.CallGpt(messages);
        newMessage.Role = "assistant";
        newMessage.Content = message;
        AppendMsg(newMessage);

        button.enabled = true;
        player_input.enabled = true;
    }

    public void PlaceButton(int place_idx){
        MapManager.mapinfo.curr_place = place_idx;
        var query_msg = new ChatMessage(){
            Role = "user",
            Content = MapManager.mapinfo.map[MapManager.mapinfo.curr_place].place_name+"으로 이동"
        };
        messages.Add(query_msg);
        var newMessage = new ChatMessage(){
            Role = "assistant",
            Content = "Narrator: \n이곳은 "+MapManager.mapinfo.map[MapManager.mapinfo.curr_place].place_name+"입니다.\n"+MapManager.mapinfo.map[MapManager.mapinfo.curr_place].place_info
        };
        messages.Add(newMessage);
        AppendMsg(newMessage);
        map_modal.gameObject.SetActive(false);
    }
}
