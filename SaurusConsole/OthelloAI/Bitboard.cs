using System;
using System.Collections.Generic;
using System.Text;

namespace SaurusConsole.OthelloAI
{
    public static class Bitboard
    {
        /// <summary>
        /// Determines if a bit in the bitboard is set
        /// </summary>
        /// <param name="board">The board to check against</param>
        /// <param name="i">the index of the bit to check starting from least significant bit</param>
        /// <returns>true if the bit is a 1 else 0</returns>
        public static bool GetBit(ulong board, int i)
        {
            return ((board >> i) & 1) == 1;
        }

        /// <summary>
        /// Sets a bit in a bitboard to 1
        /// </summary>
        /// <param name="board">The board to modify</param>
        /// <param name="i">The index of the bit to modify starting from least significant bit</param>
        /// <returns>A modified version of board with the ith bit set</returns>
        public static ulong SetBit(ulong board, int i)
        {
            return board | (ulong)1 << i;
        }

        /// <summary>
        /// Clears a bit in a bitboard to 0
        /// </summary>
        /// <param name="board">The board to modify</param>
        /// <param name="i">The index of the bit to modify starting from least significant bit</param>
        /// <returns>A modified version of board with the ith bit cleared</returns>
        public static ulong ClearBit(ulong board, int i)
        {
            return board & ~((ulong)1 << i);
        }

        /// <summary>
        /// Counts the number of bits in a bitboard set to 1
        /// </summary>
        /// <param name="board">The board to count bits on</param>
        /// <returns>The number of bits that are 1 in the board</returns>
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
