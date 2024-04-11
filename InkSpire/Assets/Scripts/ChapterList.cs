using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ChapterList : MonoBehaviour
{
    [SerializeField] GameObject Chapter1;
    [SerializeField] GameObject Chapter2;
    [SerializeField] GameObject Chapter3;
    [SerializeField] GameObject Chapter4;
    [SerializeField] GameObject Chapter5;
    [SerializeField] TextMeshProUGUI ModalTitle;
    string characterName;
    void SetTitle()
    {
        characterName = GetComponentInChildren<Text>().text;
        ModalTitle.text = characterName + " 's Story";
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