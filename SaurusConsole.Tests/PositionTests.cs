using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaurusConsole.OthelloAI;
using System.Collections.Generic;
using System.Linq;

namespace SaurusConsoleTests
{
    [TestClass]
    public class PositionTests
    {
        [TestMethod]
        public void FenConstructorStartposTest()
        {
            Position pos = new Position("startpos");
            Assert.IsTrue(pos.BlackTurn());
            Assert.AreEqual((ulong)0x0000000810000000, pos.GetWhiteBitMask());
            Assert.AreEqual((ulong)0x0000001008000000, pos.GetBlackBitMask());
            Assert.AreEqual(false, pos.GameOver());
        }

        [TestMethod]
        public void FenConstructorNormalTest()
        {
            string fen = "___________________________bw______wb___________________________-b";
            Position pos = new Position(fen);
            Assert.IsTrue(pos.BlackTurn());
            Assert.AreEqual((ulong)0x0000000810000000, pos.GetWhiteBitMask());
            Assert.AreEqual((ulong)0x0000001008000000, pos.GetBlackBitMask());
            Assert.AreEqual(false, pos.GameOver());
        }


        [TestMethod]
        public void ConstructorTest()
        {
            ulong white = 0x0000000810000000;
            ulong black = 0x0000001008000000;
            bool blackTurn = true;
            bool gameOver = false;
            Position pos = new Position(white, black, blackTurn, gameOver);
            Assert.IsTrue(pos.BlackTurn());
            Assert.AreEqual(white, pos.GetWhiteBitMask());
            Assert.AreEqual(black, pos.GetBlackBitMask());
            Assert.AreEqual(gameOver, pos.GameOver());
        }

        [TestMethod]
        public void TotalDiskTest()
        {
            Position pos = new Position("startpos");
            Assert.AreEqual(2, pos.TotalBlackDisks());
            Assert.AreEqual(2, pos.TotalWhiteDisks());
        }

        [TestMethod]
        public void GetLegalMovesTest()
        {
            // TODO for better code coverage do more example positions
            ulong white = 0x00000010101c0000;
            ulong black = 0x000000080c000000;
            Position pos = new Position(white, black, true, false);
            IEnumerable<Move> moves = pos.GetLegalMoves();
            Assert.IsTrue(moves.Any(m => m.ToString() == "C2"));
            Assert.IsTrue(moves.Any(m => m.ToString() == "C3"));
            Assert.IsTrue(moves.Any(m => m.ToString() == "C4"));
            Assert.IsTrue(moves.Any(m => m.ToString() == "C5"));
            Assert.IsTrue(moves.Any(m => m.ToString() == "C6"));
            Assert.IsTrue(moves.Any(m => m.ToString() == "D2"));
            Assert.IsTrue(moves.Any(m => m.ToString() == "E2"));
            Assert.IsTrue(moves.Any(m => m.ToString() == "F2"));
            Assert.IsTrue(moves.Any(m => m.ToString() == "G2"));
            Assert.AreEqual(9, pos.LegalMoveCount());
        }

        [TestMethod]
        public void MakeMoveTest()
        {
            // TODO for better code coverage do more example positions
            Position pos = new Position("startpos");
            Position newPos = pos.MakeMove(new Move("D3"));
            Assert.AreEqual(false, newPos.BlackTurn());
            Assert.AreEqual((ulong)0x0000000800000000, newPos.GetWhiteBitMask());

            Assert.AreEqual((ulong)0x0000001018100000, newPos.GetBlackBitMask());

        }
    }
}
