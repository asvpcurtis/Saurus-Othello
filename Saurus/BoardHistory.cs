using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saurus
{
    public class BoardHistory
    {
        private int m_numMoves;
        private int m_moveIndex;
        private BitBoard[] m_history;

        public BoardHistory()
        {
            m_history = new BitBoard[61]; // 60 moves are possible we also want to store the starting position as well making 61
            m_history[0] = BitBoard.newGame();
            m_numMoves = 0;
            m_moveIndex = 0;
        }

        public Boolean addMove(UInt64 i_move, out Boolean o_canMove)
        {
            o_canMove = true;
            //check for legality
            if ((i_move & BitBoard.getMoves(getBoard())) == 0)
            {
                return false;
            }
            BitBoard newBoard = BitBoard.getSuccessor(getBoard(), i_move);

            m_moveIndex++;
            if (!newBoard.Equals(m_history[m_moveIndex]))
            {
                m_numMoves = m_moveIndex;
            }
            m_history[m_moveIndex] = newBoard;
            if (!BitBoard.canMove(newBoard))
            {
                o_canMove = false;
            }
            return true;
        }

        public BitBoard getBoard()
        {
            return m_history[m_moveIndex];
        }
        public Int32 getMoveNumber()
        {
            return m_moveIndex;
        }
        public Boolean undo()
        {
            if (m_moveIndex > 0)
            {
                m_moveIndex--;
                return true;
            }
            return false;
        }

        public Boolean redo()
        {
            if (m_moveIndex < m_numMoves)
            {
                m_moveIndex++;
                return true;
            }
            return false;
        }

    }
}
