using System.Collections;
using System.Collections.Generic;
using OpenAI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class DiceEvent : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject dicewindow;
    [SerializeField] GameObject resultwindow;
    [SerializeField] TextMeshProUGUI result_calc;
    [SerializeField] TextMeshProUGUI result_txt;
    [SerializeField] TextMeshProUGUI tens_dice;
    [SerializeField] TextMeshProUGUI ones_dice;

    [SerializeField] PlayGPT playgpt;

    int req_value = 0;
    bool result = false;
    int pl_value = 0;
    int luk_value = 0;

    public void SetDiceEvent(int val)
    {
        dicewindow.gameObject.SetActive(true);

        resultwindow.gameObject.SetActive(false);
        pl_value = 0;
        tens_dice.text = "00";
        ones_dice.text = "00";

        req_value = val;
        Debug.Log(">>set dice event call");
    }

    public void RollDice()
    {
        Debug.Log(">>roll dice event call");
        int ones = Random.Range(0, 10);
        int tens = Random.Range(0, 10);

        ones_dice.text = ones.ToString();
        tens_dice.text = tens.ToString();
        pl_value = tens * 10 + ones;
        //행운 스탯은 모든 판정에 int(rand(0,행운)*0.5)만큼의 보정치를 더해준다.
        luk_value = Random.Range(0, PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.Luck) / 2);

        Invoke("ResultActive", 0.5f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (resultwindow.activeSelf == false )
        {
            return;
        }
        dicewindow.gameObject.SetActive(false);
    }

    void ResultActive()
    {
        int curr_place = MapManager.mapinfo.curr_place;
        resultwindow.gameObject.SetActive(true);
        result_calc.text = pl_value.ToString() + " + " + luk_value.ToString() + "(Bonus)\n";
        ChatMessage result_msg = new ChatMessage(){
            Role = "user",
            Content = "주사위 판정 "
        };
        if (pl_value + luk_value >= req_value)
        {
            result_txt.text = "<color=#074AB0>Success</color>";
            result_msg.Content += "성공";
            Debug.Log("맵 아이템 이름:"+MapManager.mapinfo.map[curr_place].item_name);
            InventoryManager.inventory.AddItem(100+curr_place,curr_place,MapManager.mapinfo.map[curr_place].item_name,MapManager.mapinfo.map[curr_place].item_stat,MapManager.mapinfo.map[curr_place].item_type.ToString(),1);
            InventoryManager.inventory.slotlist[InventoryManager.inventory.next_idx-1].SetSprites();
            MapManager.mapinfo.map[curr_place].clear = true;
            playgpt.AddToMessagesGPT(result_msg);
            result_msg.Content = MapManager.mapinfo.map[curr_place].event_succ.Replace(".", ".\n");
        }
        else
        {
            result_txt.text = "<color=#B40000>Fail</color>";
            result_msg.Content += "실패";
            playgpt.AddToMessagesGPT(result_msg);
            result_msg.Content = MapManager.mapinfo.map[curr_place].event_fail.Replace(".", ".\n");
        }
        result_msg.Role = "assistant";
        playgpt.AddToMessagesGPT(result_msg);
        playgpt.AppendMsg(result_msg);
    }


}
