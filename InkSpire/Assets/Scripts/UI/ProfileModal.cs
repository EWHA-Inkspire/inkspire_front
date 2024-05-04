using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ProfileInfo {
    public string nickname;
    public string email;
    public int endingCount;
}

class CharacterList {
    public List<Character> characters;
}

class Character {
    public int id;
    public string name;
    public string success;
    public string fail;
}

public class ProfileModal : MonoBehaviour
{
    public void LoadProfile()
    {
        if(!PlayerPrefs.HasKey("user_id")) {
            return;
        }

        int user_id = PlayerPrefs.GetInt("user_id");

        // 유저 정보 요청
        StartCoroutine(APIManager.api.GetRequest("/users/" + user_id + "/profile", ProcessProfile));
        // 캐릭터 리스트 요청
        StartCoroutine(APIManager.api.GetRequest("/users/" + user_id + "/characterList", ProcessCharacterList));
    }

    private void ProcessProfile(Response response){
        if(response.success){
            Debug.Log(">>GET 결과: "+response.data);
            ProfileInfo profile = JsonUtility.FromJson<ProfileInfo>(response.data);
            Debug.Log("프로필 정보: " + profile.ToString());
        }
    }

    private void ProcessCharacterList(Response response){
        if(response.success){
            Debug.Log(">>GET 결과: "+response.data);
            CharacterList characterList = JsonUtility.FromJson<CharacterList>(response.data);

            if(characterList != null) {
                foreach(Character character in characterList.characters){
                    Debug.Log("캐릭터: " + character.ToString());
                }
            }
        }
    }
}
