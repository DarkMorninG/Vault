using System.Collections.Generic;
using UnityEngine;

namespace Vault.Selector {
    public class GraphNode<T> {
        private List<GraphEdge<T>> edges = new();

        public List<GraphEdge<T>> Edges => edges;

        private T value;

        public T Value => value;


        public GraphNode(T value) {
            this.value = value;
        }

        public void AddEdge(Vector2 direction, GraphNode<T> to) {
            edges.Add(new GraphEdge<T>(direction, this, to));
        }
    }
}