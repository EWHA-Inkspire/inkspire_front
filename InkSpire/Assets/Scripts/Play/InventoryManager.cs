using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager i_manager;

    [SerializeField] GameObject battle_window;
    public BattleEvent battle;
    [SerializeField] GameObject inventory_window;

    [SerializeField] Image[] item_bg = new Image[8];
    [SerializeField] GameObject item_list;
    private List<Item> inventory = new();
    private List<InventorySlot> slots = new();
    public InventorySlot slotPrefab;
    private readonly int SLOT_SIZE = 8;

    private Stats p_stats = PlayerStatManager.playerstat.p_stats;
    public bool is_battle = false;
    private int target_idx = 0;
    private int cnt = 0; // inven 창 열고 닫을 때 사용

    void Awake() {
        // 씬이 바뀔 때 파괴되지 않음
        DontDestroyOnLoad(this.gameObject);

        if(i_manager == null){
            i_manager = this;
        }

        // Slot 초기화
        for (int i = 0; i < SLOT_SIZE; i++) {
            InventorySlot newSlot = Instantiate(slotPrefab);
            newSlot.DelSprites();
            newSlot.name = "ItemSlot_"+i;
            newSlot.transform.SetParent(item_list.transform);
            slots.Add(newSlot);
        }
    }

    public void SetTarget(int item_id)
    {
        if(target_idx < 0 || target_idx >= inventory.Count) return;

        // 이전 색상 선택 해제 색상으로 변경
        item_bg[target_idx].color = new Color32(237,237,233,255);

        // 선택된 색상 표기
        target_idx = inventory.FindIndex(x => x.item_id == item_id);
        item_bg[target_idx].color = new Color32(212,204,195,255);
    }

    public void AddItem(Item item)
    {
        Debug.Log("아이템 추가: \n" + item.item_name + ", " + item.item_id);
        inventory.Add(item);
        slots[inventory.Count-1].SetItem(item);
    }

    public void UseItem(Item item)
    {
        // 만약 inventory의 크기가 0 이하이거나 inventory에 item이 없는 경우 => TODO: API 연결 후 ID값 비교로 수정
        if (inventory.Count <= 0) return;
        if (!inventory.Exists(x => x.item_id == item.item_id)) return;

        int i_idx = inventory.FindIndex(x => x.item_id == item.item_id);

        switch (item.item_type) {
            case ItemType.Recover:
                battle.AppendMsg(">> 아이템 사용: 체력 회복(+"+item.item_stat.ToString()+")\n");
                p_stats.SetStatAmount(StatType.Hp, p_stats.GetStatAmount(StatType.Hp)+item.item_stat);
                slots[i_idx].DelSprites();  inventory.RemoveAt(i_idx);
                break;
            case ItemType.Mob:
                break;
            case ItemType.Report:
                battle.AppendMsg(">> 보고서 아이템은 사용할 수 없습니다.");
                break;
            case ItemType.Weapon:
                PlayerStatManager.playerstat.wheapone = item.item_stat;
                slots[i_idx].DelSprites();  inventory.RemoveAt(i_idx);
                battle.AppendMsg(">> 아이템 사용: 진행중인 전투 동안 공격력 증가(+"+item.item_stat.ToString()+")\n");
                break;
        }
    }

    public void UseTargetItem()
    {
        Debug.Log("Traget idx: "+target_idx);
        if(target_idx < 0 || target_idx >= inventory.Count) {
            return;
        }

        UseItem(inventory[target_idx]);

        if(is_battle){
            Debug.Log(">>is_battle:"+is_battle);
            inventory_window.SetActive(false);
            battle.SetNextTurn();
        }
    }

    void OpenButton(){
        battle_window.SetActive(true);
        inventory_window.SetActive(true);
    }

    void CloseButton(){
        inventory_window.SetActive(false);
        battle_window.SetActive(false);
    }

    public void InventoryButton(){
        if(cnt == 0){
            OpenButton();
        }
        else{
            CloseButton();
        }

        cnt++;
        cnt%=2;
    }
}
