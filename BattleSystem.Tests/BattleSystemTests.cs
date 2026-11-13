using BattleSystem.Exceptions;
using BattleSystem.Factories;
using BattleSystem.Models;
using BattleSystem.Services;
using Xunit;

namespace BattleSystem.Tests
{
    // Fake logger so tests do not write to disk.
    public class NullBattleLogger : IBattleLogger
    {
        public void Log(string message) { }
    }

    public class BattleSystemTests
    {
        // ---------- TEST 1: Factory creates the correct subclass ----------
        [Theory]
        [InlineData("Knight", typeof(Knight))]
        [InlineData("archer", typeof(Archer))]
        [InlineData("MAGE", typeof(Mage))]
        public void Factory_CreatesCorrectCharacterType(string type, Type expected)
        {
            Character c = CharacterFactory.Create(type, 1, "Test", 100, 10, 5);
            Assert.IsType(expected, c);
        }

        // ---------- TEST 2: Factory rejects unknown type ----------
        [Fact]
        public void Factory_UnknownType_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                CharacterFactory.Create("Dragon", 1, "Test", 100, 10, 5));
        }

        // ---------- TEST 3: Defending halves the next damage ----------
        [Fact]
        public void Defend_HalvesNextDamage()
        {
            var target = new Knight(1, "Target", 100, 10, 0);
            target.Defend();

            int taken = target.TakeDamage(20);

            Assert.Equal(10, taken);
            Assert.Equal(90, target.Health);
        }

        // ---------- TEST 4: Knight Shield Bash does more than a normal attack ----------
        [Fact]
        public void ShieldBash_DealsMoreDamageThanNormalAttack()
        {
            var knight = new Knight(1, "Knight", 100, 20, 5);
            var dummyA = new Knight(2, "DummyA", 200, 10, 5);
            var dummyB = new Knight(3, "DummyB", 200, 10, 5);

            dummyA.TakeDamage(knight.CalculateDamage(dummyA));
            knight.UseSpecialAbility(dummyB);

            Assert.True(dummyB.Health < dummyA.Health);
        }

        // ---------- TEST 5: Mage Heal never exceeds max health ----------
        [Fact]
        public void MageHeal_DoesNotExceedMaxHealth()
        {
            var mage = new Mage(1, "Mage", 100, 10, 5);
            mage.TakeDamage(10);

            mage.UseSpecialAbility(mage);

            Assert.Equal(100, mage.Health);
        }

        // ---------- TEST 6: Linear search finds and misses correctly ----------
        [Fact]
        public void LinearSearchById_FindsExistingAndReturnsNullForMissing()
        {
            var manager = new CharacterManager(new List<Character>
            {
                new Knight(1, "A", 100, 10, 5),
                new Archer(2, "B", 90, 12, 4)
            });

            Assert.NotNull(manager.LinearSearchById(2));
            Assert.Null(manager.LinearSearchById(99));
        }

        // ---------- TEST 7: Binary search works on unsorted roster ----------
        [Fact]
        public void BinarySearchById_FindsCharacter()
        {
            var manager = new CharacterManager(new List<Character>
            {
                new Knight(5, "E", 100, 10, 5),
                new Archer(2, "B", 90, 12, 4),
                new Mage(9, "I", 80, 8, 3)
            });

            Character? found = manager.BinarySearchById(9);

            Assert.NotNull(found);
            Assert.Equal("I", found!.Name);
        }

        // ---------- TEST 8: Manual sort orders by attack ascending ----------
        [Fact]
        public void SortByAttack_Ascending_OrdersCorrectly()
        {
            var manager = new CharacterManager(new List<Character>
            {
                new Knight(1, "A", 100, 30, 5),
                new Archer(2, "B", 90, 10, 4),
                new Mage(3, "C", 80, 20, 3)
            });

            manager.SortBy(SortCriteria.Attack, true);
            var attacks = manager.GetAll().Select(c => c.Attack).ToList();

            Assert.Equal(new List<int> { 10, 20, 30 }, attacks);
        }

        // ---------- TEST 9: Malformed JSON raises DataAccessException ----------
        [Fact]
        public void Repository_MalformedJson_ThrowsDataAccessException()
        {
            string path = Path.Combine(Path.GetTempPath(), $"bad_{Guid.NewGuid()}.json");
            File.WriteAllText(path, "{ this is not valid json ");

            try
            {
                var repo = new JsonCharacterRepository(path);
                Assert.Throws<DataAccessException>(() => repo.Load());
            }
            finally
            {
                File.Delete(path);
            }
        }

        // ---------- TEST 10: Battle ends with a player win when enemy dies ----------
        [Fact]
        public void Battle_PlayerDefeatsEnemy_OutcomeIsPlayerWon()
        {
            var player = new Knight(1, "Hero", 100, 500, 5);
            var enemy = new Knight(999, "Weakling", 10, 5, 0);
            var battle = new Battle(player, enemy, new NullBattleLogger());

            battle.PlayerAttack();

            Assert.True(battle.IsOver);
            Assert.Equal(BattleOutcome.PlayerWon, battle.Outcome);
        }
    }
}
