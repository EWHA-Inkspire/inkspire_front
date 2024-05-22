using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvenUIDontDestroy : MonoBehaviour
{
    void Awake(){
        DontDestroyOnLoad(gameObject);
    }
}
