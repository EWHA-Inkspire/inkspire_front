using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextInputFunctions : MonoBehaviour
{
    [SerializeField] TMP_InputField object_email;
    [SerializeField] TMP_InputField object_password;

    private string email;
    private string password;

    public void Login(){
        Debug.Log(object_email.text);
        Debug.Log(object_password.text);
        SetLoginInfo();
    }

    private void SetLoginInfo(){
        email = object_email.text;
        password = object_password.text;
    }

    public string GetLoginInfo(){
        return email+'/'+password;
    }

}
