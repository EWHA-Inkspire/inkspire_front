using Unity.VisualScripting;
using UnityEngine;

public class ScriptManager : MonoBehaviour
{
    public static ScriptManager script_manager;
    private string char_name;
    private int curr_chapter = 0;
    private int curr_place_idx = 0;

    private Script script;
    private Goal[] goals = new Goal[5];
    private Place[] map = new Place[14];
    private Item[] items = new Item[14];
    private Event[] game_events = new Event[14];
    private string[] place_names = new string[14];
    private Npc pro_npc;
    private Npc anta_npc;

    private bool init_script = false;

    // 일반함수
    void Awake()
    {
        if (script_manager == null) {
            script_manager = this;
            DontDestroyOnLoad(script_manager);
        } else if (script_manager != this) {
            Destroy(this);
        }

        // 필드 초기화
        script = new Script();
        for (int i = 0; i < goals.Length; i++)
        {
            goals[i] = new Goal();
        }

        // map 배열과 place_names 배열 초기화
        for (int i = 0; i < map.Length; i++)
        {
            map[i] = new Place();
            items[i] = new Item();
            game_events[i] = new Event();
            place_names[i] = ""; // 또는 적절한 초기값 설정
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

        // API 호출 - 스크립트 정보 저장 -> 함수로 빼기
        PostScenarioInfo();

        // 목표 생성
        await goals[4].InitGoal(time_background, space_background, script.GetWorldDetail(), genre);
        await goals[0].InitGoal(time_background, space_background, script.GetWorldDetail(), genre, goals[4]);

        // npc 정보 생성
        await pro_npc.InitNpc("P", script.GetWorldDetail(), genre, char_name);
        await anta_npc.InitNpc("A", script.GetWorldDetail(), genre, char_name);

        // 맵 정보 생성
        ChooseEventType(); // 14개의 장소 별 이벤트 타입 생성
        // PNPC 장소 초기화
        map[0].ANPC_exist = 0;
        map[0].CreatePnpcPlace(script, pro_npc);
        // 일반 장소 초기화
        for (int i = 1; i < 4; i++) {
            // 목표 정보 전달
            await items[i].InitItem(script, goals[curr_chapter], game_events[i].event_type);
            await map[i].InitPlace(i, script, items[i], game_events[i].event_type, place_names);
            place_names[i] = map[i].place_name;

            // 전투 이벤트(잡몹, 적 처치) 혹은 item_type이 null일 경우에는 이벤트 트리거 생성하지 않음
            if (items[i].item_type != ItemType.Mob && items[i].item_type != ItemType.Monster && items[i].item_type != ItemType.Null)
            {
                await game_events[i].CreateEventTrigger(script.GetWorldDetail(), goals[curr_chapter].GetDetail(), place_names[i], items[i].item_name);
            }
        }

        await script.IntroGPT(pro_npc, anta_npc, map[0].place_name, map[0].place_info, this.char_name);
        Debug.Log(script.GetIntro());
        init_script = true;

        // API 호출 - 인트로 내용 저장
        PutIntroInfo();
    }

    // 장소 별 이벤트 타입 설정 (3개 장소마다 목표 이벤트 출현 장소 정하는 로직)
    private void ChooseEventType()
    {
        int i = 1;
        while (i < 13)
        {
            int flag = Random.Range(0, 3);
            if (flag == 0) {
                game_events[i].event_type = 1;
            }
            else {
                game_events[i].event_type = 0;
            }

            if (game_events[i].event_type == 1) {
                //100
                game_events[i + 1].event_type = 0;
                game_events[i + 2].event_type = 0;
                i += 3;
                continue;
            }
            else
            {
                i++;
                game_events[i].event_type = UnityEngine.Random.Range(0, 2);
                if (game_events[i].event_type == 1){
                    game_events[i + 1].event_type = 0; //010
                }
                else {
                    game_events[i + 1].event_type = 1; //001
                }
                i += 2;
            }
        }
        //최종 에필로그
        if (i == 13) {
            game_events[i].event_type = 1;
            map[i].ANPC_exist = 0;
        }
    }
    public async void SetNextChapter(){
        string genre = script.GetGenre();
        string time_background = script.GetTimeBackground();
        string space_background = script.GetSpaceBackground();

        curr_chapter++;
        int place_base = (curr_chapter-1)*3+1;
        await goals[curr_chapter].InitGoal(time_background, space_background, script.GetWorldDetail(), genre, goals[4]);
        for(int i = 0; i<3;i++){
            // 목표 정보 전달
            await items[place_base+i].InitItem(script, goals[curr_chapter].GetGoalType(), goals[curr_chapter].GetEtc(), game_events[place_base+i].event_type);
            await map[place_base+i].InitPlace(place_base+i, script, pro_npc, game_events[place_base+i].event_type, place_names);
            place_names[place_base+i] = map[place_base+i].place_name;

            // 전투 이벤트(잡몹, 적 처치) 혹은 item_type이 null일 경우에는 이벤트 트리거 생성하지 않음
            if (items[place_base+i].item_type != ItemType.Mob && items[place_base+i].item_type != ItemType.Monster && items[place_base+i].item_type != ItemType.Null)
            {
                await game_events[place_base+i].CreateEventTrigger(script.GetWorldDetail(), goals[curr_chapter].GetDetail(), place_names[place_base+i], items[place_base+i].item_name);
            }
        }
    }

    // API 호출
    private void PostScenarioInfo()
    {
        ScenarioInfo scenarioInfo = new()
        {
            character = new CharacterInfo(char_name),
            script = new ScriptInfo(script.GetGenre(), script.GetTimeBackground(),
                        script.GetSpaceBackground(), script.GetWorldDetail())
        };
        string json = JsonUtility.ToJson(scenarioInfo);

        StartCoroutine(APIManager.api.PostRequest<PostScriptResponse>("/scripts", json, ProcessScriptResponse));
    }

    private void PutIntroInfo()
    {
        IntroInfo introInfo = new()
        {
            scriptId = PlayerPrefs.GetInt("script_id"),
            intro = script.GetIntro()
        };
        string json = JsonUtility.ToJson(introInfo);
        
        StartCoroutine(APIManager.api.PutRequest<Null>("/scripts/intro", json, (response) => { 
            if (response.success) {
                Debug.Log("Intro saved successfully");
            } else {
                Debug.Log("Failed to save intro");
            }
        }));
    }

    // API 호출 결과를 받아오는 함수
    private void ProcessScriptResponse(Response<PostScriptResponse> response)
    {
        if (response.success)
        {
            Debug.Log("Script saved successfully");

            // 현재 플레이 중인 캐릭터 아이디 & 스크립트 아이디 저장
            PlayerPrefs.SetInt("character_id", response.data.characterId);
            PlayerPrefs.SetInt("script_id", response.data.scriptId);
        }
        else
        {
            Debug.Log("Failed to save script");
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

    public void SetGoalList(GetGoalList goal_list)
    {
        for (int i = 0; i < goal_list.goals.Count; i++)
        {
            int chapter_num = goal_list.goals[i].chapter;
            this.goals[chapter_num-1].SetGoalInfo(goal_list.goals[i]);
        }
    }

    public void SetNpcList(GetNpcList npc_list)
    {
        foreach (GetNpcInfo npc in npc_list.npcs)
        {
            if(npc.pnpc)
                pro_npc.SetNpcInfo(npc);
            else
                anta_npc.SetNpcInfo(npc);
        }
    }

    public void SetMapList(GetMapList map_list)
    {
        foreach (GetMapInfo map in map_list.maps)
        {
            int idx = map.idx;
            this.map[idx].SetMapInfo(map);
            this.place_names[idx] = map.name;

            if(map.lastVisited) {
                curr_place_idx = idx;
            }
        }
    }

    public void SetItemList(GetItemList item_list)
    {
        foreach (GetItemInfo item in item_list.items)
        {
            int mapId = item.mapId;
            // map의 mapId와 item의 mapId가 같은 경우의 map 배열의 idx 찾기
            int idx = System.Array.FindIndex(map, x => x.id == mapId);
            this.items[idx].SetItemInfo(item);
        }
    }

    public void SetEventList(GetEventList event_list)
    {
        foreach (GetEventInfo game_event in event_list.events)
        {
            int mapId = game_event.mapId;
            int idx = System.Array.FindIndex(map, x => x.id == mapId);
            this.game_events[idx].SetEventInfo(game_event);
        }
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

    public Goal GetCurrGoal(){
        return goals[curr_chapter];
    }

    public Goal GetFinalGoal(){
        return goals[4];
    }

    public Place GetPlace(int idx)
    {
        return map[idx];
    }

    public Place GetCurrPlace()
    {
        return map[curr_place_idx];
    }

    public Item[] GetItems(){
        return items;
    }

    public Item[] GetCurrItems(){
        int start;  int end;
        // 최종 장소일 경우
        if (curr_chapter == 4)
        {
            start = 13; end = 13;
            return items[start..(end + 1)];
        }

        // 나머지의 경우
        start = 1 + curr_chapter*3; end = start + 2;
        return items[start..(end + 1)];
    }

    public Item GetItem(int idx){
        return items[idx];
    }

    public Item GetCurrItem(){
        return items[curr_place_idx];
    }

    public Event GetCurrEvent(){
        return game_events[curr_place_idx];
    }

    public Npc GetPnpc()
    {
        return pro_npc;
    }

    public bool GetInitScript()
    {
        return init_script;
    }
}
