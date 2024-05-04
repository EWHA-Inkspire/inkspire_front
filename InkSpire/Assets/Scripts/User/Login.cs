using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

class LoginInfo {
    public string email;
    public string password;
}

public class Login : MonoBehaviour
{
    [SerializeField] TMP_InputField input_email;
    [SerializeField] TMP_InputField input_pw;
    [SerializeField] TextMeshProUGUI wrong_pw;

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

        LoginInfo login_info = new LoginInfo();
        login_info.email = input_email.text;
        login_info.password = input_pw.text;
        string login_json = JsonUtility.ToJson(login_info);
        StartCoroutine(APIManager.api.PostRequest<int>("/users/login", login_json, ProcessResponse));
    }

    private void ProcessResponse(Response<int> response){
        if(response.success){
            Debug.Log("로그인 성공: " + response.message);
            PlayerPrefs.SetInt("user_id", response.data);
            SceneManager.LoadScene("1_Start");
        }
        else {
            Debug.Log("로그인 실패: " + response.message);
            wrong_pw.text = response.message;
        }
    }

    public void GoToSignUp(){
        SceneManager.LoadScene("Signup");
    }
}
