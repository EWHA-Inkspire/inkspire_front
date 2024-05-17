using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterListScene : MonoBehaviour
{
    [SerializeField] CharacterButton character_prefab;
    [SerializeField] GameObject chapter_list;
    [SerializeField] ScrollRect scroll_rect;
    
    void Start()
    {
        if(!PlayerPrefs.HasKey("user_id")) {
            SceneManager.LoadScene("1_Start");
            return;
        }

        int user_id = PlayerPrefs.GetInt("user_id");

        // 캐릭터 리스트 요청
        StartCoroutine(APIManager.api.GetRequest<CharacterList>("/users/" + user_id + "/characterList", ProcessCharacterList));    
    }

    private void ProcessCharacterList(Response<CharacterList> response){
        if(!response.success || response.data == null){
            return;
        }

        foreach(Character character in response.data.characters){
            CharacterButton new_prefab = Instantiate(character_prefab);
            new_prefab.SetCharacter(character, chapter_list);
            new_prefab.transform.SetParent(GameObject.Find("CharacterList").transform);
            new_prefab.transform.localScale = Vector3.one;
            
            // 스크롤 뷰의 커서 이동
            StartCoroutine(ScrollToBottom());
        }
    }

    private IEnumerator ScrollToBottom()
    {
        // 다음 프레임까지 대기
        yield return null;

        // 스크롤을 맨 아래로 설정
        scroll_rect.verticalNormalizedPosition = 0f;
    }

    public void OnClickCreateCharacter(){
        SceneManager.LoadScene("3_CreateCharacter");
    }
}
