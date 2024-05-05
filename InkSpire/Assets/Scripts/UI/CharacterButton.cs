using UnityEngine;

public class CharacterButton : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI character_name;
    private GameObject chapter_list;
    private int char_id;

    public void SetCharacter(Character character, GameObject chapter_list)
    {
        this.char_id = character.id;
        this.character_name.text = character.name;
        this.chapter_list = chapter_list;
    }

    public void OnClick()
    {
        chapter_list.SetActive(true);
        // CharacterListModal 컴포넌트를 찾아서 SetChapter 함수 호출
        chapter_list.GetComponent<CharacterListModal>().SetChapter(character_name.text, char_id);
        PlayerPrefs.SetInt("character_id", char_id);
    }
}