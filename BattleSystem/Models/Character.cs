namespace BattleSystem.Models
{
    // Abstract base class (inheritance + polymorphism).
    // Fields are private-set so state can only change through methods (encapsulation).
    public abstract class Character
    {
        public int Id { get; }
        public string Name { get; }
        public int MaxHealth { get; }
        public int Health { get; private set; }
        public int Attack { get; }
        public int Defence { get; }
        public bool IsDefending { get; private set; }
        public bool IsAlive => Health > 0;

        // Each subclass supplies its own type label and ability name.
        public abstract string Type { get; }
        public abstract string SpecialAbilityName { get; }

        protected Character(int id, string name, int health, int attack, int defence)
        {
            // Validation protects the object from invalid state.
            if (id <= 0) throw new ArgumentException("ID must be greater than 0.");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.");
            if (health <= 0) throw new ArgumentException("Health must be greater than 0.");
            if (attack <= 0) throw new ArgumentException("Attack must be greater than 0.");
            if (defence < 0) throw new ArgumentException("Defence cannot be negative.");

            Id = id;
            Name = name.Trim();
            MaxHealth = health;
            Health = health;
            Attack = attack;
            Defence = defence;
        }

        // Normal attack damage. Virtual so a subclass could override it.
        public virtual int CalculateDamage(Character target)
        {
            return Math.Max(1, Attack - target.Defence);
        }

        // Applies damage. If defending, damage is halved and the defend state is consumed.
        // Returns the damage actually taken.
        public int TakeDamage(int amount)
        {
            if (amount < 0) amount = 0;

            if (IsDefending)
            {
                amount = Math.Max(1, amount / 2);
                IsDefending = false;
            }

            Health = Math.Max(0, Health - amount);
            return amount;
        }

        // Restores health up to MaxHealth. Returns the amount actually restored.
        public int Heal(int amount)
        {
            int before = Health;
            Health = Math.Min(MaxHealth, Health + Math.Max(0, amount));
            return Health - before;
        }

        public void Defend()
        {
            IsDefending = true;
        }

        // Polymorphic method: every character type performs a different ability.
        public abstract string UseSpecialAbility(Character target);

        public override string ToString()
        {
            return $"{Id,-4}{Name,-14}{Type,-8}HP:{Health,4}/{MaxHealth,-4} ATK:{Attack,-4} DEF:{Defence,-4} Special: {SpecialAbilityName}";
        }
    }
}
