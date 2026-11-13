using BattleSystem.Models;

namespace BattleSystem.Services
{
    // REPOSITORY PATTERN: hides how characters are stored.
    // The storage (JSON) can be replaced without changing the rest of the system.
    public interface ICharacterRepository
    {
        List<Character> Load();
        void Save(IEnumerable<Character> characters);
    }
}
