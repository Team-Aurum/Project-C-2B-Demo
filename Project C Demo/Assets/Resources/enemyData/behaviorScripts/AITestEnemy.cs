using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITestEnemy : MonoBehaviour, IEnemyAI
{
    string name = "TestEnemy";
    int[] lightCombo = {0,0,0,0,0,0,0};

    public AITestEnemy(){}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Tester(){
        Debug.Log("This script is being loaded based on a string!!!");
    }

    public List<AttackTag> ChooseBehaviorChain(Character character){
        List<AttackTag> temp = new List<AttackTag>();
        Debug.Log("Things are working. TestEnemy is now attacking.");
        foreach(int index in lightCombo){
            temp.Add(character.attacks[index]);
        }
        return temp;
    }

    public List<AttackTag> ChooseBackup(Character character, int remainingAP){
        List<AttackTag> temp = new List<AttackTag>();
        while(remainingAP > 0){
            if(remainingAP > 2){
                temp.Add(character.attacks[28]);
            }else if(remainingAP == 2){
                temp.Add(character.attacks[1]);
            }else{
                temp.Add(character.attacks[0]);
            }
        }
        return temp;
    }

    public int ChooseTarget(List<Character> characters, List<Character> allCharacters){
        return 1;
    }
    
    public int[] CalculateAPCosts(List<AttackTag> attackTags, Character character){
        int[] temp = {1,1,1,1,1,1,1};
        return temp;
    }

    public string GetName(){
        return name;
    }
}
