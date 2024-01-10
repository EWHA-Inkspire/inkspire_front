using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterList : MonoBehaviour
{
    public Text displayText;
    List<string> CharacterNames = new List<string>();
    void Start()
    {

    }

    public void AppendCharacter()
    {
        //input field에서 캐릭터 이름 가져오기
        string chartacterName = InputField.text;

        if (!string.IsNullOrEmpty(characterName))
        {
            //리스트에 캐릭터 append
            CharacterNames.Add(chartacterName);

            //Scene에 반영 
            UpdateScene();
        }
    }

    private void UpdateScene()
    {
        //리스트 내용 text로 Scene에 반영
        string nameList = null;
        foreach (string name in CharacterNames)
        {
            nameList += name + "\n";
        }

        displayText.text = nameList;
    }
}
