using BattleSystem.Abilities;

namespace BattleSystem.Models
{
    public class Knight : Character
    {
        private readonly ISpecialAbility _ability = new ShieldBashAbility();

        public Knight(int id, string name, int health, int attack, int defence)
            : base(id, name, health, attack, defence) { }

        public override string Type => "Knight";
        public override string SpecialAbilityName => _ability.Name;

        public override string UseSpecialAbility(Character target)
        {
            return _ability.Execute(this, target);
        }
    }
}
