// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using OpenAI;

// public class GPTFunc
// {
//     private OpenAIApi openai = new OpenAIApi();
//     public bool isgenerating = false;
//     public async void CallGPT(ChatMessage messages){
//         // Complete the instruction
//         var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
//         {
//             Model = "gpt-3.5-turbo",
//             Messages = messages
//         });

//         if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
//         {
//             var message = completionResponse.Choices[0].Message;
//             message.Content = message.Content.Trim();

            
//         }
//         else
//         {
//             Debug.LogWarning("No text was generated from this prompt.");
//         }
//     }
// }
