using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

class NewAccount {
    public string email;
    public string password;
    public string nickname;
}

public class SignUp : MonoBehaviour
{
    [SerializeField] TMP_InputField input_email;
    [SerializeField] TMP_InputField input_nickname;
    [SerializeField] TMP_InputField input_pw;
    [SerializeField] TMP_InputField input_pwcheck;

    [SerializeField] TextMeshProUGUI wrong_pw;
    private bool post_done = false;

    public void SignupButton(){
        string user_email = input_email.text;
        string user_nickname = input_nickname.text;
        string user_pw = input_pw.text;
        string user_pwcheck = input_pwcheck.text;

        if(string.IsNullOrEmpty(user_email)) {
            wrong_pw.text = "이메일을 입력해주세요.";
            return;
        }

        if(string.IsNullOrEmpty(user_nickname)) {
            wrong_pw.text = "닉네임을 입력해주세요.";
            return;
        }

        if(string.IsNullOrEmpty(user_pw)) {
            wrong_pw.text = "비밀번호를 입력해주세요.";
            return;
        }
        
        if(user_pw != user_pwcheck){
            wrong_pw.text = "비밀번호가 일치하지 않습니다.";
            return;
        }

        NewAccount account = new NewAccount();
        account.email = user_email;
        account.nickname = user_nickname;
        account.password = user_pw;
        string account_json = JsonUtility.ToJson(account);
        StartCoroutine(APIManager.api.PostRequest("/users/signup", account_json, ProcessResponse));
    }

    private void ProcessResponse(Response response){
        if(response.success){
            Debug.Log("회원가입 성공: " + response.message);
            SceneManager.LoadScene("Login");
        }
        else {
            Debug.Log("회원가입 실패: " + response.message);
            wrong_pw.text = response.message;
        }
    }
}
