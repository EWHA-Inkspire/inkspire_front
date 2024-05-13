using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using OpenAI;

public class GptManager : MonoBehaviour
{
    public static GptManager gpt;
    private int cnt = 0;

    void Awake(){
        if(gpt == null){
            gpt = this;
            DontDestroyOnLoad(gpt);
        }
        else if(gpt != this){
            Destroy(this);
        }
    }

    private readonly OpenAIApi openai = new();

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
            cnt = 0;
            return completionResponse.Choices[0].Message.Content;
        }
        else if(cnt != 3)
        {
            cnt++;
            return await CallGpt(messages);
        }
        else
        {
            cnt = 0;
            return "No text was generated from this prompt.";
        }
    }

    public async Task<string> CallGpt4(List<ChatMessage> messages) {
        // gpt 호출
        var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
        {
            Model = "gpt-4-turbo",
            Messages = messages
        });

        // 응답이 있을 경우 응답 내용 반환
        if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
        {
            cnt = 0;
            return completionResponse.Choices[0].Message.Content;
        }
        else if(cnt != 3)
        {
            cnt++;
            return await CallGpt4(messages);
        }
        else
        {
            cnt = 0;
            return "No text was generated from this prompt.";
        }
    }
}