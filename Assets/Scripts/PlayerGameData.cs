using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGameData : MonoBehaviour
{
    public static PlayerGameData instance;

    //[Header("Main values")]

    int softGems;
    int hardGems;
    int greaterLevel;

    //[Header("Rewards")]

    int gatchaTorch = 30;
    int gatchaSoftGem = 20;
    int gatchaHardGem = 5;

    int softGemCollected = 10;
    int hardGemCollected = 3;

    //[Header("Rewards")]

    int tickBonus = 0;

    void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(gameObject);
    }

    public int GetSoftGemsCount()
    {
        return softGems;
    }

    public int GetHardGemsCount()
    {
        return hardGems;
    }

    public void AddSoftGems(int value)
    {
        softGems += value;
    }

    public void AddHardGems(int value)
    {
        hardGems += value;
    }

    // Returns true if soft gems - value is positive or equal to 0
    public bool CanBuyForSoftGems(int value)
    {
        return softGems - value >= 0;
    }

    // Returns true if hard gems - value is positive or equal to 0
    public bool CanBuyForHardGems(int value)
    {
        return hardGems - value >= 0;
    }

    public int GetGatchaTorchValue()
    {
        return gatchaTorch;
    }

    public int GetGatchaSoftGemValue()
    {
        return gatchaSoftGem;
    }

    public int GetGatchaHardGemValue()
    {
        return gatchaHardGem;
    }

    public int GetSoftGemCollectedValue()
    {
        return softGemCollected;
    }

    public int GetHardGemCollectedValue()
    {
        return hardGemCollected;
    }

    public int GetTickBonus()
    {
        return tickBonus;
    }

    public void SetTickBonus(int value)
    {
        tickBonus = value;
    }

    public int GetGreaterLevel()
    {
        return greaterLevel;
    }

    public void SetGreaterLevel(int value)
    {
        greaterLevel = value;
    }
}
