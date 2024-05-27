using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndChapterModal : MonoBehaviour
{
    [SerializeField] private GameObject end_button;
    [SerializeField] private ButtonTextChange buttonTextChange;
    public void OnClickNextChapter(Play play)
    {
        end_button.SetActive(false);
        play.SaveMessages();
        this.gameObject.SetActive(false);

        if(ScriptManager.script_manager.GetCurrChap() < Const.CHAPTER - 2)
        {
            ScriptManager.script_manager.SetNextChapter();
            return;
        }

        if(ScriptManager.script_manager.GetCurrChap() == Const.CHAPTER - 1)
        {
            // 에필로그 씬 로드 
            SceneManager.LoadScene("6_Epilogue");
            return;
        }

        // 목표 달성
        if (ScriptManager.script_manager.CheckGoalCleared())
        {
            ScriptManager.script_manager.SetFinalPlace();
            return;
        }
        else
        {
            // 에필로그 씬 로드 
            SceneManager.LoadScene("6_Epilogue");
        }
    }

    public void OnClickCurrChapter()
    {
        end_button.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
