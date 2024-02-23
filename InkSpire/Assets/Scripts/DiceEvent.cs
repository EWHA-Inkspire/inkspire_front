using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class DiceEvent : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] GameObject dicewindow;
    [SerializeField] GameObject resultwindow;
    [SerializeField] TextMeshProUGUI result_txt;
    [SerializeField] TextMeshProUGUI tens_dice;
    [SerializeField] TextMeshProUGUI ones_dice;

    int req_value = 0;
    bool result = false;
    int pl_value = 0;

    public void SetDiceEvent(int val){
        dicewindow.gameObject.SetActive(true);

        resultwindow.gameObject.SetActive(false);
        pl_value = 0;
        tens_dice.text = "00";
        ones_dice.text = "00";

        req_value = val;
        Debug.Log(">>set dice event call");
    }

    public void RollDice(){
        Debug.Log(">>roll dice event call");
        int ones = Random.Range(0,10);
        int tens = Random.Range(0,10);

        ones_dice.text = ones.ToString();
        tens_dice.text = tens.ToString();
        pl_value = tens*10+ones;

        Invoke("ResultActive",0.5f);
    }

    public void OnPointerClick(PointerEventData eventData){
        if(resultwindow.activeSelf == false){
            return;
        }
        dicewindow.gameObject.SetActive(false);
    }

    void ResultActive(){
        
        result_txt.text = pl_value.ToString()+"\n";
        resultwindow.gameObject.SetActive(true);
        if(pl_value>=req_value){
            result_txt.text += "<color=\"blue\">Success</color>";
        }
        else{
            result_txt.text += "<color=\"red\">Fail</color>";
        }

    }
    
}
