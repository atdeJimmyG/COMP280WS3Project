using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public interface IGraphLayout
    {
        IEnumerable<Vertex> vertices { get;  }
        IEnumerable<Edge> edges { get; }

        bool leftToRight { get; }

        void CalculateLayout(IGraphLayout graph);
    }

    public class Edge
    {
        public Edge(Vertex src, Vertex dest)
        {
            source = src;
            destination = dest;
        }

        public Vertex source { get; private set; }
        public Vertex destination { get; private set; }
    }

    public class Vertex
    {
        public Vector2 position { get; set; }
        public Node node { get; private set; }

        public Vertex(Node node)
        {
            this.node = node;
        }
    }
