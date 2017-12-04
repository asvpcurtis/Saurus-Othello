using System;
using System.Collections.Generic;
using System.Text;

namespace SaurusConsole.OthelloAI
{
    /// <summary>
    /// Represents an Othello Position
    /// </summary>
    public class Position
    {
        private const ulong R_SHIFT_MASK = 0x7f7f7f7f7f7f7f7f;
        private const ulong L_SHIFT_MASK = 0xfefefefefefefefe;
        bool blackTurn;
        bool gameOver;
        ulong white;
        ulong black;


        public Position(ulong white, ulong black, bool blackTurn, bool gameOver)
        {
            this.white = white;
            this.black = black;
            this.blackTurn = blackTurn;
            this.gameOver = gameOver;
        }
        public Position(string fen)
        {
            if (fen == "startpos")
            {
                white = 0x0000000810000000;
                black = 0x0000001008000000;
                blackTurn = true;
                return;
            }

            if (fen.Length != 66 && fen[64] == '-')
            {
                throw new Exception();
            }
            string pos = fen.Substring(0, 64);
            switch (fen[65])
            {
                case 'b':
                    blackTurn = true;
                    break;
                case 'w':
                    blackTurn = false;
                    break;
                case '_':
                    gameOver = true;
                    break;
                default:
                    break;
            }
            foreach (char c in pos)
            {
                white <<= 1;
                black <<= 1;
                switch (c)
                {
                    case 'b':
                        black += 1;
                        break;
                    case 'w':
                        white += 1;
                        break;
                    case '_':
                        break;
                    default:
                        throw new Exception();
                }
            }
            if (GetLegalMovesMask() == 0)
            {
                blackTurn = !blackTurn;
                if (GetLegalMovesMask() == 0)
                {
                    blackTurn = !blackTurn;
                    gameOver = true;
                }
            }
        }
        private ulong GetLegalMovesMask()
        {
            ulong opponent;
            ulong player;
            if (blackTurn)
            {
                player = black;
                opponent = white;
            }
            else
            {
                player = white;
                opponent = black;
            }
            ulong notOccupied = ~(player | opponent);
            ulong legalMoves = 0x0;
            //slide right
            ulong slider = (player >> 1) & opponent & R_SHIFT_MASK;
            while (slider != 0)
            {
                ulong temp = (slider >> 1) & R_SHIFT_MASK;
                legalMoves |= temp & notOccupied;
                slider = temp & opponent;
            }

            //slide left
            slider = (player << 1) & opponent & L_SHIFT_MASK;
            while (slider != 0)
            {
                ulong temp = (slider << 1) & L_SHIFT_MASK;
                legalMoves |= temp & notOccupied;
                slider = temp & opponent;
            }

            //slide up
            slider = (player << 8) & opponent;
            while (slider != 0)
            {
                ulong temp = (slider << 8);
                legalMoves |= temp & notOccupied;
                slider = temp & opponent;
            }

            //slide down
            slider = (player >> 8) & opponent;
            while (slider != 0)
            {
                ulong temp = (slider >> 8);
                legalMoves |= temp & notOccupied;
                slider = temp & opponent;
            }

            //slide up-right
            slider = (player << 7) & opponent & R_SHIFT_MASK;
            while (slider != 0)
            {
                ulong temp = (slider << 7) & R_SHIFT_MASK;
                legalMoves |= temp & notOccupied;
                slider = temp & opponent;
            }

            //slide down-left
            slider = (player >> 7) & opponent & L_SHIFT_MASK;
            while (slider != 0)
            {
                ulong temp = (slider >> 7) & L_SHIFT_MASK;
                legalMoves |= temp & notOccupied;
                slider = temp & opponent;
            }

            //slide up-left
            slider = (player << 9) & opponent & L_SHIFT_MASK;
            while (slider != 0)
            {
                ulong temp = (slider << 9) & L_SHIFT_MASK;
                legalMoves |= temp & notOccupied;
                slider = temp & opponent;
            }

            //slide down-right
            slider = (player >> 9) & opponent & R_SHIFT_MASK;
            while (slider != 0)
            {
                ulong temp = (slider >> 9) & R_SHIFT_MASK;
                legalMoves |= temp & notOccupied;
                slider = temp & opponent;
            }

            return legalMoves;
        }

        public IEnumerable<Move> GetLegalMoves()
        {
            ulong LegalMovesMask = GetLegalMovesMask();
            List<Move> legalMoves = new List<Move>();
            while (LegalMovesMask != 0)
            {
                ulong copy = LegalMovesMask;
                LegalMovesMask = LegalMovesMask & (LegalMovesMask - 1); // clears least significant bit
                ulong legalMove = copy ^ LegalMovesMask; // stores least significant bit by itself
                legalMoves.Add(new Move(legalMove));
            }
            return legalMoves;
        }

        public Position MakeMove(Move move)
        {
            ulong moveMask = move.GetBitMask();
            ulong slider = moveMask;
            //retrieve information
            ulong opponent;
            ulong player;
            if (blackTurn)
            {
                player = black;
                opponent = white;
            }
            else
            {
                player = white;
                opponent = black;
            }
            ulong notOccupied = ~(player | opponent);
            ulong flippedDisks = 0;
            ulong potential = 0;
            ulong valid = 0; //used to detect if we stopped at player rather than an empty square
            //left
            slider = (moveMask << 1) & opponent & L_SHIFT_MASK;
            while (slider != 0)
            {
                potential |= slider;
                valid = (slider << 1) & player & L_SHIFT_MASK;
                slider = (slider << 1) & opponent & L_SHIFT_MASK;
            }
            if (valid != 0) { flippedDisks |= potential; }
            else { potential = 0; }

            //right
            slider = (moveMask >> 1) & opponent & R_SHIFT_MASK;
            while (slider != 0)
            {
                potential |= slider;
                valid = (slider >> 1) & player & R_SHIFT_MASK;
                slider = (slider >> 1) & opponent & R_SHIFT_MASK;
            }
            if (valid != 0) { flippedDisks |= potential; }
            else { potential = 0; }

            //up
            slider = (moveMask << 8) & opponent;
            while (slider != 0)
            {
                potential |= slider;
                valid = (slider << 8) & player;
                slider = (slider << 8) & opponent;
            }
            if (valid != 0) { flippedDisks |= potential; }
            else { potential = 0; }

            //down
            slider = (moveMask >> 8) & opponent;
            while (slider != 0)
            {
                potential |= slider;
                valid = (slider >> 8) & player;
                slider = (slider >> 8) & opponent;
            }
            if (valid != 0) { flippedDisks |= potential; }
            else { potential = 0; }

            //up-left
            slider = (moveMask << 9) & opponent & L_SHIFT_MASK;
            while (slider != 0)
            {
                potential |= slider;
                valid = (slider << 9) & player & L_SHIFT_MASK;
                slider = (slider << 9) & opponent & L_SHIFT_MASK;
            }
            if (valid != 0) { flippedDisks |= potential; }
            else { potential = 0; }

            //down-right
            slider = (moveMask >> 9) & opponent & R_SHIFT_MASK;
            while (slider != 0)
            {
                potential |= slider;
                valid = (slider >> 9) & player & R_SHIFT_MASK;
                slider = (slider >> 9) & opponent & R_SHIFT_MASK;
            }
            if (valid != 0) { flippedDisks |= potential; }
            else { potential = 0; }

            //up-right
            slider = (moveMask << 7) & opponent & R_SHIFT_MASK;
            while (slider != 0)
            {
                potential |= slider;
                valid = (slider << 7) & player & R_SHIFT_MASK;
                slider = (slider << 7) & opponent & R_SHIFT_MASK;
            }
            if (valid != 0) { flippedDisks |= potential; }
            else { potential = 0; }

            //down-left
            slider = (moveMask >> 7) & opponent & L_SHIFT_MASK;
            while (slider != 0)
            {
                potential |= slider;
                valid = (slider >> 7) & player & L_SHIFT_MASK;
                slider = (slider >> 7) & opponent & L_SHIFT_MASK;
            }
            if (valid != 0) { flippedDisks |= potential; }
            else { potential = 0; }
            //----------------------------------------------
            ulong newBlack;
            ulong newWhite;
            if (blackTurn)
            {
                newBlack = black | flippedDisks | moveMask;
                newWhite = white & ~flippedDisks;
            }
            else
            {
                newWhite = white | flippedDisks | moveMask;
                newBlack = black & ~flippedDisks;
            }

            Position pos = new Position(newWhite, newBlack, !blackTurn, false);
            if (pos.GetLegalMovesMask() == 0)
            {
                Position swapTurns = new Position(newWhite, newBlack, blackTurn, false);
                if (swapTurns.GetLegalMovesMask() == 0)
                {
                    return new Position(newWhite, newBlack, !blackTurn, true);
                }
                else
                {
                    return swapTurns;
                }
            }
            else
            {
                return pos;
            }
        }
        public int TotalWhiteDisks()
        {
            return Bitboard.CountSetBits(white);
        }
        public int TotalBlackDisks()
        {
            return Bitboard.CountSetBits(black);
        }
        public ulong GetWhiteBitMask()
        {
            return white;
        }
        public ulong GetBlackBitMask()
        {
            return black;
        }
        public int LegalMoveCount()
        {
            return Bitboard.CountSetBits(GetLegalMovesMask());
        }
        public bool GameOver()
        {
            return gameOver;
        }

        public bool BlackTurn()
        {
            return blackTurn;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null) || !(obj is Position))
            {
                return false;
            }

            Position pos = (Position)obj;
            return pos.blackTurn == blackTurn && pos.black == black && pos.white == white;
        }

        public override int GetHashCode()
        {
            if (blackTurn)
            {
                return black.GetHashCode() ^ white.GetHashCode();
            }
            else
            {
                return ~(black.GetHashCode() ^ white.GetHashCode());
            }
        }

        public static bool operator ==(Position left, Position right)
        {
            if (!ReferenceEquals(left, null) && !ReferenceEquals(right, null))
            {
                return left.Equals(right);
            }
            return ReferenceEquals(left, null) && ReferenceEquals(right, null);
        }
        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }
    }
}
