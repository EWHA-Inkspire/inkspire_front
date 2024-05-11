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

    public void SetItem(Item item)
    {
        this.item = item;
        SetSprites();
    }

    public void SetSprites()
    {
        item_object.SetActive(true);
        text_name.text = item?.name;
        text_quant.text = "1";
    }

    public void SetUseItem(){
        InventoryManager.i_manager.SetTarget(item.id);
    }

    public void DelSprites()
    {
        item_object.SetActive(false);
    }

    // Getter
    public Item GetItem()
    {
        return item;
    }

}
