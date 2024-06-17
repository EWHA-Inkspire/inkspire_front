using UnityEngine;

public class StatusGraph : MonoBehaviour
{
    // 스탯 그래프 UI를 그리는 코드 파일 - Youtube 참고해서 짠 코드라 웬만해선 건드리지 말것

    [SerializeField] private Material radarMaterial;
    [SerializeField] private GameObject mesh_object;

    private Stats stats;
    private CanvasRenderer radarMeshCanvasRenderer;

    private void Awake(){
        SetMeshObject(mesh_object);
    }

    public void SetStats(Stats stats){
        this.stats = stats;
        stats.OnStatsChanged += OnStatsChanged;
        UpdateStatsVisual();
    }

    private void OnStatsChanged(object sender, System.EventArgs e){
        UpdateStatsVisual();
    }

    public void SetMeshObject(GameObject newMeshObject){
        mesh_object = newMeshObject;
        radarMeshCanvasRenderer = mesh_object?.GetComponent<CanvasRenderer>();

        if (radarMeshCanvasRenderer == null && mesh_object != null) {
            Debug.LogWarning("CanvasRenderer를 찾을 수 없습니다.");
        }
    }

    private void UpdateStatsVisual(){
        // radarMeshCanvasRenderer가 null인지 확인하고 다시 설정
        if (radarMeshCanvasRenderer == null) {
            if (mesh_object != null) {
                radarMeshCanvasRenderer = mesh_object.GetComponent<CanvasRenderer>();
            }

            if (radarMeshCanvasRenderer == null) {
                Debug.LogWarning("CanvasRenderer를 찾을 수 없습니다. UpdateStatsVisual을 중단합니다.");
                return;
            }
        }

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[6];
        Vector2[] uv = new Vector2[6];
        int[] triangles = new int[3*5];

        float angle = 360f / 5;
        float radarChartSize = 172f;

        Vector3 vertex_luk = Quaternion.Euler(0, 0, -angle * 0) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(StatType.Luck);
        Vector3 vertex_def = Quaternion.Euler(0, 0, -angle * 1) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(StatType.Defence);
        Vector3 vertex_int = Quaternion.Euler(0, 0, -angle * 2) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(StatType.Mental);
        Vector3 vertex_dex = Quaternion.Euler(0, 0, -angle * 3) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(StatType.Dexterity);
        Vector3 vertex_atk = Quaternion.Euler(0, 0, -angle * 4) * Vector3.up * radarChartSize * stats.GetStatAmountNormalized(StatType.Attack);

        int idx_luk = 1;
        int idx_def = 2;
        int idx_int = 3;
        int idx_dex = 4;
        int idx_atk = 5;

        vertices[0] = Vector3.zero;
        vertices[idx_luk] = vertex_luk;
        vertices[idx_def] = vertex_def;
        vertices[idx_int] = vertex_int;
        vertices[idx_dex] = vertex_dex;
        vertices[idx_atk] = vertex_atk;

        uv[0] = Vector2.zero;
        uv[1] = Vector2.one;
        uv[2] = Vector2.one;
        uv[3] = Vector2.one;
        uv[4] = Vector2.one;
        uv[5] = Vector2.one;

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        triangles[6] = 0;
        triangles[7] = 3;
        triangles[8] = 4;

        triangles[9] = 0;
        triangles[10] = 4;
        triangles[11] = 5;

        triangles[12] = 0;
        triangles[13] = 5;
        triangles[14] = 1;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        radarMeshCanvasRenderer.SetMesh(mesh);
        radarMeshCanvasRenderer.SetMaterial(radarMaterial, null);
    }
}
