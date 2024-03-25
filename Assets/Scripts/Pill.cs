using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pill : MonoBehaviour
{
    [SerializeField] private float rageTimeIncrease;
    [SerializeField] private int healthIncrease;
    [SerializeField] private float speedMultiplyer;
    [SerializeField] private int damageIncrease;

    public float GetTime()
    {
        return rageTimeIncrease;
    }

    public int GetHealth()
    {
        return healthIncrease;
    }

    public float GetSpeed()
    {
        return speedMultiplyer;
    }

    public int GetDamage()
    {
        return damageIncrease;
    }
}
