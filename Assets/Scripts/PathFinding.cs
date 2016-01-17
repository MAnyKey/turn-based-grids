using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

public class PathFinding {

    public interface IHasNeighbours<T> {
        IEnumerable<T> Neighbours();
    }
    
    public class Path<Node> : IEnumerable<Node> {
        public Path(Node start) : this(start, null, 0) {
        }

        private Path(Node lastStep, Path<Node> previousSteps, double totalCost) {
            LastStep = lastStep;
            PreviousSteps = previousSteps;
            TotalCost = totalCost;
        }

        public double TotalCost { get; private set; }
        public Node LastStep { get; private set; }
        public Path<Node> PreviousSteps { get; private set; }

        public Path<Node> AddStep(Node newStep, double stepCost) {
            return new Path<Node>(newStep, this, this.TotalCost + stepCost);
        }

        public IEnumerator<Node> GetEnumerator() {
            for (Path<Node> path = this; path != null; path = path.PreviousSteps) {
                yield return path.LastStep;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }

    public class FindPathQueue<P, V> {
        private SortedDictionary<P, Queue<V>> data = new SortedDictionary<P, Queue<V>>();
        //private Dictionary<V, P> minPath = new Dictionary<V, P>();

        public FindPathQueue() {
        }

        public void Add(P priority, V value) {
            Queue<V> list = GetPriority(priority);
            list.Enqueue(value);
        }

        private Queue<V> GetPriority(P priority) {
            Queue<V> list;
            data.TryGetValue(priority, out list);
            if (list == null) {
                list = new Queue<V>();
                data.Add(priority, list);
            }
            return list;
        }

        public V RemoveMin() {
            var minElement = data.First();
            var result = minElement.Value.Dequeue();
            if (minElement.Value.Count == 0) {
                data.Remove(minElement.Key);
            }
            return result;
        }

        public bool IsEmpty() {
            return data.Count == 0;
        }
    }

    public static Path<Node> FindPath<Node>(
        Node start, 
        Node end, 
        Func<Node, Node, double> distance, 
        Func<Node, Node, double> estimate) where Node : class, IHasNeighbours<Node> {
        var visited = new HashSet<Node>();
        //var minPaths = new Dictionary<Node, Path<Node>>();
        var queue = new FindPathQueue<double, Path<Node>>();
        queue.Add(0, new Path<Node>(start));
        while (!queue.IsEmpty()) {
            var path = queue.RemoveMin();
            var lastStep = path.LastStep;
            if (lastStep == end) {
                return path;
            }
            if (visited.Contains(lastStep)) {
                continue;
            }
            visited.Add(lastStep);
            foreach (var neighbour in lastStep.Neighbours()) {
                if (visited.Contains(neighbour)) {
                    continue;
                }
                double d = distance(lastStep, neighbour);
                var newPath = path.AddStep(neighbour, d);
                queue.Add(newPath.TotalCost + estimate(neighbour, end), newPath);
            }
        }
        return null;
    }
}
