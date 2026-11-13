using BattleSystem.UI;

namespace BattleSystem
{
    // Entry point kept minimal on purpose. All logic lives in GameEngine.
    public class Program
    {
        public static void Main(string[] args)
        {
            new GameEngine().Run();
        }
    }
}
