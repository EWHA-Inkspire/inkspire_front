using System;
using System.Collections.Generic;
using UnityEngine;

public class ProfileModal : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI title;
    [SerializeField] TMPro.TextMeshProUGUI nickname;
    [SerializeField] TMPro.TextMeshProUGUI email;

    private readonly int LEVEL1 = 3;
    private readonly int LEVEL2 = 5;
    private readonly int LEVEL3 = 10;
    private readonly int LEVEL4 = 20;

    public void Start()
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
            nickname.text = response.data.nickname;
            email.text = response.data.email;
            
            if(response.data.endingCount <= LEVEL1) {
                title.text = "초보 사서";
            }
            else if(response.data.endingCount <= LEVEL2) {
                title.text = "견습 사서";
            }
            else if(response.data.endingCount <= LEVEL3) {
                title.text = "일반 사서";
            }
            else if(response.data.endingCount <= LEVEL4) {
                title.text = "베테랑 사서";
            }
            else {
                title.text = "도서관장";
            }
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
