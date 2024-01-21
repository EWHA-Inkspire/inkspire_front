using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatModalButton : MonoBehaviour
{
    [SerializeField] GameObject mesh_obj;
    [SerializeField] GameObject modal_obj;
    [SerializeField] StatGraphTest stattest;
    private CanvasRenderer radarMeshCanvasRenderer;

    private void Awake(){
        radarMeshCanvasRenderer = mesh_obj.GetComponent<CanvasRenderer>();
    }

    public void OnModalOpen(){
        
        modal_obj.gameObject.SetActive(true);
        stattest.ModalActivate();

    }

    public void OnModalClose(){
        stattest.ModalDeactivate();
        Invoke("Deactivate",0.03f);
    }

    private void Deactivate(){
        modal_obj.gameObject.SetActive(false);
    }

}
