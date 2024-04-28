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

    public async Task<bool> EventCheckerGPT(List<ChatMessage> play_messages, Event game_event) {
        Debug.Log(">>Call Event Checker GPT");

        // 시스템 프롬프팅 추가
        var newMessage = new ChatMessage()
        {
            Role = "system",
            Content = @"당신은 게임의 플레이 데이터를 분석하여 이벤트 발생 여부를 판단하는 Checker이다. 
            대답은 반드시 T 혹은 F로 해야한다."
            + "트리거 장소: " + game_event.event_trigger + @"
            이벤트제목: " + game_event.event_title + @"
            도입 스크립트: " + game_event.event_intro + @"
            아래는 출력 예시이다.
            
            ex 1)
            트리거 장소: 폐허
            이벤트제목: 폐허 속의 보물
            도입 스크립트: 폐허 내부를 탐험하다가 갑자기 발을 헛디뎌 폐허 속의 이상한 기계 장치를 발견했습니다. 당신은 호기심을 느껴 손을 대자, 갑자기 기계가 작동하기 시작합니다.
            
            ex 2)
            트리거 장소: 숲, 마법의 포션(Potion)
            이벤트제목: 푸른 갈대 숲의 비밀
            도입 스크립트: 당신은 푸른 갈대 숲 속에서 이상한 빛을 발견했습니다. 빛을 쫓아가니 갈대 덤불 사이에서 작은 유리병이 놓여져 있었습니다. 병 안에는 수수한 액체가 담겨져 있고, 약한 빛을 내뿜고 있습니다. 당신은 그것이 마법의 포션이 아닐지도 모르겠다고 생각하며 손을 뻗습니다.

            ex 3)
            트리거 장소: 숲, 마법의 포션(Potion)
            이벤트제목: 푸른 갈대 숲의 비밀
            도입 스크립트: 당신은 푸른 갈대 숲 속에서 이상한 빛을 발견했습니다. 빛을 쫓아가니 갈대 덤불 사이에서 작은 유리병이 놓여져 있었습니다. 병 안에는 수수한 액체가 담겨져 있고, 약한 빛을 내뿜고 있습니다. 당신은 그것이 마법의 포션이 아닐지도 모르겠다고 생각하며 손을 뻗습니다.
            "
        };
        play_messages.Insert(0, newMessage);

        var response = await GptManager.gpt.CallGpt(play_messages);
        Debug.Log(response);

        if(response == "T") {
            return true;
        }

        // for(int i = 0; i < play_messages.Count; i++) {
        //     if(play_messages[i].Content.Contains(event_trigger)) {
        //         return true;
        //     }
        // }

        return false; // 임시 조정
    }
}
