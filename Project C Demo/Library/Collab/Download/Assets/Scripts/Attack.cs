// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

public struct Attack
{
    public Attack(string name, int power, int resistance,
                         string anim, string sound, int hitNum, int target,
                         int epCost, int apCost, int[] buff,
                         int[] debuff, int[] status, int hpRegen, int epRegen) {
        Name = name;
        Power = power;
        Resistance = resistance;
        Anim = anim;
        Sound = sound;
        HitNum = hitNum;
        Target = target;
        EPCost = epCost;
        APCost = apCost;
        Buff = buff;
        Debuff = debuff;
        Status = status;
        HPRegen = hpRegen;
        EPRegen = epRegen;
    }

    public string Name { get; }
    public int Power { get; }
    public int Resistance { get; }
    public string Anim { get; }
    public string Sound { get; }
    public int HitNum { get; }
    public int Target { get; }
    public int EPCost { get; }
    public int APCost { get; }
    public int[] Buff { get; }
    public int[] Debuff { get; }
    public int[] Status { get; }
    public int HPRegen { get; }
    public int EPRegen { get; }
}
