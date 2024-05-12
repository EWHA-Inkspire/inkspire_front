using UnityEngine;

public class StatModalButton : MonoBehaviour
{
    [SerializeField] GameObject mesh_obj;
    [SerializeField] GameObject modal_obj;
    [SerializeField] private StatusGraph status_graph;

    private readonly Stats zero_stats = new(0,0,0,0,0);

    private CanvasRenderer radarMeshCanvasRenderer;

    private void Awake(){
        radarMeshCanvasRenderer = mesh_obj.GetComponent<CanvasRenderer>();
    }

    public void OnModalOpen(){
        
        modal_obj.SetActive(true);
        ModalActivate();
    }

    public void OnModalClose(){
        status_graph.SetStats(zero_stats);
        Invoke(nameof(Deactivate), 0.03f);
    }

    private void Deactivate(){
        modal_obj.SetActive(false);
        ModalActivate();
    }

    public void ModalActivate(){
        status_graph.SetStats(PlayerStatManager.playerstat.GetStats());
    }

}
