using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vault.Selector {
    public class GraphSelector<T> : ISelector<T> {
        private GraphNode<T> selectedNode;

        public GraphSelector(List<GraphNode<T>> nodes) {
            this.Nodes = nodes;
            selectedNode = nodes[0];
        }

        public GraphSelector(List<GraphNode<T>> nodes, T startNode) {
            this.Nodes = nodes;
            selectedNode = nodes.Find(node => node.Value.Equals(startNode));
        }

        public List<GraphNode<T>> Nodes { get; }

        public event ISelector<T>.SelectionChange OnSelectionChange;
        public T CurrentSelected => selectedNode.Value;

        public void Select(Vector2 direction) {
            selectedNode.Edges
                .Where(edge => Vector2.Dot(edge.Direction.normalized, direction.normalized) > .8f)
                .OrderBy(edge => Vector2.Dot(edge.Direction.normalized, direction.normalized))
                .FirstOptional()
                .IfPresent(edge => {
                    OnSelectionChange?.Invoke(edge.To.Value, selectedNode.Value);
                    selectedNode = edge.To;
                });
        }
    }
}