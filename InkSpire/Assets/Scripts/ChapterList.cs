using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class ChapterList : MonoBehaviour
{
    string characterName = GetComponentInChildren<Text>().text;
    void SetTitle()
    {
        ModalTitle.text = characterName + " 's Story";
    }
}