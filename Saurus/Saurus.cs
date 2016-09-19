using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Saurus
{
    class Saurus : IPlayer
    {
        private static TranspositionTable m_table;
        private static CancellationTokenSource m_tokenSource;
        public static CancellationToken m_token;
        public Task m_think;
        public Saurus()
        {
            m_table = new TranspositionTable(1024*1024);
        }
        public void moveAsync(BoardHistory i_board)
        {
            //Task thinking;
            //thinking = Task.Run(() => move(ref mainBoard, moveList));
            m_tokenSource = new CancellationTokenSource();
            m_token = m_tokenSource.Token;
            BitBoard currentState = i_board.getBoard();
            Int32 currentMoveNumber = i_board.getMoveNumber();

            m_think = Task.Run(() =>
            {
                UInt64 move = 0;
                AlphaBetaPruning(currentState, ref move, currentMoveNumber, 0, 10, Int32.MinValue, Int32.MaxValue);
                if (m_token.IsCancellationRequested)
                {
                    return;
                }
                bool canMove;
                i_board.addMove(move, out canMove);
                if (!canMove)
                {
                    string player = currentState.m_blackTurn ? "white" : "black";
                    MessageBox.Show(player + " can't move!");
                }
            },
            m_tokenSource.Token);
        }
        public void cancelAsync()
        {
            m_tokenSource.Cancel();
        }
        public void Report(Int32 value)
        {
            throw new NotImplementedException();
        }
        private static Int32 HardEval(BitBoard i_board)
        {
            return (BitBoard.countSetBits(i_board.m_black) - BitBoard.countSetBits(i_board.m_white)) * 1000;
        }
        private static Int32 SoftEval(BitBoard i_board)
        {
            return mobilityEval(i_board) + cornerEval(i_board) + stabilityEval(i_board);
        }

        public static Int32 mobilityEval(BitBoard position)
        {
            int eval = 0;
            if (position.m_blackTurn)
            {
                eval += BitBoard.countSetBits(BitBoard.getMoves(position));
                BitBoard reversePosition = BitBoard.switchTurns(position);
                eval -= BitBoard.countSetBits(BitBoard.getMoves(reversePosition));
            }
            else
            {
                eval += BitBoard.countSetBits(BitBoard.getMoves(position));
                BitBoard reversePosition = BitBoard.switchTurns(position);
                eval -= BitBoard.countSetBits(BitBoard.getMoves(reversePosition));
            }
            return eval;
        }

        public static Int32 cornerEval(BitBoard position)
        {
            const Int32 CORNER_VALUE = 12;
            const Int32 KILLER1_VALUE = -6;
            const Int32 KILLER2_VALUE = -3;

            UInt64 CORNER = 0x8100000000000081;
            UInt64 killer1 = 0x0042000000004200;
            UInt64 killer2 = 0x42c300000000c342;

            UInt64 occupiedCorners = (position.m_black | position.m_white) & CORNER;
            killer1 = killer1 & ~(occupiedCorners << 9) & ~(occupiedCorners << 7) & ~(occupiedCorners >> 9) & ~(occupiedCorners >> 7);
            killer2 = killer2 & ~(occupiedCorners << 8) & ~(occupiedCorners >> 8) & ~(occupiedCorners << 1) & ~(occupiedCorners >> 1);

            Int32 eval = (BitBoard.countSetBits(position.m_black & CORNER) - BitBoard.countSetBits(position.m_white & CORNER)) * CORNER_VALUE;
            eval += (BitBoard.countSetBits(position.m_black & killer1) - BitBoard.countSetBits(position.m_white & killer1)) * KILLER1_VALUE;
            eval += (BitBoard.countSetBits(position.m_black & killer2) - BitBoard.countSetBits(position.m_white & killer2)) * KILLER2_VALUE;
            return eval;
        }

        public static Int32 stabilityEval(BitBoard position)
        {
            UInt64 black = position.m_black;
            UInt64 white = position.m_white;
            UInt64 stable = 0;
            //determine tactically stable disks along full edges
            if ((0x00000000000000ff & (black | white)) == 0x00000000000000ff)
            {
                stable |= 0xff;
            }
            if ((0xff00000000000000 & (black | white)) == 0xff00000000000000)
            {
                stable |= 0xff00000000000000;
            }
            if ((0x8080808080808080 & (black | white)) == 0x8080808080808080)
            {
                stable |= 0x8080808080808080;
            }
            if ((0x0101010101010101 & (black | white)) == 0x0101010101010101)
            {
                stable |= 0x0101010101010101;
            }

            bool stabilityChange = true;
            while (stabilityChange)
            {
                UInt64 potStableVert = stable;
                UInt64 potStableHori = stable;
                UInt64 potStableDiag = stable;
                UInt64 potStableAnti = stable;

                //vertical
                UInt64 stableBlackUp = (((stable & black) << 8) | 0xff);
                UInt64 stableBlackDown = (((stable & black) >> 8) | 0xff00000000000000);
                UInt64 stableWhiteUp = (((stable & white) << 8) | 0xff);
                UInt64 stableWhiteDown = (((stable & white) >> 8) | 0xff00000000000000);
                // black will have a stable disk if surrounded by two stable white disks or one stable black disk
                potStableVert |= black & ((stableBlackUp | stableBlackDown) | (stableWhiteDown & stableWhiteUp));
                // white will have a stable disk if surrounded by two stable black disks or one stable white disk
                potStableVert |= white & ((stableBlackUp & stableBlackDown) | (stableWhiteDown | stableWhiteUp));

                //horizontal
                UInt64 stableBlackRight = (((stable & black) >> 1) | 0x8080808080808080);
                UInt64 stableBlackLeft = (((stable & black) << 1) | 0x0101010101010101);
                UInt64 stableWhiteRight = (((stable & white) >> 1) | 0x8080808080808080);
                UInt64 stableWhiteLeft = (((stable & white) << 1) | 0x0101010101010101);
                // black will have a stable disk if surrounded by two stable white disks or one stable black disk
                potStableHori |= black & ((stableBlackRight | stableBlackLeft) | (stableWhiteRight & stableWhiteLeft));
                // white will have a stable disk if surrounded by two stable black disks or one stable white disk
                potStableHori |= white & ((stableBlackRight & stableBlackLeft) | (stableWhiteRight | stableWhiteLeft));

                //Diagonal
                UInt64 stableBlackUpLeft = (((stable & black) << 9) | 0x01010101010101ff);
                UInt64 stableBlackDownRight = (((stable & black) >> 9) | 0xff80808080808080);
                UInt64 stableWhiteUpLeft = (((stable & white) << 9) | 0x01010101010101ff);
                UInt64 stableWhiteDownRight = (((stable & white) >> 9) | 0xff80808080808080);
                // black will have a stable disk if surrounded by two stable white disks or one stable black disk
                potStableDiag |= black & ((stableBlackUpLeft | stableBlackDownRight) | (stableWhiteUpLeft & stableWhiteDownRight));
                // white will have a stable disk if surrounded by two stable black disks or one stable white disk
                potStableDiag |= white & ((stableBlackUpLeft & stableBlackDownRight) | (stableWhiteUpLeft | stableWhiteDownRight));

                //Anti-Diagonal
                UInt64 stableBlackUpRight = (((stable & black) << 7) | 0x80808080808080ff);
                UInt64 stableBlackDownLeft = (((stable & black) >> 7) | 0xff01010101010101);
                UInt64 stableWhiteUpRight = (((stable & white) << 7) | 0x80808080808080ff);
                UInt64 stableWhiteDownLeft = (((stable & white) >> 7) | 0xff01010101010101);
                // black will have a stable disk if surrounded by two stable white disks or one stable black disk
                potStableAnti |= black & ((stableBlackUpRight | stableBlackDownLeft) | (stableWhiteUpRight & stableWhiteDownLeft));
                // white will have a stable disk if surrounded by two stable black disks or one stable white disk
                potStableAnti |= white & ((stableBlackUpRight & stableBlackDownLeft) | (stableWhiteUpRight | stableWhiteDownLeft));

                UInt64 test = potStableVert & potStableHori & potStableDiag & potStableAnti;
                stabilityChange = test != stable;
                stable = test;
            }
            return (BitBoard.countSetBits(stable & black) - BitBoard.countSetBits(stable & white)) * 2;
        }
        private static Int32 AlphaBetaPruning(BitBoard i_board, ref UInt64 o_chosenMove, Int32 i_startDepth, Int32 i_currDepth, Int32 i_depthLeft, Int32 i_a, Int32 i_b)
        {
            if (m_token.IsCancellationRequested)
            {
                return 0;
            }
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
