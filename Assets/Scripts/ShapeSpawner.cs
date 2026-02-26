using UnityEngine;
using System.Collections.Generic;

public class ShapeSpawner : MonoBehaviour
{
    public List<ShapeData> availableShapes = new List<ShapeData>();
    public Transform[] spawnPositions; // 畫面下方的 3 個位置
    public GameObject shapePrefab;    // 掛載了 ShapeHandler 的 Prefab
    
    private GameObject[] currentShapes = new GameObject[3];

    [System.Serializable]
    public struct ShapeData
    {
        public string name;
        public Vector2Int[] relativeIndices;
        public Color color;
    }

    void Start()
    {
        InitializeDefaultShapes();
        SpawnBatch();
    }

    void InitializeDefaultShapes()
    {
        if (availableShapes.Count > 0) return;

        // 定義經典方塊
        availableShapes.Add(new ShapeData { name = "Dot", color = Color.yellow, relativeIndices = new Vector2Int[] { new Vector2Int(0,0) } });
        availableShapes.Add(new ShapeData { name = "I_2", color = Color.cyan, relativeIndices = new Vector2Int[] { new Vector2Int(0,0), new Vector2Int(0,1) } });
        // 使用自定義橘色，避免部分版本缺少 Color.orange
        availableShapes.Add(new ShapeData { name = "L_Small", color = new Color(1f, 0.5f, 0f), relativeIndices = new Vector2Int[] { new Vector2Int(0,0), new Vector2Int(0,1), new Vector2Int(1,0) } });
        availableShapes.Add(new ShapeData { name = "Square_2x2", color = Color.blue, relativeIndices = new Vector2Int[] { new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(1,1) } });
    }

    public void SpawnBatch()
    {
        for (int i = 0; i < 3; i++)
        {
            if (currentShapes[i] == null)
            {
                SpawnShape(i);
            }
        }
    }

    void SpawnShape(int index)
    {
        GameObject newShape = Instantiate(shapePrefab, spawnPositions[index].position, Quaternion.identity);
        ShapeHandler handler = newShape.GetComponent<ShapeHandler>();
        
        // 隨機抽一個形狀數據
        ShapeData data = availableShapes[Random.Range(0, availableShapes.Count)];
        handler.relativeIndices = new List<Vector2Int>(data.relativeIndices);
        handler.color = data.color; // 傳遞顏色
        
        // 這裡可以傳遞顏色，讓 ShapeHandler 更新視覺
        handler.CreateVisuals();
        
        currentShapes[index] = newShape;
    }

    public void OnShapePlaced(GameObject shape)
    {
        for (int i = 0; i < 3; i++)
        {
            if (currentShapes[i] == shape)
            {
                currentShapes[i] = null;
                break;
            }
        }

        // 檢查是否 3 個都用完了
        bool allEmpty = true;
        foreach (var s in currentShapes) if (s != null) allEmpty = false;

        if (allEmpty) SpawnBatch();
    }
}
