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
    public void PlaceObject(int x, int y, GameObject blockObj)
    {
        if (x >= 0 && x < Columns && y >= 0 && y < Rows)
        {
            grid[x, y] = blockObj;
        }
    }

    public void FinalizePlacement()
    {
        CheckAndClearLines();
    }

    /// <summary>
    /// 檢查是否有填滿的行或列並執行消除
    /// </summary>
    public void CheckAndClearLines()
    {
        System.Collections.Generic.List<int> rowsToClear = new System.Collections.Generic.List<int>();
        System.Collections.Generic.List<int> colsToClear = new System.Collections.Generic.List<int>();

        for (int y = 0; y < Rows; y++)
        {
            bool full = true;
            for (int x = 0; x < Columns; x++)
            {
                if (grid[x, y] == null) { full = false; break; }
            }
            if (full) rowsToClear.Add(y);
        }

        for (int x = 0; x < Columns; x++)
        {
            bool full = true;
            for (int y = 0; y < Rows; y++)
            {
                if (grid[x, y] == null) { full = false; break; }
            }
            if (full) colsToClear.Add(x);
        }

        foreach (int y in rowsToClear) ClearRow(y);
        foreach (int x in colsToClear) ClearColumn(x);
    }

    private void ClearRow(int y)
    {
        Debug.Log($"Row {y} Cleared!");
        for (int x = 0; x < Columns; x++)
        {
            if (grid[x, y] != null)
            {
                CheckElementalEffect(x, y);
                Destroy(grid[x, y]);
                grid[x, y] = null;
            }
        }
    }

    private void ClearColumn(int x)
    {
        Debug.Log($"Column {x} Cleared!");
        for (int y = 0; y < Rows; y++)
        {
            if (grid[x, y] != null)
            {
                CheckElementalEffect(x, y);
                Destroy(grid[x, y]);
                grid[x, y] = null;
            }
        }
    }

    private void CheckElementalEffect(int x, int y)
    {
        // 獲取該方块的元素屬性 (假設方塊上有一個標記或腳本)
        // 為了簡單起見，我們先通過物體名字或標籤判斷，稍後優化
        if (grid[x, y].name.Contains("Fire"))
        {
            TriggerFireExplosion(x, y);
        }
    }

    private void TriggerFireExplosion(int centerX, int centerY)
    {
        Debug.Log($"Fire Explosion at {centerX}, {centerY}!");
        for (int x = centerX - 1; x <= centerX + 1; x++)
        {
            for (int y = centerY - 1; y <= centerY + 1; y++)
            {
                if (x >= 0 && x < Columns && y >= 0 && y < Rows)
                {
                    if (grid[x, y] != null)
                    {
                        Destroy(grid[x, y]);
                        grid[x, y] = null;
                    }
                }
            }
        }
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
