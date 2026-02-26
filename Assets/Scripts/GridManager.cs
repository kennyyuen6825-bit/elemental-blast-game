using UnityEngine;

public class GridManager : MonoBehaviour
{
    // 定義 8x8 網格
    public const int Columns = 8;
    public const int Rows = 8;
    
    // 儲存網格上的方塊物件（null 代表空位）
    private GameObject[,] grid = new GameObject[Columns, Rows];

    // 方塊在場景中的間距
    [SerializeField] private float cellSize = 1.1f;
    [SerializeField] private Vector2 gridOffset = new Vector2(-3.85f, -3.85f);

    /// <summary>
    /// 檢查指定的座標是否可以放置方塊
    /// </summary>
    public bool IsSpaceAvailable(int x, int y)
    {
        if (x < 0 || x >= Columns || y < 0 || y >= Rows) return false;
        return grid[x, y] == null;
    }

    /// <summary>
    /// 在網格中佔據一個位置
    /// </summary>
    public void PlaceObject(int x, int y, GameObject obj)
    {
        if (IsSpaceAvailable(x, y))
        {
            grid[x, y] = obj;
            // 這裡未來會加入消除行/列的檢查
            CheckAndClearLines();
        }
    }

    /// <summary>
    /// 檢查是否有填滿的行或列並執行消除
    /// </summary>
    public void CheckAndClearLines()
    {
        // 1.2 階段會實作具體的消除邏輯與元素特效
        Debug.Log("Checking lines...");
    }

    /// <summary>
    /// 將網格索引轉換為世界座標
    /// </summary>
    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(
            gridPos.x * cellSize + gridOffset.x,
            gridPos.y * cellSize + gridOffset.y,
            0
        );
    }
    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt((worldPosition.x - gridOffset.x) / cellSize);
        int y = Mathf.RoundToInt((worldPosition.y - gridOffset.y) / cellSize);
        return new Vector2Int(x, y);
    }
}
