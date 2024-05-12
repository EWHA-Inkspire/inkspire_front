using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayAPI : MonoBehaviour
{
    public static PlayAPI play_api;

    private void Awake()
    {
        if (play_api == null)
        {
            play_api = this;
            DontDestroyOnLoad(play_api);
        }
        else if (play_api != this)
        {
            Destroy(this);
            return;
        }
    }

    public void UpdateCharacterStat(Stats stats)
    {
        int character_id = PlayerPrefs.GetInt("character_id");
        CharacterStatInfo stat_info = new(stats.GetStatAmount(StatType.Attack), stats.GetStatAmount(StatType.Defence), stats.GetStatAmount(StatType.Luck), stats.GetStatAmount(StatType.Mental), stats.GetStatAmount(StatType.Dexterity), stats.GetStatAmount(StatType.Hp));
        string json = JsonUtility.ToJson(stat_info);
        
        StartCoroutine(APIManager.api.PutRequest<Null>("/characters/"+character_id+"/stat", json, (response) => {
            Debug.Log(response.success);
        }));
    }

    public void UpdateGoalSuccess(int goal_id)
    {
        StartCoroutine(APIManager.api.PutRequest<Goal>("/goals/" + goal_id + "/success", null, (response) => {
            if (response.success)
            {
                Debug.Log("Goal Success");
            }
        }));
    }

    public void UpdateCurrPlace(int place_id)
    {
        StartCoroutine(APIManager.api.PutRequest<Place>("/maps/" + PlayerPrefs.GetInt("script_id") + "/" + place_id + "/visited", null, (response) => {
            if (response.success)
            {
                Debug.Log("Place Updated");
            }
        }));
    }

    public void PostInventory(int item_id)
    {
        PostInventory postInventory = new PostInventory
        {
            characterId = PlayerPrefs.GetInt("character_id"),
            itemId = item_id
        };

        string json = JsonUtility.ToJson(postInventory);
        StartCoroutine(APIManager.api.PostRequest<Item>("/inventory", json, (response) => {
            if (response.success)
            {
                Debug.Log("Item Added");
            }
        }));
    }

    public void DeleteInventory(int item_id)
    {
        StartCoroutine(APIManager.api.DeleteRequest<Item>("/inventory/" + PlayerPrefs.GetInt("character_id") + "/" + item_id, (response) => {
            if (response.success)
            {
                Debug.Log("Item Deleted");
            }
        }));
    }
}