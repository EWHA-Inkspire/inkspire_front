using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatGraphTest : MonoBehaviour
{
    [SerializeField] private StatusGraph status_graph;

    private Stats zero_stats = new Stats(0,0,0,0,0);
    private Stats stats = new Stats(20, 1, 20, 15,5);
    void Start()
    {
        status_graph.SetStats(stats);
    }

    public void ModalDeactivate(){
        
        status_graph.SetStats(zero_stats);
    }
    public void ModalActivate(){
        status_graph.SetStats(stats);
    }
}
