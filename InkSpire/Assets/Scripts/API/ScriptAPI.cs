using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScriptAPI : MonoBehaviour
{
    public static ScriptAPI script_api;

    private void Awake()
    {
        if (script_api == null)
        {
            script_api = this;
            DontDestroyOnLoad(script_api);
        }
        else if (script_api != this)
        {
            Destroy(this);
            return;
        }
    }

    public void GetImageTexture(string url, System.Action<Texture2D> callback)
    {
        StartCoroutine(APIManager.api.GetTexture(url, callback));
    }

    public void PostScenarioInfo(Script script, string char_name)
    {
        ScenarioInfo scenarioInfo = new()
        {
            character = new CharacterInfo(char_name),
            script = new ScriptInfo(script.GetGenre(), script.GetTimeBackground(),
                        script.GetSpaceBackground(), script.GetWorldDetail())
        };

        string json = JsonUtility.ToJson(scenarioInfo);

        StartCoroutine(APIManager.api.PostRequest<PostScriptResponse>("/scripts", json, (response) => {
            if (response.success) {
                // 현재 플레이 중인 캐릭터 아이디 & 스크립트 아이디 저장
                PlayerPrefs.SetInt("character_id", response.data.characterId);
                PlayerPrefs.SetInt("script_id", response.data.scriptId);
            }
        }));
    }

    public void PostNpcInfo(List<Place> map, Npc pro_npc, Npc anta_npc)
    {
        PostNpcInfo pnpc = new()
        {
            mapId = map[0].id,
            name = pro_npc.GetName(),
            pnpc = true,
            detail = pro_npc.GetDetail(),
            luck = pro_npc.GetStat().GetStatAmount(StatType.Luck),
            defence = pro_npc.GetStat().GetStatAmount(StatType.Defence),
            mental = pro_npc.GetStat().GetStatAmount(StatType.Mental),
            dexterity = pro_npc.GetStat().GetStatAmount(StatType.Dexterity),
            attack = pro_npc.GetStat().GetStatAmount(StatType.Attack),
            hp = pro_npc.GetStat().GetStatAmount(StatType.Hp),
        };

        string json = JsonUtility.ToJson(pnpc);
        StartCoroutine(APIManager.api.PostRequest<int>("/npcs", json, (response) => {
            if (response.success) {
                ScriptManager.script_manager.SetNpcId(response.data, true);
            }
        }));

        int anpc_idx = map.FindIndex(x => x.ANPC_exist == 1);
        PostNpcInfo anpc = new()
        {
            mapId = anpc_idx >= 0 ? map[anpc_idx].id : map[0].id,
            name = anta_npc.GetName(),
            pnpc = false,
            detail = anta_npc.GetDetail(),
            luck = anta_npc.GetStat().GetStatAmount(StatType.Luck),
            defence = anta_npc.GetStat().GetStatAmount(StatType.Defence),
            mental = anta_npc.GetStat().GetStatAmount(StatType.Mental),
            dexterity = anta_npc.GetStat().GetStatAmount(StatType.Dexterity),
            attack = anta_npc.GetStat().GetStatAmount(StatType.Attack),
            hp = anta_npc.GetStat().GetStatAmount(StatType.Hp),
        };
        string json2 = JsonUtility.ToJson(anpc);

        StartCoroutine(APIManager.api.PostRequest<int>("/npcs", json2, (response) => {
            if (response.success) {
                ScriptManager.script_manager.SetNpcId(response.data, false);
            }
        }));
    }

    public void PutIntroInfo(Script script)
    {
        IntroInfo introInfo = new()
        {
            scriptId = PlayerPrefs.GetInt("script_id"),
            intro = script.GetIntro()
        };

        string json = JsonUtility.ToJson(introInfo);
        
        StartCoroutine(APIManager.api.PutRequest<Null>("/scripts/intro", json, (response) => { }));
    }

    public void PostMapInfo(Place place, Item item, Event game_event, int idx, int chapter_num)
    {
        PostMapInfo mapInfo = new()
        {
            scriptId = PlayerPrefs.GetInt("script_id"),
            idx = idx,
            chapter = chapter_num,
            name = place.place_name,
            info = place.place_info,
            anpc = place.ANPC_exist == 1
        };

        string json = JsonUtility.ToJson(mapInfo);

        StartCoroutine(APIManager.api.PostRequest<int>("/maps", json, (response) => {
            if (response.success) {
                ScriptManager.script_manager.SetMapId(response.data, idx);

                if(item.type != ItemType.Null) {
                    PostItemInfo(item, response.data);
                }
                if(item.type != ItemType.Mob && item.type != ItemType.Monster && item.type != ItemType.Null) {
                    PostEventInfo(game_event, response.data);
                }
            }
        }));
    }

    public void PostItemInfo(Item item, int map_id)
    {
        PostItemInfo itemInfo = new()
        {
            mapId = map_id,
            name = item.name,
            info = item.info,
            type = item.type.ToString(),
            stat = item.stat
        };

        string json = JsonUtility.ToJson(itemInfo);

        StartCoroutine(APIManager.api.PostRequest<int>("/inventory/item", json, (response) => {
            if (response.success) {
                ScriptManager.script_manager.SetItemId(response.data, map_id);
            }
        }));
    }

    public void PostEventInfo(Event game_event, int map_id)
    {
        PostEventInfo eventInfo = new()
        {
            mapId = map_id,
            isGoal = game_event.type == 1,
            eventTrigger = game_event.trigger,
            title = game_event.title,
            intro = game_event.intro,
            success = game_event.succ,
            failure = game_event.fail
        };

        string json = JsonUtility.ToJson(eventInfo);

        StartCoroutine(APIManager.api.PostRequest<int>("/events", json, (response) => {
            if (response.success) {
                ScriptManager.script_manager.SetEventId(response.data, map_id);
            }
        }));
    }

    internal void PostGoalInfo(Goal goal, int chapter_num)
    {
        PostGoalInfo goalInfo = new()
        {
            scriptId = PlayerPrefs.GetInt("script_id"),
            chapter = chapter_num,
            type = ConvertGoalType(goal.GetGoalType()),
            title = goal.GetTitle(),
            detail = goal.GetDetail(),
            etc = goal.GetEtc()
        };

        string json = JsonUtility.ToJson(goalInfo);

        StartCoroutine(APIManager.api.PostRequest<int>("/goals", json, (response) => {
            if (response.success) {
                ScriptManager.script_manager.SetGoalId(response.data, chapter_num);
            }
        }));
    }

    // 업적 정보 수정
    public void UpdateAchievement(string achievement)
    {
        string json = JsonUtility.ToJson(new Achivement { achievement = achievement });
        StartCoroutine(APIManager.api.PutRequest<Null>("/characters/" + PlayerPrefs.GetInt("character_id") + "/achievement", json, (response) => {
            if (response.success)
            {
                Debug.Log("Achievement Updated");
            }
        }));
    }

    // 챕터별 이미지 정보 저장
    public void PostImageInfo(int chapter, byte[] url)
    {
        PostImageInfo imageInfo = new()
        {
            scriptId = PlayerPrefs.GetInt("script_id"),
            chapter = chapter,
            url = url.ToString()
        };

        string json = JsonUtility.ToJson(imageInfo);

        StartCoroutine(APIManager.api.PostRequest<Null>("/images", json, (response) => { }));
    }

    // 게임 클리어 업데이트
    public void UpdateGameClear()
    {
        StartCoroutine(APIManager.api.PutRequest<Null>("/scripts/"+PlayerPrefs.GetInt("script_id")+"/isEnd", "", (response) => {
            if (response.success)
            {
                Debug.Log("Game Clear Updated");
            }
        }));
    }

    private string ConvertGoalType(int type)
    {
        return type switch
        {
            1 => "Monster",
            2 => "Item",
            3 => "Report",
            _ => "Null",
        };
    }
}