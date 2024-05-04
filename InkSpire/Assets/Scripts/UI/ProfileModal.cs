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
        StartCoroutine(APIManager.api.GetRequest<ProfileInfo>("/users/" + user_id + "/profile", ProcessProfile));
        // 캐릭터 리스트 요청
        StartCoroutine(APIManager.api.GetRequest<CharacterList>("/users/" + user_id + "/characterList", ProcessCharacterList));
    }

    private void ProcessProfile(Response<ProfileInfo> response){
        if(response.success){
            Debug.Log(">>GET 결과: "+response.data);
        }
    }

    private void ProcessCharacterList(Response<CharacterList> response){
        if(response.success){
            Debug.Log(">>GET 결과: "+response.data);

            if(response.data != null) {
                foreach(Character character in response.data.characters){
                    Debug.Log("캐릭터: " + character.ToString());
                }
            }
        }
    }
}
