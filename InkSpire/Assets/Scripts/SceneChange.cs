using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField] string next_scene;

    public void ChangeScene(){
        SceneManager.LoadScene(next_scene);
    }
}
