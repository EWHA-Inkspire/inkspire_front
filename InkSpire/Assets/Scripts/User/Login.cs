using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    [SerializeField] TMP_InputField input_email;
    [SerializeField] TMP_InputField input_pw;

    string email;
    string password;
    public void LoginButton(){
        if(true){
            LoginInfo login_info = new LoginInfo();
            login_info.email = email;
            login_info.password = password;
            string login_json = JsonUtility.ToJson(login_info);
            Debug.Log(login_json);
            // 서버와 통신하여 로그인
            SceneManager.LoadScene("");
        }
        else{
            Debug.Log("SignupError: pw and pwcheck not same");
        }
    }

    public void SignupButton(){
        SceneManager.LoadScene("Signup");
    }

     public void SetEmail(){
        email = input_email.text;
    }

    public void SetPassword(){
        password = input_pw.text;
    }


    public class LoginInfo{
        public string email;
        public string password;
    }
}
