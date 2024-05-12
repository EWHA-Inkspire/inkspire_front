using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OpenAI;

public class HintEvent : MonoBehaviour
{
    [SerializeField] private GameObject hint_modal;
    [SerializeField] private TMP_InputField player_input;
    [SerializeField] private Button send_button;
    [SerializeField] Play play_manager;
    private ScriptManager s_manager;
    private List<ChatMessage> pnpc_messages = new();
    private ChatMessage input_msg = new();
    private TextScrollUI text_scroll;
    private Npc pro_npc;
    private int save_idx = 0; // 인덱스 몇번까지 play_manager로 보냈는지 체크

    public void Start()
    {
        pnpc_messages.Clear();
        save_idx = 0;
        s_manager = ScriptManager.script_manager;
        pro_npc = ScriptManager.script_manager.GetPnpc();
        SetSystemPrompt();
    }

    public void SetHint(TextScrollUI text_scroll)
    {
        this.text_scroll = text_scroll;
    }

    public void PlaceHint()
    {
        List<Item> items = s_manager.GetCurrItems();
        int idx = items.FindIndex(item => item.type == ItemType.Monster || item.type == ItemType.Item || item.type == ItemType.Report);
        
        // 목표 이벤트에 해당하는 아이템이 없는 경우
        if (idx == -1) {
            text_scroll.AppendMsg(pro_npc.GetName() + ":\n" + "자유롭게 모든 장소들을 탐방해보세요!");
            return;
        }

        idx += 1 + s_manager.GetCurrChap()*3; // 장소 인덱스 보정
        Place place = s_manager.GetPlace(idx);
        text_scroll.AppendMsg(pro_npc.GetName() + ":\n" + place.place_name + Play.EulorReul(place.place_name) + " 살펴보는 건 어떨까요? 재밌는 사건이 기다리고 있을지도 몰라요!");
    }

    public void GoalHint()
    {
        Goal chapter_obj = s_manager.GetCurrGoal();

        // 적 처치 목표일 경우
        if (chapter_obj.GetGoalType() == 1) {
            text_scroll.AppendMsg(pro_npc.GetName() + ":\n" + "때때로 존재를 감추기 위해 수상한 생명체가 그림자 속에 숨어있을 때가 있어요. 지나가는 때를 노려서 공격해 올 수도 있으니, 조심하세요.");
        }

        // 아이템 획득이 목표일 경우
        if (chapter_obj.GetGoalType() == 2) {
            text_scroll.AppendMsg(pro_npc.GetName() + ":\n" + "가끔은 뜻밖의 아이템을 발견할 수 있는 곳이 있어요. 눈에 띄는 것은 없지만, 주변을 조심스럽게 살펴보면 무언가를 발견할지도 모르겠죠.");
        }

        // 사건의 진상 조사가 목표일 경우
        if (chapter_obj.GetGoalType() == 3) {
            text_scroll.AppendMsg(pro_npc.GetName() + ":\n" + "가끔은 이야기의 전체 그림을 완성하기 위해 숨은 단서들을 찾아야 할 때가 있어요. 주변을 주의 깊게 살펴보고, 이상한 점을 발견하면 기록해두세요.");
        }
    }

    public void RecoverHP()
    {
        List<Item> items = s_manager.GetCurrItems();

        if(!items.Exists(item => item.type == ItemType.Monster || item.type == ItemType.Mob)) {
            text_scroll.AppendMsg(pro_npc.GetName() + ":\n당분간은 hp를 사용하지 않아도 플레이가 가능해요.");
            return;
        }

        int hp = PlayerStatManager.playerstat.GetStatAmount(StatType.Hp);
        if (hp < 100) {
            text_scroll.AppendMsg(pro_npc.GetName() + ":\n" + (100 - hp) + "hp를 회복했어요. 다음에는 꼭 전투에서 승리하시길 기원합니다!");
        } else {
            text_scroll.AppendMsg(pro_npc.GetName() + ":\n아직 최소 체력에 도달하지 않았어요. 체력 회복은 hp가 100 이하일 경우에만 가능하니 다음에 찾아와주세요!");
        }
    }

    private void SetSystemPrompt()
    {
        var newMessage = new ChatMessage()
        {
            Role = "system",
            Content = "당신은 게임 속 조력자 NPC인 \"" + pro_npc.GetName() + "\"이며 한국말을 사용한다. 당신의 직업, 성격, 말투는 다음과 같다. {" + pro_npc.GetDetail() + "}\n"
            + "현재 플레이중인 게임은" + s_manager.GetScript().GetTimeBackground() + "시대 " + s_manager.GetScript().GetSpaceBackground() + " 배경으로 하는 " + s_manager.GetScript().GetGenre() + "장르의 게임이며 세계관은 다음과 같다.\n{"
            + s_manager.GetScript().GetWorldDetail() + "}\n"
            + "현재 플레이어가 있는 장소는 \"" + s_manager.GetPlace(0).place_name + "\"로, 장소에 대한 설명은 다음과 같다. {" + s_manager.GetPlace(0).place_info + "}"
        };

        if(pnpc_messages.Count > 0) {
            pnpc_messages[0] = newMessage;
        } else {
            pnpc_messages.Add(newMessage);
        }
    }

    public async void SendButton()
    {
        input_msg.Role = "user";
        input_msg.Content = player_input.text;

        text_scroll.AppendMsg(input_msg);
        pnpc_messages.Add(input_msg);

        send_button.enabled = false;
        player_input.text = "";
        player_input.enabled = false;

        var message = await GptManager.gpt.CallGpt(pnpc_messages);
        message = message.Replace(pro_npc.GetName() + ":\n", "");
        message = message.Replace(pro_npc.GetName() + ": ", "");

        var newMessage = new ChatMessage()
        {
            Role = "assistant",
            Content = pro_npc.GetName() + ":\n" + message
        };
        pnpc_messages.Add(newMessage);
        text_scroll.AppendMsg(newMessage);

        send_button.enabled = true;
        player_input.enabled = true;
    }

    public void EndChat()
    {
        if(save_idx == pnpc_messages.Count - 1) {
            return;
        }
        play_manager.AppendToMessageGPT(pnpc_messages.GetRange(save_idx + 1, pnpc_messages.Count - save_idx - 1));
        save_idx = pnpc_messages.Count - 1;
    }
}
