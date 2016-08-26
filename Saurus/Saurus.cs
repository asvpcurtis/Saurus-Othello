using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Saurus
{
    class Saurus : IPlayer
    {
        private static TranspositionTable m_table;
        public CancellationTokenSource m_tokenSource;
        public CancellationToken m_token;
        public Task m_think;
        public Saurus()
        {
            m_table = new TranspositionTable(1024*1024);
        }
        public void moveAsync()
        {
            //Task thinking;
            //thinking = Task.Run(() => move(ref mainBoard, moveList));
            m_tokenSource = new CancellationTokenSource();
            m_token = m_tokenSource.Token;
            m_think = Task.Run(() =>
            {
            },m_tokenSource.Token);

            m_tokenSource.Cancel();
        }
        public void cancelAsync()
        {
            throw new NotImplementedException();
        }
        public void Report(int value)
        {
            throw new NotImplementedException();
        }
        private static Int32 HardEval(BitBoard i_board)
        {
            return (BitBoard.countSetBits(i_board.m_black) - BitBoard.countSetBits(i_board.m_white)) * 1000;
        }
        private static Int32 SoftEval(BitBoard i_board)
        {
            return 0;
        }
        private static Int32 AlphaBetaPruning(BitBoard i_board, ref UInt64 o_chosenMove, Int32 i_startDepth, Int32 i_currDepth, Int32 i_depthLeft, Int32 i_a, Int32 i_b)
        {
            //base case
            if (BitBoard.gameOver(i_board))
            {
                return HardEval(i_board);
            }
            else if (i_depthLeft == 0)
            {
                return SoftEval(i_board);
            }

            List<UInt64> moves = BitBoard.moveList(i_board);
            SortMoves(i_board, moves);
            //recursion
            UInt64 subBestMove = 0;
            if (i_board.m_blackTurn)
            {
                foreach (UInt64 move in moves)
                {
                    BitBoard sucessor = BitBoard.getSuccessor(i_board, move);
                    PositionMetadata metadata;
                    Int32 eval;
                    if (!m_table.TryGetValue(sucessor, out metadata))
                    {
                        eval = AlphaBetaPruning(sucessor, ref subBestMove, i_startDepth, i_currDepth + 1, i_depthLeft - 1, i_a, i_b);
                        metadata = new PositionMetadata(sucessor, eval, i_startDepth, i_currDepth, i_currDepth + i_depthLeft);
                        m_table.add(metadata);
                    }
                    else
                    {
                        eval = metadata.m_eval;
                    }
                    if (eval > i_a)
                    {
                        i_a = eval;
                        o_chosenMove = move;
                    }
                    if (i_b <= i_a) { break; }
                }
                return i_a;
            }
            else
            {
                foreach (UInt64 move in moves)
                {
                    BitBoard sucessor = BitBoard.getSuccessor(i_board, move);
                    PositionMetadata metadata;
                    Int32 eval;
                    if (!m_table.TryGetValue(sucessor, out metadata) && metadata.m_currentDepth == i_currDepth)
                    {
                        eval = AlphaBetaPruning(sucessor, ref subBestMove, i_startDepth, i_currDepth + 1, i_depthLeft - 1, i_a, i_b);
                        metadata = new PositionMetadata(sucessor, eval, i_startDepth, i_currDepth, i_currDepth + i_depthLeft);
                        m_table.add(metadata);
                    }
                    else
                    {
                        eval = metadata.m_eval;
                    }
                    if (eval < i_b)
                    {
                        i_b = eval;
                        o_chosenMove = move;
                    }
                    if (i_b <= i_a) { break; }
                }
                return i_b;
            }
        }
        
        private static void SortMoves(BitBoard i_curPos, List<UInt64> i_moveList)
        {
            List<PositionMetadata> posList = new List<PositionMetadata>();
            if (i_curPos.m_blackTurn)
            {
                foreach (UInt64 move in i_moveList)
                {
                    BitBoard sucessor = BitBoard.getSuccessor(i_curPos, move);
                    PositionMetadata metadata;
                    if (!m_table.TryGetValue(sucessor, out metadata))
                    {
                        metadata = new PositionMetadata(sucessor, Saurus.SoftEval(sucessor), 0, 0, 0);
                    }
                    posList.Add(metadata);
                }
            }
            else
            {
                foreach (UInt64 move in i_moveList)
                {
                    BitBoard sucessor = BitBoard.getSuccessor(i_curPos, move);
                    PositionMetadata metadata;
                    if (!m_table.TryGetValue(sucessor, out metadata))
                    {
                        metadata = new PositionMetadata(sucessor, Saurus.SoftEval(sucessor), 0, 0, 0);
                    }
                    posList.Add(metadata);
                }
            }

            for (int i = 0; i < i_moveList.Count - 1; i++)
            {
                int j = i + 1;

                while (j > 0)
                {
                    if (posList[j - 1].CompareTo(posList[j]) < 0)
                    {
                        PositionMetadata tempPos = posList[j - 1];
                        posList[j - 1] = posList[j];
                        posList[j] = tempPos;

                        UInt64 tempMove = i_moveList[j - 1];
                        i_moveList[j - 1] = i_moveList[j];
                        i_moveList[j] = tempMove;
                    }
                    j--;
                }
            }
        }
    }
}
