using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
    [Header("Cài đặt Prefab")]
    public GameObject nodePrefab;
    public GameObject linePrefab;
    public Transform mapContainer;

    [Header("Cấu hình Loại Nút")]
    public List<NodeTypeConfig> nodeTypes;
    public NodeTypeConfig bossNode;

    [Header("Cấu hình Bản đồ")]
    public List<int> nodesPerLayer = new List<int> { 3, 3, 3, 3, 1 };
    public float xSpacing = 200f;
    public float ySpacing = 150f;

    private List<List<MapNode>> mapGrid = new List<List<MapNode>>();

    void Start()
    {
        GenerateNodes();
        ConnectNodes();
        DrawLines();
    }

    void GenerateNodes()
    {
        float mapWidth = (nodesPerLayer.Count - 1) * xSpacing;
        float startX = -mapWidth / 2;

        for (int i = 0; i < nodesPerLayer.Count; i++)
        {
            List<MapNode> currentLayerNodes = new List<MapNode>();
            int nodeCount = nodesPerLayer[i];

            List<string> usedSpecialTypesInThisLayer = new List<string>();

            for (int j = 0; j < nodeCount; j++)
            {
                GameObject nodeObj = Instantiate(nodePrefab, mapContainer);
                nodeObj.name = $"Node L{i}-{j}";

                float xPos = startX + (i * xSpacing);
                float yPos = (j - (nodeCount - 1) / 2.0f) * ySpacing;
                nodeObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);

                MapNode nodeScript = nodeObj.GetComponent<MapNode>();
                NodeTypeConfig config;

                if (i == nodesPerLayer.Count - 1)
                {
                    config = bossNode;
                }
                else
                {
                    List<NodeTypeConfig> allowedTypes = new List<NodeTypeConfig>();

                    foreach (var type in nodeTypes)
                    {
                        bool isRestricted = (type.typeName == "Casino" ||
                                             type.typeName == "Rest");

                        if (isRestricted && usedSpecialTypesInThisLayer.Contains(type.typeName))
                        {
                            continue;
                        }

                        allowedTypes.Add(type);
                    }

                    if (allowedTypes.Count > 0)
                    {
                        config = allowedTypes[Random.Range(0, allowedTypes.Count)];

                        if (config.typeName == "Casino" ||
                            config.typeName == "Rest")
                        {
                            usedSpecialTypesInThisLayer.Add(config.typeName);
                        }
                    }
                    else
                    {
                        config = nodeTypes[0];
                    }
                }

                nodeScript.Setup(config.icon, config.color, i != 0, config.sceneName);
                currentLayerNodes.Add(nodeScript);
            }
            mapGrid.Add(currentLayerNodes);
        }
    }

    void ConnectNodes()
    {
        for (int i = 0; i < mapGrid.Count - 1; i++)
        {
            List<MapNode> currentLayer = mapGrid[i];
            List<MapNode> nextLayer = mapGrid[i + 1];

            foreach (var node in currentLayer)
            {
                MapNode target = GetRandomNode(nextLayer);
                if (!node.outgoingNodes.Contains(target))
                {
                    node.outgoingNodes.Add(target);
                }
            }

            foreach (var targetNode in nextLayer)
            {
                bool hasConnection = false;
                foreach (var previousNode in currentLayer)
                {
                    if (previousNode.outgoingNodes.Contains(targetNode))
                    {
                        hasConnection = true;
                        break;
                    }
                }

                if (!hasConnection)
                {
                    MapNode randomParent = currentLayer[Random.Range(0, currentLayer.Count)];
                    if (!randomParent.outgoingNodes.Contains(targetNode))
                    {
                        randomParent.outgoingNodes.Add(targetNode);
                    }
                }
            }
        }
    }

    MapNode GetRandomNode(List<MapNode> nextLayer)
    {
        return nextLayer[Random.Range(0, nextLayer.Count)];
    }

    void DrawLines()
    {
        foreach (var layer in mapGrid)
        {
            foreach (var node in layer)
            {
                foreach (var target in node.outgoingNodes)
                {
                    CreateLineConnection(node.GetComponent<RectTransform>(), target.GetComponent<RectTransform>());
                }
            }
        }
    }

    void CreateLineConnection(RectTransform dotA, RectTransform dotB)
    {
        GameObject lineObj = Instantiate(linePrefab, mapContainer);
        lineObj.transform.SetAsFirstSibling();

        RectTransform rect = lineObj.GetComponent<RectTransform>();

        Vector2 dir = (dotB.anchoredPosition - dotA.anchoredPosition).normalized;
        float distance = Vector2.Distance(dotA.anchoredPosition, dotB.anchoredPosition);

        rect.anchoredPosition = dotA.anchoredPosition + dir * distance * 0.5f;
        rect.sizeDelta = new Vector2(distance, 3f);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rect.localRotation = Quaternion.Euler(0, 0, angle);
    }
}