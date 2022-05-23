using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI2B : MonoBehaviour, IEnemyAI
{
    string name = "2B";
    int internalTurn = 1;
    int R010ChargeLevel = 1;
    int A090ChargeLevel = 1;
    int A150ChargeLevel = 1;
    int A130ChargeLevel = 1;
    int[] lightCombo = {0,0,0,0,0,0,0};
    int[] heavyCombo = {1,1,1};
    int[] podEvasion = {2,3};
    int[] focusedGatling = {2,2};
    int[] R010Charge = {7,7};
    int[] R010ChargeHit = {7,-1}; //-1 represents R010 damage
    int[] R010DoubleHit = {-1,-1};
    int[] R010ToGatling = {-1,2,2};
    int[] A090Charge = {11,11};
    int[] A090ChargeHit = {11,-2}; //-2 represents A090 damage
    int[] A090DoubleHit = {-2,-2};
    int[] A090ToGatling = {-2,2,2};
    int[] A150Charge = {15,15};
    int[] A150ChargeHit = {15,-3}; //-3 represents A150 damage
    int[] A150DoubleHit = {-3,-3};
    int[] A130Charge = {19,19};
    int[] A130ChargeHit = {19,-4}; //-4 represents A130 damage
    int[] A130DoubleHit = {-4,-4};

    public AI2B(){}
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
        Debug.Log("Things are working. 2B is now attacking. Internal Turn: " + internalTurn);
        switch(internalTurn){
            case 1: //Execute Light Attack x7
                foreach(int index in lightCombo){
                    temp.Add(character.attacks[index]);
                }
                internalTurn++;
                break;
            case 2: //Execute R010 Charge x2
                foreach(int index in R010Charge){
                    temp.Add(character.attacks[index]);
                    if(index == 7 && R010ChargeLevel < 3){
                        R010ChargeLevel++;
                    }
                }
                internalTurn++;
                break;
            case 3: //Execute R010: Laser x2
                foreach(int index in R010DoubleHit){
                    //TODO: Potentially isolate this into its own separate function that gets called each time to cut out on redundant code
                    if(index == -1){
                        temp.Add(character.attacks[3 + R010ChargeLevel]);
                        R010ChargeLevel = 1;
                    }else{
                        temp.Add(character.attacks[index]);
                        if(index == -1 && R010ChargeLevel < 3){
                            R010ChargeLevel++;
                        }
                    }
                }
                internalTurn = 1;
                break;
            default:
                break;
        }
        return temp;
    }

    public List<AttackTag> ChooseBackup(Character character, int remainingAP){
        List<AttackTag> temp = new List<AttackTag>();
        while(remainingAP > 0){
            if(remainingAP > 2){
                temp.Add(character.attacks[3]);
                remainingAP -= 3;
                Debug.Log(character.attacks[3]);
            }else if(remainingAP == 2){
                temp.Add(character.attacks[1]);
                remainingAP -= 2;
                Debug.Log(character.attacks[1]);
            }else{
                temp.Add(character.attacks[0]);
                remainingAP -= 1;
                Debug.Log(character.attacks[0]);
            }
        }
        return temp;
    }

    public int ChooseTarget(List<Character> characters, List<Character> allCharacters){
        for(int i = 0;i < characters.Count;i++){
            Debug.Log(characters[i].name);
            if(!(allCharacters.Find(x => x.name == characters[i].name).name == null)){
                Debug.Log(allCharacters.Find(x => x.name == characters[i].name).name == null);
                Debug.Log(i+1);
                return i + 1;
            }
        }
        return 1;
    }

    public int[] CalculateAPCosts(List<AttackTag> attackTags, Character character){
        int[] temp = new int[7];
        int count = 0;
        foreach(AttackTag attackTag in attackTags){
            if(character.attacks[0].name == attackTag.name){
                temp[count] = 1;
                count++;
            }else if(character.attacks[1].name == attackTag.name){
                temp[count] = 2;
                count++;
            }else{
                temp[count] = 3;
                count++;
            }
        }
        // int[] temp = {1,1,1,1,1,1,1};
        return temp;
    }

    public string GetName(){
        return name;
    }
}
