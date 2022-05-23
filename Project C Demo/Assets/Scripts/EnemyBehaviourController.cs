using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyBehaviourController
{
    public List<Attack> enemyAttacks = new List<Attack>();
    public PlayerFXController playerFXController;
    public EnemyFXController enemyFXController;

    public TextMeshProUGUI[] playerDamageNums = new TextMeshProUGUI[4];
    public GameObject[] weakIndicators = new GameObject[4];

    public Image enemy1Health;
    public Image enemy1HealthConsume;
    public Image enemy2Health;
    public Image enemy2HealthConsume;
    public Image enemy3Health;
    public Image enemy3HealthConsume;
    public Image enemy4Health;
    public Image enemy4HealthConsume;

    public Character targetedCharacter;
    public int targetedIndex;
    public int supportTargetedIndex;
    public int[] APCosts;

    public EnemyBehaviourController(){
        playerFXController = new PlayerFXController();
        enemyFXController = new EnemyFXController();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Attack PullAttackData(string name) {
        string temp;
        Attack result = new Attack();
        TextAsset attackData = Resources.Load("attackData/" + name.ToLower()) as TextAsset;
        if(attackData == null){
            attackData = Resources.Load("attackData/blank") as TextAsset;
        }  
        temp = attackData.text;
        result = JsonUtility.FromJson<Attack>(temp);
        Debug.Log(result);
        return result;
    }

    public void Attack(string attackName){
        Attack temp = new Attack();
        temp = enemyAttacks.Find(x => x.anim == attackName);
        Debug.Log(attackName);
        if(temp.anim == null){
            temp = enemyAttacks.Find(x => x.anim == "NULL");
        }
        Debug.Log("Animation: " + temp.anim);
        playerFXController.Attack(temp.anim, targetedIndex, temp.resistance);
    }

    public void Support(string attackName){
        Attack temp = new Attack();
        temp = enemyAttacks.Find(x => x.anim == attackName);
        Debug.Log(attackName);
        if(temp.anim == null){
            temp = enemyAttacks.Find(x => x.anim == "NULL");
        }
        Debug.Log("Animation: " + temp.anim);
        enemyFXController.Attack(temp.anim, supportTargetedIndex, temp.resistance);
    }

    public List<AttackTag> CalculateTurn(IEnemyAI enemyAIScript, Character character, List<Character> characters, List<Character> allCharacters){
        Debug.Log("Currently Running through CalculateTurn");
        targetedIndex = enemyAIScript.ChooseTarget(characters, allCharacters); //TODO: Implement targeting
        supportTargetedIndex = 1; //TODO: Implement support skill targeting
        return enemyAIScript.ChooseBehaviorChain(character);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
