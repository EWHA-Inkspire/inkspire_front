using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OpenAI;
using System.Linq;

public class Play : MonoBehaviour
{
    [SerializeField] private DiceEvent dice_event;
    [SerializeField] private BattleEvent battle_event;
    [SerializeField] private TMP_InputField player_input;
    [SerializeField] public TextScrollUI text_scroll;
    [SerializeField] private Button send_button;
    [SerializeField] private GameObject map_modal;

    private List<ChatMessage> messages = new List<ChatMessage>();
    private ChatMessage input_msg = new ChatMessage();
    private static ScriptManager s_manager = ScriptManager.script_manager;
    private string system_prompt = "";

    void Start(){
        SetSystemPrompt();
        if (messages.Count == 1)
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = "게임을 시작하고 게임의 인트로를 보여줘"
            };
            messages.Add(newMessage);

            var introMessage = new ChatMessage()
            {
                Role = "assistant",
                Content = s_manager.GetScript().GetIntro() + "\n\nNarrator:\n" + s_manager.GetCharName() + "님, 처음으로 조사할 장소를 선택해주십시오.\n" 
                +"Map 창의 "+ s_manager.GetPlace(0).place_name + EulorReul(s_manager.GetPlace(0).place_name) + " 선택할 시, " 
                + s_manager.GetPnpc().GetName()+ EorGa(s_manager.GetPnpc().GetName()) + " 당신을 반겨줄 것입니다."
            };
            messages.Add(introMessage);
            Debug.Log("PlayStart: \n"+introMessage.Content);
            text_scroll.AppendMsg(introMessage);
        }
    }
    private void SetSystemPrompt()
    {
        s_manager = ScriptManager.script_manager;
        int curr_chap = s_manager.GetCurrChap();
        system_prompt = @"당신은 게임 속 세계관을 전부 알고 있는 전능한 존재이자 스토리 게임을 진행하는 Narrator이다.
        플레이어가 선택해야 하는 모든 선택지들은 플레이어의 선택을 기다려야 한다.
        TRPG 진행을 하듯 진행하되, TRPG라는 단어는 언급하면 안된다.

        아래와 같은 양식으로 사용자가 입력한 배경과 분위기에 맞는 다른 내용의 게임 시나리오를 출력한다.
        게임의 최종 목표나 챕터 목표를 절대로 직접언급해서는 안 되며, npc 정보 또한 절대로 직접언급해서는 안 된다.
        게임 배경에 대한 정보는 출력을 위한 참고사항이며, 해당 정보들을 바탕으로 다음 시나리오 진행한다.
        npc 정보들을 토대로 적절한 시점에 npc를 등장시킨다.

        현재 플레이중인 게임은" + s_manager.GetScript().GetTimeBackground() + "시대 " + s_manager.GetScript().GetSpaceBackground() + " 배경으로 하는 " + s_manager.GetScript().GetGenre() + "장르의 게임이며 세계관은 다음과 같다.\n"
        + s_manager.GetScript().GetWorldDetail() + "\n\n"
        + "게임의 최종 목표는 " + s_manager.GetGoal(4).GetTitle() + "\n" + s_manager.GetGoal(4).GetDetail() + "이며"
        + "현재 챕터의 목표는 다음과 같다." + s_manager.GetGoal(curr_chap).GetTitle() + "\n" + s_manager.GetGoal(curr_chap).GetDetail()
        + "\n현재 플레이어가 있는 장소는 \"" + s_manager.GetCurrPlace().place_name + "\"로, " + s_manager.GetCurrPlace().place_info
        + @"이 아래로 게임 진행 양식이 이어진다. ** 이 표시 안의 내용은 문맥에 맞게 채운 후 *기호는 모두 삭제한다. 
        ------------------------------------------------
        Narrator (내레이터):
        *게임 스토리 진행 멘트와 플레이어의 선택지 생성, 선택지는 반드시 4가지 생성되며 각 선택지끼리의 내용은 절대 중복되지 않는다. *

        *필요할 경우 현재 상황에 대한 설명*

        당신은 아래의 선택지를 고르거나, 다른 행동을 입력할 수 있습니다. 

        1. *플레이어가 할 수 있는 행동 첫번째*
        2. *플레이어가 할 수 있는 행동 두번째*
        3. *플레이어가 할 수 있는 행동 세번째*
        4. *플레이어가 할 수 있는 행동 네번째*

        *NPC 이름*:
        *npc 대사 내용*";
        if(messages.Count == 0){
            messages.Add(new ChatMessage{Role = "system", Content = system_prompt});
        }
        else{
            messages[0] = new ChatMessage{Role = "system", Content = system_prompt};
        }
    }

    public async void SendButton()
    {
        input_msg.Role = "user";
        input_msg.Content = player_input.text;

        text_scroll.AppendMsg(input_msg);

        var item_type = s_manager.GetCurrItem().item_type;
        if (item_type == ItemType.Recover || item_type == ItemType.Weapon || item_type == ItemType.Item || item_type == ItemType.Report)
        {
            if (!s_manager.GetCurrPlace().clear && await EventChecker.eventChecker.EventCheckerGPT(messages.Last().Content, input_msg.Content, s_manager.GetCurrEvent()))
            {
                // 이벤트 트리거 도입 스크립트 출력
                text_scroll.AppendMsg("\n<b>:: 판정 이벤트 발생 ::</b>\n");
                ChatMessage event_msg = new ChatMessage
                {
                    Role = "user",
                    Content = "판정 이벤트 발생"
                };
                messages.Add(event_msg);
                event_msg.Role = "assistant";

                string event_title = s_manager.GetCurrEvent().event_title.Replace(".", ".\n");
                string event_intro = s_manager.GetCurrEvent().event_intro.Replace(".", ".\n");

                event_msg.Content = event_title + "\n" + event_intro + "\n\n";
                messages.Add(event_msg);
                text_scroll.AppendMsg(event_msg.Content);

                dice_event.SetDiceEvent(50+(int)Mathf.Pow(-1,Random.Range(0,2))*Random.Range(0,30));

                player_input.text = "";
            }
            else
            {
                SendReply();
            }
        }
        else
        {
            SendReply();
        }
    }

    private async void SendReply()
    {
        var newMessage = new ChatMessage()
        {
            Role = "user",
            Content = player_input.text
        };
        messages.Add(newMessage);

        send_button.enabled = false;
        player_input.text = "";
        player_input.enabled = false;

        var message = await GptManager.gpt.CallGpt(messages);
        newMessage.Role = "assistant";
        newMessage.Content = message;
        messages.Add(newMessage);

        text_scroll.AppendMsg(newMessage);

        send_button.enabled = true;
        player_input.enabled = true;
    }

    public void PlaceButton(int place_idx)
    {
        ScriptManager.script_manager.SetCurrPlace(place_idx);
        SetSystemPrompt();

        var query_msg = new ChatMessage()
        {
            Role = "user",
            Content = s_manager.GetCurrPlace().place_name + "으로 이동"
        };
        messages.Add(query_msg);

        var newMessage = new ChatMessage()
        {
            Role = "assistant",
            Content = "Narrator: \n이곳은 " +s_manager.GetCurrPlace().place_name + "입니다.\n" + s_manager.GetCurrPlace().place_info
        };
        if (s_manager.GetCurrPlaceIdx() == 0)
        {
            newMessage.Content += "이곳에서는 NPC " + s_manager.GetPnpc().GetName() + EulorReul(s_manager.GetPnpc().GetName()) + " 만날 수 있습니다.";
        }
        messages.Add(newMessage);
        text_scroll.AppendMsg(newMessage);
        map_modal.gameObject.SetActive(false);

        // 장소의 아이템 유형이 Mob이거나 Monster일 경우 전투 이벤트 발동
        var item_type = s_manager.GetItem(place_idx).item_type;
        if (item_type == ItemType.Mob || item_type == ItemType.Monster)
        {
            // 전투 이벤트
            battle_event.SetBattle(BattleEvent.BType.MOB, 3);
        }
        Debug.Log("placeButton item name: " +s_manager.GetCurrItem().item_name);
    }
    

    // 조사 선택 함수
    static string EorGa(string noun)
    {
        char lastChar = noun[noun.Length - 1]; // 마지막 글자 추출
        bool hasJongsung = (lastChar - '가') % 28 > 0; // 받침 여부 확인

        // 받침 여부에 따라 조사 선택
        return hasJongsung ? "이" : "가";
    }

    static string EunorNeun(string noun)
    {
        char lastChar = noun[noun.Length - 1]; // 마지막 글자 추출
        bool hasJongsung = (lastChar - '가') % 28 > 0; // 받침 여부 확인

        // 받침 여부에 따라 조사 선택
        return hasJongsung ? "은" : "는";
    }

    static string EulorReul(string noun)
    {
        char lastChar = noun[noun.Length - 1]; // 마지막 글자 추출
        bool hasJongsung = (lastChar - '가') % 28 > 0; // 받침 여부 확인

        // 받침 여부에 따라 조사 선택
        return hasJongsung ? "을" : "를";
    }


    // Setter
    public void AddToMessagesGPT(ChatMessage msg)
    {
        messages.Add(msg);
    }

}
