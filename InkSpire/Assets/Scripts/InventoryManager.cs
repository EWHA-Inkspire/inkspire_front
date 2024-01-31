using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // 인벤토리 내용을 싱글톤으로 관리
    public static InventoryManager inventory;

    public Items[] inventorylist = new Items[8];
    public int next_idx = 0;

    void Awake(){
        // 씬이 바뀔 때 파괴되지 않음
        DontDestroyOnLoad(this.gameObject);

        if(inventory == null){
            inventory=this;
        }

        AddItem(1,3,"테스트테스트","이것은 테스트용 아이템 설명입니다.","Battle",3);
    }

    public void AddItem(int itemid, int mapid, string itemname, string itemdetail, string itemtype, int iquant){
        inventorylist[next_idx] = new Items(itemid,mapid,itemname, itemdetail, itemtype,iquant);
        next_idx++;
    }
}
