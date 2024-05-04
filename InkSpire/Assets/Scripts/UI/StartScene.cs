using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StartScene : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI button_top;
    [SerializeField] private TextMeshProUGUI button_bottom;
    [SerializeField] private Button profile_button;

    void Awake()
    {   
        // 로그인 여부에 따라 버튼 내용 변경
        if (PlayerPrefs.HasKey("user_id"))
        {
            SetButtonsForLoggedInUser();
        }
        else
        {
            SetButtonsForLoggedOutUser();
        }
    }

    // 탐험목록 or 로그인 버튼
    public void OnClickButtonTop() {
        if (PlayerPrefs.HasKey("user_id"))
        {
            SceneManager.LoadScene("2_CharacterList");
        }
        else
        {
            SceneManager.LoadScene("Login");
        }
    }

    // 시작하기 or 회원가입 버튼
    public void OnClickButtonBottom() {
        if (PlayerPrefs.HasKey("user_id"))
        {
            SceneManager.LoadScene("3_CreateCharacter");
        }
        else
        {
            SceneManager.LoadScene("Signup");
        }
    }

    // 프로필 버튼
    public void OnClickProfile() {
        SceneManager.LoadScene("7_Profile");
    }

    public void SetButtonsForLoggedInUser()
    {
        // 프로필 버튼 활성화
        profile_button.gameObject.SetActive(true);
        // 버튼 내용 변경
        button_top.text = "탐험 목록 보러가기";
        button_bottom.text = "새 게임 시작하기";
    }

    public void SetButtonsForLoggedOutUser()
    {
        // 프로필 버튼 바활성화
        profile_button.gameObject.SetActive(false);
        // 로그인 버튼 활성화
        button_top.text = "로그인";
        button_bottom.text = "회원가입";
    }
}
