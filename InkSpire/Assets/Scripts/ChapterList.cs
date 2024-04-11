using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ChapterList : MonoBehaviour
{
    //string characterName = GetComponentInChildren<Text>().text;
    void SetTitle()
    {
        //ModalTitle.text = characterName + " 's Story";
    }

    void activateChapter()
    {
        Chapter1.gameObject.SetActive(true);
        if (ScriptManager.scriptinfo.curr_chapter >= 2)
            Chapter2.gameObject.SetActive(true);
        else if (ScriptManager.scriptinfo.curr_chapter >= 3)
            Chapter3.gameObject.SetActive(true);
        else if (ScriptManager.scriptinfo.curr_chapter >= 4)
            Chapter4.gameObject.SetActive(true);
        else
            Chapter5.gameObject.SetActive(true);
    }
}