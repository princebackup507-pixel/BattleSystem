namespace BattleSystem.Models
{
    // Data Transfer Object used only for JSON serialisation.
    // Keeps the JSON format simple and avoids polymorphic serialisation problems.
    public class CharacterData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defence { get; set; }
    }
}
