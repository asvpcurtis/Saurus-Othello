using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaurusConsole.OthelloAI;
namespace SaurusConsoleTests
{
    [TestClass]
    public class MoveTests
    {
        [TestMethod]
        public void NotationConstuctorTest()
        {
            Move e4move = new Move("E4");
            Assert.AreEqual("E4", e4move.ToString());

            Move a1move = new Move("A1");
            Assert.AreEqual("A1", a1move.ToString());

            Move a8move = new Move("A8");
            Assert.AreEqual("A8", a8move.ToString());

            Move h1move = new Move("H1");
            Assert.AreEqual("H1", h1move.ToString());

            Move h8move = new Move("H8");
            Assert.AreEqual("H8", h8move.ToString());
        }

        [TestMethod]
        public void ToStringTest()
        {
            ulong h1Mask = 1;
            Move h1Move = new Move(h1Mask);
            Assert.AreEqual("H1", h1Move.ToString());

            ulong e4Mask = 0x8000000;
            Move e4Move = new Move(e4Mask);
            Assert.AreEqual("E4", e4Move.ToString());
        }

        [TestMethod]
        public void GetBitMaskTest()
        {
            ulong mask = 0b1000000000000;
            Move move = new Move(mask);
            Assert.AreEqual(mask, move.GetBitMask());
        }

        [TestMethod]
        public void EqualsTest()
        {
            Move move = new Move("E4");
            Move same = new Move("E4");
            Assert.IsTrue(move.Equals(same));
            Assert.IsTrue(move == same);
            Assert.IsFalse(move != same);

            Move different = new Move("D3");
            Assert.IsFalse(move.Equals(different));
            Assert.IsFalse(move == different);
            Assert.IsTrue(move != different);
        }

        [TestMethod]
        public void GetHashCodeTest()
        {
            Move move = new Move("E4");
            Move same = new Move("E4");
            Assert.IsTrue(move.GetHashCode() == same.GetHashCode());
        }
    }
}
