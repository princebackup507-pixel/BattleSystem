using BattleSystem.Models;

namespace BattleSystem.Abilities
{
    // Archer ability: deals 2x attack damage (minus target defence, minimum 1).
    public class PowerShotAbility : ISpecialAbility
    {
        public string Name => "Power Shot";

        public string Execute(Character user, Character target)
        {
            int raw = Math.Max(1, (user.Attack * 2) - target.Defence);
            int dealt = target.TakeDamage(raw);
            return $"{user.Name} uses {Name} on {target.Name} for {dealt} damage.";
        }
    }
}
