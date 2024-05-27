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

    [SerializeField] Play play_manager;
    [SerializeField] GameObject EndChapterModal;
    [SerializeField] ItemWindow item_window;

    int req_value = 0;
    bool result = false;
    int pl_value = 0;
    int luk_value = 0;

    public void SetDiceEvent(int val)
    {
        dicewindow.SetActive(true);

        resultwindow.SetActive(false);
        pl_value = 0;
        tens_dice.text = "00";
        ones_dice.text = "00";

        req_value = val;
    }

    public void RollDice()
    {
        int ones = Random.Range(0, 10);
        int tens = Random.Range(0, 10);

        ones_dice.text = ones.ToString();
        tens_dice.text = tens.ToString();
        pl_value = tens * 10 + ones;
        //정신력 스탯은 모든 판정에 int(rand(0,정신력)*0.5)만큼의 보정치를 더해준다.
        luk_value = Random.Range(0, PlayerStatManager.playerstat.GetStatAmount(StatType.Mental) / 2);

        Invoke(nameof(ResultActive), 0.5f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (resultwindow.activeSelf == false)
        {
            return;
        }
        dicewindow.SetActive(false);
    }

    void ResultActive()
    {
        resultwindow.SetActive(true);
        result_calc.text = pl_value.ToString() + " + " + luk_value.ToString() + "(Bonus)\n";
        Item map_item = ScriptManager.script_manager.GetCurrItem();

        ChatMessage result_msg = new()
        {
            Role = "user",
            Content = "주사위 판정 "
        };
        if (pl_value + luk_value >= req_value)
        {
            result_txt.text = "<color=#074AB0>Success</color>";
            result_msg.Content += "성공";
            item_window.AddSlotPrefab(map_item);
            ScriptManager.script_manager.SetPlaceClear(true);
            play_manager.AddToMessagesGPT(result_msg);
            result_msg.Content = ScriptManager.script_manager.GetCurrEvent().succ.Replace(".", ".\n");

            if(map_item.type == ItemType.Item || map_item.type == ItemType.Report)
            {
                ScriptManager.script_manager.SetCurrGoalClear(true);
            }
        }
        else
        {
            result_txt.text = "<color=#B40000>Fail</color>";
            result_msg.Content += "실패";
            play_manager.AddToMessagesGPT(result_msg);
            result_msg.Content = ScriptManager.script_manager.GetCurrEvent().fail.Replace(".", ".\n");
        }
        result_msg.Role = "assistant";
        play_manager.AddToMessagesGPT(result_msg);
        play_manager.text_scroll.AppendMsg(result_msg, true);

        if (map_item.type == ItemType.Item || map_item.type == ItemType.Report)
        {
            EndChapterModal.SetActive(true);
        }
    }
}
