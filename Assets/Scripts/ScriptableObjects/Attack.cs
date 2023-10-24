using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack
{
    public enum AtkTypes {Attack, SaveEffect, CertainHit }
    public AtkTypes AtkType;

    public int ATKMinRange;
    public int ATKMaxRange;
    public bool rangePenalties;

    public CharacterStat ATKBonus;

    public Effect[] HitEffects;
    public Effect[] MissEffects;

    public void RollToHit(StatBlock attacker, StatBlock target)
    {
        int roll = DiceGods.instance.rollOneDie(20, ATKBonus.Value, false);

        if (roll >= target.Defense.Value)
            Hit();
        else Miss();
    }

    public void Hit()
    {

    }

    public void Miss()
    {

    }
}
