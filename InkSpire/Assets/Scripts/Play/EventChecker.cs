using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI;

public class EventChecker : MonoBehaviour {
    public static EventChecker eventChecker;

    void Awake(){
        if(eventChecker == null){
            eventChecker = this;
        }
        else if(eventChecker != this){
            Destroy(gameObject);
        }
    }

    private List<ChatMessage> gpt_messages = new();

    public async Task<bool> EventCheckerGPT(string assistant_msg, string input_msg, Event game_event) {
        assistant_msg = assistant_msg.Replace("Narrator:\n", "");
        assistant_msg = assistant_msg.Replace("Narrator:", "");

        Debug.Log("이벤트 정보\n" + game_event.event_trigger + "\n" + game_event.event_title + "\n" + game_event.event_intro + "\n" + game_event.event_succ);
        // 시스템 프롬프팅 추가
        var newMessage = new ChatMessage()
        {
            Role = "system",
            Content = @"주어진 이벤트 트리거, 이벤트 제목, 도입 스크립트, 시나리오 텍스트를 분석하여 이벤트 발생 여부를 출력하시오. 이벤트 발생 여부는 시나리오 텍스트에 이벤트 트리거와 관련된 행동이 있어야 발동한다. 이 때 답변은 반드시 T 혹은 F로 해야한다. 이 때 답변은 반드시 T 혹은 F로 해야한다.
            아래는 출력 예시이다.
            
            ex 1)
            이벤트 트리거: 책장
            이벤트 제목: 어둠의 서약서
            도입 스크립트: 당신은 죽음의 흉터 안에 있는 책장을 뒤적이다가, 모든 먼지를 털어내고 있는 서약서를 발견합니다. 서약서는 누구의 흔적인지 알 수 없는 가상의 인물이 쓴 것으로 보입니다. 서약서를 조심스럽게 넘겨보니 그 안에는 어두운 마력을 담은 글귀가 새겨져 있습니다. 이것이 과연 어떤 의미를 갖는 것일까요?
            시나리오: 이곳은 죽음의 흉터입니다.
            죽음의 그림자가 덮치는 곳, 썩어가는 시체의 냄새가 코를 찌르며 공포를 부르는 곳입니다.
            >> 주변을 둘러본다
            출력: F

            ex 2)
            이벤트 트리거: 책장
            이벤트 제목: 어둠의 서약서
            도입 스크립트: 당신은 죽음의 흉터 안에 있는 책장을 뒤적이다가, 모든 먼지를 털어내고 있는 서약서를 발견합니다. 서약서는 누구의 흔적인지 알 수 없는 가상의 인물이 쓴 것으로 보입니다. 서약서를 조심스럽게 넘겨보니 그 안에는 어두운 마력을 담은 글귀가 새겨져 있습니다. 이것이 과연 어떤 의미를 갖는 것일까요?
            시나리오: 주변을 살펴보니 썩어가는 시체들이 흩어져있고 끼어들지 않아도 밀려오는 썩은 냄새가 코를 찌릅니다. 어둠 속에서 이상한 소리가 들리며 당신을 불안하게 만듭니다. 혹시 죽음의 그림자 속에서 무언가를 발견할 수도 있을 것 같습니다.
            >> 주변에 시체 외의 다른 특이한 점은 없나요?
            출력: F

            ex 3)
            이벤트 트리거: 책장
            이벤트 제목: 어둠의 서약서
            도입 스크립트: 당신은 죽음의 흉터 안에 있는 책장을 뒤적이다가, 모든 먼지를 털어내고 있는 서약서를 발견합니다. 서약서는 누구의 흔적인지 알 수 없는 가상의 인물이 쓴 것으로 보입니다. 서약서를 조심스럽게 넘겨보니 그 안에는 어두운 마력을 담은 글귀가 새겨져 있습니다. 이것이 과연 어떤 의미를 갖는 것일까요?
            시나리오: 어둠 속에서 주변을 둘러보니, 썩은 나무로 만들어진 이상한 책장이 보입니다. 책장에 걸린 낡은 갈고리에는 피가 맺힌 옷 조각들이 달려있습니다. 또한 시체들 사이에서 빛나는 물체가 보이는데, 무엇인지 알 수 없지만 눈길을 끕니다. 이 물체에 대해 조사해볼까요?
            >> 빛나는 물체를 조사해볼래요
            출력: F

            ex 4)
            이벤트 트리거: 책장
            이벤트 제목: 어둠의 서약서
            도입 스크립트: 당신은 죽음의 흉터 안에 있는 책장을 뒤적이다가, 모든 먼지를 털어내고 있는 서약서를 발견합니다. 서약서는 누구의 흔적인지 알 수 없는 가상의 인물이 쓴 것으로 보입니다. 서약서를 조심스럽게 넘겨보니 그 안에는 어두운 마력을 담은 글귀가 새겨져 있습니다. 이것이 과연 어떤 의미를 갖는 것일까요?
            시나리오: 어둠 속에서 주변을 둘러보니, 썩은 나무로 만들어진 이상한 책장이 보입니다. 책장에 걸린 낡은 갈고리에는 피가 맺힌 옷 조각들이 달려있습니다. 또한 시체들 사이에서 빛나는 물체가 보이는데, 무엇인지 알 수 없지만 눈길을 끕니다. 이 물체에 대해 조사해볼까요?
            >> 책장을 먼저 조사해볼래요
            출력: T
            "
        };
        gpt_messages.Add(newMessage);

        newMessage.Role = "user";
        newMessage.Content = "이벤트 트리거: " + game_event.event_trigger + @"
            이벤트 제목: " + game_event.event_title + @"
            도입 스크립트: " + game_event.event_intro + @"
            시나리오: " + assistant_msg + @"
            >> " + input_msg + @"
            출력: ";
        gpt_messages.Add(newMessage);

        var response = await GptManager.gpt.CallGpt(gpt_messages);
        Debug.Log(">>이벤트 체커 결과: " + response);

        if(response == "T") {
            return true;
        }

        return false;
    }
}
