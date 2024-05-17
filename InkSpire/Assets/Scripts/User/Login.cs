using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    [SerializeField] TMP_InputField input_email;
    [SerializeField] TMP_InputField input_pw;
    [SerializeField] TextMeshProUGUI wrong_pw;

    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene("1_Start");
            }
        }
    }

    public void LoginButton(){
        string user_email = input_email.text;
        string user_pw = input_pw.text;

        if(string.IsNullOrEmpty(user_email)) {
            wrong_pw.text = "이메일을 입력해주세요.";
            return;
        }

        if(string.IsNullOrEmpty(user_pw)) {
            wrong_pw.text = "비밀번호를 입력해주세요.";
            return;
        }

        LoginInfo login_info = new()
        {
            email = input_email.text,
            password = input_pw.text
        };
        string login_json = JsonUtility.ToJson(login_info);
        StartCoroutine(APIManager.api.PostRequest<int>("/users/login", login_json, ProcessResponse));
    }

    private void ProcessResponse(Response<int> response){
        if(response == null) {
            wrong_pw.text = "로그인에 실패했습니다. 다시 시도해주세요.";
            return;
        }

        if(response.success){
            PlayerPrefs.SetInt("user_id", response.data);
            SceneManager.LoadScene("1_Start");
        }
        else {
            wrong_pw.text = response.message;
        }
    }

    public void GoToSignUp(){
        SceneManager.LoadScene("Signup");
    }

    public void OnClickBack(){
        SceneManager.LoadScene("1_Start");
    }
}
