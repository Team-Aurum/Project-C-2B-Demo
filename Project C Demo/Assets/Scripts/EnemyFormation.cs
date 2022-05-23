using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct EnemyFormation{
    public List<CharacterTag> initialEnemies;
    public List<CharacterTag> reinforcements;

    public override string ToString()
    {
        return "Initial Enemy: " + initialEnemies[0].name;
    }
}