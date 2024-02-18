using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

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
            StartCoroutine(PostNewAccount(new_email,new_password,new_nickname));
            //씬이 넘어가면 코루틴 안되는듯 수정할것
            //SceneManager.LoadScene("Login");
        }
        else{
            Debug.Log("SignupError: pw and pwcheck not same");
            wrong_pw.text = "비밀번호가 일치하지 않습니다.";
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

    IEnumerator PostNewAccount(string email, string password, string nickname){
        Debug.Log("upload start");
        WWWForm form = new WWWForm();
        form.AddField("email",email);
        form.AddField("password",password);
        form.AddField("nickname",nickname);

        UnityWebRequest www = UnityWebRequest.Post("http://3.35.61.61:8080/users/signup",form);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success){
            Debug.Log(www.error);
        }
        else{
            Debug.Log(">>NewAccount upload complete.");
        }
    }
}
