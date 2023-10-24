using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBlock : ScriptableObject
{
    public string charName;
    public enum creatureTypes {Aberration, Beast, Celestial, Construct, Dragon, Elemental, Fey, Fiend, Giant, Humanoid, Monstrosity, Ooze, Plant, Undead }
    creatureTypes creatureType;
    public enum alignments { Harmony, Balance, Discord}
    alignments Alignment;

    public int Level;

    #region VitalStats
    public int hitDie;
    public CharacterStat currentHP, maxHP;
    public int Initiative = 4;
    public int Perception = 1;
    public int Noise = 2;
    public CharacterStat Defense;
    public CharacterStat[] SavingThrows = new CharacterStat[6];
    public CharacterStat Movement;
    public CharacterStat atkBonus;
    public Trait[] Traits;

    private void Awake()
    {
        Initialize();
    }
    private void Initialize()
    {
        maxHP = new CharacterStat(Level * hitDie);
        currentHP = maxHP;
        SavingThrows = new CharacterStat[6];

    }

    #endregion VitalStats
}
