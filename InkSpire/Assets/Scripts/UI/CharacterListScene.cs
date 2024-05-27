using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterListScene : MonoBehaviour
{
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

    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene("1_Start");
            }
        }
    }

    private void ProcessCharacterList(Response<CharacterList> response){
        if(!response.success || response.data == null){
            return;
        }

        foreach(Character character in response.data.characters){
            CharacterButton new_prefab = Instantiate(Resources.Load<CharacterButton>("Prefabs/Character"), GameObject.Find("CharacterList").transform);
            new_prefab.SetCharacter(character, chapter_list);
        }
    }

    public void OnClickCreateCharacter(){
        PlayerPrefs.DeleteKey("character_id");
        PlayerPrefs.DeleteKey("script_id");
        PlayerPrefs.DeleteKey("character_name");
        PlayerPrefs.DeleteKey("Call API");
        SceneManager.LoadScene("3_CreateCharacter");
    }

    public void OnClickBack(){
        SceneManager.LoadScene("1_Start");
        PlayerPrefs.DeleteKey("script_id");
        PlayerPrefs.DeleteKey("character_id");
        PlayerPrefs.DeleteKey("character_name");
    }
}
