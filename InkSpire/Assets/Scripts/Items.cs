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
    string detail;
    Type type;
    int quantity;

    public Items(int itemid, int mapid, string itemname, string itemdetail, string itemtype, int iquant)
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
}
