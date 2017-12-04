using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaurusConsole.OthelloAI;
namespace SaurusConsoleTests
{
    [TestClass]
    public class BitboardTests
    {
        [TestMethod]
        public void GetBitTest()
        {
            ulong mask = 8;
            Assert.IsTrue(Bitboard.GetBit(mask, 3));
            Assert.IsFalse(Bitboard.GetBit(mask, 4));
            Assert.IsFalse(Bitboard.GetBit(mask, 2));
        }
        [TestMethod]
        public void SetBitTest()
        {
            ulong mask = 0;
            mask = Bitboard.SetBit(mask, 3);
            Assert.AreEqual((ulong)8, mask);
            mask = Bitboard.SetBit(mask, 3);
            Assert.AreEqual((ulong)8, mask);
        }
        [TestMethod]
        public void ClearBitTest()
        {
            ulong mask = 8;
            mask = Bitboard.ClearBit(mask, 3);
            Assert.AreEqual((ulong)0, mask);
            mask = Bitboard.ClearBit(mask, 3);
            Assert.AreEqual((ulong)0, mask);
        }
        [TestMethod]
        public void CountSetBitsTest()
        {
            ulong deadbeef = 0xdeadbeef;
            int count = Bitboard.CountSetBits(deadbeef);
            Assert.AreEqual(24, count);
        }
    }
}
