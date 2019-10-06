using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ludumdare45
{
    class s_pathfind
    {
        public static double Heuristic(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        public static void GeneratePathTo(Point start, Point goal, ref List<Point> path, int[] compat)
        {
            //path = Task.Run(() => GeneratePath(start, goal)).Result;

            path = GeneratePath(start, goal, compat);
        }


        public static bool DoesPathExist(Point start, Point end, int[] compat)
        {
            List<Point> path = GeneratePath(start, end, compat);
            return path.Count > 1;
        }

        static List<Point>  path = new List<Point>();
        static Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();
        static Dictionary<Point, double> costSoFar = new Dictionary<Point, double>();
        static PriorityQueue<Point> frontier = new PriorityQueue<Point>();
        private static List<Point> GeneratePath(Point start, Point goal, int[] compat)
        {
            path.Clear();
            cameFrom.Clear();
            costSoFar.Clear();
            frontier.Clear();
            frontier.Enqueue(start, 0);

            cameFrom[start] = start;
            costSoFar[start] = 0;

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                if (current.Equals(goal)) break;

                foreach (var next in GetNeighbours(current))
                {
                    if (next.X < 0 || next.X > g_map.size || next.Y < 0 || next.Y > g_map.size) continue;
                    if (!compat.Contains(g_map.GetBuildingAt(next.X, next.Y))) continue;

                    double newCost = costSoFar[current] + 1;
                    if (!costSoFar.ContainsKey(next)
                        || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        double priority = newCost + Heuristic(next, goal);
                        frontier.Enqueue(next, priority);
                        //Console.WriteLine(string.Format(" -> added node. ({0}, {1})", next, priority));
                        cameFrom[next] = current;
                    }
                }
            }

            path.Clear();
            var c = goal;
            while (c != start)
            {
                path.Add(c);
                if (!cameFrom.ContainsKey(c))
                    return path;
                c = cameFrom[c];
            }

            //path.Add(start); // optional: for complete path
            path.Reverse();

           // Console.WriteLine(string.Format("path calculations finished. (length {0}, took {1}ms)", start, 0));
            return path;
        }

        private static Point[] GetNeighbours(Point v)
        {
            return new[]
            {
                new Point(v.X - 1, v.Y), // left
                new Point(v.X + 1, v.Y), // right
                new Point(v.X, v.Y - 1), // up
                new Point(v.X, v.Y + 1), // down
                /*
                new Point(v.X - 1, v.Y - 1), // up, left
                new Point(v.X + 1, v.Y - 1), // up, right
                new Point(v.X - 1, v.Y + 1), // down, left
                new Point(v.X + 1, v.Y + 1) // down, right*/
            };
        }
    }
    public class PriorityQueue<T>
    {
        private readonly List<Tuple<T, double>> _elements = new List<Tuple<T, double>>();

        public int Count => _elements.Count;


        public void Clear()
        {
            _elements.Clear();
        }
        public void Enqueue(T item, double priority)
        {
            _elements.Add(Tuple.Create(item, priority));
        }

        public T Dequeue()
        {
            var bestIndex = 0;

            for (var i = 0; i < _elements.Count; i++)
                if (_elements[i].Item2 < _elements[bestIndex].Item2)
                    bestIndex = i;

            var bestItem = _elements[bestIndex].Item1;
            _elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }
}
