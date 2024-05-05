using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartScene : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI button_top;
    [SerializeField] private TextMeshProUGUI button_bottom;
    [SerializeField] private GameObject logged_in;
    [SerializeField] private GameObject logged_out;

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

    // 로그아웃 버튼
    public void OnClickLogout() {
        PlayerPrefs.DeleteAll();
        SetButtonsForLoggedOutUser();
    }

    public void SetButtonsForLoggedInUser()
    {
        // 로그인 모달 활성화
        logged_out.SetActive(false);
        logged_in.SetActive(true);
        // 버튼 내용 변경
        button_top.text = "탐험 목록 보러가기";
        button_bottom.text = "새 게임 시작하기";
    }

    public void SetButtonsForLoggedOutUser()
    {
        // 로그아웃 모달 활성화
        logged_in.SetActive(false);
        logged_out.SetActive(true);
        // 로그인 버튼 활성화
        button_top.text = "로그인";
        button_bottom.text = "회원가입";
    }
}
