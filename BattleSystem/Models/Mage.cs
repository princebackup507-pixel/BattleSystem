using BattleSystem.Abilities;

namespace BattleSystem.Models
{
    public class Mage : Character
    {
        private readonly ISpecialAbility _ability = new HealAbility();

        public Mage(int id, string name, int health, int attack, int defence)
            : base(id, name, health, attack, defence) { }

        public override string Type => "Mage";
        public override string SpecialAbilityName => _ability.Name;

        public override string UseSpecialAbility(Character target)
        {
            return _ability.Execute(this, target);
        }
    }
}
