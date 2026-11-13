    using BattleSystem.Models;

namespace BattleSystem.Services
{
    // Manages the roster and holds the search and sort algorithms.
    public class CharacterManager
    {
        private readonly List<Character> _characters;

        public CharacterManager(IEnumerable<Character> characters)
        {
            _characters = new List<Character>(characters);
        }

        public int Count => _characters.Count;

        public IReadOnlyList<Character> GetAll()
        {
            return _characters.AsReadOnly();
        }

        public int NextId()
        {
            return _characters.Count == 0 ? 1 : _characters.Max(c => c.Id) + 1;
        }

        public void Add(Character character)
        {
            if (_characters.Any(c => c.Id == character.Id))
                throw new InvalidOperationException($"A character with ID {character.Id} already exists.");

            _characters.Add(character);
        }

        // ---------- SEARCH ALGORITHMS ----------

        // Linear search by ID: checks each element in order. O(n).
        public Character? LinearSearchById(int id)
        {
            foreach (Character c in _characters)
            {
                if (c.Id == id) return c;
            }
            return null;
        }

        // Binary search by ID: sorts a copy by ID, then repeatedly halves the search range. O(log n).
        public Character? BinarySearchById(int id)
        {
            var sorted = new List<Character>(_characters);
            InsertionSort(sorted, (a, b) => a.Id.CompareTo(b.Id));

            int low = 0;
            int high = sorted.Count - 1;

            while (low <= high)
            {
                int mid = low + (high - low) / 2;

                if (sorted[mid].Id == id) return sorted[mid];
                if (sorted[mid].Id < id) low = mid + 1;
                else high = mid - 1;
            }
            return null;
        }

        // Linear search by name (case-insensitive partial match). Returns all matches.
        public List<Character> SearchByName(string text)
        {
            var results = new List<Character>();
            if (string.IsNullOrWhiteSpace(text)) return results;

            foreach (Character c in _characters)
            {
                if (c.Name.Contains(text.Trim(), StringComparison.OrdinalIgnoreCase))
                    results.Add(c);
            }
            return results;
        }

        // ---------- SORT ALGORITHM ----------

        // Sorts the roster in place using the manual insertion sort.
        public void SortBy(SortCriteria criteria, bool ascending)
        {
            Comparison<Character> comparison = criteria switch
            {
                SortCriteria.Id      => (a, b) => a.Id.CompareTo(b.Id),
                SortCriteria.Name    => (a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase),
                SortCriteria.Health  => (a, b) => a.MaxHealth.CompareTo(b.MaxHealth),
                SortCriteria.Attack  => (a, b) => a.Attack.CompareTo(b.Attack),
                SortCriteria.Defence => (a, b) => a.Defence.CompareTo(b.Defence),
                _ => (a, b) => a.Id.CompareTo(b.Id)
            };

            if (!ascending)
            {
                Comparison<Character> asc = comparison;
                comparison = (a, b) => asc(b, a);
            }

            InsertionSort(_characters, comparison);
        }

        // Generic manual insertion sort (no built-in Sort used).
        // Takes each element and inserts it into its correct place in the sorted part on the left. O(n^2).
        public static void InsertionSort<T>(List<T> list, Comparison<T> comparison)
        {
            for (int i = 1; i < list.Count; i++)
            {
                T key = list[i];
                int j = i - 1;

                while (j >= 0 && comparison(list[j], key) > 0)
                {
                    list[j + 1] = list[j];
                    j--;
                }
                list[j + 1] = key;
            }
        }
    }
}
