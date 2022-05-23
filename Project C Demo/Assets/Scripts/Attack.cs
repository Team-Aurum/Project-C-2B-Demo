using System;
using UnityEngine;

[Serializable]
public struct Attack
{
    public string name;
    public int power;
    public int resistance;
    public int weaponUsed;
    public string anim;
    public string sound;
    public int hitNum;
    public int[] target;
    public int epCost;
    public int apCost;
    public int[][] buff;
    public int[][] debuff;
    public Status[] status;
    public int hpRegen;
    public int epRegen;

    public override string ToString()
    {
        return "Attack Name: " + name;
    }
}