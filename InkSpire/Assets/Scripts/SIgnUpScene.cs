using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SIgnUpScene : MonoBehaviour
{
    [SerializeField] TMP_InputField input_email;
    [SerializeField] TMP_InputField input_nickname;
    [SerializeField] TMP_InputField input_pw;
    [SerializeField] TMP_InputField input_pwcheck;

    [SerializeField] TextMeshProUGUI wrong_pw;

    string new_email;
    string new_nickname;
    string new_password;
    string checkpw;

    public void SignupButton(){
        if(new_password == checkpw){
            NewAccount account = new NewAccount();
            account.email = new_email;
            account.nickname = new_nickname;
            account.password = new_password;
            string account_json = JsonUtility.ToJson(account);
            Debug.Log(account_json);
            // 서버와 통신하여 계정 생성
            SceneManager.LoadScene("Login");
        }
        else{
            Debug.Log("SignupError: pw and pwcheck not same");
            wrong_pw.text = "비밀번호가 맞는지 다시 확인해주세요";
        }
    }

    public void SetEmail(){
        new_email = input_email.text;
    }

    public void SetNickname(){
        new_nickname = input_nickname.text;
    }

    public void SetPassword(){
        new_password = input_pw.text;
    }

    public void SetCheckPW(){
        checkpw = input_pwcheck.text;
    }

    public class NewAccount{
        public string email;
        public string nickname;
        public string password;
    }
}
