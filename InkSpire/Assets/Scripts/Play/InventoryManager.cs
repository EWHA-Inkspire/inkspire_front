using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager i_manager;
    
    private List<Item> inventory = new();
    private int target_idx = 0;
    private bool target_changed = false;

    void Awake() {
        if (i_manager == null) {
            i_manager = this;
            DontDestroyOnLoad(i_manager);
        } else if (i_manager != this) {
            Destroy(this);
        }
    }

    public void SetTarget(int item_id)
    {
        if(target_idx < 0 || target_idx >= inventory.Count) return;

        target_idx = inventory.FindIndex(x => x.id == item_id);
        target_changed = true;
    }

    public void AddItem(Item item)
    {
        Debug.Log("아이템 추가: \n" + item.name + ", " + item.id);
        inventory.Add(item);

        // 인벤토리 아이템 등록
        PlayAPI.play_api.PostInventory(item.id);
    }

    public void SetInventory(List<Item> items)
    {
        inventory = items;
    }

    public void UseTargetItem()
    {
        if(target_idx < 0 || target_idx >= inventory.Count) {
            return;
        }

        if(inventory[target_idx].type == ItemType.Report) {
            return;
        }

        DeleteTargetItem();
    }

    public void DeleteTargetItem()
    {
        // 인벤토리 아이템 삭제
        PlayAPI.play_api.DeleteInventory(inventory[target_idx].id);
        inventory.RemoveAt(target_idx);
    }

    public int GetTargetIdx()
    {
        return target_idx;
    }

    public int GetInventorySize()
    {
        return inventory.Count;
    }

    public List<Item> GetInventory()
    {
        return inventory;
    }

    public bool IsTargetChanged()
    {
        return target_changed;
    }

    public void SetTargetChanged(bool changed)
    {
        target_changed = changed;
    }
}
