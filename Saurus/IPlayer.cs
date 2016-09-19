using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saurus
{
    public interface IPlayer : IProgress<Int32>
    {
        void moveAsync(BoardHistory i_board);
        void cancelAsync();
    }
}
