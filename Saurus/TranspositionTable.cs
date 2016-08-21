using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saurus
{
    class TranspositionTable
    {
        int m_capacity;
        int m_count;
        PositionMetadata[] m_positions;

        public TranspositionTable(int i_capacity)
        {
            m_positions = new PositionMetadata[i_capacity];
        }
        private Int32 hash(BitBoard i_position)
        {
            return i_position.GetHashCode();
        }
        private Int32 hash(PositionMetadata i_position)
        {
            return i_position.GetHashCode();
        }
        public Boolean TryGetValue(BitBoard i_position, out PositionMetadata o_metadata)
        {
            o_metadata = m_positions[hash(i_position)];
            if (o_metadata == null || !i_position.Equals(o_metadata.m_board))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public void add(PositionMetadata i_position)
        {
            #region assume there is never a reason to keep the older value
            /* 
            PositionMetadata occupier = m_positions[hash(i_position)];
            if (occupier == null) //nothing there might as well fill it
            {
                m_positions[hash(i_position)] = i_position;
            }
            else if (occupier.Equals(i_position)) //position is a less up to date version of itself replace it
            {
                m_positions[hash(i_position)] = i_position;
            }
            else if (occupier.m_currentDepth < i_position.m_startDepth) //position is in the past thus useless replace it
            {
                m_positions[hash(i_position)] = i_position;
            }
            else if ()
            {

            }
            */
            #endregion
            m_positions[hash(i_position)] = i_position;
        }

        
    }
}
