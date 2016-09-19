using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Saurus
{
    enum GameState
    {
        whiteTurn,
        blackTurn,
        gameOver
    }
    public partial class SaurusForm : Form
    {
        IPlayer m_blackPlayer;
        IPlayer m_whitePlayer;
        BoardHistory m_board;
        GameState currentState = GameState.blackTurn;
        public SaurusForm()
        {
            InitializeComponent();
            m_board = new BoardHistory();
            m_blackPlayer = new Human(m_board);
            m_whitePlayer = new Human(m_board);
            currentState = GameState.blackTurn;
        }

        private void boardPbx_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == MouseButtons.Left)
            {
                Point clickLocation = me.Location;
                if (currentState == GameState.blackTurn)
                {
                    Int32 x = (clickLocation.X * 8) / boardPbx.Width;
                    Int32 y = (clickLocation.Y * 8) / boardPbx.Height;
                    UInt64 move = (UInt64)1 << ((y << 3) | x);
                }
                else if (currentState == GameState.whiteTurn)
                {

                }
            }
            else if (me.Button == MouseButtons.Right)
            {

            }
            
        }
    }
}
