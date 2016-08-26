using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saurus
{
    class Othello<B,W> where B : IPlayer where W : IPlayer
    {
        B blackPlayer;
        W whitePlayer;
    }
}   
