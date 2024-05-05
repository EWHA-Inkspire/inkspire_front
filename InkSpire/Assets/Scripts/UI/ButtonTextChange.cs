using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonTextChange : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public Button button;
    Color newColor;
    string newColor_code;
    void Start()
    {
        if (button == null)
        {
            Debug.LogError("Button reference is not set. Please assign the button in the Inspector.");
            enabled = false; // 버튼이 없으면 스크립트를 비활성화
            return;
        }

        button.onClick.AddListener(OnClickButton_ChangeColor);
        buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
    }
    public void OnClickButton_ChangeColor()
    {
        newColor_code = "#B0B0B0"; //연회색 (빨간색은 #C53D3D)
        if (buttonText == null)
        {
            Debug.LogError("Text reference is not set. Please assign the text in the Inspector.");
            enabled = false; // 텍스트가 없으면 스크립트를 비활성화
            return;
        }
        if (ColorUtility.TryParseHtmlString(newColor_code, out newColor))
        {
            buttonText.color = newColor;
        }
    }
}
