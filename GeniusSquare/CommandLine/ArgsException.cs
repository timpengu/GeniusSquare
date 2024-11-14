namespace GeniusSquare.CommandLine
{
    public class ArgsException : Exception
    {
        public ArgsException(string message) : base(message) { }
        public ArgsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
