using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OpenAI;
using System.Collections.Generic;

public class TextScrollUI : MonoBehaviour
{
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private TextMeshProUGUI story_object;

    void Awake()
    {
        story_object.text = "";
    }

    public void AppendMsg(ChatMessage msg)
    {
        if (msg.Role == "system")
        {
            return;
        }

        if (msg.Content == "게임을 시작하고 게임의 인트로를 보여줘")
        {
            return;
        }

        string add_text = "";
        if (msg.Role == "user")
        {
            add_text += ScriptManager.script_manager.GetCharName() + "> ";
        }

        add_text += msg.Content;

        story_object.text += add_text + "\n\n";
        LayoutRebuilder.ForceRebuildLayoutImmediate(scroll.content);
        scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, scroll.content.sizeDelta.y);
        scroll.verticalNormalizedPosition = 0f;
    }

    public void AppendMsg(string msg)
    {
        AppendMsg(new ChatMessage() { Role = "assistant", Content = msg });
    }

    internal void InitStoryObj(List<ChatMessage> messages)
    {
        story_object.text = "";
        foreach (var msg in messages)
        {
            AppendMsg(msg);
        }
    }
}
