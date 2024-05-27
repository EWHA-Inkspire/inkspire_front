using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemWindow : MonoBehaviour
{
    [SerializeField] GameObject battle_window;
    [SerializeField] GameObject back_button;
    [SerializeField] BattleEvent battle;
    [SerializeField] GameObject item_list;
    [SerializeField] GameObject inventory_window;
    [SerializeField] Image[] item_bg = new Image[8];
    public bool is_battle = false;
    private List<InventorySlot> slots = new();
    private readonly int SLOT_SIZE = 8;
    public InventorySlot slotPrefab;
    private int cnt = 0; // inven 창 열고 닫을 때 사용

    void Awake()
    {
        // Slot 초기화
        for (int i = 0; i < SLOT_SIZE; i++) {
            InventorySlot newSlot = Instantiate(slotPrefab);
            newSlot.DelSprites();
            newSlot.name = "ItemSlot_"+i;
            newSlot.transform.SetParent(item_list.transform);
            slots.Add(newSlot);
        }

        // 인벤토리 매니저로 불러온 인벤토리 아이템을 UI에 표기
        SetInventorySlot(InventoryManager.i_manager.GetInventory());
    }

    void Update()
    {
        if(InventoryManager.i_manager.IsTargetChanged()) {
            ChangeSelectItem();
            InventoryManager.i_manager.SetTargetChanged(false);
        }
    }

    public void ChangeSelectItem()
    {
        int target_idx = InventoryManager.i_manager.GetTargetIdx();

        // 색상 초기화
        foreach(Image bg in item_bg) {
            bg.color = new Color32(237,237,233,255);
        }

        // 선택된 색상 표기
        item_bg[target_idx].color = new Color32(212,204,195,255);
    }

    public void AddSlotPrefab(Item item)
    {
        int inventory_size = InventoryManager.i_manager.GetInventorySize();
        int idx = Mathf.Max(0, inventory_size);
        slots[idx].SetItem(item);
        InventoryManager.i_manager.AddItem(item);
    }

    public void SetInventorySlot(List<Item> items)
    {
        for (int i = 0; i < items.Count; i++) {
            slots[i].SetItem(items[i]);
        }

        for (int i = items.Count; i < SLOT_SIZE; i++) {
            slots[i].DelSprites();
        }
    }

    public void UseTargetItem()
    {
        int target_idx = InventoryManager.i_manager.GetTargetIdx();
        if(target_idx < 0 || target_idx >= InventoryManager.i_manager.GetInventorySize()) return;

        Item target_item = slots[target_idx].GetItem();
        if(target_item == null) return;

        switch (target_item.type) {
            case ItemType.Recover:
                battle.AppendMsg(">> 아이템 사용: 체력 회복(+"+target_item.stat.ToString()+")\n");
                PlayerStatManager.playerstat
                .SetStatAmount(StatType.Hp, PlayerStatManager.playerstat.GetStatAmount(StatType.Hp)+target_item.stat);
                slots[target_idx].DelSprites();
                break;
            case ItemType.Mob:
                battle.AppendMsg(">> 아이템 사용: 행운력 증가(+"+target_item.stat.ToString()+")\n");
                PlayerStatManager.playerstat
                .SetStatAmount(StatType.Luck, PlayerStatManager.playerstat.GetStatAmount(StatType.Luck)+target_item.stat);
                slots[target_idx].DelSprites();
                break;
            case ItemType.Report:
                battle.AppendMsg(">> 보고서 아이템은 사용할 수 없습니다.\n");
                return;
            case ItemType.Weapon:
                PlayerStatManager.playerstat.wheapone = target_item.stat;
                slots[target_idx].DelSprites();
                battle.AppendMsg(">> 아이템 사용: 진행중인 전투 동안 공격력 증가(+"+target_item.stat.ToString()+")\n");
                break;
        }

        // 아이템 사용
        InventoryManager.i_manager.UseTargetItem();

        // 업데이트된 인벤토리 정보로 UI 업데이트
        SetInventorySlot(InventoryManager.i_manager.GetInventory());

        if(is_battle){
            inventory_window.SetActive(false);
            battle.SetNextTurn();
        }
    }

    public void DeleteTargetItem()
    {
        int target_idx = InventoryManager.i_manager.GetTargetIdx();
        if(target_idx < 0 || target_idx >= InventoryManager.i_manager.GetInventorySize()) return;

        slots[target_idx].DelSprites();
        InventoryManager.i_manager.DeleteTargetItem();
        SetInventorySlot(InventoryManager.i_manager.GetInventory());
    }

    void OpenButton(){
        battle_window.SetActive(true);
        inventory_window.SetActive(true);
        if(!is_battle){
            back_button.gameObject.SetActive(false);
        }
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

    public void BackButton(){
        battle.BackButton();
    }
}
