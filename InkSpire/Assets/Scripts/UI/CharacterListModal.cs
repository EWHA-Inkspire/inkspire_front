using UnityEngine;

public class CharacterListModal : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI title;
    [SerializeField] private GameObject chapter_1;
    [SerializeField] private GameObject chapter_2;
    [SerializeField] private GameObject chapter_3;
    [SerializeField] private GameObject chapter_4;
    [SerializeField] private GameObject chapter_5;

    void Awake()
    {
        chapter_1.SetActive(false);
        chapter_2.SetActive(false);
        chapter_3.SetActive(false);
        chapter_4.SetActive(false);
        chapter_5.SetActive(false);
    }

    public void SetChapter(string character_name, int char_id)
    {
        title.text = character_name + "\'s Story";
        PlayerPrefs.SetString("character_name", character_name);

        // 챕터 리스트 요청
        StartCoroutine(APIManager.api.GetRequest<ChapterList>("/characters/" + char_id + "/chapterList", ProcessChapterList));
    }

    private void ProcessChapterList(Response<ChapterList> response){
        if(!response.success){
            Debug.Log("챕터 리스트 요청 실패: " + response.message);
            return;
        }

        if(response.data == null){
            Debug.Log("챕터 리스트가 없습니다.");
            return;
        }

        Debug.Log(response.data.chapters);

        foreach(int chapter_num in response.data.chapters){
            switch(chapter_num){
                case 1:
                    chapter_1.SetActive(true);
                    break;
                case 2:
                    chapter_2.SetActive(true);
                    break;
                case 3:
                    chapter_3.SetActive(true);
                    break;
                case 4:
                    chapter_4.SetActive(true);
                    break;
                case 5:
                    chapter_5.SetActive(true);
                    break;
            }
        }
    }
}
