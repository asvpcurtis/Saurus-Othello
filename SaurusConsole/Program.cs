using SaurusConsole.OthelloAI;
using System;

namespace SaurusConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            IOthelloAI saurus = new Saurus();
            OthelloRepl repl = new OthelloRepl(saurus);
            repl.Run();
        }
    }
}
