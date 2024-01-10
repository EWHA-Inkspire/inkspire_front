using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatGraphTest : MonoBehaviour
{
    [SerializeField] private StatusGraph status_graph;
    void Start()
    {
        Stats stats = new Stats(20, 1, 20, 15,5);

        status_graph.SetStats(stats);
    }
}
