using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using OpenAI;

public class GptManager : MonoBehaviour
{
    public static GptManager gpt;

    void Awake(){
        // 씬이 바뀔 때 파괴되지 않음
        DontDestroyOnLoad(this.gameObject);

        if(gpt == null){
            gpt = this;
        }
    }

    private OpenAIApi openai = new OpenAIApi();

    public async Task<string> CallGpt(List<ChatMessage> messages) {
        // gpt 호출
        var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
        {
            Model = "gpt-3.5-turbo",
            Messages = messages
        });

        // 응답이 있을 경우 응답 내용 반환
        if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
        {
            return completionResponse.Choices[0].Message.Content;
        }
        else
        {
            return await CallGpt(messages);
        }
    }
}