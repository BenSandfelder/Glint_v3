using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[Serializable]
public class CharacterStat
{

    /// <summary>
    /// An exceptionally dynamic script for any kind of stat. Stats have a base value and a list of modifiers.
    /// Modifiers can be added, removed, or removed by source.
    /// by referencing Value (capital V), other scripts can get the current value of the stat, which will
    /// automatically recalculate if modifiers have been added/removed, or if the stat doesn't match its own record.
    /// </summary> 

    //The base value of the stat.
    protected int baseValue;
    protected bool isDirty = true;
    protected int _value;
    private int lastBaseValue;

    //A read-only list of all modifiers to the listed stat.
    private readonly List<StatModifier> statModifiers;

    //A public, uneditable copy of the statModifiers list.
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;

    //Empty constructor.
    public CharacterStat()
    {
        statModifiers = new List<StatModifier>();
        StatModifiers = statModifiers.AsReadOnly();
    }

    //Constructor that extends the empty one by adding a preset baseValue.
    public CharacterStat(int value) : this()
    {
        baseValue = value;
    }

    //Only recalculates the stat if it's been flagged as "dirty."
    public virtual int Value
    {
        get
        {
            //Only recalculates the value if it's been set to "dirty" or doesn't match our last recorded Base Value.
            if (isDirty || lastBaseValue != baseValue)
            {
                lastBaseValue = baseValue;
                _value = CalculateFinalValue();
                isDirty = false;
            } 
                return _value;
        }
    }

    //Adds modifiers to a stat.
    public virtual void AddModifier(StatModifier mod)
    {
        //Sets the stat to dirty, so the game knows to recalculate it.
        isDirty = true;
        statModifiers.Add(mod);
        statModifiers.Sort(CompareModifierPriority);
    }

    //Removes modifiers from a stat.
    public virtual bool RemoveModifier(StatModifier mod)
    {
        if (statModifiers.Remove(mod))
        {
            //If the modifier is removed successfully, sets the stat to dirty, so the game knows to recalculate it.
            isDirty = true;
            return true;
        }
        return false;
    }

    //Removes all modifiers from a specific source (I.E, a piece of equipment, or a condition).
    public virtual bool RemoveModifiersBySource(object source)
    {
        bool didRemove = false;

        //"Reverse For Loop" because the list of statModifiers gets shorter each time we remove one.
        for (int i = statModifiers.Count -1; i >= 0; i--)
        {
            if (statModifiers[i].Source == source)
            {
                isDirty = true;
                didRemove = true;
                statModifiers.RemoveAt(i);
            }
        }

        return didRemove;
    }

    //Returns the modified Stat.
    protected virtual int CalculateFinalValue()
    {
        float currentValue = baseValue;
        float sumPercentAdd = 0;
        int finalValue = 0;
        for (int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier mod = statModifiers[i];

            if (mod.Type == StatModifier.StatModType.Flat)
                currentValue += statModifiers[i].Value;

            //If the mod is an AddMultiplier...
            else if (mod.Type == StatModifier.StatModType.AddMultiplier)
            {
                //Calculate the sum of all AddMultipliers.
                sumPercentAdd += mod.Value;

                //If we hit the end of the list, or we run out of AddModifiers, calculate the total.
                if (i + 1 >= statModifiers.Count || statModifiers[i+1].Type != StatModifier.StatModType.AddMultiplier)
                {
                    //Multiplies currentValue by the sum of all AddMultipliers, then resets the variable (in case there are more with a lower priority).
                    currentValue *= sumPercentAdd;
                    sumPercentAdd = 0;
                }
            }
            else if (mod.Type == StatModifier.StatModType.Multiplier)
                currentValue *= statModifiers[i].Value;
        }

        //Rounds the final value down to the nearest whole number.
        finalValue = (int)MathF.Floor(currentValue);

        return finalValue;
    }

    //Determines the order of priority among StatModifiers.
    protected virtual int CompareModifierPriority(StatModifier a, StatModifier b)
    {
        //Compares two StatModifiers. The one with the LOWER priority is moved SOONER in the list.
        if (a.Priority < b.Priority)
            return -1;
        else if (a.Priority > b.Priority)
            return 1;
        else return 0;
    }
}
