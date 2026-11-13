using BattleSystem.Exceptions;
using BattleSystem.Factories;
using BattleSystem.Models;
using BattleSystem.Services;

namespace BattleSystem.UI
{
    // Runs the menu system and connects all services together.
    // Dependencies are created here and passed in (simple dependency injection).
    public class GameEngine
    {
        private readonly ICharacterRepository _repository;
        private readonly JsonBattleHistoryStore _historyStore;
        private readonly IBattleLogger _logger;
        private readonly Random _random = new Random();
        private CharacterManager _manager;

        public GameEngine()
        {
            // Files are stored next to the executable (bin/Debug/netX.0).
            string baseDir = AppContext.BaseDirectory;
            _repository = new JsonCharacterRepository(Path.Combine(baseDir, "characters.json"));
            _historyStore = new JsonBattleHistoryStore(Path.Combine(baseDir, "battle_history.json"));
            _logger = new FileBattleLogger(Path.Combine(baseDir, "battle_log.txt"));
            _manager = new CharacterManager(new List<Character>());
        }

        // ---------- MAIN LOOP ----------
        public void Run()
        {
            Console.WriteLine("=== Game Character & Battle System ===");
            LoadRoster();

            bool running = true;
            while (running)
            {
                ShowMainMenu();
                int choice = InputReader.ReadInt("Choose an option: ", 0, 7);

                switch (choice)
                {
                    case 1: ViewCharacters(); break;
                    case 2: AddCharacter(); break;
                    case 3: SearchCharacters(); break;
                    case 4: SortCharacters(); break;
                    case 5: StartBattle(); break;
                    case 6: ViewBattleHistory(); break;
                    case 7: SaveRoster(); break;
                    case 0:
                        SaveRoster();
                        Console.WriteLine("Goodbye!");
                        running = false;
                        break;
                }
            }
        }

        private static void ShowMainMenu()
        {
            Console.WriteLine();
            Console.WriteLine("----- MAIN MENU -----");
            Console.WriteLine("1. View characters");
            Console.WriteLine("2. Add character");
            Console.WriteLine("3. Search characters");
            Console.WriteLine("4. Sort characters");
            Console.WriteLine("5. Start battle");
            Console.WriteLine("6. View battle history");
            Console.WriteLine("7. Save characters");
            Console.WriteLine("0. Exit");
        }

        // ---------- LOAD AND SAVE ----------
        private void LoadRoster()
        {
            try
            {
                _manager = new CharacterManager(_repository.Load());
                Console.WriteLine($"Loaded {_manager.Count} characters from file.");
            }
            catch (DataAccessException ex)
            {
                // Missing or malformed file: fall back to a default roster.
                Console.WriteLine($"Warning: {ex.Message}");
                Console.WriteLine("A default roster will be used instead.");
                _manager = new CharacterManager(CreateDefaultRoster());
                SaveRoster();
            }
        }

        private void SaveRoster()
        {
            try
            {
                _repository.Save(_manager.GetAll());
                Console.WriteLine("Characters saved to characters.json.");
            }
            catch (DataAccessException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static List<Character> CreateDefaultRoster()
        {
            return new List<Character>
            {
                CharacterFactory.Create("Knight", 1, "Arthur",   120, 18, 10),
                CharacterFactory.Create("Archer", 2, "Robin",     90, 22, 5),
                CharacterFactory.Create("Mage",   3, "Merlin",    80, 16, 4),
                CharacterFactory.Create("Knight", 4, "Lancelot", 110, 20, 8),
                CharacterFactory.Create("Archer", 5, "Legolas",   85, 24, 4)
            };
        }

        // ---------- VIEW ----------
        private void ViewCharacters()
        {
            Console.WriteLine();
            if (_manager.Count == 0)
            {
                Console.WriteLine("No characters available.");
                return;
            }

            foreach (Character c in _manager.GetAll())
            {
                Console.WriteLine(c);
            }
        }

        // ---------- ADD ----------
        private void AddCharacter()
        {
            Console.WriteLine();
            Console.WriteLine("1. Knight  2. Archer  3. Mage");
            int typeChoice = InputReader.ReadInt("Choose type: ", 1, 3);
            string type = typeChoice == 1 ? "Knight" : typeChoice == 2 ? "Archer" : "Mage";

            string name = InputReader.ReadNonEmpty("Name: ");
            int health = InputReader.ReadInt("Health (1-500): ", 1, 500);
            int attack = InputReader.ReadInt("Attack (1-100): ", 1, 100);
            int defence = InputReader.ReadInt("Defence (0-100): ", 0, 100);

            try
            {
                Character c = CharacterFactory.Create(type, _manager.NextId(), name, health, attack, defence);
                _manager.Add(c);
                Console.WriteLine($"Added: {c}");
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                Console.WriteLine($"Could not add character: {ex.Message}");
            }
        }

        // ---------- SEARCH ----------
        private void SearchCharacters()
        {
            Console.WriteLine();
            Console.WriteLine("1. Search by ID (linear search)");
            Console.WriteLine("2. Search by ID (binary search)");
            Console.WriteLine("3. Search by name");
            int choice = InputReader.ReadInt("Choose: ", 1, 3);

            if (choice == 3)
            {
                string text = InputReader.ReadNonEmpty("Enter name or part of name: ");
                List<Character> results = _manager.SearchByName(text);

                if (results.Count == 0) Console.WriteLine("No characters found.");
                else results.ForEach(c => Console.WriteLine(c));
                return;
            }

            int id = InputReader.ReadInt("Enter ID: ", 1, int.MaxValue);
            Character? found = choice == 1 ? _manager.LinearSearchById(id) : _manager.BinarySearchById(id);

            Console.WriteLine(found == null ? "No character found with that ID." : found.ToString());
        }

        // ---------- SORT ----------
        private void SortCharacters()
        {
            Console.WriteLine();
            Console.WriteLine("1. ID  2. Name  3. Health  4. Attack  5. Defence");
            int choice = InputReader.ReadInt("Sort by: ", 1, 5);
            bool ascending = InputReader.ReadYesNo("Ascending order?");

            _manager.SortBy((SortCriteria)choice, ascending);
            Console.WriteLine("Sorted result:");
            ViewCharacters();
        }

        // ---------- BATTLE ----------
        private void StartBattle()
        {
            if (_manager.Count == 0)
            {
                Console.WriteLine("No characters available to battle.");
                return;
            }

            ViewCharacters();
            int id = InputReader.ReadInt("Enter the ID of the character to fight with: ", 1, int.MaxValue);
            Character? chosen = _manager.LinearSearchById(id);

            if (chosen == null)
            {
                Console.WriteLine("Invalid selection. No character with that ID.");
                return;
            }

            // A fresh copy is used so roster characters always start at full health.
            Character player = CharacterFactory.Create(chosen.Type, chosen.Id, chosen.Name,
                                                       chosen.MaxHealth, chosen.Attack, chosen.Defence);
            Character enemy = EnemyGenerator.Generate(_random);
            var battle = new Battle(player, enemy, _logger);

            Console.WriteLine();
            Console.WriteLine($"Battle: {player.Name} vs {enemy.Name} ({enemy.Type})!");

            while (!battle.IsOver)
            {
                Console.WriteLine();
                Console.WriteLine($"{player.Name}: {player.Health}/{player.MaxHealth} HP    " +
                                  $"{enemy.Name}: {enemy.Health}/{enemy.MaxHealth} HP");
                Console.WriteLine($"1. Attack  2. Defend  3. Special ({player.SpecialAbilityName})  4. View info  5. Quit");

                int action = InputReader.ReadInt("Choose action: ", 1, 5);
                List<string>? messages = null;

                switch (action)
                {
                    case 1: messages = battle.PlayerAttack(); break;
                    case 2: messages = battle.PlayerDefend(); break;
                    case 3: messages = battle.PlayerSpecial(); break;
                    case 4:
                        Console.WriteLine(player);
                        Console.WriteLine(enemy);
                        break;
                    case 5:
                        battle.Quit();
                        break;
                }

                if (messages != null)
                {
                    foreach (string m in messages) Console.WriteLine(m);
                }
            }

            SaveBattleResult(battle);
        }

        private void SaveBattleResult(Battle battle)
        {
            string result = battle.Outcome switch
            {
                BattleOutcome.PlayerWon => "Win",
                BattleOutcome.EnemyWon => "Loss",
                _ => "Quit"
            };

            Console.WriteLine($"Battle ended: {result}.");

            try
            {
                _historyStore.Append(new BattleRecord
                {
                    PlayerName = battle.Player.Name,
                    PlayerType = battle.Player.Type,
                    EnemyName = battle.Enemy.Name,
                    Result = result,
                    Turns = battle.Turns,
                    Date = DateTime.Now
                });
            }
            catch (DataAccessException ex)
            {
                Console.WriteLine($"Could not save battle result: {ex.Message}");
            }
        }

        // ---------- HISTORY ----------
        private void ViewBattleHistory()
        {
            try
            {
                List<BattleRecord> history = _historyStore.Load();
                Console.WriteLine();

                if (history.Count == 0)
                {
                    Console.WriteLine("No battles recorded yet.");
                    return;
                }

                foreach (BattleRecord r in history)
                {
                    Console.WriteLine($"{r.Date:yyyy-MM-dd HH:mm}  {r.PlayerName} ({r.PlayerType}) vs {r.EnemyName}  " +
                                      $"Result: {r.Result}  Turns: {r.Turns}");
                }
            }
            catch (DataAccessException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
