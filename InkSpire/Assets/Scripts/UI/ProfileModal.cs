using UnityEngine;

public class ProfileModal : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI title;
    [SerializeField] TMPro.TextMeshProUGUI nickname;
    [SerializeField] TMPro.TextMeshProUGUI email;
    [SerializeField] GameObject start_scene;
    public GameObject characterName;

    public void Awake()
    {
        if(!PlayerPrefs.HasKey("user_id")) {
            return;
        }

        int user_id = PlayerPrefs.GetInt("user_id");
        SetProfile();
        // 캐릭터 리스트 요청
        StartCoroutine(APIManager.api.GetRequest<CharacterList>("/users/" + user_id + "/characterList", ProcessCharacterList));
    }

    private void SetProfile() {
        title.text = StartScene.start_scene.title;
        nickname.text = StartScene.start_scene.nickname;
        email.text = StartScene.start_scene.email;
    }

    private void ProcessCharacterList(Response<CharacterList> response){
        if(response == null) {
            Debug.Log("서버와의 통신에 실패했습니다.");
            return;
        }

        if(response.success && response.data != null){
            foreach(Character character in response.data.characters){
                // 캐릭터 이름 표시 게임 오브젝트 새로 생성
                GameObject new_character = Instantiate(characterName);
                new_character.GetComponent<TMPro.TextMeshProUGUI>().text = character.name;
                new_character.transform.SetParent(GameObject.Find("CharacterList").transform);
            }
        }
    }
}
