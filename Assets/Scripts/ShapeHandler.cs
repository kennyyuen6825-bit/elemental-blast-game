using UnityEngine;
using System.Collections.Generic;

public class ShapeHandler : MonoBehaviour
{
    public List<Vector2Int> relativeIndices = new List<Vector2Int>();
    public GameObject blockPrefab;
    public float scaleOnDrag = 1.2f;
    
    private Vector3 startPosition;
    private Vector3 offset;
    private bool isDragging = false;

    void Start()
    {
        startPosition = transform.position;
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

        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        bool first = true;

        foreach (var pos in relativeIndices)
        {
            GameObject block = Instantiate(blockPrefab, transform);
            block.transform.localPosition = new Vector3(pos.x, pos.y, 0);
            
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
        // 這裡未來會加入「放置判定」邏輯
        transform.position = startPosition; 
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10; // 距離攝像機的距離
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
