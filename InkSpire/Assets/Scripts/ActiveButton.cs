using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActiveButton : MonoBehaviour
{
    [SerializeField] public GameObject obj;

    public void ActivateObj(){
        obj.gameObject.SetActive(true);
    }

    public void DeactivateObj(){
        obj.gameObject.SetActive(false);
    }
}
