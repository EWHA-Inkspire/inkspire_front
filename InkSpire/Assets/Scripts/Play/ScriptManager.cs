using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScriptManager : MonoBehaviour
{
    public static ScriptManager script_manager;
    private string char_name;
    private int curr_chapter = 0;
    private int view_chapter = 0;
    private int curr_place_idx = 0;

    private Script script;
    private List<Goal> goals = new();
    private List<Place> map = new();
    private List<Item> items = new();
    private List<Event> game_events = new();
    private List<string> place_names = new();
    private Npc pro_npc;
    private Npc anta_npc;
    private string achivement;

    private bool init_script = false;

    // 일반함수
    private void Awake()
    {
        if (script_manager == null)
        {
            script_manager = this;
            DontDestroyOnLoad(script_manager);
        }
        else if (script_manager != this)
        {
            Destroy(this);
            return;
        }

        // 필드 초기화
        script = new Script();
        for (int i = 0; i < Const.CHAPTER; i++)
        {
            goals.Add(new Goal());
        }

        for (int i = 0; i < Const.PLACE_COUNT; i++)
        {
            map.Add(new Place());
            items.Add(new Item());
            game_events.Add(new Event());
            place_names.Add("");
        }

        pro_npc = new Npc();
        anta_npc = new Npc();
    }

    // 초기 스크립트 틀 생성 (장소 4개)
    public async void SetScriptInfo(string char_name, string genre, string time_background, string space_background)
    {
        this.char_name = char_name;

        // 세계관 생성
        await script.InitScript(genre, time_background, space_background);
        ScriptAPI.script_api.PostScenarioInfo(script, char_name);

        // 목표 생성
        await goals[Const.CHAPTER-1].InitGoal(time_background, space_background, script.GetWorldDetail(), genre);
        await goals[0].InitGoal(time_background, space_background, script.GetWorldDetail(), genre, goals[Const.CHAPTER-1]);
        ScriptAPI.script_api.PostGoalInfo(goals[Const.CHAPTER-1], Const.CHAPTER);
        ScriptAPI.script_api.PostGoalInfo(goals[0], curr_chapter + 1);

        // npc 정보 생성
        await pro_npc.InitNpc("P", script.GetWorldDetail(), genre, char_name);
        await anta_npc.InitNpc("A", script.GetWorldDetail(), genre, char_name);
        // PNPC 장소 초기화
        map[0].ANPC_exist = 0;

        // 맵 정보 생성
        ChooseEventType(); // 챕터 장소별 이벤트 타입 설정
        // PNPC 장소 초기화
        map[0].ANPC_exist = 0;
        await map[0].CreatePnpcPlace(script, pro_npc);
        ScriptAPI.script_api.PostMapInfo(map[0], items[0], game_events[0], 0, curr_chapter + 1);

        // 일반 장소 초기화
        for (int i = 1; i < 4; i++)
        {
            // 목표 정보 전달
            await items[i].InitItem(script, goals[curr_chapter], game_events[i].type);
            await map[i].InitPlace(i, script, items[i], game_events[i].type, place_names);
            place_names[i] = map[i].place_name;

            // 전투 이벤트(잡몹, 적 처치) 혹은 item_type이 null일 경우에는 이벤트 트리거 생성하지 않음
            if (items[i].type != ItemType.Mob && items[i].type != ItemType.Monster && items[i].type != ItemType.Null)
            {
                await game_events[i].CreateEventTrigger(script.GetWorldDetail(), goals[curr_chapter].GetDetail(), place_names[i], items[i].name);
            }

            ScriptAPI.script_api.PostMapInfo(map[i], items[i], game_events[i], i, curr_chapter + 1);
        }

        achivement = await script.AchivementGPT();
        ScriptAPI.script_api.UpdateAchievement(achivement);
        Debug.Log("업적명:" + achivement);
        await script.IntroGPT(pro_npc, anta_npc, map[0].place_name, map[0].place_info, this.char_name);
        ScriptAPI.script_api.PutIntroInfo(script);
        init_script = true;
    }

    // 장소 별 이벤트 타입 설정 (3개 장소마다 목표 이벤트 출현 장소 정하는 로직)
    private void ChooseEventType()
    {
        int place_base = curr_chapter*3 + 1;
        int flag = Random.Range(0, 3);
        int[] idxs = { place_base, place_base+1, place_base+2 };
        game_events[idxs[flag]].type = 1;
    }

    public async void SetNextChapter()
    {
        PlayScene.play_scene.LoadNextChapUI();
        // 회귀용 다음 챕터 넘기는 기능 
        if(goals[curr_chapter+1].GetTitle()!=""){
            for(int i = curr_chapter+1; i<Const.CHAPTER;i++){
                if(!goals[i].GetClear()){
                    curr_chapter = i;
                    break;
                }
            }
            if(goals[curr_chapter].GetTitle()!=""){
                PlayScene.play_scene.LoadChapter(false);
                return;
            }
        }


        string genre = script.GetGenre();
        string time_background = script.GetTimeBackground();
        string space_background = script.GetSpaceBackground();

        // 이전 챕터 결과 요약
        string prev_result = GetPrevResult(curr_chapter);

        script.ChapterIntroGPT(goals[curr_chapter].GetDetail(), prev_result, curr_chapter + 1);

        curr_chapter++;
        view_chapter = curr_chapter;
        int place_base = curr_chapter * 3 + 1;
        ChooseEventType(); // 챕터 장소별 이벤트 타입 설정
        await goals[curr_chapter].InitGoal(time_background, space_background, script.GetWorldDetail(), genre, goals[Const.CHAPTER-1]);
        ScriptAPI.script_api.PostGoalInfo(goals[curr_chapter], curr_chapter + 1);
        for (int i = 0; i < 3; i++)
        {
            // 목표 정보 전달
            Debug.Log("장소 idx: "+(place_base + i));
            await items[place_base + i].InitItem(script, goals[curr_chapter], game_events[place_base + i].type);
            await map[place_base + i].InitPlace(place_base + i, script, items[place_base + i], game_events[place_base + i].type, place_names);
            place_names[place_base + i] = map[place_base + i].place_name;

            // 전투 이벤트(잡몹, 적 처치) 혹은 item_type이 null일 경우에는 이벤트 트리거 생성하지 않음
            if (items[place_base + i].type != ItemType.Mob && items[place_base + i].type != ItemType.Monster && items[place_base + i].type != ItemType.Null)
            {
                await game_events[place_base + i].CreateEventTrigger(script.GetWorldDetail(), goals[curr_chapter].GetDetail(), place_names[place_base + i], items[place_base + i].name);
            }

            ScriptAPI.script_api.PostMapInfo(map[place_base + i], items[place_base + i], game_events[place_base + i], place_base + i, curr_chapter + 1);
        }

        if(script.GetIntroImage() == null)
        {
            StartCoroutine(DelayLoadChapter());
        }
        else
        {
            PlayScene.play_scene.LoadChapter(true);
        }
    }

    IEnumerator DelayLoadChapter()
    {
        while(script.GetIntroImage() == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        PlayScene.play_scene.LoadChapter(true);
    }

    public string GetPrevResult(int chapter)
    {
        int place_base = chapter * 3 + 1;

        if(goals[chapter].GetClear())
        {
            if(goals[chapter].GetGoalType() == 1)
            {
                return "적을 처치했다.";
            }
            for (int i = 0; i < 3; i++)
            {
                if (game_events[place_base + i].type == 0)
                {
                    continue;
                }
                return game_events[place_base + i].succ;
            }

            return "목표 달성 성공";
        }

        if(goals[chapter].GetGoalType() == 1)
        {
            return "적을 처치하지 못했다.";
        }

        for (int i = 0; i < 3; i++)
        {
            if (game_events[place_base + i].type == 0)
            {
                continue;
            }
            return game_events[place_base + i].fail;
        }

        return "목표 달성 실패";
    }

    public async void SetFinalPlace()
    {
        PlayScene.play_scene.LoadNextChapUI();

        // 이전 챕터 결과 요약
        string prev_result = GetPrevResult(curr_chapter);

        script.ChapterIntroGPT(goals[curr_chapter].GetDetail(), prev_result, curr_chapter + 1);
        
        curr_chapter = Const.CHAPTER-1;
        view_chapter = curr_chapter;
        int place_base = curr_chapter * 3 + 1;

        // 최종 장소 목표 정보 전달
        game_events[place_base].type = 1;
        await items[place_base].InitItem(script, goals[curr_chapter], game_events[place_base].type);
        await map[place_base].InitPlace(curr_chapter, script, items[place_base], game_events[place_base].type, place_names);
        place_names[place_base] = map[place_base].place_name;

        if (items[place_base].type != ItemType.Monster)
        {
            await game_events[place_base].CreateEventTrigger(script.GetWorldDetail(), goals[curr_chapter].GetDetail(), place_names[place_base], items[place_base].name);
        }

        ScriptAPI.script_api.PostMapInfo(map[place_base], items[place_base], game_events[place_base], place_base, curr_chapter + 1);

        if(script.GetIntroImage() == null)
        {
            StartCoroutine(DelayLoadChapter());
        }
        else
        {
            PlayScene.play_scene.LoadChapter(true);
        }
    }

    // Settter
    public void SetCurrChap(int chap)
    {
        curr_chapter = chap;
    }

    public void SetCharName(string name)
    {
        char_name = name;
    }

    public void SetCurrPlace(int idx)
    {
        curr_place_idx = idx;
    }

    public void SetMapId(int id, int idx)
    {
        map[idx].SetMapId(id);
    }

    public void SetPlaceClear(bool clr)
    {
        map[curr_place_idx].SetClear(clr);
    }

    public void SetInitScript(bool setscript)
    {
        init_script = setscript;
    }

    public void SetScriptInfo(GetScriptInfo script_info)
    {
        script.SetScriptInfo(script_info);
    }

    public void SetGoalId(int id, int chapter)
    {
        this.goals[chapter - 1].SetGoalId(id);
    }

    public void SetGoalList(GetGoalList goal_list)
    {
        for (int i = 0; i < goal_list.goals.Count; i++)
        {
            int chapter_num = goal_list.goals[i].chapter;
            this.goals[chapter_num - 1].SetGoalInfo(goal_list.goals[i]);
        }
    }

    public void SetNpcId(int id, bool pnpc)
    {
        if (pnpc)
            pro_npc.SetNpcId(id);
        else
            anta_npc.SetNpcId(id);
    }

    public void SetNpcList(GetNpcList npc_list)
    {
        foreach (GetNpcInfo npc in npc_list.npcs)
        {
            if (npc.pnpc)
                pro_npc.SetNpcInfo(npc);
            else
                anta_npc.SetNpcInfo(npc);
        }
    }

    public void SetMapList(GetMapList map_list)
    {
        map_list.maps.Sort((a, b) => a.idx.CompareTo(b.idx));
        foreach (GetMapInfo map in map_list.maps)
        {
            int idx = map.idx;
            this.map[idx].SetMapInfo(map);
            this.place_names[idx] = map.name;

            if (map.lastVisited)
            {
                curr_place_idx = idx;
            }
        }

        curr_chapter = map_list.maps[^1].chapter - 1;
        Debug.Log("현재 챕터: "+curr_chapter);
    }

    public void SetItemId(int id, int map_id)
    {
        int idx = map.FindIndex(x => x.id == map_id);
        items[idx].id = id;
    }

    public void SetItemList(GetItemList item_list)
    {
        foreach (GetItemInfo item in item_list.items)
        {
            int mapId = item.mapId;
            // map의 mapId와 item의 mapId가 같은 경우의 map 배열의 idx 찾기
            int idx = map.FindIndex(x => x.id == mapId);
            items[idx].SetItemInfo(item);
        }
    }

    public void SetViewChap(int num)
    {
        view_chapter = num;
    }

    public void SetEventId(int id, int map_id)
    {
        int idx = map.FindIndex(x => x.id == map_id);
        game_events[idx].id = id;
    }

    public void SetEventList(GetEventList event_list)
    {
        foreach (GetEventInfo game_event in event_list.events)
        {
            int mapId = game_event.mapId;
            int idx = map.FindIndex(x => x.id == mapId);
            game_events[idx].SetEventInfo(game_event);
        }
    }

    public void SetCurrGoalClear(bool clr)
    {
        goals[curr_chapter].SetClear(clr);

        // API 저장용
        int goal_idx = goals[curr_chapter].GetId();
        PlayAPI.play_api.UpdateGoalSuccess(goal_idx);
    }

    public bool CheckGoalCleared()
    {
        int i = 0;
        while (i <= curr_chapter)
        {
            if (!goals[i].GetClear())
            {
                return false;
            }
            i++;
        }

        return true;
    }

    // Getter
    public string GetCharName()
    {
        return char_name;
    }

    public int GetCurrChap()
    {
        return curr_chapter;
    }

    public int GetCurrPlaceIdx()
    {
        return curr_place_idx;
    }

    public Script GetScript()
    {
        return script;
    }

    public Goal GetCurrGoal()
    {
        return goals[curr_chapter];
    }

    public Goal GetGoal(int idx){
        return goals[idx];
    }

    public Goal GetFinalGoal()
    {
        return goals[Const.CHAPTER-1];
    }

    public List<Place> GetMap()
    {
        return map;
    }

    public Place GetPlace(int idx)
    {
        return map[idx];
    }

    public Place GetCurrPlace()
    {
        return map[curr_place_idx];
    }

    public List<Item> GetItems()
    {
        return items;
    }

    public List<Item> GetCurrItems()
    {
        int start;
        // 최종 장소일 경우
        if (curr_chapter == Const.CHAPTER-1)
        {
            return items.GetRange(Const.PLACE_COUNT-1, 1);
        }

        // 나머지의 경우
        start = 1 + curr_chapter * 3;
        return items.GetRange(start, 3);
    }

    public Item GetItem(int idx)
    {
        return items[idx];
    }

    public Item GetCurrItem()
    {
        return items[curr_place_idx];
    }

    public Event GetCurrEvent()
    {
        return game_events[curr_place_idx];
    }

    public Event GetEvent(int idx)
    {
        return game_events[idx];
    }

    public Npc GetPnpc()
    {
        return pro_npc;
    }

    public Npc GetAnpc()
    {
        return anta_npc;
    }

    public bool GetInitScript()
    {
        return init_script;
    }

    public int GetViewChap()
    {
        return view_chapter;
    }

    public string GetAchivement()
    {
        return achivement;
    }
}
