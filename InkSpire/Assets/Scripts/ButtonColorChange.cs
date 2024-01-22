using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChange : MonoBehaviour
{
    public Button button;
    private bool isChoose = false;

    private void Start()
    {
        if (button != null)
        {
            // 버튼에 OnClick 이벤트 등록
            button.onClick.AddListener(ColorChange);
        }
        else
        {
            Debug.LogError("Button reference is not set. Please assign the button in the Inspector.");
        }
    }

    // HEX 컬러 코드를 Color로 변환
    private Color HexToColor(string hex, float O)
    {
        Color color = new Color();
        ColorUtility.TryParseHtmlString(hex, out color);
        color.a = O;
        return color;
    }

    public void ColorChange()
    {
        ColorBlock colorBlock = button.colors;

        isChoose = !isChoose; // 상태 토글

        // 선택 여부에 따라 색상 및 투명도 설정
        colorBlock.normalColor = isChoose ? new Color(0, 0f, 0, 0f) : Color.white;
        colorBlock.selectedColor = isChoose ? HexToColor("#E8CBCB", 0.5f) : Color.white; //#E8CBCB 분홍색

        button.colors = colorBlock;
    }
}