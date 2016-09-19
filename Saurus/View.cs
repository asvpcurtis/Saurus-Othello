using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saurus
{
    public class View
    {
        Boolean m_moveListEnabled;
        BoardHistory m_board;
        public View(BoardHistory i_board)
        {
            m_moveListEnabled = true;
            m_board = i_board;
        }
    }
}
