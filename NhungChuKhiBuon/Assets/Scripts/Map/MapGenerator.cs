using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

// Các class dữ liệu giữ nguyên
public enum NodeState { Locked, Unlocked, Completed }

[System.Serializable]
public class MapNodeData
{
    public int x, y;
    public string typeName;
    public string sceneName;
    public NodeState state;
    public List<Vector2Int> outgoing = new List<Vector2Int>();
}

[System.Serializable]
public class NodeTypeConfig
{
    public string typeName;
    public Sprite icon;
    public Color color;
    public string sceneName;
}

public class MapGenerator : MonoBehaviour
{
    [Header("Cài đặt")]
    public GameObject nodePrefab;
    public GameObject linePrefab;
    public Transform mapContainer;

    [Header("Cấu hình Map")]
    public List<int> nodesPerLayer = new List<int> { 3, 3, 3, 3, 3, 3, 2, 1 };
    public float xSpacing = 300f;
    public float ySpacing = 150f;

    [Header("Loại nút")]
    public List<NodeTypeConfig> nodeTypes;
    public NodeTypeConfig bossNode;

    public static List<List<MapNodeData>> savedMapData;

    [Header("Boss Config")]
    // Kéo 3 file BossData vào list này theo thứ tự
    public List<BossData> levelBossList;

    // Biến lưu level hiện tại (0, 1, 2...)
    public int currentLevelIndex = 0;

    void Start()
    {
        if (savedMapData == null || savedMapData.Count == 0)
        {
            GenerateMapData();
        }

        SpawnMapVisuals();
    }

    void GenerateMapData()
    {
        savedMapData = new List<List<MapNodeData>>();
        int lastRestLayerIndex = -100;

        for (int i = 0; i < nodesPerLayer.Count; i++)
        {
            List<MapNodeData> currentLayer = new List<MapNodeData>();
            int count = nodesPerLayer[i];
            List<NodeTypeConfig> layerConfigs = new List<NodeTypeConfig>();

            if (i == nodesPerLayer.Count - 1)
            {
                layerConfigs.Add(bossNode);
            }
            else
            {
                NodeTypeConfig fightConfig = nodeTypes.Find(t => t.typeName == "Fight");
                if (fightConfig == null) fightConfig = nodeTypes[0];
                layerConfigs.Add(fightConfig);

                bool addRest = false;
                bool validGap = (i - lastRestLayerIndex > 2);

                if (i == 2 && validGap) { if (Random.value > 0.5f) addRest = true; }
                else if (i == 3) { if (lastRestLayerIndex != 2) addRest = true; }
                else if (i == 5 && validGap) { if (Random.value > 0.5f) addRest = true; }
                else if (i == 6) { if (lastRestLayerIndex != 5 && validGap) addRest = true; }

                if (addRest)
                {
                    NodeTypeConfig restConfig = nodeTypes.Find(t => t.typeName == "Rest" || t.typeName == "Nghỉ ngơi");
                    if (restConfig != null) { layerConfigs.Add(restConfig); lastRestLayerIndex = i; }
                }

                while (layerConfigs.Count < count)
                {
                    List<NodeTypeConfig> allowedTypes = nodeTypes.Where(t => t.typeName != "Rest" && t.typeName != "Nghỉ ngơi").ToList();
                    if (allowedTypes.Count > 0) layerConfigs.Add(allowedTypes[Random.Range(0, allowedTypes.Count)]);
                    else layerConfigs.Add(fightConfig);
                }
                layerConfigs = layerConfigs.OrderBy(x => Random.value).ToList();
            }

            for (int j = 0; j < count; j++)
            {
                MapNodeData data = new MapNodeData();
                data.x = i;
                data.y = j;
                NodeTypeConfig config = (j < layerConfigs.Count) ? layerConfigs[j] : nodeTypes[0];
                data.typeName = config.typeName;
                data.sceneName = config.sceneName;
                data.state = (i == 0) ? NodeState.Unlocked : NodeState.Locked;
                currentLayer.Add(data);
            }
            savedMapData.Add(currentLayer);
        }

        for (int i = 0; i < savedMapData.Count - 1; i++)
        {
            var currentLayer = savedMapData[i];
            var nextLayer = savedMapData[i + 1];

            // Đảm bảo mỗi node đều có ít nhất 1 đường đi
            foreach (var node in currentLayer)
            {
                AddConnection(node, GetRandomNode(nextLayer));
            }

            // Đảm bảo mỗi node đích đều có ít nhất 1 đầu vào
            foreach (var target in nextLayer)
            {
                bool hasParent = false;
                foreach (var prev in currentLayer)
                    if (prev.outgoing.Contains(new Vector2Int(target.x, target.y))) hasParent = true;

                if (!hasParent)
                    AddConnection(GetRandomNode(currentLayer), target);
            }

            // Tăng cường kết nối
            foreach (var node in currentLayer)
            {
                if (nextLayer.Count <= 1) continue;

                if (node.outgoing.Count < 2 || Random.value < 0.2f)
                {
                    int attempts = 0;
                    while (attempts < 5)
                    {
                        var candidate = GetRandomNode(nextLayer);

                        // Logic "Gần gũi": Ưu tiên nối với các node ở gần y (hàng xóm) để dây đỡ chéo quá xa (Optional)
                        // Nếu muốn map rối rắm thì bỏ qua check này cũng được.
                        if (Mathf.Abs(candidate.y - node.y) > 1 && Random.value > 0.3f)
                        {
                            attempts++;
                            continue; // Bỏ qua nếu quá xa (để map đẹp hơn)
                        }

                        if (!node.outgoing.Contains(new Vector2Int(candidate.x, candidate.y)))
                        {
                            AddConnection(node, candidate);
                            break; // Đã thêm được, thoát
                        }
                        attempts++;
                    }
                }
            }


        }
    }

    MapNodeData GetRandomNode(List<MapNodeData> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    void AddConnection(MapNodeData from, MapNodeData to)
    {
        if (!from.outgoing.Contains(new Vector2Int(to.x, to.y)))
            from.outgoing.Add(new Vector2Int(to.x, to.y));
    }

    void SpawnMapVisuals()
    {
        foreach (Transform child in mapContainer) ;

        RectTransform containerRect = mapContainer.GetComponent<RectTransform>();

        containerRect.anchorMin = new Vector2(0, 0);
        containerRect.anchorMax = new Vector2(0, 1);
        containerRect.pivot = new Vector2(0, 0.5f);

        float startX = 200f;
        float endPadding = 200f;
        float contentWidth = startX + ((nodesPerLayer.Count - 1) * xSpacing) + endPadding;

        containerRect.sizeDelta = new Vector2(contentWidth, 0);
        containerRect.anchoredPosition = new Vector2(0, 0);

        MapNode[,] nodeLookup = new MapNode[savedMapData.Count, 20];

        for (int i = 0; i < savedMapData.Count; i++)
        {
            int nodeCount = savedMapData[i].Count;
            for (int j = 0; j < nodeCount; j++)
            {
                MapNodeData data = savedMapData[i][j];
                NodeTypeConfig config = nodeTypes.Find(t => t.typeName == data.typeName);
                if (i == savedMapData.Count - 1) config = bossNode;
                if (config == null && nodeTypes.Count > 0) config = nodeTypes[0];

                GameObject obj = Instantiate(nodePrefab, mapContainer);
                RectTransform nodeRect = obj.GetComponent<RectTransform>();

                nodeRect.anchorMin = new Vector2(0, 0.5f);
                nodeRect.anchorMax = new Vector2(0, 0.5f);
                nodeRect.pivot = new Vector2(0.5f, 0.5f);

                float xPos = startX + (i * xSpacing);
                float yPos = (j - (nodeCount - 1) / 2.0f) * ySpacing;

                nodeRect.anchoredPosition = new Vector2(xPos, yPos);

                MapNode script = obj.GetComponent<MapNode>();
                script.Setup(config.icon, config.color, data.sceneName, i, j, data.state);
                nodeLookup[i, j] = script;
            }
        }

        for (int i = 0; i < savedMapData.Count; i++)
        {
            foreach (var nodeData in savedMapData[i])
            {
                foreach (var targetPos in nodeData.outgoing)
                {
                    MapNode fromObj = nodeLookup[i, nodeData.y];
                    MapNode toObj = nodeLookup[targetPos.x, targetPos.y];
                    if (fromObj != null && toObj != null)
                        CreateLine(fromObj.GetComponent<RectTransform>(), toObj.GetComponent<RectTransform>());
                }
            }
        }

        Canvas.ForceUpdateCanvases();
        ScrollRect scrollRect = mapContainer.GetComponentInParent<ScrollRect>();
        if (scrollRect != null) scrollRect.horizontalNormalizedPosition = 0f;

        if (MapPlayerController.Instance != null)
        {
            // 1. Đưa Hero xuống dưới cùng để nổi lên trên (Sửa lỗi hiển thị bị chìm)
            MapPlayerController.Instance.transform.SetAsLastSibling();

            MapNode lastCompletedNode = null;

            // 2. Tìm node cuối cùng đã hoàn thành (để Resume game)
            // Duyệt ngược từ map cuối về đầu
            for (int i = savedMapData.Count - 1; i >= 0; i--)
            {
                bool found = false;
                for (int j = 0; j < savedMapData[i].Count; j++)
                {
                    if (savedMapData[i][j].state == NodeState.Completed)
                    {
                        lastCompletedNode = nodeLookup[i, j];
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }

            // --- LOGIC MỚI Ở ĐÂY ---

            if (lastCompletedNode != null)
            {
                // TRƯỜNG HỢP 1: ĐANG CHƠI DỞ (Resume)
                // Node có Transform nên dùng SnapToPosition (World Position) là đúng
                MapPlayerController.Instance.SnapToPosition(lastCompletedNode.transform.position);
            }
            else
            {
                // TRƯỜNG HỢP 2: NEW GAME (Chưa đi ải nào)

                float startPlayerMapX = 200f;
                float waitingX = startPlayerMapX - 150f; // Đứng lùi lại 150 đơn vị

                // Tính tọa độ Cục bộ (Local)
                Vector3 waitingLocalPos = new Vector3(waitingX, 0, 0);

                // --- SỬA Ở ĐÂY: Gán thẳng vào localPosition ---
                // Thay vì gọi SnapToPosition, ta set localPosition để nó hiểu là tọa độ trong MapContainer
                MapPlayerController.Instance.transform.localPosition = waitingLocalPos;

                // Reset Z về 0 cho chắc chắn
                MapPlayerController.Instance.transform.localPosition = new Vector3(waitingX, 0, 0);
            }
        }
    }

    void CreateLine(RectTransform A, RectTransform B)
    {
        GameObject line = Instantiate(linePrefab, mapContainer);
        line.transform.SetAsFirstSibling();
        RectTransform rect = line.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0, 0.5f);
        rect.anchorMax = new Vector2(0, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        Vector2 dir = (B.anchoredPosition - A.anchoredPosition).normalized;
        float dist = Vector2.Distance(A.anchoredPosition, B.anchoredPosition);

        rect.anchoredPosition = A.anchoredPosition + dir * dist * 0.5f;
        rect.sizeDelta = new Vector2(dist, 3f);
        rect.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    public static void OnNodeSelected(int x, int y)
    {
        var currentNode = savedMapData[x][y];
        currentNode.state = NodeState.Completed;

        foreach (var node in savedMapData[x])
        {
            if (node != currentNode) node.state = NodeState.Locked;
        }

        foreach (var targetIndex in currentNode.outgoing)
        {
            var targetNode = savedMapData[targetIndex.x][targetIndex.y];
            if (targetNode.state == NodeState.Locked)
                targetNode.state = NodeState.Unlocked;
        }
    }
}