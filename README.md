# Game Character & Battle System

A console-based, turn-based battle system written in C# and developed in Visual Studio 2026 as a software engineering assignment (Scenario 1: Game Character & Battle System).

## Features

- Manage a roster of characters: view, add, search and sort
- Three character types with different special abilities
- Turn-based battle against a randomly generated computer enemy
- Battle actions: attack, defend, special ability, view info, quit
- Roster and battle history saved and loaded as JSON
- Every battle action recorded in a text log
- Input validation and handling of missing or malformed files
- Unit tests with xUnit (12 test cases)

## Character Types

| Type | Special Ability | Effect |
|---|---|---|
| Knight | Shield Bash | Deals 1.5x attack damage |
| Archer | Power Shot | Deals 2x attack damage |
| Mage | Heal | Restores 30 health (never above maximum) |

## Battle Rules

- A normal attack deals `attack - target defence` damage (minimum 1).
- Defending halves the damage from the enemy's next attack.
- After every player action, the enemy automatically performs a normal attack.
- The battle ends when either character reaches 0 health, or the player quits.

## Getting Started

### Prerequisites

- Visual Studio 2026 with the **.NET desktop development** workload
- .NET 10 SDK (if a different version is installed, change `TargetFramework` in both `.csproj` files so they match)

### Run the application

1. Clone or download this repository.
2. Open `BattleSystem.sln` in Visual Studio.
3. Set `BattleSystem` as the startup project.
4. Choose **Debug > Start Without Debugging**.

Alternatively, from a terminal in the repository root:

```
dotnet run --project BattleSystem
```

### Run the unit tests

In Visual Studio open **Test > Test Explorer** and click **Run All Tests**, or run:

```
dotnet test
```

## Menu Options

1. View characters
2. Add character
3. Search characters (linear search by ID, binary search by ID, search by name)
4. Sort characters (ID, name, health, attack, defence; ascending or descending)
5. Start battle
6. View battle history
7. Save characters
0. Exit

## Project Structure

```
BattleSystem.sln
BattleSystem/
  Program.cs
  Abilities/      ISpecialAbility, ShieldBashAbility, PowerShotAbility, HealAbility
  Models/         Character (abstract), Knight, Archer, Mage, CharacterData, BattleRecord, SortCriteria
  Factories/      CharacterFactory, EnemyGenerator
  Exceptions/     DataAccessException
  Services/       ICharacterRepository, JsonCharacterRepository, JsonBattleHistoryStore,
                  IBattleLogger, FileBattleLogger, CharacterManager, Battle
  UI/             GameEngine, InputReader
BattleSystem.Tests/
  BattleSystemTests.cs
```

## Object-Oriented Design

- **Inheritance and polymorphism:** `Knight`, `Archer` and `Mage` extend the abstract `Character` class and override `UseSpecialAbility`.
- **Interfaces:** `ISpecialAbility`, `ICharacterRepository` and `IBattleLogger`.
- **Encapsulation:** state such as health can only change through methods, and `Program.cs` contains a single call into `GameEngine`.
- **Collections and generics:** `List<Character>`, `List<BattleRecord>` and a generic `InsertionSort<T>`.

## Design Patterns

| Pattern | Where |
|---|---|
| Factory | `CharacterFactory` creates characters when loading, adding and starting battles |
| Strategy | `ISpecialAbility` with a separate class for each ability |
| Repository | `ICharacterRepository` hides how characters are stored |
| Dependency Injection | `Battle` receives an `IBattleLogger`, allowing a fake logger in tests |

## Algorithms

| Algorithm | Used for |
|---|---|
| Linear search | Finding a character by ID or by name |
| Binary search | Finding a character by ID |
| Insertion sort (manual, generic) | Sorting characters by any criteria |

## Data Files

Created at runtime next to the executable (for example `BattleSystem/bin/Debug/net10.0/`):

- `characters.json`: the character roster (a default roster is created on first run)
- `battle_history.json`: results of finished battles
- `battle_log.txt`: timestamped log of every battle action

## Error Handling

- Menu and number input is validated and re-requested until valid.
- A missing or malformed `characters.json` produces a clear warning, and a default roster is used.
- File errors are wrapped in a custom `DataAccessException`.
- Logging failures never crash the game.

## Testing

The `BattleSystem.Tests` project covers the factory, damage and defend rules, special abilities, search, sorting, malformed JSON handling and the battle win condition.

