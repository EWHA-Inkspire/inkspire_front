using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatGraphTest : MonoBehaviour
{
    [SerializeField] private StatusGraph status_graph;

    private Stats zero_stats = new Stats(0,0,0,0,0);
    
    void Start()
    {
        //PlayerStatManager.playerstat.p_stats = zero_stats;
    }

    void Update(){
        //status_graph.SetStats(PlayerStatManager.playerstat.p_stats);
    }

    public void ModalDeactivate(){
        
        status_graph.SetStats(zero_stats);
    }
    public void ModalActivate(){
        status_graph.SetStats(PlayerStatManager.playerstat.p_stats);
        //Debug.Log(PlayerStatManager.playerstat.p_stats.GetStatAmount(Stats.Type.Attack));
    }
}
