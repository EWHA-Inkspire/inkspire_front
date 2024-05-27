using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapButtonClip : MonoBehaviour
{
    [SerializeField] RectTransform clip_to_obj;
    public float additional_y;
    public float additional_x;
    private RectTransform this_rect;

    void Start(){
        this_rect = GetComponent<RectTransform>();
    }
    void OnEnable(){
        this_rect = GetComponent<RectTransform>();
        this_rect.position = clip_to_obj.position;
        if(additional_y != 0 || additional_x != 0){
            this_rect.position = new Vector3(this_rect.position.x+additional_x,this_rect.position.y+additional_y,this_rect.position.z);
        }
    }
}
