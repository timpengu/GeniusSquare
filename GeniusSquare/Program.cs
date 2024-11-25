using CommandLine;
using CommandLine.Text;
using GeniusSquare.CommandLine;
using GeniusSquare.Configuration;

namespace GeniusSquare;
public class Program
{
    public static int Main(string[] args)
    {
        try
        {
            bool success = false;

            ParserResult<Options> parserResult = ParseArgs(args);
            parserResult
                .WithParsed(options =>
                {
                    options.Validate();
                    var config = Config.Load("Config.json"); // TODO: Add config file path to options
                    var runner = new ConsoleRunner(config, options);
                    success = runner.Run();
                })
                .WithNotParsed(errors =>
                    throw new OptionsException(GetHelpText(parserResult, errors))
                );

            return success ? 0 : 1;
        }
        catch (OptionsException e)
        {
            Console.WriteLine(e.Message);
            return -1;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return -2;
        }
    }

    private static ParserResult<Options> ParseArgs(string[] args)
    {
        using var parser = new Parser(with =>
        {
            with.GetoptMode = true;
            with.HelpWriter = null;
        });

        return parser.ParseArguments<Options>(args);
    }

    private static string GetHelpText<T>(ParserResult<T> result, IEnumerable<Error> errs)
    {
        return errs.IsVersion()
            ? HelpText.AutoBuild(result)
            : HelpText.AutoBuild(result, h =>
                {
                    h.AdditionalNewLineAfterOption = false;
                    return HelpText.DefaultParsingErrorsHandler(result, h);
                },
                e => e
            );
    }
}
