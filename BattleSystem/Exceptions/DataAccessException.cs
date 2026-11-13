namespace BattleSystem.Exceptions
{
    // Custom exception wrapping all file and data problems
    // (missing file, malformed JSON, invalid values, access denied).
    public class DataAccessException : Exception
    {
        public DataAccessException(string message) : base(message) { }
        public DataAccessException(string message, Exception inner) : base(message, inner) { }
    }
}
