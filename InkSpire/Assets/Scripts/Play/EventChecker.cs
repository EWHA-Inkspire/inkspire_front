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

    public async Task<bool> EventCheckerGPT(string input_msg, Event game_event) {
        Debug.Log("이벤트 정보\n" + game_event.trigger + "\n" + game_event.title + "\n" + game_event.intro + "\n" + game_event.succ);
        // 시스템 프롬프팅 추가
        var newMessage = new ChatMessage()
        {
            Role = "system",
            Content = @"플레이어의 선택이 주어진 아이템 위치와 이벤트 도입 스크립트와 일치하는 행동인지 분석하여 T 혹은 F로 답변한다.
주어진 아이템 위치, 이벤트 제목, 이벤트 도입 스크립트, 플레이어 선택 텍스트를 분석하여 이벤트 발생 여부를 출력하시오.
플레이어 선택이 주어진 아이템 위치를 건드리거나 이벤트 도입 스크립트 속 행동을 한 경우에만 이벤트가 발생한다. 이 때 답변은 반드시 T 혹은 F로 해야한다. 이 때 답변은 반드시 T 혹은 F로 해야한다.
아래는 출력 예시이다.

ex 1)
아이템 위치: 책장
이벤트 제목: 어둠의 서약서
이벤트 도입 스크립트: 당신은 죽음의 흉터 안에 있는 책장을 뒤적이다가, 모든 먼지를 털어내고 있는 서약서를 발견합니다. 서약서는 누구의 흔적인지 알 수 없는 가상의 인물이 쓴 것으로 보입니다. 서약서를 조심스럽게 넘겨보니 그 안에는 어두운 마력을 담은 글귀가 새겨져 있습니다. 이것이 과연 어떤 의미를 갖는 것일까요?
플레이어 선택: 주변을 둘러본다
이벤트 발생: F

ex 2)
아이템 위치: 책장
이벤트 제목: 어둠의 서약서
이벤트 도입 스크립트: 당신은 죽음의 흉터 안에 있는 책장을 뒤적이다가, 모든 먼지를 털어내고 있는 서약서를 발견합니다. 서약서는 누구의 흔적인지 알 수 없는 가상의 인물이 쓴 것으로 보입니다. 서약서를 조심스럽게 넘겨보니 그 안에는 어두운 마력을 담은 글귀가 새겨져 있습니다. 이것이 과연 어떤 의미를 갖는 것일까요?
플레이어 선택: 주변에 시체 외의 다른 특이한 점은 없나요?
이벤트 발생: F

ex 3)
아이템 위치: 책장
이벤트 제목: 어둠의 서약서
이벤트 도입 스크립트: 당신은 죽음의 흉터 안에 있는 책장을 뒤적이다가, 모든 먼지를 털어내고 있는 서약서를 발견합니다. 서약서는 누구의 흔적인지 알 수 없는 가상의 인물이 쓴 것으로 보입니다. 서약서를 조심스럽게 넘겨보니 그 안에는 어두운 마력을 담은 글귀가 새겨져 있습니다. 이것이 과연 어떤 의미를 갖는 것일까요?
플레이어 선택: 빛나는 물체를 조사해볼래요
이벤트 발생: F

ex 4)
아이템 위치: 책장
이벤트 제목: 어둠의 서약서
이벤트 도입 스크립트: 당신은 죽음의 흉터 안에 있는 책장을 뒤적이다가, 모든 먼지를 털어내고 있는 서약서를 발견합니다. 서약서는 누구의 흔적인지 알 수 없는 가상의 인물이 쓴 것으로 보입니다. 서약서를 조심스럽게 넘겨보니 그 안에는 어두운 마력을 담은 글귀가 새겨져 있습니다. 이것이 과연 어떤 의미를 갖는 것일까요?
플레이어 선택: 책장을 먼저 조사해볼래요
이벤트 발생: T"
        };
        gpt_messages.Add(newMessage);

        newMessage.Role = "user";
        newMessage.Content = "아이템 위치: " + game_event.trigger + @"
            이벤트 제목: " + game_event.title + @"
            이벤트 도입 스크립트: " + game_event.intro + @"
            플레이어 선택: " + input_msg + @"
            이벤트 발생: ";
        gpt_messages.Add(newMessage);

        var response = await GptManager.gpt.CallGpt4(gpt_messages);

        if(response == "T") {
            return true;
        }

        return false;
    }
}
