using System;
using UnityEngine;

[Serializable]
public struct AttackTag{
    public string name;
    public int level;
    public int weaponUsed;

    public override string ToString()
    {
        return "Attack: " + name + "\nRequired Level: " + level;
    }
}