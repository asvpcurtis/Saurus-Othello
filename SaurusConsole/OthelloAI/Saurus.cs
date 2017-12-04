using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace SaurusConsole.OthelloAI
{
    class Saurus : IOthelloAI
    {
        Position currPos;

        /// <summary>
        /// Initializes a new instance of Saurus with the starting position
        /// </summary>
        public Saurus()
        {
            currPos = new Position("startpos");
        }

        /// <summary>
        /// Gets information about the engine it's name version and author
        /// </summary>
        /// <returns></returns>
        public string About()
        {
            return "Saurus 1.0.0 - developed by Curtis Barlow-Wilkes";
        }

        /// <summary>
        /// Get the best principal variation and its evaluation
        /// </summary>
        /// <param name="depth">The depth to search the position to</param>
        /// <param name="token">A cancellation token can be used to prematurely abandon the search</param>
        /// <returns>A tuple containing an evaluation of the position and the pricipal variation</returns>
        public Task<(int eval, List<Move> pv)> GoDepth(int depth, CancellationToken token)
        {
            return Task.Run(() => 
            {
                return AlphaBetaSearch(int.MinValue, int.MaxValue, depth, currPos, token);
            }, token);
        }

        /// <summary>
        /// Change the position for the Engine to work with
        /// </summary>
        /// <param name="pos">The new position</param>
        public void SetPosition(string pos)
        {
            currPos = new Position(pos);
        }

        private (int eval, List<Move> pv) AlphaBetaSearch(int a, int b, int depth, Position pos, CancellationToken token)
        {
            if (pos.GameOver() || depth == 0)
            {
                return (Evaluation(pos), new List<Move>());
            }
            // There should be always be atleast 1 move if the game is not over since Position knows who's turn it is
            IEnumerable<Move> moves = pos.GetLegalMoves();
            SortMoves(moves, pos);
            if (pos.BlackTurn())
            {
                (int eval, List<Move> pv) bestPV = (int.MinValue, null);
                foreach (Move move in moves)
                {
                    Position branch = pos.MakeMove(move);
                    var result = AlphaBetaSearch(a, b, depth - 1, branch, token);
                    if (result.eval > a)
                    {
                        a = result.eval;
                        result.pv.Insert(0, move);
                        bestPV = result;
                    }
                    if (b <= a)
                    {
                        break;
                    }
                }

                return bestPV;
            }
            else
            {
                (int eval, List<Move> pv) bestPV = (int.MaxValue, null);
                foreach (Move move in moves)
                {
                    Position branch = pos.MakeMove(move);
                    var result = AlphaBetaSearch(a, b, depth - 1, branch, token);
                    if (result.eval < b)
                    {
                        b = result.eval;
                        result.pv.Insert(0, move);
                        bestPV = result;
                    }
                    if (b <= a)
                    {
                        break;
                    }
                }

                return bestPV;
            }
            
        }
        private void SortMoves(IEnumerable<Move> moves, Position pos)
        {
            moves.OrderBy(move => 
            {
                pos.MakeMove(move);
                return Evaluation(pos);
            });
        }
        private int Evaluation(Position pos)
        {
            if (pos.GameOver())
            {
                return HardEvaluation(pos);
            }
            else
            {
                return SoftEvaluation(pos);
            }
        }
        private int HardEvaluation(Position pos)
        {
            return (pos.TotalBlackDisks() - pos.TotalWhiteDisks()) * 1000;
        }
        private int SoftEvaluation(Position pos)
        {
            return Mobility(pos);
        }
        private int Mobility(Position pos)
        {
            int mobility;
            Position reversed = new Position(pos.GetWhiteBitMask(), pos.GetBlackBitMask(), !pos.BlackTurn(), false);
            if (pos.BlackTurn())
            {
                mobility = pos.LegalMoveCount() - reversed.LegalMoveCount();
            }
            else
            {
                mobility = reversed.LegalMoveCount() - pos.LegalMoveCount();
            }
            return mobility;
        }

    }
}
