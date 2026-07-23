using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    [Header("설치할 건물")]
    [SerializeField] private GameObject ChikenHousePrefab;

    private GameObject previewObject;

    private bool isBuildMode;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleBuildMode();
        }
        if (isBuildMode && previewObject != null)
        {
            Vector3 mousePos =
                Camera.main.ScreenToWorldPoint(Input.mousePosition);

            mousePos.z = 0;

            previewObject.transform.position = mousePos;
        }
    }

    void ToggleBuildMode()
    {
        isBuildMode = !isBuildMode;

        if (isBuildMode)
        {
            previewObject = Instantiate(ChikenHousePrefab);
        }
        else
        {
            Destroy(previewObject);
        }
    }
}