using BattleSystem.Models;

namespace BattleSystem.Abilities
{
    // Knight ability: deals 1.5x attack damage (minus target defence, minimum 1).
    public class ShieldBashAbility : ISpecialAbility
    {
        public string Name => "Shield Bash";

        public string Execute(Character user, Character target)
        {
            int raw = Math.Max(1, (int)(user.Attack * 1.5) - target.Defence);
            int dealt = target.TakeDamage(raw);
            return $"{user.Name} uses {Name} on {target.Name} for {dealt} damage.";
        }
    }
}
