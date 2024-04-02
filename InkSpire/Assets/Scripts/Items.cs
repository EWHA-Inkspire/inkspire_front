using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items
{
    public enum Type
    {
        Recover,
        Mob,
        Weapon, 
        Report
    }

    int item_id;
    int map_id;
    string name;
    int detail;
    Type type;
    int quantity;

    public Items(){
        item_id = -1;
        map_id = -1;
        name = "";
        detail = 0;
        type = Type.Mob;
        quantity = 0;
    }
    public Items(int itemid, int mapid, string itemname, int itemdetail, string itemtype, int iquant)
    {
        item_id = itemid;
        map_id = mapid;
        name = itemname;
        detail = itemdetail;
        type = parseEnumType(itemtype);
        quantity = iquant;

    }
    public string GetItemName()
    {
        return name;
    }
    public int GetItemID(){
        return item_id;
    }
    public int GetItemQuant()
    {
        return quantity;
    }
    public Type parseEnumType(string orgstring)
    {
        if (Enum.IsDefined(typeof(Type), orgstring))
        {
            return (Type)Enum.Parse(typeof(Type), orgstring);
        }
        else
        {
            return Type.Recover;
        }
    }

    public void addQuant(int num){
        quantity+=num;
    }

    public void delQuant(int num){
        if(quantity-num <= 0){
            delItem();
            return;
        }
        else{
            quantity-=num;
        }
    }

    public void delItem(){
        quantity = 0;
    }

    public void UseItem(){
        if(GetItemQuant()<=0){
            return;
        }
        delQuant(1);
        switch(type){
            case Type.Recover:
                AppendMsg(">> 아이템 사용: 체력 회복(+"+detail.ToString()+")");
                PlayerStatManager.playerstat.p_stats.SetStatAmount(Stats.Type.CurrHP,PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.CurrHP)+detail);
                break;
            case Type.Mob:
                break;
            case Type.Report:
                AppendMsg(">> 보고서 아이템은 사용할 수 없습니다.");
                addQuant(1);
                break;
            case Type.Weapon:
                PlayerStatManager.playerstat.wheapone = detail;
                AppendMsg(">> 아이템 사용: 진행중인 전투 동안 공격력 증가(+"+detail.ToString()+")");
                break;
        }
    }

    void AppendMsg(string msg){
        InventoryManager.inventory.battle.AppendMsg(msg);
    }
}
