using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerBehaviourController
{
    public List<Attack> playerAttacks;
    public EnemyFXController enemyFXController;
    public PlayerFXController playerFXController;

    public Image char1Health;
    public Image char1HealthConsume;
    public Image char1EP;
    public Image char2Health;
    public Image char2HealthConsume;
    public Image char2EP;
    public Image char3Health;
    public Image char3HealthConsume;
    public Image char3EP;
    public Image char4Health;
    public Image char4HealthConsume;
    public Image char4EP;

    public GameObject[] enemies = new GameObject[4];
    public Button[] enemyClickable = new Button[4];

    public Button[] portraitClickable = new Button[4];
    public Image[] reticles = new Image[4];
    public TextMeshProUGUI[] enemyDamageNums = new TextMeshProUGUI[4];
    public GameObject[] weakIndicators = new GameObject[4];

    public Character targetedEnemy;
    public Character targetedSupportCharacter;
    public int targetedIndex;
    public int targetedSupportIndex;

    public GameObject explanationBack;
    public GameObject battleBack;
    public Button attack;
    public Button analyze;
    public Button defend;
    public Button escape;
    public List<Button> battleButtons = new List<Button>();

    public GameObject attackBack;
    public Button attack1;
    public Button attack2;
    public Button attack3;
    public Button backButton;
    public List<Button> attackButtons = new List<Button>();

    public GameObject specialBack;
    public Button tech1;
    public Button tech2;
    public Button tech3;
    public Button tech4;
    public Button tech5;
    public Button tech6;
    public List<Button> techButtons = new List<Button>();
    public Text tech1Text;
    public Text tech2Text;
    public Text tech3Text;
    public Text tech4Text;
    public Text tech5Text;
    public Text tech6Text;
    public List<Text> techButtonText = new List<Text>();
    public Button bMag1;
    public Button bMag2;
    public Button bMag3;
    public Button bMag4;
    public Button bMag5;
    public Button bMag6;
    public List<Button> bMagButtons = new List<Button>();
    public Text bMag1Text;
    public Text bMag2Text;
    public Text bMag3Text;
    public Text bMag4Text;
    public Text bMag5Text;
    public Text bMag6Text;
    public List<Text> bMagButtonText = new List<Text>();
    public Button wMag1;
    public Button wMag2;
    public Button wMag3;
    public Button wMag4;
    public Button wMag5;
    public Button wMag6;
    public List<Button> wMagButtons = new List<Button>();
    public Text wMag1Text;
    public Text wMag2Text;
    public Text wMag3Text;
    public Text wMag4Text;
    public Text wMag5Text;
    public Text wMag6Text;
    public List<Text> wMagButtonText = new List<Text>();
    public Button item1;
    public Button item2;
    public Button item3;
    public Button item4;
    public Button item5;
    public Button item6;
    public List<Button> itemButtons = new List<Button>();
    public Text item1Text;
    public Text item2Text;
    public Text item3Text;
    public Text item4Text;
    public Text item5Text;
    public Text item6Text;
    public List<Text> itemButtonText = new List<Text>();

    public PlayerBehaviourController(){
        playerAttacks = new List<Attack>();
        enemyFXController = new EnemyFXController();
        playerFXController = new PlayerFXController();
    }

    // Start is called before the first frame update
    void Start()
    {
        //enemyFXController = new EnemyFXController();
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
        Debug.Log(result.target);
        return result;
    }

    public void NormAttack(int buttonNum, Character character) {
        Attack temp = new Attack();
        temp = playerAttacks.Find(x => x.name == character.attacks[buttonNum].name);
        Debug.Log(character.attacks[buttonNum].name);
        if(temp.anim == null){
            temp = playerAttacks.Find(x => x.name == "NULL");
        }
        Debug.Log("Animation: " + temp.anim);
        enemyFXController.Attack(temp.anim, targetedIndex, temp.resistance);
    }

    public void Attack(string attackName){
        Attack temp = new Attack();
        temp = playerAttacks.Find(x => x.name == attackName);
        Debug.Log(attackName);
        if(temp.anim == null){
            temp = playerAttacks.Find(x => x.name == "NULL");
        }
        Debug.Log("Animation: " + temp.anim);
        enemyFXController.Attack(temp.anim, targetedIndex, temp.resistance);
    }

    public void Support(string attackName){
        Attack temp = new Attack();
        temp = playerAttacks.Find(x => x.anim == attackName);
        Debug.Log(attackName);
        if(temp.anim == null){
            temp = playerAttacks.Find(x => x.anim == "NULL");
        }
        Debug.Log("Animation: " + temp.anim);
        playerFXController.Attack(temp.anim, targetedSupportIndex, temp.resistance);
    }

    public void ChangeTarget(int position, Character enemy){
        foreach(Image reticle in reticles){
            reticle.gameObject.SetActive(false);
        }
        reticles[position].gameObject.SetActive(true);
        targetedEnemy = enemy;
        targetedIndex = position;
        Debug.Log(enemy.name + " in position " + position);
    }

    public void ChangeSupportTarget(int position, Character player){
        // foreach(Image reticle in reticles){
        //     reticle.gameObject.SetActive(false);
        // }
        // reticles[position].gameObject.SetActive(true);
        targetedSupportCharacter = player;
        targetedSupportIndex = position;
        Debug.Log(player.name + " in position " + position);
    }

    public void SwitchAttackMenu(){
        Debug.Log("Attack menu is opening");
        attackBack.gameObject.SetActive(true);
        foreach(Button button in attackButtons){
            button.gameObject.SetActive(true);
        }

        battleBack.gameObject.SetActive(false);
        // foreach(Button button in battleButtons){
        //     button.gameObject.SetActive(false);
        // } 
    }

    public void OpenSpecialsMenu(){
        Debug.Log("Tech menu is opening");
        specialBack.gameObject.SetActive(true);
        // foreach(Button button in techButtons){
        //     button.gameObject.SetActive(true);
        // }
        ValidateVisibility();

        attack1.gameObject.SetActive(false);
        attack2.gameObject.SetActive(false);
        attack3.gameObject.SetActive(false);
    }

    public void Close(){
        if(specialBack.gameObject.activeSelf){
            Debug.Log("Tech menu is closing");
            specialBack.gameObject.SetActive(false);
            // foreach(Button button in techButtons){
            //     button.gameObject.SetActive(false);
            // }
            attack1.gameObject.SetActive(true);
            attack2.gameObject.SetActive(true);
            attack3.gameObject.SetActive(true);
        }else{
            Debug.Log("Attack menu is closing");
            attackBack.gameObject.SetActive(false);
            // foreach(Button button in attackButtons){
            //     button.gameObject.SetActive(false);
            // }

            battleBack.gameObject.SetActive(true);
            // foreach(Button button in battleButtons){
            //     button.gameObject.SetActive(true);
            // } 
        }
    }

    public void ValidateVisibility(){
        for(int i = 0;i < techButtons.Count;i++){
            if(techButtonText[i].text == "Blank"){
                techButtons[i].gameObject.SetActive(false);
            }
        }
        for(int i = 0;i < bMagButtons.Count;i++){
            if(bMagButtonText[i].text == "Blank"){
                bMagButtons[i].gameObject.SetActive(false);
            }
        }
        for(int i = 0;i < wMagButtons.Count;i++){
            if(wMagButtonText[i].text == "Blank"){
                wMagButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
