using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UserAccountInfo : MonoBehaviour
{
    // 유저 정보를 싱글톤으로 관리
    public static UserAccountInfo user;

    string email;
    string nickname;
    
    // 이외 필요 정보 변수로 만들어두기

    void Awake(){
        // 씬이 바뀔 때 파괴되지 않음
        DontDestroyOnLoad(this.gameObject);

        if(user == null){
            user=this;
        }
    }

    public string GetEmail(){
        return email;
    }

    public string GetNickname(){
        return nickname;
    }

    public void SetUserInfo(string input_email, string input_nickname){
        email = input_email;
        nickname = input_nickname;
    }
}
