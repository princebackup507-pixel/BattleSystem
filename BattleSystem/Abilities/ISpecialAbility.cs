using BattleSystem.Models;

namespace BattleSystem.Abilities
{
    // STRATEGY PATTERN: each special ability is an interchangeable algorithm.
    public interface ISpecialAbility
    {
        string Name { get; }

        // Executes the ability and returns a description of what happened.
        string Execute(Character user, Character target);
    }
}
