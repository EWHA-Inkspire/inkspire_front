using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndChapterModal : MonoBehaviour
{
    public void OnClickNextChapter()
    {
        this.gameObject.SetActive(false);
        // 목표 달성
        if(ScriptManager.script_manager.GetCurrGoalClear())
        {
            if (ScriptManager.script_manager.GetCurrChap() == 3)
            {
                ScriptManager.script_manager.SetFinalPlace();
            }
            else if (ScriptManager.script_manager.GetCurrChap() == 4)
            {
                ScriptManager.script_manager.SetEpilogue();
            }
            else
            {
                ScriptManager.script_manager.SetNextChapter();
            }
        }
        else
        {
            if (ScriptManager.script_manager.GetCurrChap() == 3 || ScriptManager.script_manager.GetCurrChap() == 4)
            {
                ScriptManager.script_manager.SetEpilogue();
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
