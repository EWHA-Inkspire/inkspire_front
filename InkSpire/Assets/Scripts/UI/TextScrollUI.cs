using UnityEngine;
using UnityEngine.UI;
using TMPro;
using OpenAI;
using System.Collections.Generic;
using System.Collections;

public class TextScrollUI : MonoBehaviour
{
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private GameObject assi_chat;
    [SerializeField] private GameObject user_chat;
    private readonly float TYPING_SPEED = 0.03f;

    public void AppendMsg(ChatMessage msg, bool isTyping)
    {
        if (msg.Role == "system" || msg.Content == "게임을 시작하고 게임의 인트로를 보여줘")
        {
            return;
        }

        if (msg.Role == "assistant" && isTyping)
        {
            StartCoroutine(TypeMessage(msg.Content));
        }
        else if (msg.Role == "assistant")
        {
            // msg.Content를 \n\n으로 나눠서 각각 생성
            string[] split_msg = msg.Content.Split(new[] { "\n\n" }, System.StringSplitOptions.None);
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

        // 스크롤 위치 업데이트
        LayoutRebuilder.ForceRebuildLayoutImmediate(scroll.content);
        StartCoroutine(UpdateScrollPosition());
    }

    private IEnumerator TypeMessage(string message)
    {
        string[] splitMessages = message.Split("\n\n");

        foreach (string splitMessage in splitMessages)
        {
            TextMeshProUGUI textComponent = Instantiate(assi_chat, scroll.content).GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = "";

            foreach (char letter in splitMessage.ToCharArray())
            {
                textComponent.text += letter;
                yield return new WaitForSeconds(TYPING_SPEED);
                scroll.verticalNormalizedPosition = 0f;
            }

            yield return new WaitForSeconds(TYPING_SPEED); // 각 메시지의 끝에 추가 딜레이
        }
    }

    private IEnumerator UpdateScrollPosition()
    {
        // 레이아웃 업데이트 대기
        yield return null;
        scroll.verticalNormalizedPosition = 0f;
    }

    public void ApplyTextureToGameObject(Texture2D texture)
    {
        GameObject imageObject = new("Intro Image");
        imageObject.transform.SetParent(scroll.content.transform);

        Image imageComponent = imageObject.AddComponent<Image>();
        RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(700, 700); // 원하는 크기로 설정

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        imageComponent.sprite = sprite;
    }

    public void AppendMsg(string msg, bool isTyping)
    {
        AppendMsg(new ChatMessage() { Role = "assistant", Content = msg }, isTyping);
    }

    internal void InitStoryObj(List<ChatMessage> messages)
    {
        // scroll.content 아래 게임 오브젝트 파괴
        foreach (Transform child in scroll.content)
        {
            Destroy(child.gameObject);
        }

        ApplyTextureToGameObject(ScriptManager.script_manager.GetScript().GetIntroImage());

        foreach (var msg in messages)
        {
            AppendMsg(msg, false);
        }
    }
}
