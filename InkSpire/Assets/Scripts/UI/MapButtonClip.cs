using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapButtonClip : MonoBehaviour
{
    [SerializeField] RectTransform clip_to_obj;
    private RectTransform this_rect;

    void Start(){
        this_rect = GetComponent<RectTransform>();
    }
    void OnEnable(){
        this_rect = GetComponent<RectTransform>();
        this_rect.position = clip_to_obj.position;
    }
}
