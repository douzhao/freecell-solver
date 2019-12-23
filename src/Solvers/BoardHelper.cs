using System.Linq;
using System.Collections.Generic;

namespace FreeCellSolver.Solvers
{
    internal static class BoardHelper
    {
        public static (IEnumerable<Board> edges, bool backtrack) GetEdges(this Board board, bool reverse = false)
        {
            var (moves, foundFoundation) = board.GetValidMoves(true);

            if (moves.Count == 0 || (board.MovesSinceFoundation >= 17 && !foundFoundation))
            {
                return (Enumerable.Empty<Board>(), true);
            }

            var addedBoards = new List<Board>();

            if (reverse)
            {
                for (var i = moves.Count - 1; i >= 0; i--)
                {
                    var next = board.Clone();
                    next.ExecuteMove(moves[i], true);
                    addedBoards.Add(next);
                }
            }
            else
            {
                foreach (var move in moves)
                {
                    var next = board.Clone();
                    next.ExecuteMove(move, true);
                    addedBoards.Add(next);
                }
            }

            return reverse
                ? (addedBoards.OrderBy(p => p.LastMoveRating), false)
                : (addedBoards.OrderByDescending(p => p.LastMoveRating), false);
        }
    }
}