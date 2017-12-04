using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SaurusConsole.OthelloAI;
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

                case "stop":
                    tokenSource.Cancel();
                    return "done!";

                case "about":
                    return ai.About();

                case "help":
                    return "go depth <int>       -> Search current position by depth for best move and evaluation\n" 
                        + "pos [startpos|<fen>] -> Set the position\n"
                        + "stop                 -> Stops searching the position\n"
                        + "about                -> Get information about the engine\n"
                        + "help                 -> List all the commands\n"
                        + "clear                -> Clears the Console\n"
                        + "exit                 -> Terminate the process";
                default:
                    return $"Unknown Command {split[0]}";
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
                        Task<(int eval, List<Move> pv)> depthSearch = ai.GoDepth(depth, tokenSource.Token);
                        Console.WriteLine("here");
                        Console.ReadKey();
                        var answer = await depthSearch;
                        return $"{answer}";
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
            ai.SetPosition(split[1]);
            return "done!";
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
