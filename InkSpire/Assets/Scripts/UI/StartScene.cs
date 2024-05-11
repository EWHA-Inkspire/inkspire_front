using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartScene : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI button_top;
    [SerializeField] private TextMeshProUGUI button_bottom;
    [SerializeField] private GameObject logged_in;
    [SerializeField] private GameObject logged_out;

    public static string title;
    public static string nickname;
    public static string email;

    private readonly int LEVEL1 = 3;
    private readonly int LEVEL2 = 5;
    private readonly int LEVEL3 = 10;
    private readonly int LEVEL4 = 20;

    void Awake()
    {
        int user_id = PlayerPrefs.GetInt("user_id");
        // 유저 정보 요청
        StartCoroutine(APIManager.api.GetRequest<ProfileInfo>("/users/" + user_id + "/profile", ProcessProfile));
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("user_id"))
        {
            SetButtonsForLoggedInUser();
        }
        else
        {
            SetButtonsForLoggedOutUser();
        }
    }

    private void ProcessProfile(Response<ProfileInfo> response){
        if(response == null || !response.success || response.data == null) {
            return;
        }

        if(response.success){
            nickname = response.data.nickname;
            email = response.data.email;
            
            if(response.data.endingCount <= LEVEL1) {
                title = "초보 사서";
            }
            else if(response.data.endingCount <= LEVEL2) {
                title = "견습 사서";
            }
            else if(response.data.endingCount <= LEVEL3) {
                title = "일반 사서";
            }
            else if(response.data.endingCount <= LEVEL4) {
                title = "베테랑 사서";
            }
            else {
                title = "도서관장";
            }
        } else {
            PlayerPrefs.DeleteAll();
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
