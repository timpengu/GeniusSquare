namespace GeniusSquare.CommandLine
{
    public class OptionsException : Exception
    {
        public OptionsException(string message) : base(message) { }
        public OptionsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
