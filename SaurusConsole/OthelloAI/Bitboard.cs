using System;
using System.Collections.Generic;
using System.Text;

namespace SaurusConsole.OthelloAI
{
    public static class Bitboard
    {
        public static bool GetBit(ulong board, int i)
        {
            return ((board >> i) & 1) == 1;
        }

        public static ulong SetBit(ulong board, int i)
        {
            return board | (ulong)1 << i;
        }

        public static ulong ClearBit(ulong board, int i)
        {
            return board & ~((ulong)1 << i);
        }

        public static int CountSetBits(ulong board)
        {
            int count = 0;
            while (board != 0)
            {
                count++;
                board &= board - 1;
            }
            return count;
        }
    }
}
