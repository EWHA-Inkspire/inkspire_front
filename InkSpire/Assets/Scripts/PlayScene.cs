using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayScene : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI header_name;
    [SerializeField] TextMeshProUGUI header_HP;
    [SerializeField] TextMeshProUGUI title_chapnum;
    [SerializeField] TextMeshProUGUI battle_stat;

    [SerializeField] GameObject item_list;
    [SerializeField] Slider slider_HP;


    public InventorySlot slotPrefab;

    void Start(){

        Changechap(ScriptManager.scriptinfo.curr_chapter);
        header_name.text = PlayerStatManager.playerstat.charname+" HP";
        header_HP.text = PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.CurrHP).ToString()+" / "+PlayerStatManager.playerstat.GetStatAmount("MaxHP").ToString();
        battle_stat.text = "공격: "+PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.Attack).ToString()+" | 방어: "+PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.Defence).ToString()+" | 민첩: "+PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.Dexterity).ToString()+" | 행운: "+PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.Luck).ToString();
        
        if(slotPrefab!=null){
            for(int i = 0; i<8; i++){
                InventorySlot newSlot = Instantiate(slotPrefab);
                if(i<InventoryManager.inventory.next_idx){
                    newSlot.setItem(InventoryManager.inventory.inventorylist[i].GetItemName(),InventoryManager.inventory.inventorylist[i].GetItemQuant(),InventoryManager.inventory.inventorylist[i].GetItemID());
                }
                else{
                    newSlot.SetSprites();
                }
                newSlot.name = "ItemSlot_"+i;
                newSlot.transform.SetParent(item_list.transform);
                InventoryManager.inventory.slotlist[i] = newSlot;
            }
        }
    }

    void Update(){
        slider_HP.value=(float)PlayerStatManager.playerstat.GetStatAmount("CurrHP")/PlayerStatManager.playerstat.GetStatAmount("MaxHP");
        header_HP.text = PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.CurrHP).ToString()+" / "+PlayerStatManager.playerstat.GetStatAmount("MaxHP").ToString();
    }

    public void Changechap(int int_chapnum){
        string rome_chapnum;
        switch(int_chapnum){
            case 1:
                rome_chapnum = "I";
                break;
            case 2:
                rome_chapnum = "II";
                break;
            case 3:
                rome_chapnum = "III";
                break;
            case 4:
                rome_chapnum = "IV";
                break;
            case 5:
                rome_chapnum = "V";
                break;
            default:
                rome_chapnum = "NULL";
                break;
        }
        title_chapnum.text = "Chapter "+rome_chapnum;
    }
}
