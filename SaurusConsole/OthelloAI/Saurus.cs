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
        public Saurus()
        {
            currPos = new Position("startpos");
        }

        public string About()
        {
            return "Saurus 1.0.0 - developed by Curtis Barlow-Wilkes";
        }

        public Task<(int eval, List<Move> pv)> GoDepth(int depth, CancellationToken token)
        {
            return Task.Run(() => 
            {
                return AlphaBetaSearch(int.MinValue, int.MaxValue, depth, currPos, token);
            }, token);
        }
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
