using UnityEngine;

public class CharacterButton : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI character_name;
    private GameObject chapter_list;

    public void SetCharacter(Character character, GameObject chapter_list)
    {
        this.character_name.text = character.name;
        this.chapter_list = chapter_list;
    }

    public void OnClick()
    {
        chapter_list.SetActive(true);
    }
}