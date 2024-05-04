using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OpenAI;

public class TextScrollUI : MonoBehaviour
{
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private TextMeshProUGUI story_object;

    void Awake(){
        story_object.text = "";
    }

    public void AppendMsg(ChatMessage msg)
    {
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
        AppendMsg(new ChatMessage(){ Role = "assistant", Content = msg});
    }
}
