using System;
using System.Collections.Generic;
using System.Linq;
using OpenAI;
using Unity.VisualScripting;
using UnityEngine;

public class PlayAPI : MonoBehaviour
{
    public static PlayAPI play_api;
    private int save_idx = 0;
    private readonly int SAVING_INTERVAL = 2;

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

    public void GetChatList(Play play)
    {
        int script_id = PlayerPrefs.GetInt("script_id");
        int chapter_num = ScriptManager.script_manager.GetViewChap() + 1;

        // 스크립트 대화 내용 조회
        StartCoroutine(APIManager.api.GetRequest<ChatList>("/chat/" + script_id + "/" + chapter_num, (response) => {
            if (!response.success || response.data == null || response.data.chats == null)
            {
                return;
            }

            List<ChatMessage> messages = new();

            foreach (var chat in response.data.chats)
            {
                var newMessage = new ChatMessage()
                {
                    Role = chat.role,
                    Content = chat.content
                };
                messages.Add(newMessage);
            }
            save_idx = Mathf.Max(messages.Count, 0); // 프롬프팅 인덱싱 추가
            play.InitMessages(messages);
            play.text_scroll.InitStoryObj(messages);
        }));
    }

    // API 호출 - 채팅 리스트 저장
    public void PostChatList(List<ChatMessage> messages)
    {
        PostChatList(messages, ScriptManager.script_manager.GetCurrChap() + 1);
    }

    public void PostChatList(List<ChatMessage> messages, int chapter_num)
    {
        if (messages.Count - save_idx < SAVING_INTERVAL)
        {
            return;
        }

        var chatList = new ChatList()
        {
            chats = messages.GetRange(save_idx, messages.Count - save_idx)
                .Select(msg => new ChatInfo()
                {
                    scriptId = PlayerPrefs.GetInt("script_id"),
                    role = msg.Role,
                    content = msg.Content,
                    chapter = chapter_num
                }).ToList()
        };

        string chats = JsonUtility.ToJson(chatList);

        StartCoroutine(APIManager.api.PostRequest<ChatList>("/chat", chats, response =>
        {
            if (!response.success)
            {
                return;
            }
            save_idx = messages.Count;
        }));
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

    public void GetInventory()
    {
        int character_id = PlayerPrefs.GetInt("character_id");
        StartCoroutine(APIManager.api.GetRequest<GetInventory>("/inventory/" + character_id, (response) => {
            if (!response.success || response.data == null)
            {
                return;
            }

            List<Item> inventories = new();

            foreach (var item in response.data.items)
            {
                List<Item> items = ScriptManager.script_manager.GetItems();
                int idx = items.FindIndex(x => x.id == item.itemId);
                inventories.Add(items[idx]);
            }

            InventoryManager.i_manager.SetInventory(inventories);
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

    public void InitSaveIdx()
    {
        save_idx = 0;
    }
}