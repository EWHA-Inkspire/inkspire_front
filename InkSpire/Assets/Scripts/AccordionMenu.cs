using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccordionMenu : MonoBehaviour
{
    public GameObject[] panels; // 각 아코디언 패널 저장 배열
    public Button[] toggleButtons; // 각 패널을 토글하는 버튼 배열

    void Start()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            int index = i; // 인덱스 저장(클로저 문제)
            toggleButtons[i].onClick.AddListener(() => TogglePanel(index));
        }
    }

    // 패널 토글 함수
    void TogglePanel(int index)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (i == index)
            {
                // 선택한 패널이면 활성화/비활성화 토글
                panels[i].SetActive(!panels[i].activeSelf);
            }
            else
            {
                // 선택하지 않은 패널은 비활성화
                panels[i].SetActive(false);
            }
        }
    }
}

