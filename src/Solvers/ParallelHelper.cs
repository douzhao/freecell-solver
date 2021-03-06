using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using FreeCellSolver.Game;

namespace FreeCellSolver.Solvers
{
    internal static class ParallelHelper
    {
        internal static HashSet<Board> GetStates(Board initialBoard, int num)
        {
            var tree = new Dictionary<int, HashSet<Board>>() { { 0, new HashSet<Board> { initialBoard } } };
            var depth = 0;

            while (true)
            {
                foreach (var b in tree[depth++])
                {
                    if (!tree.ContainsKey(depth))
                    {
                        tree.Add(depth, new HashSet<Board>());
                    }

                    foreach (var move in b.GetValidMoves())
                    {
                        var next = b.ExecuteMove(move);
                        tree[depth].Add(next);
                    }
                }

                if (tree[depth].Count > num)
                {
                    break;
                }
            }

            Debug.Assert(tree[depth].Count > num);

            // TODO:
            // Find a better way to utilize all cpu cores
            var nominees = new List<HashSet<Board>>();
            foreach (var node in tree[depth - 1])
            {
                var descendants = tree[depth].Where(t => t.Prev == node);
                var adjacents = tree[depth - 1].Where(n => n != node);
                nominees.Add(new HashSet<Board>(descendants.Concat(adjacents)));
            }

            var boards = nominees.OrderByDescending(p => p.Count).FirstOrDefault(p => p.Count <= num);
            return boards ?? tree[depth - 1];
        }
    }
}