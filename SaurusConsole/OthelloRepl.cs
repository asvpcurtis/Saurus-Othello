using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SaurusConsole.OthelloAI;
using System.Linq;
namespace SaurusConsole
{
    /// <summary>
    /// Represents a Universal Othello Interface on behalf of an Othello AI
    /// </summary>
    class OthelloRepl
    {
        private IOthelloAI ai;
        private CancellationTokenSource tokenSource;

        /// <summary>
        /// Instantiates an instance of OthelloRepl
        /// </summary>
        /// <param name="ai">The Othello AI to invoke commands on</param>
        public OthelloRepl(IOthelloAI ai)
        {
            this.ai = ai;
            tokenSource = new CancellationTokenSource();
        }

        private async Task<string> Evaluate(string input)
        {
            var split = input.Split(' ');
            switch (split[0])
            {
                case "go":
                    return await GoHandler(split);

                case "pos":
                    return PosHandler(split);

                case "move":
                    return MoveHandler(split);

                case "stop":
                    tokenSource.Cancel();
                    return "done!";

                case "about":
                    return ai.About();

                case "help":
                    return "go depth <int>       -> Search current position by depth for best move and evaluation\n" 
                        + "pos [startpos|<fen>] -> Set the position\n"
                        + "move <co-ords>       -> Make the move on the position\n"
                        + "stop                 -> Stops searching the position\n"
                        + "about                -> Get information about the engine\n"
                        + "help                 -> List all the commands\n"
                        + "clear                -> Clears the Console\n"
                        + "exit                 -> Terminate the process";
                default:
                    return $"Unknown Command {split[0]}";
            }
        }

        private string MoveHandler(string[] split)
        {
            if (split.Length < 2)
            {
                return "Move required";
            }
            string move = split[1].ToUpper();
            if (move.Length == 2 && "ABCDEFGH".Contains(move[0]) && "12345678".Contains(move[1]))
            {
                Move temp = new Move(move);
                if (!ai.GetPosition().GetLegalMoves().Contains(temp))
                {
                    return "Not a legal move";
                }
                ai.SetPosition(ai.GetPosition().MakeMove(temp));
                return "done!";
            }
            else
            {
                return $"{split[1]} is not recognized as a move";
            }
        }

        private async Task<string> GoHandler(string[] split)
        {
            if (split.Length < 2)
            {
                return "Parameter required";
            }
            switch (split[1])
            {
                case "depth":
                    if (split.Length < 3)
                    {
                        return "Depth Required";
                    }
                    else if (int.TryParse(split[2], out int depth))
                    {
                        Task<(int, List<Move>)> task = ai.GoDepth(depth, tokenSource.Token);
                        task.Wait();
                        (int eval, List<Move> pv) answer = await task;

                        return $"{answer.eval}: {ParsePV(answer.pv)}";
                    }
                    else
                    {
                        return $"{split[2]} is not an integer";
                    }
                default:
                    return $"Unknown Parameter {split[1]}";
            }
        }

        private string PosHandler(string[] split)
        {
            if (split.Length < 2)
            {
                return "Position required";
            }
            ai.SetPosition(new Position(split[1]));
            return "done!";
        }

        private string ParsePV(IEnumerable<Move> pv)
        {
            string pvString = "";
            foreach(Move move in pv)
            {
                pvString += $"{move}, ";
            }
            if (pv.Count() > 0)
            {
                pvString = pvString.Substring(0, pvString.Length - 2);
            }
            return pvString;
        }

        /// <summary>
        /// Start the REPL
        /// </summary>
        public async void Run()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Welcome to the Saurus Console!");
            Console.WriteLine("Type [help] for more commands.");
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("Saurus Console > ");
                Console.ForegroundColor = ConsoleColor.White;
                string input = Console.ReadLine();
                if (input == "exit")
                {
                    break;
                }
                else if (input == "clear")
                {
                    Console.Clear();
                }
                else
                {
                    string response = await Evaluate(input);
                    Console.WriteLine(response);
                }
            }
            Console.WriteLine("done!");
        }
    }
}
