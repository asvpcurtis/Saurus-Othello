using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaurusConsole.OthelloAI;
namespace SaurusConsoleTests
{
    [TestClass]
    public class MoveTests
    {
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
            ulong mask = 0b0000000000000001;
            Move move = new Move(mask);
            Assert.AreEqual(mask, move.GetBitMask());
        }
    }
}
