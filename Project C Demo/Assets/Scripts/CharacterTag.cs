using System;
using UnityEngine;

[Serializable]
public struct CharacterTag{
    public string name;
    public int level;

    public override string ToString()
    {
        return "Level " + level + " " + name;
    }
}