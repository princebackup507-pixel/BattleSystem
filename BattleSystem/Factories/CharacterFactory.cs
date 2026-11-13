using BattleSystem.Models;

namespace BattleSystem.Factories
{
    // FACTORY PATTERN: centralises object creation so callers never use "new Knight(...)" directly.
    // Used when loading from JSON, adding characters, and creating battle copies.
    public static class CharacterFactory
    {
        public static Character Create(string type, int id, string name, int health, int attack, int defence)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Character type cannot be empty.");

            switch (type.Trim().ToLowerInvariant())
            {
                case "knight": return new Knight(id, name, health, attack, defence);
                case "archer": return new Archer(id, name, health, attack, defence);
                case "mage":   return new Mage(id, name, health, attack, defence);
                default:
                    throw new ArgumentException($"Unknown character type: '{type}'.");
            }
        }
    }
}
