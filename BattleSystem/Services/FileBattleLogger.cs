namespace BattleSystem.Services
{
    // Appends timestamped battle actions to a TXT file.
    public class FileBattleLogger : IBattleLogger
    {
        private readonly string _path;

        public FileBattleLogger(string path)
        {
            _path = path;
        }

        public void Log(string message)
        {
            try
            {
                File.AppendAllText(_path, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
            }
            catch (IOException)
            {
                // Logging must never crash the game, so file errors are ignored here.
            }
            catch (UnauthorizedAccessException)
            {
                // Same reason as above.
            }
        }
    }
}
