using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterListScene : MonoBehaviour
{
    [SerializeField] CharacterButton character_prefab;
    [SerializeField] GameObject chapter_list;
    
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
        if(!response.success){
            Debug.Log("캐릭터 리스트 요청 실패: " + response.message);
            return;
        }

        if(response.data == null){
            Debug.Log("캐릭터 리스트가 없습니다.");
            return;
        }

        foreach(Character character in response.data.characters){
            CharacterButton new_prefab = Instantiate(character_prefab);
            new_prefab.SetCharacter(character, chapter_list);
            new_prefab.transform.SetParent(GameObject.Find("CharacterList").transform);
        }
    }

    public void OnClickCreateCharacter(){
        SceneManager.LoadScene("3_CreateCharacter");
    }
}
