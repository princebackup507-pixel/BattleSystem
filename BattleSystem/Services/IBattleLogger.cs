namespace BattleSystem.Services
{
    // Abstraction for logging so tests can use a fake logger.
    public interface IBattleLogger
    {
        void Log(string message);
    }
}
