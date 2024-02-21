using System.Collections;
using System.Collections.Generic;
using OpenAI;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public struct place
    {
        public string place_name;
        public string place_info; //장소 설명
        public string item_name;
        public string item_type; //recover, weapon, mob, null (목표이벤트일 경우 report 추가)
        public bool event_type; //일반 이벤트 == 0, 목표 이벤트 == 1 
        public bool ANPC_exist; //ANPC 미등장 == 0, 등장 == 1 (목표이벤트일 경우 무조건 0)
    }
    private string[] map = new string[14];
    public string PNPC_place;
    private OpenAIApi openai = new OpenAIApi();
    private List<ChatMessage> messages = new List<ChatMessage>();
    private ChatMessage input_msg = new ChatMessage();
    private string system_prompt = "너는 플레이어가 탐색할 수 있는 장소를 한 단어로 출력해야 해.";
    //background + " 배경의 " + genre + "분위기에 어울리는 장소명 1개를 출력해줘";

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
        }
    }

    // def getProtaNPCName(background, genre):
    // npc_setting = "너는 조력자 NPC 캐릭터의 이름을 한 단어로 출력해야 해."
    // query = background + " 배경의 " + genre + "분위기에 어울리는 조력자 NPC 이름 1개를 출력해줘"

    // messages = [
    //     {"role": "system", "content": npc_setting},
    //     {"role": "user", "content": query}
    // ]

    // PNPC_name = callGPT(messages=messages, stream=False)

    // return PNPC_name

    public void SendButton()
    {
        input_msg.Role = "user";
        input_msg.Content = background + " 배경의 " + genre + "분위기에 어울리는 장소명 1개를 출력해줘";

        AppendMsg(input_msg);
        SendReply();



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

        // Complete the instruction
        var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
        {
            Model = "gpt-3.5-turbo",
            Messages = messages
        });

        if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
        {
            var message = completionResponse.Choices[0].Message;
            message.Content = message.Content.Trim();

            messages.Add(message);
            AppendMsg(message);
        }
        else
        {
            Debug.LogWarning("No text was generated from this prompt.");
        }

        button.enabled = true;
        player_input.enabled = true;
    }
}
