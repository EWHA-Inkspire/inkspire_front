using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text_name;
    [SerializeField] TextMeshProUGUI text_quant;
    [SerializeField] Image item_icon;
    [SerializeField] GameObject item_object;
    private Item item;

    public void setItem(Item item)
    {
        this.item = item;
        SetSprites();
    }

    public void SetSprites()
    {
        item_object.gameObject.SetActive(true);
        text_name.text = item?.item_name;
        text_quant.text = "1";
    }

    public void setUseItem(){
        InventoryManager.i_manager.SetTarget(item.item_id);
    }

    public void DelSprites()
    {
        item_object.gameObject.SetActive(false);
    }

    // Getter
    public Item getItem()
    {
        return item;
    }

}
