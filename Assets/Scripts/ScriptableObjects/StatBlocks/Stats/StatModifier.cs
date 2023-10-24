

public class StatModifier
{
    /// <summary>
    /// A script for modifiers to CharacterStats. Modifiers can be instantiated by other sources (such as scripts), and applied to a character's stats.
    /// There are three types of Modifier: Flat (added to stat), AddMultiplier(cumulative multipliers that are added together before affecting the stat),
    /// or Multiplier (multiplies the stat - including all previous multipliers!!! I call these "Bethesda Buffs."
    /// For this project, most Multipliers will likely be decimals, to get fractions of stats such as the character's level.
    /// 
    /// All modifiers have their value, their type, a Priority (determines the order in which they are applied) and a Source, which can be any object.
    /// 
    /// NOTE: Most modifiers in this project will likely be instantiated by Traits or Effects, scriptable objects that can be attached to characters.
    /// </summary>
    
    public enum StatModType { Flat = 100, AddMultiplier = 200, Multiplier = 300}

    public readonly float Value;
    public readonly StatModType Type;
    public readonly int Priority;
    public readonly object Source;

    public StatModifier(float value, StatModType type, int priority, object source)
    {
        Value = value;
        Type = type;

        //Priority affects the order in which modifiers are applied.
        Priority = priority;

        Source = source;
    }

    //Constructor that sets a default priority and source. By default, flat modifiers are always applied before multipliers, and source is blank.
    public StatModifier(float value, StatModType type) : this(value, type, (int)type, null) { }

    //Constructor with a custom priority. Source is left blank.
    public StatModifier(float value, StatModType type, int priority) : this(value, type, priority, null) { }

    //Constructor with a Value, Type, and Source, but default order.
    public StatModifier(float value, StatModType type, object source) : this(value, type, (int)type, source) { }
}
