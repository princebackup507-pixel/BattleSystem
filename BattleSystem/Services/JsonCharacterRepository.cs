using System.Text.Json;
using BattleSystem.Exceptions;
using BattleSystem.Factories;
using BattleSystem.Models;

namespace BattleSystem.Services
{
    public class JsonCharacterRepository : ICharacterRepository
    {
        private readonly string _path;
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public JsonCharacterRepository(string path)
        {
            _path = path;
        }

        // Loads characters from JSON, converting DTOs to proper subclasses via the factory.
        public List<Character> Load()
        {
            if (!File.Exists(_path))
                throw new DataAccessException($"Character file not found: {_path}");

            try
            {
                string json = File.ReadAllText(_path);
                List<CharacterData>? data = JsonSerializer.Deserialize<List<CharacterData>>(json, Options);

                if (data == null)
                    throw new DataAccessException("Character file is empty or invalid.");

                var characters = new List<Character>();
                foreach (CharacterData d in data)
                {
                    characters.Add(CharacterFactory.Create(d.Type, d.Id, d.Name, d.Health, d.Attack, d.Defence));
                }
                return characters;
            }
            catch (JsonException ex)
            {
                throw new DataAccessException("Character file contains malformed JSON.", ex);
            }
            catch (ArgumentException ex)
            {
                throw new DataAccessException($"Character file contains invalid data: {ex.Message}", ex);
            }
            catch (IOException ex)
            {
                throw new DataAccessException("Could not read the character file.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new DataAccessException("Access to the character file was denied.", ex);
            }
        }

        // Saves characters as JSON (using DTOs).
        public void Save(IEnumerable<Character> characters)
        {
            try
            {
                var data = characters.Select(c => new CharacterData
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    Health = c.MaxHealth,
                    Attack = c.Attack,
                    Defence = c.Defence
                }).ToList();

                File.WriteAllText(_path, JsonSerializer.Serialize(data, Options));
            }
            catch (IOException ex)
            {
                throw new DataAccessException("Could not write the character file.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new DataAccessException("Access to the character file was denied.", ex);
            }
        }
    }
}
