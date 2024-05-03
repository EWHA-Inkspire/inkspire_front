using System.Collections;
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

    private List<ChatMessage> pnpc_messages = new List<ChatMessage>();
    private ChatMessage input_msg = new ChatMessage();
    private TextScrollUI text_scroll;
    private Npc pro_npc;
    private int save_idx = 0; // 인덱스 몇번까지 play_manager로 보냈는지 체크

    public void Awake()
    {
        pnpc_messages.Clear();
        save_idx = 0;
        SetSystemPrompt();
    }

    public void SetHint(TextScrollUI text_scroll)
    {
        this.text_scroll = text_scroll;
        pro_npc = ScriptManager.script_manager.GetPnpc();
        SetSystemPrompt();
    }

    public void PlaceHint()
    {
        text_scroll.AppendMsg(pro_npc.GetName() + ": " + "장소 힌트 어떤걸로 줌??");
    }

    public void GoalHint()
    {
        text_scroll.AppendMsg(pro_npc.GetName() + ": " + "목표 힌트 어떤걸로 줌??");
    }

    public void RecoverHP()
    {
        text_scroll.AppendMsg(pro_npc.GetName() + ": " + "HP 회복 얼만큼??");
    }

    private void SetSystemPrompt()
    {
        // 이미 프롬프팅 존재할 경우 세팅하지 않아도 됨
        if(pnpc_messages.Exists(x => x.Role == "system")) {
            return;
        }

        var s_manager = ScriptManager.script_manager;

        var newMessage = new ChatMessage()
        {
            Role = "system",
            Content = "당신은 게임 속 조력자 NPC인 \"" + pro_npc.GetName() + "\"이며 한국말을 사용한다. 당신의 직업, 성격, 말투는 다음과 같다. {" + pro_npc.GetDetail() + "}\n"
            + "현재 플레이중인 게임은" + s_manager.GetScript().GetTimeBackground() + "시대 " + s_manager.GetScript().GetSpaceBackground() + " 배경으로 하는 " + s_manager.GetScript().GetGenre() + "장르의 게임이며 세계관은 다음과 같다.\n{"
            + s_manager.GetScript().GetWorldDetail() + "}\n"
            + "현재 플레이어가 있는 장소는 \"" + s_manager.GetPlace(0).place_name + "\"로, 장소에 대한 설명은 다음과 같다. {" + s_manager.GetPlace(0).place_info + "}"
        };
        pnpc_messages.Insert(0, newMessage);
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
        message = message.Replace(pro_npc.GetName() + ": ", "");

        var newMessage = new ChatMessage()
        {
            Role = "assistant",
            Content = pro_npc.GetName() + ": " + message
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

        Debug.Log(">>저장된 대화");
        for (int i = 1; i <= save_idx; i++) {
            Debug.Log(pnpc_messages[i].Role+": "+pnpc_messages[i].Content);
        }
    }
}
