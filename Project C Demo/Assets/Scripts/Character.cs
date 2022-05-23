using System;
using UnityEngine;

[Serializable]
public struct Character
{
    public Sprite portrait;
    public int position;
    public string name;
    public int level;
    public int currentHP;
    public int currentEP;
    public double[] maxHP;
    public double[] maxEP;
    public double[] attack;
    public double[] magic;
    public double[] speed;
    public double[] defense;
    public double[] resistance;
    public int[] resistances;
    public AttackTag[] attacks;

    public override string ToString()
    {
        return "Character Name: " + name;
    }
}