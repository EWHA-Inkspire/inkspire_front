using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeButtonsOnLogin : MonoBehaviour
{
    public Button loginButton;
    public Button signUpButton;

    public void SetButtonsForLoggedInUser()
    {
        //버튼 내용 변경
        loginButton.GetComponentInChildren<Text>().text = "이어하기";
        signUpButton.GetComponentInChildren<Text>().text = "새 게임 시작하기";
    }

    public void SetButtonsForLoggedOutUser()
    {
        // 로그인 버튼 활성화
        //loginButton.gameObject.SetActive(true);
        loginButton.GetComponentInChildren<Text>().text = "Login";
        signUpButton.GetComponentInChildren<Text>().text = "Sign Up";
    }
}
