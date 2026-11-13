using BattleSystem.Models;

namespace BattleSystem.Abilities
{
    // Mage ability: restores 30 health to the user (never above max health).
    public class HealAbility : ISpecialAbility
    {
        private const int HealAmount = 30;

        public string Name => "Heal";

        public string Execute(Character user, Character target)
        {
            int restored = user.Heal(HealAmount);
            return $"{user.Name} uses {Name} and restores {restored} health.";
        }
    }
}
