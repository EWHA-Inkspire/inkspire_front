using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonTextChange : MonoBehaviour
{
    private TextMeshProUGUI[] buttonTexts;
    private Button[] buttons;
    private Button lastClickedButton = null;
    private Color clickedTextColor = Color.gray;
    private Color redColor = Color.red; // 빨간색

    void Start()
    {
        // 초기 버튼의 텍스트 색상 설정
        InitButton();
    }

    public void InitButton()
    {
        lastClickedButton = null;
        buttons = GetComponentsInChildren<Button>();
        buttonTexts = GetComponentsInChildren<TextMeshProUGUI>();

        foreach (Button button in buttons)
        {
            Debug.Log(button.name);
            button.onClick.AddListener(() => OnClickButton_ChangeColor(button));
        }

        // 초기 버튼의 텍스트 색상 설정
        foreach (TextMeshProUGUI buttonText in buttonTexts)
        {
            Debug.Log(buttonText.text);
            buttonText.color = Color.black;
        }
    }

    public void OnClickButton_ChangeColor(Button clickedButton)
    {
        // 이전에 클릭된 버튼의 텍스트 색상을 회색으로 변경
        if (lastClickedButton != null)
        {
            lastClickedButton.GetComponentInChildren<TextMeshProUGUI>().color = clickedTextColor;
        }

        // 현재 클릭된 버튼을 저장하여 이후에 다시 회색으로 변경할 수 있도록 함
        lastClickedButton = clickedButton;

        // 가장 최근에 클릭된 버튼의 텍스트 색상을 빨간색으로 변경
        clickedButton.GetComponentInChildren<TextMeshProUGUI>().color = redColor;
    }
}
