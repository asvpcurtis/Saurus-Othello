using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Saurus
{
    class Human : IPlayer
    {
        UInt64 m_move;
        BoardHistory m_board;
        public Task m_think;
        private static CancellationTokenSource m_tokenSource;
        public static CancellationToken m_token;
        public Human(BoardHistory i_board)
        {
            m_move = 0;
            m_board = i_board;
        }

        public void moveAsync(BoardHistory i_board)
        {
            m_think = Task.Run(() =>
            {
                Boolean canMove;
                BitBoard currentState = i_board.getBoard();
                while (m_move == 0 || !m_board.addMove(m_move, out canMove))
                {
                    m_move = 0;
                    if (m_token.IsCancellationRequested)
                    {
                        return;
                    }
                }
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
            throw new NotImplementedException();
        }

        public void Report(int value)
        {
            throw new NotImplementedException();
        }
    }
}
