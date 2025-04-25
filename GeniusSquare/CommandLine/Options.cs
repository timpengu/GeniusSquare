using CommandLine;

namespace GeniusSquare.CommandLine
{
    // Most difficult https://medium.com/@redlogo/cracking-the-genius-square-61c0df797f73 (11 solutions)
    // -b 6 -o A6 B1 B5 D1 E3 F2 F4

    // Played 2024-11-28 (1027 solutions)
    // -b 6 -o A6 B2 B5 C4 F2 F3 F4

    // Played 2024-11-28 (3298 solutions)
    // -b 6 -o A4 B3 B4 E2 E5 F1 F2

    // Played 2024-11-28 (4900 solutions)
    // -b 6 -o A1 B2 B4 D6 E6 F1 F2

    internal sealed class Options
    {
        [Option('b', Min = 1, HelpText = "Board size (takes one or two integer dimension parameters)")]
        public IEnumerable<int> BoardSize { get; set; } = [];

        [Option('o', HelpText = "Occupied positions (e.g. A1 B2 ...)")]
        public IEnumerable<string> OccupiedPositions { get; set; } = [];

        [Option('r', HelpText = "Number of positions occupied randomly")]
        public int? OccupiedRandoms { get; set; }

        [Option('h', HelpText = "Output HTML to given file name")]
        public string? HtmlFileName { get; set; } 

        [Option('v', FlagCounter = true, HelpText = "Console verbosity level")]
        public int Verbosity { get; set; } = 0;
    }
}
