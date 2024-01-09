using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToProfile : MonoBehaviour
{
    public void SceneChange()
    {
        SceneManager.LoadScene("Profile");
    }
}
