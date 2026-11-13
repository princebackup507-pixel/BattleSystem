using BattleSystem.Models;

namespace BattleSystem.Factories
{
    // Generates a random computer controlled enemy using the factory.
    public static class EnemyGenerator
    {
        private static readonly string[] Types = { "Knight", "Archer", "Mage" };
        private static readonly string[] Names = { "Goblin", "Orc", "Bandit", "Warlock", "Skeleton", "Troll" };

        public static Character Generate(Random random)
        {
            string type = Types[random.Next(Types.Length)];
            string name = Names[random.Next(Names.Length)];
            int health = random.Next(80, 141);
            int attack = random.Next(12, 26);
            int defence = random.Next(3, 11);

            // ID 999 marks a generated enemy (not part of the roster).
            return CharacterFactory.Create(type, 999, name, health, attack, defence);
        }
    }
}
