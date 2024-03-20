using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    // 인벤토리 내용을 싱글톤으로 관리
    public static InventoryManager inventory;

    [SerializeField] GameObject battle_window;
    [SerializeField] GameObject inventory_window;
    [SerializeField] Image[] item_bg = new Image[8];
    [SerializeField] public BattleEvent battle;
    public Items[] inventorylist = new Items[8];
    public InventorySlot[] slotlist = new InventorySlot[8];

    public int next_idx = 0;
    public int target_idx = 0;
    public bool is_battle = false;
    int cnt = 0;
    void Awake(){
        // 씬이 바뀔 때 파괴되지 않음
        DontDestroyOnLoad(this.gameObject);

        if(inventory == null){
            inventory=this;
        }

        AddItem(1,3,"테스트테스트",50,"Recover",3);
        AddItem(2,4,"테스트2",5,"Weapon",1);
    }

    public void AddItem(int itemid, int mapid, string itemname, int itemdetail, string itemtype, int iquant){
        inventorylist[next_idx] = new Items(itemid,mapid,itemname, itemdetail, itemtype,iquant);
        next_idx++;
    }

    public void SetTarget(int item_id){
        for(int i = 0; i<next_idx;i++){
            if(item_id == inventorylist[i].GetItemID() && inventorylist[i].GetItemQuant()>0){
                Debug.Log("itemQ: "+inventorylist[i].GetItemQuant());
                item_bg[target_idx].color = new Color32(237,237,233,255);
                target_idx = i;
                item_bg[target_idx].color = new Color32(212,204,195,255);
            }
        }
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
    void OpenButton(){
        battle_window.gameObject.SetActive(true);
        inventory_window.gameObject.SetActive(true);
    }
    void CloseButton(){
        inventory_window.gameObject.SetActive(false);
        battle_window.gameObject.SetActive(false);
    }

    public void UseTargetItem(){
        inventorylist[target_idx].UseItem();
        slotlist[target_idx].delQuant(1);
        if(inventorylist[target_idx].GetItemQuant()<=0){
            for(int i = target_idx+1; i<next_idx; i++){
                inventorylist[i-1]=inventorylist[i];
                slotlist[i-1].setItem(inventorylist[i].GetItemName(),inventorylist[i].GetItemQuant(),inventorylist[i].GetItemID());
                slotlist[i-1].SetSprites();
                Debug.Log(inventorylist[i-1].GetItemName());
            }
            inventorylist[next_idx-1] = new Items();
            slotlist[next_idx-1].delItem();
            next_idx--;
        }

        if(is_battle){
            Debug.Log(">>is_battle:"+is_battle);
            inventory_window.gameObject.SetActive(false);
            battle.SetNextTurn();
        }
    }
}
