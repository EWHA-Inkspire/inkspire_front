using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClickHandler : MonoBehaviour
{
    public void OnClickStart() {
        Debug.Log("Start 버튼 클릭");
        SceneManager.LoadScene("3_CreateCharacter");
    }

    public void OnClickExplore() {
        Debug.Log("Explore 버튼 클릭");
        SceneManager.LoadScene("2_CharacterList");
    }

    public void OnClickProfile() {
        Debug.Log("Profile 버튼 클릭");
        SceneManager.LoadScene("7_Profile");
    }
}
