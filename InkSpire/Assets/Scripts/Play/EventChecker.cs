using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenAI;
using TMPro;

public class EventChecker : MonoBehaviour {
    public static EventChecker eventChecker;

    void Awake(){
        // 씬이 바뀔 때 파괴되지 않음
        DontDestroyOnLoad(this.gameObject);

        if(eventChecker == null){
            eventChecker = this;
        }
    }

    private List<ChatMessage> gpt_messages = new List<ChatMessage>();

    public async Task<bool> EventCheckerGPT(string assistant_msg, string input_msg, Event game_event) {
        // 시스템 프롬프팅 추가
        var newMessage = new ChatMessage()
        {
            Role = "system",
            Content = @"주어진 트리거 장소, 이벤트 제목, 도입 스크립트, 시나리오, 행동지문 텍스트를 분석하여 이벤트 발생 여부를 판단하여 출력하시오. 이 때 답변은 반드시 T 혹은 F로 해야한다.
            아래는 출력 예시이다.
            
            ex)
            트리거 장소: 폐허
            이벤트 제목: 폐허 속의 보물
            도입 스크립트: 폐허 내부를 탐험하다가 갑자기 발을 헛디뎌 폐허 속의 이상한 기계 장치를 발견했습니다. 당신은 호기심을 느껴 손을 대자, 갑자기 기계가 작동하기 시작합니다.
            시나리오: 
            행동지문: 주변을 둘러본다.
            이벤트 발생 여부: F

            트리거 장소: 폐허
            이벤트 제목: 폐허 속의 보물
            도입 스크립트: 폐허 내부를 탐험하다가 갑자기 발을 헛디뎌 폐허 속의 이상한 기계 장치를 발견했습니다. 당신은 호기심을 느껴 손을 대자, 갑자기 기계가 작동하기 시작합니다.
            시나리오: 
            행동지문: 폐허로 들어가 내부를 탐험한다.
            이벤트 발생 여부: T
            
            트리거 장소: 숲, 마법의 포션(Potion)
            이벤트 제목: 푸른 갈대 숲의 비밀
            도입 스크립트: 당신은 푸른 갈대 숲 속에서 이상한 빛을 발견했습니다. 빛을 쫓아가니 갈대 덤불 사이에서 작은 유리병이 놓여져 있었습니다. 병 안에는 수수한 액체가 담겨져 있고, 약한 빛을 내뿜고 있습니다. 당신은 그것이 마법의 포션이 아닐지도 모르겠다고 생각하며 손을 뻗습니다.
            시나리오: 
            행동지문: 
            이벤트 발생 여부: F
            
            트리거 장소: 숲, 마법의 포션(Potion)
            이벤트 제목: 푸른 갈대 숲의 비밀
            도입 스크립트: 당신은 푸른 갈대 숲 속에서 이상한 빛을 발견했습니다. 빛을 쫓아가니 갈대 덤불 사이에서 작은 유리병이 놓여져 있었습니다. 병 안에는 수수한 액체가 담겨져 있고, 약한 빛을 내뿜고 있습니다. 당신은 그것이 마법의 포션이 아닐지도 모르겠다고 생각하며 손을 뻗습니다.
            시나리오: 
            행동지문: 숲 속의 이상한 빛을 발견한다.
            이벤트 발생 여부: T"
        };
        gpt_messages.Add(newMessage);

        newMessage.Role = "user";
        newMessage.Content = "트리거 장소: " + game_event.event_trigger + @"
            이벤트 제목: " + game_event.event_title + @"
            도입 스크립트: " + game_event.event_intro + @"
            시나리오: " + assistant_msg + @"
            행동지문: " + input_msg + @"
            이벤트 발생 여부: ";
        gpt_messages.Add(newMessage);

        var response = await GptManager.gpt.CallGpt(gpt_messages);
        Debug.Log(">>이벤트 체커 결과: " + response);

        if(response == "T") {
            return false;
        }

        return false;
    }
}
