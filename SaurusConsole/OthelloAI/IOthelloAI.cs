using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SaurusConsole.OthelloAI
{
    /// <summary>
    /// Can play a game of Othello
    /// </summary>
    interface IOthelloAI
    {
        /// <summary>
        /// Sets the board state
        /// </summary>
        /// <param name="fen">The new position</param>
        void SetPosition(Position fen);

        /// <summary>
        /// Gets the current position of the engine
        /// </summary>
        /// <returns>The position the engine is analyzing</returns>
        Position GetPosition();

        /// <summary>
        /// Search the current position to a given depth
        /// </summary>
        /// <param name="depth">The max depth to search the position</param>
        /// <param name="token">The token used to cancel the search</param>
        /// <returns>The evaluation and the principle variation</returns>
        Task<(int eval, List<Move> pv)> GoDepth(int depth, CancellationToken token);

        /// <summary>
        /// Get information about the engine
        /// </summary>
        /// <returns>information about the engine</returns>
        string About();
        
    }
}
