using BattleSystem.Models;

namespace BattleSystem.Services
{
    public enum BattleOutcome
    {
        InProgress,
        PlayerWon,
        EnemyWon,
        Quit
    }

    // Controls one battle between the player's character and a computer enemy.
    // After every player action the enemy automatically performs a normal attack.
    public class Battle
    {
        private readonly IBattleLogger _logger;

        public Character Player { get; }
        public Character Enemy { get; }
        public BattleOutcome Outcome { get; private set; } = BattleOutcome.InProgress;
        public int Turns { get; private set; }
        public bool IsOver => Outcome != BattleOutcome.InProgress;

        public Battle(Character player, Character enemy, IBattleLogger logger)
        {
            Player = player;
            Enemy = enemy;
            _logger = logger;
            _logger.Log($"Battle started: {Player.Name} ({Player.Type}) vs {Enemy.Name} ({Enemy.Type}).");
        }

        public List<string> PlayerAttack()
        {
            return Resolve(() =>
            {
                int damage = Player.CalculateDamage(Enemy);
                int dealt = Enemy.TakeDamage(damage);
                return $"{Player.Name} attacks {Enemy.Name} for {dealt} damage.";
            });
        }

        public List<string> PlayerDefend()
        {
            return Resolve(() =>
            {
                Player.Defend();
                return $"{Player.Name} braces for the next attack.";
            });
        }

        public List<string> PlayerSpecial()
        {
            // Polymorphism: the correct ability runs depending on the character type.
            return Resolve(() => Player.UseSpecialAbility(Enemy));
        }

        public void Quit()
        {
            if (IsOver) return;
            Outcome = BattleOutcome.Quit;
            _logger.Log($"{Player.Name} quit the battle.");
        }

        // Runs the player's action, then the enemy's automatic normal attack.
        private List<string> Resolve(Func<string> playerAction)
        {
            if (IsOver)
                throw new InvalidOperationException("The battle has already ended.");

            var messages = new List<string>();
            Turns++;

            messages.Add(playerAction());

            if (!Enemy.IsAlive)
            {
                Outcome = BattleOutcome.PlayerWon;
                messages.Add($"{Enemy.Name} is defeated. {Player.Name} wins!");
            }
            else
            {
                int damage = Enemy.CalculateDamage(Player);
                int dealt = Player.TakeDamage(damage);
                messages.Add($"{Enemy.Name} attacks {Player.Name} for {dealt} damage.");

                if (!Player.IsAlive)
                {
                    Outcome = BattleOutcome.EnemyWon;
                    messages.Add($"{Player.Name} is defeated. {Enemy.Name} wins!");
                }
            }

            foreach (string m in messages) _logger.Log(m);
            return messages;
        }
    }
}
