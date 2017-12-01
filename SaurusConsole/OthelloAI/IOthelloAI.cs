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
        /// Sets the board state by fenstring
        /// </summary>
        /// <param name="fen">The fenstring to set the postion to</param>
        void SetPosition(string fen);

        /// <summary>
        /// Search the current position to a given depth
        /// </summary>
        /// <param name="depth">The max depth to search the position</param>
        /// <param name="token">The token used to cancel the search</param>
        /// <returns>The evaluation and the principle variation</returns>
        Task<(int eval, List<Move> pv)> GoDepth(int depth, CancellationToken token);

        /// <summary>
        /// Get information about the Engine
        /// </summary>
        /// <returns>information about the engine</returns>
        string About();
        
    }
}
