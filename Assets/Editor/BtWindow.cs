using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class BtWindow : EditorWindow
{

    //private IGraphLayout m_layout;
    //private IGraphRenderer m_renderer;

    BtNode m_root;

    protected BtNode createTree()
    {


        BtNode isTargetSelected = new Sequence(new IsTargeting("pill"), new Inverter(new IsClose(1)));
        BtNode stickyTarget = new Selector(isTargetSelected, new TargetRandom("pill"));

        BtNode wonderToPill = new Sequence(stickyTarget, new AwayFromTarget());
        return wonderToPill;

        //BtNode chasePlayer = new Sequence(new TargetPlayer("Player"), new IsClose(3), new TowardsTarget());
        //return chasePlayer;
        //return new Selector(chasePlayer, wonderToPill);
    }

    [MenuItem("Window/Bt Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(BtWindow));
    }

    public BtWindow()
    {
        m_root = createTree();
    }

    class NodeGroup
    {
        public BtNode parent;
        public BtNode child;
        public int depth;

        public NodeGroup(BtNode parent, BtNode child, int depth)
        {
            this.parent = parent;
            this.child = child;
            this.depth = depth;
        }
    }

    public void upsert<K, V>(Dictionary<K, List<V>> dict, K key, V node)
    {
        List<V> valueAgg;
        if (!dict.TryGetValue(key, out valueAgg))
        {
            valueAgg = new List<V>();
            dict.Add(key, valueAgg);
        }
        valueAgg.Add(node);
    }

    private List<NodeGroup> m_vertex = new List<NodeGroup>();
    private Dictionary<int, List<BtNode>> treeToLayers(BtNode root) {
        Dictionary<int, List<BtNode>> layerAgg = new Dictionary<int, List<BtNode>>();

        Queue<NodeGroup> unexpanded = new Queue<NodeGroup>();
        unexpanded.Enqueue(new NodeGroup(null, root, 0));
        m_vertex.Add(new NodeGroup(null, root, 0));

        while (unexpanded.Count != 0)
        {
            NodeGroup current = unexpanded.Dequeue();
            upsert(layerAgg, current.depth, current.child);

            foreach (BtNode child in current.child.children() )
            {
                unexpanded.Enqueue(new NodeGroup(current.child, child, current.depth + 1));
                m_vertex.Add(new NodeGroup(current.child, child, current.depth + 1));
            }
        }

        return layerAgg;
    }


    private int getMaxLayerWidth( Dictionary<int, List<BtNode>> layers )
    {
        int largestLayer = 0;
        foreach ( var pair in layers ) {
            largestLayer = System.Math.Max( largestLayer, pair.Value.Count );
        }
        return largestLayer;
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        EditorGUILayout.TextField("Text Field", "text");

        Dictionary<int, List<BtNode>> layers = treeToLayers(m_root);
        int maxWidth = getMaxLayerWidth(layers);
        float windowWidth = (75 + 20) * maxWidth;

        Dictionary<BtNode, Vector2> nodePositions = new Dictionary<BtNode, Vector2>();

        int stepSize = 75;
        float layerHeight = 75;
        foreach( var pair in layers )
        {
            float size = pair.Value.Count * 75;
            float paddingSpace = windowWidth - size;

            float padding = paddingSpace / (float)pair.Value.Count;
            float x = padding / 2;

            foreach ( BtNode child in pair.Value ) {
                Rect nodeRect = new Rect(x, layerHeight, 75, 50);
                drawNode(nodeRect, child, false);

                nodePositions.Add(child, new Vector2(x + nodeRect.width / 2, layerHeight + nodeRect.height / 2));

                x += padding + nodeRect.width;
            }

            layerHeight += stepSize;
        }

        // draw vertex list
        foreach( var vertex in m_vertex ) {
            if (vertex.parent != null)
            {
                Handles.DrawDottedLine(nodePositions[vertex.parent], nodePositions[vertex.child], 5.0f);
            }
        }


        stepSize = 75;
        layerHeight = 75;
        foreach (var pair in layers)
        {
            float size = pair.Value.Count * 75;
            float paddingSpace = windowWidth - size;

            float padding = paddingSpace / (float)pair.Value.Count;
            float x = padding / 2;

            foreach (BtNode child in pair.Value)
            {
                Rect nodeRect = new Rect(x, layerHeight, 75, 50);
                drawNode(nodeRect, child, false);

                //nodePositions.Add(child, new Vector2(x + nodeRect.width / 2, layerHeight + nodeRect.height / 2));

                x += padding + nodeRect.width;
            }

            layerHeight += stepSize;
        }

        //groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        //myBool = EditorGUILayout.Toggle("Toggle", false);
        //myFloat = EditorGUILayout.Slider("Slider", 0.3, -3, 3);
        //EditorGUILayout.EndToggleGroup();
    }

    private void drawNode(Rect nodeRect, BtNode node, bool selected)
    {
        //string nodeType = "test";

        drawRect(nodeRect, Color.white, node.getName(), false, false);
    }

    private void drawRect(Rect rect, Color color, string label, bool active, bool selected = false)
    {
        var defaultColour = GUI.color;
        GUI.color = color;
        GUI.Box(rect, label);
        GUI.color = defaultColour;
    }
}
