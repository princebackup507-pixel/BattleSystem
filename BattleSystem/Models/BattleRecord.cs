namespace BattleSystem.Models
{
    // One finished (or quit) battle, stored in battle_history.json.
    public class BattleRecord
    {
        public string PlayerName { get; set; } = string.Empty;
        public string PlayerType { get; set; } = string.Empty;
        public string EnemyName { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public int Turns { get; set; }
        public DateTime Date { get; set; }
    }
}
