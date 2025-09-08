using UnityEngine;

namespace Vault.Selector {
    public class GraphEdge<T> {
        private EdgeType edgeType;
        private Vector2 direction;
        private GraphNode<T> from;
        private GraphNode<T> to;

        public EdgeType EdgeType => edgeType;

        public Vector2 Direction => direction;

        public GraphNode<T> From => from;

        public GraphNode<T> To => to;

        public GraphEdge(Vector2 direction, GraphNode<T> from, GraphNode<T> to) {
            this.direction = direction;
            this.from = from;
            this.to = to;
        }
    }

    public enum EdgeType {
        Bidirectional,
        Input,
        Output
    }
}