using GeniusSquare.Core.Coords;
using GeniusSquare.Core.Game;
using System.Xml.Linq;

namespace GeniusSquare.Output;

internal class HtmlWriter : IOutputWriter
{
    private readonly string _htmlFileName;
    private readonly XDocument _xDoc; // TODO: Use an HTML API instead of XML :/

    public HtmlWriter(string htmlFileName)
    {
        _htmlFileName = htmlFileName ?? throw new ArgumentNullException(nameof(htmlFileName));
        _xDoc = XDocument.Load(@"Output\Html\solutions.template.html");
    }

    private XElement GetBodyElement() => _xDoc.Element("html")?.Element("body") ?? throw new Exception("No html/body element");

    public void WriteInitialState(Board board, IReadOnlyCollection<Piece> pieces)
    {
        XElement xeBody = GetBodyElement();

        // Append title element
        xeBody.Add(
           new XElement("div",
                new XAttribute("class", "title"),
                "Initial board state:"));

        // Append grid elements
        var xeGrid = new XElement("div", new XAttribute("class", "grid"), String.Empty);
        xeBody.Add(xeGrid);

        xeGrid.Add(
            CreateGridElements(board, board.GetLayout()));
    }

    public void WriteSolution(Board board, Solution solution, int solutionCount, TimeSpan elapsed)
    {
        XElement xeBody = GetBodyElement();

        // Append title element
        xeBody.Add(
           new XElement("div",
                new XAttribute("class", "title"),
                $"Solution {solutionCount} @ {elapsed:hh\\:mm\\:ss\\.fff}"));

        // Append grid elements
        var xeGrid = new XElement("div", new XAttribute("class", "grid"), String.Empty);
        xeBody.Add(xeGrid);

        xeGrid.Add(
            CreateGridElements(board, solution.GetLayout(board.Size)));
    }

    private static IEnumerable<XElement> CreateGridElements(Board board, Piece?[,] layout)
    {
        yield return new XElement("div", new XAttribute("class", "axis"), String.Empty); // row label placeholder

        // Generate axis row
        foreach (int x in board.Bounds.EnumerateX())
        {
            int columnLabel = 1 + x;
            yield return new XElement("div", new XAttribute("class", "axis"), columnLabel); // column label
        }

        // Generate grid rows
        foreach (int y in board.Bounds.EnumerateY())
        {
            // Generate axis column (row label)
            char rowLabel = (char)('A' + y);
            yield return new XElement("div", new XAttribute("class", "axis"), rowLabel); // row label

            // Generate grid columns
            foreach (int x in board.Bounds.EnumerateX())
            {
                Piece? piece = layout[x, y];
                string posClass = piece == null
                    ? board.IsOccupied(new Coord(x, y)) ? "peg" : "space"
                    : $"block {piece.Name.ToLower()}";

                yield return new XElement("div", new XAttribute("class", posClass), String.Empty); // peg/space/block
            }
        }
    }

    public void WriteSummary(int solutionCount, TimeSpan elapsed)
    {
        GetBodyElement().Add(
           new XElement("div",
                new XAttribute("class", "title"),
                $"Found {solutionCount} solution{(solutionCount == 1 ? "" : "s")} in {elapsed:hh\\:mm\\:ss\\.fff}."));
    }

    public void Flush()
    {
        _xDoc.Save(_htmlFileName);
    }
}
