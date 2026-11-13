using System.Text.Json;
using BattleSystem.Exceptions;
using BattleSystem.Models;

namespace BattleSystem.Services
{
    // Saves and loads battle data (results) using JSON.
    public class JsonBattleHistoryStore
    {
        private readonly string _path;
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public JsonBattleHistoryStore(string path)
        {
            _path = path;
        }

        // A missing file simply means no battles yet. A corrupt file raises DataAccessException.
        public List<BattleRecord> Load()
        {
            if (!File.Exists(_path)) return new List<BattleRecord>();

            try
            {
                string json = File.ReadAllText(_path);
                return JsonSerializer.Deserialize<List<BattleRecord>>(json, Options) ?? new List<BattleRecord>();
            }
            catch (JsonException ex)
            {
                throw new DataAccessException("Battle history file contains malformed JSON.", ex);
            }
            catch (IOException ex)
            {
                throw new DataAccessException("Could not read the battle history file.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new DataAccessException("Access to the battle history file was denied.", ex);
            }
        }

        public void Append(BattleRecord record)
        {
            List<BattleRecord> history;
            try
            {
                history = Load();
            }
            catch (DataAccessException)
            {
                // Corrupt history: start a fresh list rather than lose the new record.
                history = new List<BattleRecord>();
            }

            history.Add(record);

            try
            {
                File.WriteAllText(_path, JsonSerializer.Serialize(history, Options));
            }
            catch (IOException ex)
            {
                throw new DataAccessException("Could not write the battle history file.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new DataAccessException("Access to the battle history file was denied.", ex);
            }
        }
    }
}
