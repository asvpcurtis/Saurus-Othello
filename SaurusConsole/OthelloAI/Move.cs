using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace SaurusConsole.OthelloAI
{
    /// <summary>
    /// Represents an Othello Move
    /// </summary>
    public class Move
    {
        ulong move;

        /// <summary>
        /// Initializes an instance 
        /// </summary>
        /// <param name="move"></param>
        public Move(ulong move)
        {
            // Input not validated to avoid a performance hit
            //if ((move == 0) || ((move & (move - 1)) != 0))
            //{
            //    throw new ArgumentException();
            //}
            this.move = move;
            move = 0;
        }

        /// <summary>
        /// Gets the move in co-ordinate notation Example: A5
        /// </summary>
        /// <returns>The move</returns>
        override public string ToString()
        {
            int x;
            int y;
            ulong rowMask = 0xff;
            ulong colMask = 0x8080808080808080;
            for (y = 0; y < 8; y++)
            {
                if ((move & rowMask) != 0)
                {
                    break;
                }
                rowMask <<= 8;
            }
            for (x = 0; x < 8; x++)
            {
                if ((move & colMask) != 0)
                {
                    break;
                }
                colMask >>= 1;
            }
            string col = ((char)(x + 65)).ToString();
            int row = y + 1;
            return $"{col}{row}";
        }

        /// <summary>
        /// Gets the bitmask that represents the move on the othello board
        /// </summary>
        /// <returns>A ulong with 1 set bit denoting the square on the board</returns>
        public ulong GetBitMask()
        {
            return move;
        }
    }
}
