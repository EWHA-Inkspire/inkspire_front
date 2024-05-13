using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager i_manager;

    [SerializeField] GameObject battle_window;
    [SerializeField] GameObject back_button;
    public BattleEvent battle;
    [SerializeField] GameObject inventory_window;

    [SerializeField] Image[] item_bg = new Image[8];
    [SerializeField] GameObject item_list;
    private List<Item> inventory = new();
    private List<InventorySlot> slots = new();
    public InventorySlot slotPrefab;
    private readonly int SLOT_SIZE = 8;

    private readonly PlayerStatManager p_manager = PlayerStatManager.playerstat;
    public bool is_battle = false;
    private int target_idx = 0;
    private int cnt = 0; // inven 창 열고 닫을 때 사용

    void Awake() {
        if (i_manager == null) {
            i_manager = this;
            DontDestroyOnLoad(i_manager);
        } else if (i_manager != this) {
            Destroy(this);
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
        target_idx = inventory.FindIndex(x => x.id == item_id);
        item_bg[target_idx].color = new Color32(212,204,195,255);
    }

    public void AddItem(Item item)
    {
        Debug.Log("아이템 추가: \n" + item.name + ", " + item.id);
        inventory.Add(item);
        int idx = Mathf.Max(0, inventory.Count-1);
        slots[idx].SetItem(item);

        // 인벤토리 아이템 등록
        PlayAPI.play_api.PostInventory(item.id);
    }

    public void UseItem(Item item)
    {
        // 만약 inventory의 크기가 0 이하일 경우 사용할 아이템 없음
        if (inventory.Count <= 0) return;

        int i_idx = inventory.FindIndex(x => x.id == item.id);
        if (i_idx == -1) return;

        switch (item.type) {
            case ItemType.Recover:
                battle.AppendMsg(">> 아이템 사용: 체력 회복(+"+item.stat.ToString()+")\n");
                p_manager.SetStatAmount(StatType.Hp, p_manager.GetStatAmount(StatType.Hp)+item.stat);
                slots[i_idx].DelSprites();  inventory.RemoveAt(i_idx);
                break;
            case ItemType.Mob:
                // Mob 사용시 스탯 늘려줌??
                break;
            case ItemType.Report:
                battle.AppendMsg(">> 보고서 아이템은 사용할 수 없습니다.");
                return;
            case ItemType.Weapon:
                PlayerStatManager.playerstat.wheapone = item.stat;
                slots[i_idx].DelSprites();  inventory.RemoveAt(i_idx);
                battle.AppendMsg(">> 아이템 사용: 진행중인 전투 동안 공격력 증가(+"+item.stat.ToString()+")\n");
                break;
        }

        // 인벤토리 아이템 삭제
        PlayAPI.play_api.DeleteInventory(item.id);
    }

    public void UseTargetItem()
    {
        if(target_idx < 0 || target_idx >= inventory.Count) {
            return;
        }

        UseItem(inventory[target_idx]);

        if(is_battle){
            inventory_window.SetActive(false);
            battle.SetNextTurn();
        }
    }

    private void ProcessUseItem(Response<Null> response)
    {
        if (response.success) {
            Debug.Log("아이템 사용 성공");
        } else {
            Debug.Log("아이템 사용 실패");
        }
    }

    public void DeleteTargetItem(){

        if(target_idx < 0 || target_idx >= inventory.Count) {
            return;
        }
        
        slots[target_idx].DelSprites();  inventory.RemoveAt(target_idx);
    }

    public void BackButton(){
        battle.BackButton();
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
}
