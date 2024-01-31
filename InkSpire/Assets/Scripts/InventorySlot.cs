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

    private bool exist = false;
    private string itemname;
    private int quant = 0;
    private int type = 0;

    public void SetSprites(){
        if(!exist){
            Debug.Log("false item call");
            item_object.gameObject.SetActive(false);
            return;
        }
        text_name.text = itemname;
        text_quant.text = quant.ToString();
    }

    public void setItem(string iname, int iquant){
        exist = true;
        itemname = iname;
        quant = iquant;
        //type = itype;
        SetSprites();
    }

    public void addQuant(int num){
        quant+=num;
        SetSprites();
    }

    public void delQuant(int num){
        if(quant== 1){
            delItem();
            return;
        }
        else{
            quant-=num;
            SetSprites();
        }
    }

    public void delItem(){
        exist = false;
        itemname = null;
        quant = 0;
        type = 0;
    }

}
