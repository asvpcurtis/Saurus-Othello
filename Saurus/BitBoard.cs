using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saurus
{
    public struct BitBoard
    {
        private const UInt64 R_SHIFT_MASK = 0x7f7f7f7f7f7f7f7f;
        private const UInt64 L_SHIFT_MASK = 0xfefefefefefefefe;
        readonly public UInt64 m_white;
        readonly public UInt64 m_black;
        readonly public Boolean m_blackTurn;
        public BitBoard(Boolean i_a)
        {
            m_blackTurn = true;
            m_white = 0x0000001008000000;
            m_black = 0x0000000810000000;
        }
        public BitBoard(BitBoard i_original)
        {
            m_blackTurn = i_original.m_blackTurn;
            m_black = i_original.m_black;
            m_white = i_original.m_white;
        }
        public BitBoard(UInt64 i_black, UInt64 i_white, Boolean i_blackTurn)
        {
            this.m_black = i_black;
            this.m_white = i_white;
            this.m_blackTurn = i_blackTurn;
        }

        public static BitBoard newGame()
        {
            return new BitBoard(0x0000000810000000, 0x0000001008000000, true);
        }

        public override Int32 GetHashCode()
        {
            ulong whiteKey = m_white;
            ulong blackKey = m_black;

            whiteKey = (~whiteKey) + (whiteKey << 21); // key = (key << 21) - key - 1;
            whiteKey = whiteKey ^ (whiteKey >> 24);
            whiteKey = (whiteKey + (whiteKey << 3)) + (whiteKey << 8); // key * 265
            whiteKey = whiteKey ^ (whiteKey >> 14);
            whiteKey = (whiteKey + (whiteKey << 2)) + (whiteKey << 4); // key * 21
            whiteKey = whiteKey ^ (whiteKey >> 28);
            whiteKey = whiteKey + (whiteKey << 31);

            blackKey = (~blackKey) + (blackKey << 21); // key = (key << 21) - key - 1;
            blackKey = blackKey ^ (blackKey >> 24);
            blackKey = (blackKey + (blackKey << 3)) + (blackKey << 8); // key * 265
            blackKey = blackKey ^ (blackKey >> 14);
            blackKey = (blackKey + (blackKey << 2)) + (blackKey << 4); // key * 21
            blackKey = blackKey ^ (blackKey >> 28);
            blackKey = blackKey + (blackKey << 31);

            if (m_blackTurn)
            {
                return (int)~(whiteKey ^ blackKey);
            }
            return (int)(whiteKey ^ blackKey);
        }

        public override Boolean Equals(object obj)
        {
            if (obj == null | !(obj is BitBoard)) return false;
            return (((BitBoard)obj).m_black == m_black) && (((BitBoard)obj).m_white == m_white) && (((BitBoard)obj).m_blackTurn == m_blackTurn);
        }
        public static UInt64 getMoves(BitBoard i_curPos)
        {
            //retrieve information
            UInt64 opponent;
            UInt64 player;
            if (i_curPos.m_blackTurn)
            {
                player = i_curPos.m_black;
                opponent = i_curPos.m_white;
            }
            else
            {
                player = i_curPos.m_white;
                opponent = i_curPos.m_black;
            }
            UInt64 notOccupied = ~(player | opponent);
            UInt64 legalMoves = 0;

            //slide right
            UInt64 slider = (player >> 1) & opponent & R_SHIFT_MASK;
            while (slider != 0)
            {
                UInt64 temp = (slider >> 1) & R_SHIFT_MASK;
                legalMoves |= temp & notOccupied;
                slider = temp & opponent;
            }

            //slide left
            slider = (player << 1) & opponent & L_SHIFT_MASK;
            while (slider != 0)
            {
                UInt64 temp = (slider << 1) & L_SHIFT_MASK;
                legalMoves |= temp & notOccupied;
                slider = temp & opponent;
            }

            //slide up
            slider = (player << 8) & opponent;
            while (slider != 0)
            {
                UInt64 temp = (slider << 8);
                legalMoves |= temp & notOccupied;
                slider = temp & opponent;
            }

            //slide down
            slider = (player >> 8) & opponent;
            while (slider != 0)
            {
                UInt64 temp = (slider >> 8);
                legalMoves |= temp & notOccupied;
                slider = temp & opponent;
            }

            //slide up-right
            slider = (player << 7) & opponent & R_SHIFT_MASK;
            while (slider != 0)
            {
                UInt64 temp = (slider << 7) & R_SHIFT_MASK;
                legalMoves |= temp & notOccupied;
                slider = temp & opponent;
            }

            //slide down-left
            slider = (player >> 7) & opponent & L_SHIFT_MASK;
            while (slider != 0)
            {
                UInt64 temp = (slider >> 7) & L_SHIFT_MASK;
                legalMoves |= temp & notOccupied;
                slider = temp & opponent;
            }

            //slide up-left
            slider = (player << 9) & opponent & L_SHIFT_MASK;
            while (slider != 0)
            {
                UInt64 temp = (slider << 9) & L_SHIFT_MASK;
                legalMoves |= temp & notOccupied;
                slider = temp & opponent;
            }

            //slide down-right
            slider = (player >> 9) & opponent & R_SHIFT_MASK;
            while (slider != 0)
            {
                UInt64 temp = (slider >> 9) & R_SHIFT_MASK;
                legalMoves |= temp & notOccupied;
                slider = temp & opponent;
            }

            return legalMoves;
        }
        public static BitBoard getSuccessor(BitBoard i_curPos, UInt64 i_moveMask)
        {
            UInt64 slider = i_moveMask;
            //retrieve information
            UInt64 opponent;
            UInt64 player;
            if (i_curPos.m_blackTurn)
            {
                player = i_curPos.m_black;
                opponent = i_curPos.m_white;
            }
            else
            {
                player = i_curPos.m_white;
                opponent = i_curPos.m_black;
            }
            UInt64 notOccupied = ~(player | opponent);
            UInt64 flippedDisks = 0;
            UInt64 potential = 0;
            UInt64 valid = 0; //used to detect if we stopped at player rather than an empty square

            //left
            slider = (i_moveMask << 1) & opponent & L_SHIFT_MASK;
            while (slider != 0)
            {
                potential |= slider;
                valid = (slider << 1) & player & L_SHIFT_MASK;
                slider = (slider << 1) & opponent & L_SHIFT_MASK;
            }
            if (valid != 0) { flippedDisks |= potential; }
            else { potential = 0; }

            //right
            slider = (i_moveMask >> 1) & opponent & R_SHIFT_MASK;
            while (slider != 0)
            {
                potential |= slider;
                valid = (slider >> 1) & player & R_SHIFT_MASK;
                slider = (slider >> 1) & opponent & R_SHIFT_MASK;
            }
            if (valid != 0) { flippedDisks |= potential; }
            else { potential = 0; }

            //up
            slider = (i_moveMask << 8) & opponent;
            while (slider != 0)
            {
                potential |= slider;
                valid = (slider << 8) & player;
                slider = (slider << 8) & opponent;
            }
            if (valid != 0) { flippedDisks |= potential; }
            else { potential = 0; }

            //down
            slider = (i_moveMask >> 8) & opponent;
            while (slider != 0)
            {
                potential |= slider;
                valid = (slider >> 8) & player;
                slider = (slider >> 8) & opponent;
            }
            if (valid != 0) { flippedDisks |= potential; }
            else { potential = 0; }

            //up-left
            slider = (i_moveMask << 9) & opponent & L_SHIFT_MASK;
            while (slider != 0)
            {
                potential |= slider;
                valid = (slider << 9) & player & L_SHIFT_MASK;
                slider = (slider << 9) & opponent & L_SHIFT_MASK;
            }
            if (valid != 0) { flippedDisks |= potential; }
            else { potential = 0; }

            //down-right
            slider = (i_moveMask >> 9) & opponent & R_SHIFT_MASK;
            while (slider != 0)
            {
                potential |= slider;
                valid = (slider >> 9) & player & R_SHIFT_MASK;
                slider = (slider >> 9) & opponent & R_SHIFT_MASK;
            }
            if (valid != 0) { flippedDisks |= potential; }
            else { potential = 0; }

            //up-right
            slider = (i_moveMask << 7) & opponent & R_SHIFT_MASK;
            while (slider != 0)
            {
                potential |= slider;
                valid = (slider << 7) & player & R_SHIFT_MASK;
                slider = (slider << 7) & opponent & R_SHIFT_MASK;
            }
            if (valid != 0) { flippedDisks |= potential; }
            else { potential = 0; }

            //down-left
            slider = (i_moveMask >> 7) & opponent & L_SHIFT_MASK;
            while (slider != 0)
            {
                potential |= slider;
                valid = (slider >> 7) & player & L_SHIFT_MASK;
                slider = (slider >> 7) & opponent & L_SHIFT_MASK;
            }
            if (valid != 0) { flippedDisks |= potential; }
            else { potential = 0; }
            //----------------------------------------------

            UInt64 testBlack = i_curPos.m_black;
            UInt64 testWhite = i_curPos.m_white;
            if (i_curPos.m_blackTurn)
            {
                testBlack |= flippedDisks | i_moveMask;
                testWhite &= ~flippedDisks;
            }
            else
            {
                testBlack |= flippedDisks | i_moveMask;
                testBlack &= ~flippedDisks;
            }
            return new BitBoard(testBlack, testWhite, !i_curPos.m_blackTurn);
        }
        public static BitBoard switchTurns(BitBoard i_curPos)
        {
            return new BitBoard(i_curPos.m_black, i_curPos.m_white, !i_curPos.m_blackTurn);
        }
        public static Boolean canMove(BitBoard i_curPos)
        {
            return BitBoard.getMoves(i_curPos) != 0;
        }
        public static Boolean gameOver(BitBoard i_curPos)
        {
            if (BitBoard.getMoves(i_curPos) != 0)
                return false;

            if (BitBoard.getMoves(BitBoard.switchTurns(i_curPos)) != 0)
                return false;
            return true;

        }
    }
}
