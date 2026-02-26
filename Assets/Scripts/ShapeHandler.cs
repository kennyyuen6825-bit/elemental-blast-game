using UnityEngine;
using System.Collections.Generic;

public class ShapeHandler : MonoBehaviour
{
    public List<Vector2Int> relativeIndices = new List<Vector2Int>();
    public GameObject blockPrefab;
    public float scaleOnDrag = 1.2f;
    
    public GridManager gridManager;
    private Vector3 startPosition;
    private Vector3 offset;
    private bool isDragging = false;

    void Start()
    {
        startPosition = transform.position;
        if (gridManager == null) gridManager = FindObjectOfType<GridManager>();
    }

    // 根據相對座標生成組成形狀的小方塊
    [ContextMenu("Generate Visuals")]
    public void CreateVisuals()
    {
        // 先清除舊的
        foreach (Transform child in transform) {
            #if UNITY_EDITOR
            DestroyImmediate(child.gameObject);
            #else
            Destroy(child.gameObject);
            #endif
        }

        if (blockPrefab == null) return;

        // 獲取網格間距，確保方塊間距與網格一致
        float spacing = 1.1f;
        if (gridManager == null) gridManager = FindObjectOfType<GridManager>();
        // 注意：這裡假設 GridManager 有一個公有的 cellSize 變量，我們稍後更新它
        
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        bool first = true;

        foreach (var pos in relativeIndices)
        {
            GameObject block = Instantiate(blockPrefab, transform);
            // 使用 1.1f 作為間距進行排列
            block.transform.localPosition = new Vector3(pos.x * 1.1f, pos.y * 1.1f, 0);
            
            if (first) {
                bounds = new Bounds(block.transform.localPosition, Vector3.one);
                first = false;
            } else {
                bounds.Encapsulate(new Bounds(block.transform.localPosition, Vector3.one));
            }
        }

        // 自動調整 BoxCollider2D
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null) {
            collider.offset = bounds.center;
            collider.size = bounds.size;
        }
    }

    void OnMouseDown()
    {
        isDragging = true;
        transform.localScale = Vector3.one * scaleOnDrag;
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        transform.localScale = Vector3.one;

        if (gridManager != null)
        {
            Vector2Int gridPos = gridManager.WorldToGrid(transform.position);
            
            // 檢查形狀中的所有小方塊是否都能放下
            bool canPlace = true;
            foreach (var pos in relativeIndices)
            {
                if (!gridManager.IsSpaceAvailable(gridPos.x + pos.x, gridPos.y + pos.y))
                {
                    canPlace = false;
                    break;
                }
            }

            if (canPlace)
            {
                // 正式放置
                foreach (var pos in relativeIndices)
                {
                    gridManager.PlaceObject(gridPos.x + pos.x, gridPos.y + pos.y, gameObject);
                }
                
                // 讓這個 Shape 直接停在對齊後的位置
                transform.position = gridManager.GridToWorld(gridPos);
                // 禁用拖拽，防止重複放置
                enabled = false; 
                return;
            }
        }
        
        // 如果不能放置，彈回原位
        transform.position = startPosition; 
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10; // 距離攝像機的距離
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
