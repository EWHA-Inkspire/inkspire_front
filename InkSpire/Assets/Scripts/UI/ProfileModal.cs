using System;
using UnityEngine;
using UnityEngine.UI;

public class ProfileModal : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI title;
    [SerializeField] TMPro.TextMeshProUGUI nickname;
    [SerializeField] TMPro.TextMeshProUGUI email;
    [SerializeField] GameObject start_scene;
    [SerializeField] GameObject book;
    public GameObject characterName;

    public void Awake()
    {
        if(!PlayerPrefs.HasKey("user_id")) {
            return;
        }

        int user_id = PlayerPrefs.GetInt("user_id");
        SetProfile();

        // 업적 정보 요청
        StartCoroutine(APIManager.api.GetRequest<AchieveList>("/users/" + user_id + "/achievements", ProcessAchieveList));
    }

    private void ProcessAchieveList(Response<AchieveList> response)
    {
        if(!response.success || response.data == null){
            return;
        }

        foreach(Achieve acheive in response.data.achievements){
            GameObject new_prefab = Instantiate(Resources.Load<GameObject>("Prefabs/Book"), GameObject.Find("AchieveList").transform);
            new_prefab.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>(acheive.genre) ?? Resources.Load<Sprite>("추리");
            new_prefab.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = acheive.title;
        }
    }

    private void SetProfile() {
        title.text = StartScene.title;
        nickname.text = StartScene.nickname;
        email.text = StartScene.email;
    }
}
