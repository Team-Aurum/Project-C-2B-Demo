using System;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyAI{
    List<AttackTag> ChooseBehaviorChain(Character character);
    List<AttackTag> ChooseBackup(Character character, int remainingAP);
    int ChooseTarget(List<Character> characters, List<Character> allCharacters);
    int[] CalculateAPCosts(List<AttackTag> attackTags, Character character);
    string GetName();
}