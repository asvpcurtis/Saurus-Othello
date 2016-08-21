using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saurus
{
    public class PositionMetadata : IComparable<PositionMetadata>, IEquatable<PositionMetadata>
    {
        public readonly BitBoard m_board;
        public readonly Int32 m_eval;

        public readonly Int32 m_startDepth; // the depth when the AI started calculating at
        public readonly Int32 m_currentDepth; // the depth this move is at
        public readonly Int32 m_endDepth; // the depth calculated to to get this moves evaluation

        public readonly UInt64 m_stableDisks;
        public readonly Boolean m_gameOver;
        //consider a moves cut to help determine it's value to transposition table
        //consider a likelyhood to occur to determine it's value to transpostion table
        public PositionMetadata(BitBoard i_board, Int32 i_eval, Int32 i_depth, Int32 i_stableDisks)
        {
            m_board = i_board;
        }

        public int CompareTo(PositionMetadata i_other)
        {
            return m_eval - i_other.m_eval;
        }

        public bool Equals(PositionMetadata i_other)
        {
            return m_board.Equals(i_other.m_board);
        }
        public override Int32 GetHashCode()
        {
            return m_board.GetHashCode();
        }
}
