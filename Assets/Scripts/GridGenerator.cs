using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public int columns = 8;
    public int rows = 8;
    public float cellSize = 1.1f;
    public GameObject tilePrefab; // 拖入一個簡單的白色正方形 Sprite

    public bool autoFitCamera = true;

    void Start()
    {
        GenerateBackgroundGrid();
        if (autoFitCamera) FitCamera();
    }

    void FitCamera()
    {
        Camera cam = Camera.main;
        float gridWidth = columns * cellSize;
        // 根據螢幕寬度計算攝像機所需的 Size
        float screenAspect = (float)Screen.width / Screen.height;
        float targetSize = (gridWidth / screenAspect) / 2f;
        // 增加一點邊距
        cam.orthographicSize = targetSize + 1f;
    }

    void GenerateBackgroundGrid()
    {
        // 計算起始位置，讓網格居中
        Vector3 startPos = new Vector3(-(columns * cellSize) / 2f + cellSize / 2f, -(rows * cellSize) / 2f + cellSize / 2f, 0);

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 pos = startPos + new Vector3(x * cellSize, y * cellSize, 0);
                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity);
                tile.transform.parent = this.transform;
                tile.name = $"Tile_{x}_{y}";
                
                // 調整背景格子的顏色（深灰色比較好看）
                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                if (sr != null) sr.color = new Color(0.2f, 0.2f, 0.2f, 1.0f);
            }
        }
    }
}
