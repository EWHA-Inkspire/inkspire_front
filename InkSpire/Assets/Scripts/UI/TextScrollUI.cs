using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OpenAI;
using System.Collections.Generic;
using System.Linq;

public class TextScrollUI : MonoBehaviour
{
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private GameObject assi_chat;
    [SerializeField] private GameObject user_chat;
    [SerializeField] private Play play;

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

        if (msg.Role == "assistant")
        {
            // msg.Content를 \n\n으로 나눠서 각각 생성
            string[] split_msg = msg.Content.Split("\n\n");
            foreach (var split in split_msg)
            {
                GameObject new_chat = Instantiate(assi_chat, scroll.content);
                new_chat.GetComponentInChildren<TextMeshProUGUI>().text = split;
            }
        }
        else if (msg.Role == "user")
        {
            GameObject new_chat = Instantiate(user_chat, scroll.content);
            new_chat.GetComponentInChildren<TextMeshProUGUI>().text = msg.Content;
        }

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
        // scroll.content 아래 게임 오브젝트 파괴
        foreach (Transform child in scroll.content)
        {
            Destroy(child.gameObject);
        }

        foreach (var msg in messages)
        {
            AppendMsg(msg);
        }
    }
}
