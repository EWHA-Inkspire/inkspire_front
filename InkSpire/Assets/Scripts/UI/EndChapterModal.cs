using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndChapterModal : MonoBehaviour
{
    public void OnClickNextChapter(Play play)
    {
        play.SaveMessages();
        this.gameObject.SetActive(false);
        // 목표 달성
        if (ScriptManager.script_manager.GetCurrGoalClear())
        {
            if (ScriptManager.script_manager.GetCurrChap() == 1)
            {
                ScriptManager.script_manager.SetFinalPlace();
            }
            else if (ScriptManager.script_manager.GetCurrChap() == 2)
            {
                // 에필로그 씬 로드 
                SceneManager.LoadScene("6_Epilogue");
                // ScriptManager.script_manager.SetEpilogue();
            }
            else
            {
                ScriptManager.script_manager.SetNextChapter();
            }
        }
        else
        {
            if (ScriptManager.script_manager.GetCurrChap() == 1 || ScriptManager.script_manager.GetCurrChap() == 2)
            {
                // 에필로그 씬 로드
                SceneManager.LoadScene("6_Epilogue");
                // ScriptManager.script_manager.SetEpilogue();
            }
            else
            {
                ScriptManager.script_manager.SetNextChapter();
            }
        }
    }

    public void OnClickCurrChapter()
    {
        this.gameObject.SetActive(false);
    }
}
