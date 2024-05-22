using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField][Header("Tutorial")] GameObject[] items;
    [SerializeField] PlayScene playScene;
    [SerializeField] string tutorialKey; // 각 씬에 대한 튜토리얼 키
    int itemIdx = 0;

    void Start()
    {
        // PlayerPrefs.DeleteKey(tutorialKey);
        if (PlayerPrefs.GetInt(tutorialKey, 0) == 1)
        {
            gameObject.SetActive(false);
            return;
        }

        if (items == null || items.Length == 0)
            return;

        foreach (var item in items)
        {
            item.SetActive(false);
        }

        itemIdx = -1;
        ActiveNextItem();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ActiveNextItem();
        }
    }

    public void ActiveNextItem()
    {
        if (itemIdx > -1 && itemIdx < items.Length)
        {
            items[itemIdx].SetActive(false);
        }

        itemIdx++;

        if (itemIdx > -1 && itemIdx < items.Length)
        {
            items[itemIdx].SetActive(true);
        }
        else
        {
            PlayerPrefs.SetInt(tutorialKey, 1);
            PlayerPrefs.Save();
            gameObject.SetActive(false);
            playScene.LoadPlayScene();
            playScene.PrintIntro();
        }
    }
}
