using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceGods : MonoBehaviour
{
    //Limit to only one dice roller at a time.
    public static DiceGods instance;

    // Start is called before the first frame update
    void Start()
    {
        //Check if the Player already exists. If it does, delete this player.
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        //Don't destroy the player on scene changes.
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Testing 3d6 roll " + rollDice(3, 6, 0, 0, true));
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("Testing 4d6k3 roll " + RollandKeep(4, 3, 6, 0, 0, true));
        }
    }

    public int rollOneDie(int sideNum, int diceMod, bool minOne)
    {
        //Rolls a "die" with sideNum sides, then adds diceMod as a modifier.
        int sum = UnityEngine.Random.Range(1, sideNum +1) + diceMod;

        //If minOne is true and the sum is less than zero, returns 1. 
        if (minOne && sum <= 0) return 1;
        else return Mathf.Max(sum, 0);
    }

    //Roll multiple dice, adding values to each die and/or the sum.
    public int rollDice(int diceNum, int sideNum, int diceMod, int rollMod, bool minOne)
    {
        //Create an array with a number of ints equal to diceNum.
        int[] numRolls = new int[diceNum];
        int sum = 0;

        //Roll a number of dice equal to numRolls.
        foreach (int roll in numRolls)
        {
            //For each "roll" in numRolls, roll a die with sideNum sides, adding diceMod.
            numRolls[roll] = rollOneDie(sideNum, diceMod, minOne);

            sum += numRolls[roll];
        }

        //Add rollMod to the sum of all rolls.
        sum += rollMod;

        //If minOne is true and the sum is less than zero, returns 1.
        if (minOne && sum <= 0) return 1;
        else return Mathf.Max(sum, 0);
    }

    //For rolling multiple dice, but only keeping the highest/lowest outcome.
    //NOTE that this function DOES NOT support "roll five dice, keep the highest three." It returns ONE roll ONLY.
    //You can use it for advantage/disadvantage, or a Blades in the Dark style roll, but not, say, "4d6 drop the lowest."
    public int takeBestWorst(int rollNum, int diceNum, int sideNum, int diceMod, int rollMod, bool greater)
    {
        int[] sums = new int[rollNum];

        //Generate a number of dice rolls equal to numKeep.
        foreach (int roll in sums)
        {
            sums[roll] = rollDice(diceNum, sideNum, diceMod, rollMod, false);
        }

        //This result does not need to account for negative sums because rollDice() already does.
        if (greater) return Mathf.Max(sums);
        else return Mathf.Min(sums);
    }

    //Rolls several dice and returns the sum of only some of those dice (keepNum). More memory-intensive than takeBestWorst, if it matters.
    public int RollandKeep(int diceNum, int keepNum, int sideNum, int diceMod, int rollMod, bool greater)
    {
        int total = 0;
        List<int> sums = new List<int>();

        //Generate a number of dice rolls equal to diceNum.
        for (int i = 0; i < diceNum; i++)
        {
            sums.Add(rollOneDie(sideNum, diceMod, false));
            Debug.Log(sums[i]);
        }

        //Sorts the results from lowest to highest.
        sums.Sort();
 
        //Removes (diceNum - keepNum) entries, starting with the first (lowest) one.
        if (greater)
        {
            sums.RemoveRange(0, diceNum - keepNum);
        } else
        {
            //Reverses the list, then starts removing from the front (which is now the highest result).
            sums.Reverse();
            sums.RemoveRange(0, diceNum - keepNum);
        }

        for (int i = 0; i < sums.Count; i++)
        {
            
            total += sums[i];
        }

        return total;

    }

    //Return a "Minimized" or "Maximized" result
    public int minmaxRoll(int diceNum, int sideNum, int diceMod, int rollMod, bool max, bool minOne)
    {
        int sum;

        if (max)
        {
            //"Maximize" the roll by adding the dice mod to the number of sides, multiply by the number of dice, then add the roll modifier.
            sum = ((sideNum + diceMod) * diceNum) + rollMod;
        }

        else
        {
            //"Minimize" the roll by multiplying 1 + the dice modifier (minimum 1) by the number of dice. Then add the roll modifier.
            sum = (Mathf.Max(1, (1 + diceMod)) * diceNum) + rollMod;
        }

        //Return the maximized or minimized roll result, one (if minOne), or zero.
        if (minOne) return Mathf.Max(sum, 1);
        else return Mathf.Max(sum, 0);
    }
}