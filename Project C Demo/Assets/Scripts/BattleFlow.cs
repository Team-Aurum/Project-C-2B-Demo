using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BattleFlow : MonoBehaviour
{
    private IEnumerator coroutine;

    //Characters
    public List<Character> allCharacters;
    public List<Character> playerCharacters;
    public List<Character> enemies;
    public List<CharacterTag> activePlayers;
    public List<CharacterTag> activeEnemies;
    public List<IEnemyAI> activeEnemyAI = new List<IEnemyAI>();
    // public List<string> displayedTechs; 
    public List<IStatusEffect>[] playerStatusEffects = new List<IStatusEffect>[4];
    public List<IStatusEffect>[] enemyStatusEffects = new List<IStatusEffect>[4];

    //Turn Shenanigans
    public List<Character> turnOrder;
    public List<int> TPCount = new List<int>();
    public int turn;
    public int cumulativeTurns;
    public int currentAP;
    public bool weaknessHit;
    public int maxAP;
    public Image APBar;
    public Image APBarOverage;
    public TextMeshProUGUI APCount;
    public Character activeCharacter;
    public int activeIndex;

    //Turn Order Display
    public Image turnOrder1Back;
    public Image turnOrder1;
    public CanvasGroup turnOrder1Opacity;
    public Image turnOrder2Back;
    public Image turnOrder2;
    public CanvasGroup turnOrder2Opacity;
    public Image turnOrder3Back;
    public Image turnOrder3;
    public CanvasGroup turnOrder3Opacity;

    //Player UI Shenanigans
    public List<Sprite> portraits;
    // //TODO: This may not be optimal
    // public Sprite arataSprite;
    // public Sprite diyaSprite;
    public GameObject[] characterPortraits = new GameObject[4];
    public Text[][] charData = new Text[4][];
    public Image[][] charImgData = new Image[4][];

    //Enemy Display
    public Image[] enemyImgData = new Image[4];

    public TextMeshProUGUI attackText;
    public GameObject attackTextBack;
    public PlayerBehaviourController playerController;
    public EnemyBehaviourController enemyController;
    public string[] stems = {"sut", "gusut", "txim", "gutxim", "iz", "giz", "haiz", "guhaiz"};

    public AudioSource audioSource;
    public AudioClip loopAudio;

    //TODO: Remove when all testing is done
    public Button tester;

    // Start is called before the first frame update
    void Start()
    {

        audioSource = GameObject.Find("UI").GetComponent<AudioSource>();
        loopAudio = Resources.Load<AudioClip>("Music/battleLoop");

        // tester = GameObject.Find("Button").GetComponent<Button>();
        // tester.onClick.AddListener(delegate{Test();});
        playerController = new PlayerBehaviourController();
        enemyController = new EnemyBehaviourController();

        APBar = GameObject.Find("AP Bar").GetComponent<Image>();
        APBarOverage = GameObject.Find("AP Bar Overage").GetComponent<Image>();
        APCount = GameObject.Find("APCount").GetComponent<TextMeshProUGUI>();

        attackTextBack = GameObject.Find("Move Display");
        attackText = GameObject.Find("MoveText").GetComponent<TextMeshProUGUI>();

        //Turn Order
        turnOrder1Back = GameObject.Find("Turn Order 1").GetComponent<Image>();
        turnOrder1 = GameObject.Find("TPortrait1").GetComponent<Image>();
        turnOrder1Opacity = GameObject.Find("Turn Order 1").GetComponent<CanvasGroup>();
        turnOrder2Back = GameObject.Find("Turn Order 2").GetComponent<Image>();
        turnOrder2 = GameObject.Find("TPortrait2").GetComponent<Image>();
        turnOrder2Opacity = GameObject.Find("Turn Order 2").GetComponent<CanvasGroup>();
        turnOrder3Back = GameObject.Find("Turn Order 3").GetComponent<Image>();
        turnOrder3 = GameObject.Find("TPortrait3").GetComponent<Image>();
        turnOrder3Opacity = GameObject.Find("Turn Order 3").GetComponent<CanvasGroup>();

        //Enemy Display
        playerController.enemies[0] = GameObject.Find("Enemy 1");
        playerController.enemies[1] = GameObject.Find("Enemy 2");
        playerController.enemies[2] = GameObject.Find("Enemy 3");
        playerController.enemies[3] = GameObject.Find("Enemy 4");

        playerController.enemyClickable[0] = playerController.enemies[0].GetComponent<Button>();
        playerController.enemyClickable[1] = playerController.enemies[1].GetComponent<Button>();
        playerController.enemyClickable[2] = playerController.enemies[2].GetComponent<Button>();
        playerController.enemyClickable[3] = playerController.enemies[3].GetComponent<Button>();

        playerController.reticles[0] = GameObject.Find("TargetIcon1").GetComponent<Image>();
        playerController.reticles[1] = GameObject.Find("TargetIcon2").GetComponent<Image>();
        playerController.reticles[2] = GameObject.Find("TargetIcon3").GetComponent<Image>();
        playerController.reticles[3] = GameObject.Find("TargetIcon4").GetComponent<Image>();

        playerController.enemyDamageNums[0] = GameObject.Find("EnemyDamageNum1").GetComponent<TextMeshProUGUI>();        
        playerController.enemyDamageNums[1] = GameObject.Find("EnemyDamageNum2").GetComponent<TextMeshProUGUI>();
        playerController.enemyDamageNums[2] = GameObject.Find("EnemyDamageNum3").GetComponent<TextMeshProUGUI>();        
        playerController.enemyDamageNums[3] = GameObject.Find("EnemyDamageNum4").GetComponent<TextMeshProUGUI>();

        playerController.weakIndicators[0] = GameObject.Find("EnemyWeak1");
        playerController.weakIndicators[1] = GameObject.Find("EnemyWeak2");
        playerController.weakIndicators[2] = GameObject.Find("EnemyWeak3");
        playerController.weakIndicators[3] = GameObject.Find("EnemyWeak4");

        enemyController.playerDamageNums[0] = GameObject.Find("PlayerDamageNum1").GetComponent<TextMeshProUGUI>();        
        enemyController.playerDamageNums[1] = GameObject.Find("PlayerDamageNum2").GetComponent<TextMeshProUGUI>();
        enemyController.playerDamageNums[2] = GameObject.Find("PlayerDamageNum3").GetComponent<TextMeshProUGUI>();        
        enemyController.playerDamageNums[3] = GameObject.Find("PlayerDamageNum4").GetComponent<TextMeshProUGUI>();

        enemyController.weakIndicators[0] = GameObject.Find("PlayerWeak1");
        enemyController.weakIndicators[1] = GameObject.Find("PlayerWeak2");
        enemyController.weakIndicators[2] = GameObject.Find("PlayerWeak3");
        enemyController.weakIndicators[3] = GameObject.Find("PlayerWeak4");

        enemyController.enemy1Health = GameObject.Find("Enemy1 HP Back").GetComponentsInChildren<Image>()[2];
        enemyController.enemy1HealthConsume = GameObject.Find("Enemy1 HP Back").GetComponentsInChildren<Image>()[1];
        enemyController.enemy2Health = GameObject.Find("Enemy2 HP Back").GetComponentsInChildren<Image>()[2];
        enemyController.enemy2HealthConsume = GameObject.Find("Enemy2 HP Back").GetComponentsInChildren<Image>()[1];
        enemyController.enemy3Health = GameObject.Find("Enemy3 HP Back").GetComponentsInChildren<Image>()[2];
        enemyController.enemy3HealthConsume = GameObject.Find("Enemy3 HP Back").GetComponentsInChildren<Image>()[1];
        enemyController.enemy4Health = GameObject.Find("Enemy4 HP Back").GetComponentsInChildren<Image>()[2];
        enemyController.enemy4HealthConsume = GameObject.Find("Enemy4 HP Back").GetComponentsInChildren<Image>()[1];   

        playerController.enemyFXController.anim1 = GameObject.Find("EnemyAnim1").GetComponent<Animator>();
        playerController.enemyFXController.anim2 = GameObject.Find("EnemyAnim2").GetComponent<Animator>();
        playerController.enemyFXController.anim3 = GameObject.Find("EnemyAnim3").GetComponent<Animator>();
        playerController.enemyFXController.anim4 = GameObject.Find("EnemyAnim4").GetComponent<Animator>();

        playerController.playerFXController.anim1 = GameObject.Find("PlayerAnim1").GetComponent<Animator>();
        playerController.playerFXController.anim2 = GameObject.Find("PlayerAnim2").GetComponent<Animator>();
        playerController.playerFXController.anim3 = GameObject.Find("PlayerAnim3").GetComponent<Animator>();
        playerController.playerFXController.anim4 = GameObject.Find("PlayerAnim4").GetComponent<Animator>();

        playerController.playerFXController.anim1Sprite = GameObject.Find("PlayerAnim1").GetComponent<SpriteRenderer>();
        playerController.playerFXController.anim2Sprite = GameObject.Find("PlayerAnim2").GetComponent<SpriteRenderer>();
        playerController.playerFXController.anim3Sprite = GameObject.Find("PlayerAnim3").GetComponent<SpriteRenderer>();
        playerController.playerFXController.anim4Sprite = GameObject.Find("PlayerAnim4").GetComponent<SpriteRenderer>();

        enemyImgData[0] = GameObject.Find("Enemy 1").GetComponent<Image>();
        enemyImgData[1] = GameObject.Find("Enemy 2").GetComponent<Image>();
        enemyImgData[2] = GameObject.Find("Enemy 3").GetComponent<Image>();    
        enemyImgData[3] = GameObject.Find("Enemy 4").GetComponent<Image>();

        //Character Cards
        playerController.char1Health = GameObject.Find("Character 1").GetComponentsInChildren<Image>()[6];
        playerController.char1HealthConsume = GameObject.Find("Character 1").GetComponentsInChildren<Image>()[4];
        playerController.char1EP = GameObject.Find("Character 1").GetComponentsInChildren<Image>()[7];
        playerController.char2Health = GameObject.Find("Character 2").GetComponentsInChildren<Image>()[6];
        playerController.char2HealthConsume = GameObject.Find("Character 2").GetComponentsInChildren<Image>()[4];
        playerController.char2EP = GameObject.Find("Character 2").GetComponentsInChildren<Image>()[7];
        playerController.char3Health = GameObject.Find("Character 3").GetComponentsInChildren<Image>()[6];
        playerController.char3HealthConsume = GameObject.Find("Character 3").GetComponentsInChildren<Image>()[4];
        playerController.char3EP = GameObject.Find("Character 3").GetComponentsInChildren<Image>()[7];
        playerController.char4Health = GameObject.Find("Character 4").GetComponentsInChildren<Image>()[6];
        playerController.char4HealthConsume = GameObject.Find("Character 4").GetComponentsInChildren<Image>()[4];
        playerController.char4EP = GameObject.Find("Character 4").GetComponentsInChildren<Image>()[7];

        characterPortraits[0] = GameObject.Find("Character 1");
        charData[0] = GameObject.Find("Character 1").GetComponentsInChildren<Text>();
        charImgData[0] = GameObject.Find("Character 1").GetComponentsInChildren<Image>();
        characterPortraits[1] = GameObject.Find("Character 2");
        charData[1] = GameObject.Find("Character 2").GetComponentsInChildren<Text>();
        charImgData[1] = GameObject.Find("Character 2").GetComponentsInChildren<Image>();
        characterPortraits[2] = GameObject.Find("Character 3");
        charData[2] = GameObject.Find("Character 3").GetComponentsInChildren<Text>();
        charImgData[2] = GameObject.Find("Character 3").GetComponentsInChildren<Image>();
        characterPortraits[3] = GameObject.Find("Character 4");
        charData[3] = GameObject.Find("Character 4").GetComponentsInChildren<Text>();
        charImgData[3] = GameObject.Find("Character 4").GetComponentsInChildren<Image>();

        playerController.portraitClickable[0] = GameObject.Find("Character 1").GetComponentInChildren<Button>();
        playerController.portraitClickable[1] = GameObject.Find("Character 2").GetComponentInChildren<Button>();
        playerController.portraitClickable[2] = GameObject.Find("Character 3").GetComponentInChildren<Button>();
        playerController.portraitClickable[3] = GameObject.Find("Character 4").GetComponentInChildren<Button>();

        enemyController.playerFXController.anim1 = GameObject.Find("PlayerAnim1").GetComponent<Animator>();
        enemyController.playerFXController.anim2 = GameObject.Find("PlayerAnim2").GetComponent<Animator>();
        enemyController.playerFXController.anim3 = GameObject.Find("PlayerAnim3").GetComponent<Animator>();
        enemyController.playerFXController.anim4 = GameObject.Find("PlayerAnim4").GetComponent<Animator>();

        enemyController.playerFXController.anim1Sprite = GameObject.Find("PlayerAnim1").GetComponent<SpriteRenderer>();
        enemyController.playerFXController.anim2Sprite = GameObject.Find("PlayerAnim2").GetComponent<SpriteRenderer>();
        enemyController.playerFXController.anim3Sprite = GameObject.Find("PlayerAnim3").GetComponent<SpriteRenderer>();
        enemyController.playerFXController.anim4Sprite = GameObject.Find("PlayerAnim4").GetComponent<SpriteRenderer>();

        enemyController.enemyFXController.anim1 = GameObject.Find("EnemyAnim1").GetComponent<Animator>();
        enemyController.enemyFXController.anim2 = GameObject.Find("EnemyAnim2").GetComponent<Animator>();
        enemyController.enemyFXController.anim3 = GameObject.Find("EnemyAnim3").GetComponent<Animator>();
        enemyController.enemyFXController.anim4 = GameObject.Find("EnemyAnim4").GetComponent<Animator>();

        //Battle UI
        playerController.explanationBack = GameObject.Find("Support Explanation");

        playerController.battleBack = GameObject.Find("Battle Menu");
        playerController.attack = GameObject.Find("Attack").GetComponent<Button>();
        playerController.battleButtons.Add(playerController.attack);
        playerController.analyze = GameObject.Find("Analyze").GetComponent<Button>();
        playerController.battleButtons.Add(playerController.analyze);
        playerController.defend = GameObject.Find("Defend").GetComponent<Button>();
        playerController.battleButtons.Add(playerController.defend);
        playerController.escape = GameObject.Find("Escape").GetComponent<Button>();
        playerController.battleButtons.Add(playerController.escape);

        playerController.attackBack = GameObject.Find("Attack Menu");
        playerController.attack1 = GameObject.Find("1AP Attack").GetComponent<Button>();
        playerController.attackButtons.Add(playerController.attack1);
        playerController.attack2 = GameObject.Find("2AP Attack").GetComponent<Button>();
        playerController.attackButtons.Add(playerController.attack2);
        playerController.attack3 = GameObject.Find("3AP Attack").GetComponent<Button>();
        playerController.attackButtons.Add(playerController.attack3);
        playerController.backButton = GameObject.Find("Cancel").GetComponent<Button>();
        playerController.attackButtons.Add(playerController.backButton);

        //SEPARATOR

        playerController.specialBack = GameObject.Find("Specials Menu");
        playerController.tech1 = GameObject.Find("Tech1").GetComponent<Button>();
        playerController.techButtons.Add(playerController.tech1);
        playerController.tech2 = GameObject.Find("Tech2").GetComponent<Button>();
        playerController.techButtons.Add(playerController.tech2);
        playerController.tech3 = GameObject.Find("Tech3").GetComponent<Button>();
        playerController.techButtons.Add(playerController.tech3);
        playerController.tech4 = GameObject.Find("Tech4").GetComponent<Button>();
        playerController.techButtons.Add(playerController.tech4);
        playerController.tech5 = GameObject.Find("Tech5").GetComponent<Button>();
        playerController.techButtons.Add(playerController.tech5);
        playerController.tech6 = GameObject.Find("Tech6").GetComponent<Button>();
        playerController.techButtons.Add(playerController.tech6);

        playerController.tech1Text = GameObject.Find("Tech1").GetComponentInChildren<Text>();
        playerController.techButtonText.Add(playerController.tech1Text);
        playerController.tech2Text = GameObject.Find("Tech2").GetComponentInChildren<Text>();
        playerController.techButtonText.Add(playerController.tech2Text);
        playerController.tech3Text = GameObject.Find("Tech3").GetComponentInChildren<Text>();
        playerController.techButtonText.Add(playerController.tech3Text);
        playerController.tech4Text = GameObject.Find("Tech4").GetComponentInChildren<Text>();
        playerController.techButtonText.Add(playerController.tech4Text);
        playerController.tech5Text = GameObject.Find("Tech5").GetComponentInChildren<Text>();
        playerController.techButtonText.Add(playerController.tech5Text);
        playerController.tech6Text = GameObject.Find("Tech6").GetComponentInChildren<Text>();
        playerController.techButtonText.Add(playerController.tech6Text);

        //SEPARATOR

        playerController.bMag1 = GameObject.Find("BMag1").GetComponent<Button>();
        playerController.bMagButtons.Add(playerController.bMag1);
        playerController.bMag2 = GameObject.Find("BMag2").GetComponent<Button>();
        playerController.bMagButtons.Add(playerController.bMag2);
        playerController.bMag3 = GameObject.Find("BMag3").GetComponent<Button>();
        playerController.bMagButtons.Add(playerController.bMag3);
        playerController.bMag4 = GameObject.Find("BMag4").GetComponent<Button>();
        playerController.bMagButtons.Add(playerController.bMag4);
        playerController.bMag5 = GameObject.Find("BMag5").GetComponent<Button>();
        playerController.bMagButtons.Add(playerController.bMag5);
        playerController.bMag6 = GameObject.Find("BMag6").GetComponent<Button>();
        playerController.bMagButtons.Add(playerController.bMag6);

        playerController.bMag1Text = GameObject.Find("BMag1").GetComponentInChildren<Text>();
        playerController.bMagButtonText.Add(playerController.bMag1Text);
        playerController.bMag2Text = GameObject.Find("BMag2").GetComponentInChildren<Text>();
        playerController.bMagButtonText.Add(playerController.bMag2Text);
        playerController.bMag3Text = GameObject.Find("BMag3").GetComponentInChildren<Text>();
        playerController.bMagButtonText.Add(playerController.bMag3Text);
        playerController.bMag4Text = GameObject.Find("BMag4").GetComponentInChildren<Text>();
        playerController.bMagButtonText.Add(playerController.bMag4Text);
        playerController.bMag5Text = GameObject.Find("BMag5").GetComponentInChildren<Text>();
        playerController.bMagButtonText.Add(playerController.bMag5Text);
        playerController.bMag6Text = GameObject.Find("BMag6").GetComponentInChildren<Text>();
        playerController.bMagButtonText.Add(playerController.bMag6Text);

        //SEPARATOR

        playerController.wMag1 = GameObject.Find("WMag1").GetComponent<Button>();
        playerController.wMagButtons.Add(playerController.wMag1);
        playerController.wMag2 = GameObject.Find("WMag2").GetComponent<Button>();
        playerController.wMagButtons.Add(playerController.wMag2);
        playerController.wMag3 = GameObject.Find("WMag3").GetComponent<Button>();
        playerController.wMagButtons.Add(playerController.wMag3);
        playerController.wMag4 = GameObject.Find("WMag4").GetComponent<Button>();
        playerController.wMagButtons.Add(playerController.wMag4);
        playerController.wMag5 = GameObject.Find("WMag5").GetComponent<Button>();
        playerController.wMagButtons.Add(playerController.wMag5);
        playerController.wMag6 = GameObject.Find("WMag6").GetComponent<Button>();
        playerController.wMagButtons.Add(playerController.wMag6);

        playerController.wMag1Text = GameObject.Find("WMag1").GetComponentInChildren<Text>();
        playerController.wMagButtonText.Add(playerController.wMag1Text);
        playerController.wMag2Text = GameObject.Find("WMag2").GetComponentInChildren<Text>();
        playerController.wMagButtonText.Add(playerController.wMag2Text);
        playerController.wMag3Text = GameObject.Find("WMag3").GetComponentInChildren<Text>();
        playerController.wMagButtonText.Add(playerController.wMag3Text);
        playerController.wMag4Text = GameObject.Find("WMag4").GetComponentInChildren<Text>();
        playerController.wMagButtonText.Add(playerController.wMag4Text);
        playerController.wMag5Text = GameObject.Find("WMag5").GetComponentInChildren<Text>();
        playerController.wMagButtonText.Add(playerController.wMag5Text);
        playerController.wMag6Text = GameObject.Find("WMag6").GetComponentInChildren<Text>();
        playerController.wMagButtonText.Add(playerController.wMag6Text);

        //Listeners
        playerController.enemyClickable[0].onClick.AddListener(delegate{playerController.ChangeTarget(1, enemies[0]);});
        playerController.enemyClickable[1].onClick.AddListener(delegate{playerController.ChangeTarget(2, enemies[1]);});
        playerController.enemyClickable[2].onClick.AddListener(delegate{playerController.ChangeTarget(3, enemies[2]);});
        playerController.enemyClickable[3].onClick.AddListener(delegate{playerController.ChangeTarget(4, enemies[3]);});

        playerController.portraitClickable[0].onClick.AddListener(delegate{if(playerCharacters.Count > 0){playerController.ChangeSupportTarget(1, playerCharacters[0]);}});
        playerController.portraitClickable[1].onClick.AddListener(delegate{if(playerCharacters.Count > 1){playerController.ChangeSupportTarget(2, playerCharacters[1]);}});
        playerController.portraitClickable[2].onClick.AddListener(delegate{if(playerCharacters.Count > 2){playerController.ChangeSupportTarget(3, playerCharacters[2]);}});
        playerController.portraitClickable[3].onClick.AddListener(delegate{if(playerCharacters.Count > 3){playerController.ChangeSupportTarget(4, playerCharacters[3]);}});

        playerController.attack.onClick.AddListener(delegate{playerController.SwitchAttackMenu();});
        // playerController.analyze.onClick.AddListener(delegate{});
        // playerController.defend.onClick.AddListener(delegate{});
        // playerController.escape.onClick.AddListener(delegate{});
        
        playerController.attack1.onClick.AddListener(delegate{/*playerController.NormAttack(0, playerCharacters[0]);*/Act(1, "");});
        playerController.attack2.onClick.AddListener(delegate{/*playerController.NormAttack(1, playerCharacters[0]);*/Act(2, "");});
        playerController.attack3.onClick.AddListener(delegate{playerController.OpenSpecialsMenu();});
        playerController.backButton.onClick.AddListener(delegate{playerController.Close();});

        playerController.tech1.onClick.AddListener(delegate{Act(3, playerController.tech1Text.text);});
        playerController.tech2.onClick.AddListener(delegate{Act(3, playerController.tech2Text.text);});
        playerController.tech3.onClick.AddListener(delegate{Act(3, playerController.tech3Text.text);});
        playerController.tech4.onClick.AddListener(delegate{Act(3, playerController.tech4Text.text);});
        playerController.tech5.onClick.AddListener(delegate{Act(3, playerController.tech5Text.text);});
        playerController.tech6.onClick.AddListener(delegate{Act(3, playerController.tech6Text.text);});

        playerController.bMag1.onClick.AddListener(delegate{Act(3, playerController.bMag1Text.text);});
        playerController.bMag2.onClick.AddListener(delegate{Act(3, playerController.bMag2Text.text);});
        playerController.bMag3.onClick.AddListener(delegate{Act(3, playerController.bMag3Text.text);});
        playerController.bMag4.onClick.AddListener(delegate{Act(3, playerController.bMag4Text.text);});
        playerController.bMag5.onClick.AddListener(delegate{Act(3, playerController.bMag5Text.text);});
        playerController.bMag6.onClick.AddListener(delegate{Act(3, playerController.bMag6Text.text);});

        //TODO: Add a heal command that directs the effect onto players instead of enemies
        playerController.wMag1.onClick.AddListener(delegate{Act(3, playerController.wMag1Text.text);});
        playerController.wMag2.onClick.AddListener(delegate{Act(3, playerController.wMag2Text.text);});
        playerController.wMag3.onClick.AddListener(delegate{Act(3, playerController.wMag3Text.text);});
        playerController.wMag4.onClick.AddListener(delegate{Act(3, playerController.wMag4Text.text);});
        playerController.wMag5.onClick.AddListener(delegate{Act(3, playerController.wMag5Text.text);});
        playerController.wMag6.onClick.AddListener(delegate{Act(3, playerController.wMag6Text.text);});

        //Preset Visibility
        attackTextBack.SetActive(false);

        playerController.reticles[0].gameObject.SetActive(false);
        playerController.reticles[1].gameObject.SetActive(false);
        playerController.reticles[2].gameObject.SetActive(false);
        playerController.reticles[3].gameObject.SetActive(false);

        playerController.enemyDamageNums[0].gameObject.SetActive(false);
        playerController.enemyDamageNums[1].gameObject.SetActive(false);
        playerController.enemyDamageNums[2].gameObject.SetActive(false);
        playerController.enemyDamageNums[3].gameObject.SetActive(false);

        enemyController.playerDamageNums[0].gameObject.SetActive(false);
        enemyController.playerDamageNums[1].gameObject.SetActive(false);
        enemyController.playerDamageNums[2].gameObject.SetActive(false);
        enemyController.playerDamageNums[3].gameObject.SetActive(false);  

        playerController.explanationBack.gameObject.SetActive(false);      

        playerController.specialBack.gameObject.SetActive(false);
        // playerController.tech1.gameObject.SetActive(false);
        // playerController.tech2.gameObject.SetActive(false);
        // playerController.tech3.gameObject.SetActive(false);
        // playerController.tech4.gameObject.SetActive(false);

        playerController.attackBack.gameObject.SetActive(false);
        // playerController.attack1.gameObject.SetActive(false);
        // playerController.attack2.gameObject.SetActive(false);
        // playerController.attack3.gameObject.SetActive(false);
        // playerController.backButton.gameObject.SetActive(false);

        foreach(TextMeshProUGUI damageNum in playerController.enemyDamageNums){
            damageNum.gameObject.SetActive(false);
        }
        foreach(GameObject weakIndicator in playerController.weakIndicators){
            weakIndicator.SetActive(false);
        }
        foreach(TextMeshProUGUI damageNum in enemyController.playerDamageNums){
            damageNum.gameObject.SetActive(false);
        }
        foreach(GameObject weakIndicator in enemyController.weakIndicators){
            weakIndicator.SetActive(false);
        }

        //TODO: Replace with something that loads players from save file.
        CharacterTag ciaran = new CharacterTag();
        ciaran.name = "Ciaran";
        ciaran.level = 99;
        CharacterTag diya = new CharacterTag();
        diya.name = "Diya";
        diya.level = 99;
        activePlayers.Add(ciaran);
        activePlayers.Add(diya);

        TextAsset enemyFormation = Resources.Load("enemyData/formationData/boss1") as TextAsset;
        EnemyFormation temp = JsonUtility.FromJson<EnemyFormation>(enemyFormation.text);
        activeEnemies = temp.initialEnemies;
        // activeEnemies.Add("diya");
        // activeEnemies.Add("diya");
        // activeEnemies.Add("ciaran");
        // activeEnemies.Add("diya");

        turn = 0;
        cumulativeTurns = 0;
        maxAP = 7;

        //TODO: Figure out how to do this with Resources.Load (shouldn't be too hard)
        // portraits.Add(arataSprite);
        // portraits.Add(diyaSprite);
        PullCharacterData();
        CalculateTurnOrder(false);
        SwitchCharacter(false);
        playerController.targetedEnemy = enemies[0];
        playerController.targetedIndex = 0;
        playerController.ChangeTarget(1, enemies[0]);
        coroutine = PlayAudioLoop();
        StartCoroutine(coroutine);
        coroutine = DisplayDamageNumber(0,0,false,true,false);
    }

    void Test(){
        Debug.Log("Testing Things!");
        Debug.Log(allCharacters.Find(x => x.name == "BRUH"));     
    }

    void PullCharacterData(){
        int[] currentHPValues = {999, 347, 876, 11};
        Character tempChar;
        for(int i = 0;i < activePlayers.Count;i++){
            TextAsset charTextData = Resources.Load("characterData/" + activePlayers[i].name) as TextAsset;
            tempChar = JsonUtility.FromJson<Character>(charTextData.text);

            //Test code. Will eventually be pulled from save data.
            tempChar.position = 0;
            tempChar.level = activePlayers[i].level;
            tempChar.currentHP = 999;
            tempChar.currentEP = 999;
            foreach(AttackTag attackTag in tempChar.attacks){
                playerController.playerAttacks.Add(playerController.PullAttackData(attackTag.name));
                // playerController.PullAttackData(attackTag.name);
                // displayedTechs.Add(attackTag.name);
            }
            // playerController.PullAttackData("Blank");
            playerCharacters.Add(tempChar);
            allCharacters.Add(tempChar);
            TPCount.Add(0);
            SetCharacterData(tempChar, i);
        }  
        for(int i = 0;i < activeEnemies.Count;i++){
            TextAsset enemyData = Resources.Load("enemyData/" + activeEnemies[i].name) as TextAsset;
            tempChar = JsonUtility.FromJson<Character>(enemyData.text);

            //TODO: Add to array
            Type generic = Type.GetType("AI" + tempChar.name);
            this.gameObject.AddComponent(generic);
            IEnemyAI genericAI = this.gameObject.GetComponent(generic) as IEnemyAI;
            activeEnemyAI.Add(genericAI);

            //Test code. See above.
            tempChar.position = 1;
            tempChar.level = activeEnemies[i].level;
            tempChar.currentHP = (int)(tempChar.maxHP[0] + (tempChar.level * tempChar.maxHP[1]));
            tempChar.currentEP = (int)(tempChar.maxEP[0] + (tempChar.level * tempChar.maxEP[1]));
            foreach(AttackTag attackTag in tempChar.attacks){
                enemyController.enemyAttacks.Add(enemyController.PullAttackData(attackTag.name));
            }
            enemies.Add(tempChar);
            allCharacters.Add(tempChar);
            TPCount.Add(0);
            SetEnemyData(tempChar, i);
        }
        if(enemies.Count < 4){
            switch(enemies.Count){
                case 1:
                    playerController.enemies[1].SetActive(false);
                    playerController.enemies[2].SetActive(false);
                    playerController.enemies[3].SetActive(false);
                    playerController.enemies[0].transform.localPosition = new Vector3(0,63,0);
                    break;
                case 2:
                    playerController.enemies[2].SetActive(false);
                    playerController.enemies[3].SetActive(false);
                    playerController.enemies[0].transform.localPosition = new Vector3(-150,63,0);
                    playerController.enemies[1].transform.localPosition = new Vector3(150,103,0);
                    break;
                case 3:
                    playerController.enemies[3].SetActive(false);
                    playerController.enemies[0].transform.localPosition = new Vector3(-300,63,0);
                    playerController.enemies[1].transform.localPosition = new Vector3(0,103,0);
                    playerController.enemies[2].transform.localPosition = new Vector3(300,63,0);
                    break;
                default:
                    break;
            }
        }
    }

    void SetEnemyData(Character character, int pos){
        Debug.Log("Setting Enemy " + character.name + " data for position " + pos);
        float tempFillHP = (float)character.currentHP / (int)(character.maxHP[0] + character.maxHP[1] * (character.level));
        switch(pos){
            case 0:
                enemyController.enemy1Health.fillAmount = tempFillHP;
                enemyController.enemy1HealthConsume.fillAmount = tempFillHP;
                break;
            case 1:
                enemyController.enemy2Health.fillAmount = tempFillHP;
                enemyController.enemy2HealthConsume.fillAmount = tempFillHP;
                break;
            case 2:
                enemyController.enemy3Health.fillAmount = tempFillHP;
                enemyController.enemy3HealthConsume.fillAmount = tempFillHP;
                break;
            case 3:
                enemyController.enemy4Health.fillAmount = tempFillHP;
                enemyController.enemy4HealthConsume.fillAmount = tempFillHP;
                break;
            default:
                break;
        }

        //Setting Enemy Models
        Sprite tempModel = Resources.Load<Sprite>("Textures/" + character.name + "Model");
        enemyImgData[pos].sprite = tempModel;
        enemyImgData[pos].rectTransform.sizeDelta = new Vector2(tempModel.rect.width/100,5.27f);
        Debug.Log("Borders: " + tempModel.rect);

        //Loading Enemy Portraits for Turn Order
        Sprite tempSprite = Resources.Load<Sprite>("Textures/" + character.name + "Portrait");
        portraits.Add(tempSprite);
    }

    void SetCharacterData(Character character, int pos){
        Debug.Log("Setting " + character.name + " data for position " + pos);
        //Setting Character Stats
        charData[pos][0].text = "" + character.currentHP + "/" + (int)(character.maxHP[0] + character.maxHP[1] * (character.level));
        charData[pos][1].text = "" + character.currentEP + "/" + (int)(character.maxEP[0] + character.maxEP[1] * (character.level));
        Debug.Log("HP Fill Amount: " + ((float)character.currentHP / (int)(character.maxHP[0] + character.maxHP[1] * (character.level))));
        float tempFillHP = (float)character.currentHP / (int)(character.maxHP[0] + character.maxHP[1] * (character.level));
        float tempFillEP = (float)character.currentEP / (int)(character.maxEP[0] + character.maxEP[1] * (character.level));
        switch(pos){
            case 0:
                playerController.char1Health.fillAmount = tempFillHP;
                playerController.char1HealthConsume.fillAmount = tempFillHP;
                playerController.char1EP.fillAmount = tempFillEP;
                break;
            case 1:
                playerController.char2Health.fillAmount = tempFillHP;
                playerController.char2HealthConsume.fillAmount = tempFillHP;
                playerController.char2EP.fillAmount = tempFillEP;
                break;
            case 2:
                playerController.char3Health.fillAmount = tempFillHP;
                playerController.char3HealthConsume.fillAmount = tempFillHP;
                playerController.char3EP.fillAmount = tempFillEP;
                break;
            case 3:
                playerController.char4Health.fillAmount = tempFillHP;
                playerController.char4HealthConsume.fillAmount = tempFillHP;
                playerController.char4EP.fillAmount = tempFillEP;
                break;
            default:
                break;
        }

        //Setting Character Portraits
        Sprite tempSprite = Resources.Load<Sprite>("Textures/" + character.name + "Portrait");
        charImgData[pos][1].sprite = tempSprite;
        portraits.Add(tempSprite);
    }

    void SetUIData(Character character){
        List<string> available = new List<string>();
        //Level Validation
        foreach(AttackTag attack in character.attacks){
            if(character.level >= attack.level){
                available.Add(attack.name);
            }else{
                available.Add("Blank");
            }
        }

        //Setting Tech Buttons
        playerController.tech1Text.text = available[2];
        playerController.tech2Text.text = available[3];
        playerController.tech3Text.text = available[4];
        playerController.tech4Text.text = available[5];
        playerController.tech5Text.text = available[6];
        playerController.tech6Text.text = available[7];

        //Setting BMag Buttons
        List<string> active = new List<string>();
        //Checks if two attacks have the same stem
        for(int i = 13;i < 28;i++){
            Debug.Log("Name: " + available[i] + "| CheckName Result: " + CheckName(available[i]));
            if(!(CheckName(available[i]) == CheckName(available[i+1])) && CheckName(available[i]) != -1){
                Debug.Log("Added " + available[i]);
                active.Add(available[i]);
            }
        }
        switch(active.Count){
            case 6:
                playerController.bMag6Text.text = active[5];
                goto case 5;
            case 5:
                playerController.bMag5Text.text = active[4];
                goto case 4;
            case 4:
                playerController.bMag4Text.text = active[3];
                goto case 3;
            case 3:
                playerController.bMag3Text.text = active[2];
                goto case 2;
            case 2:
                playerController.bMag2Text.text = active[1];
                goto case 1;
            case 1:
                playerController.bMag1Text.text = active[0];
                goto case 0;
            case 0:
                break;
        }
        switch(active.Count){
            case 0:
                playerController.bMag1Text.text = "Blank";
                goto case 1;
            case 1:
                playerController.bMag2Text.text = "Blank";
                goto case 2;
            case 2:
                playerController.bMag3Text.text = "Blank";
                goto case 3;
            case 3:
                playerController.bMag4Text.text = "Blank";
                goto case 4;
            case 4:
                playerController.bMag5Text.text = "Blank";
                goto case 5;
            case 5:
                playerController.bMag6Text.text = "Blank";
                goto case 6;
            case 6:
                break;
        }

        //Setting WMag Buttons
        active = new List<string>();
        for(int i = 28;i < available.Count-1;i++){
            if(available[i] != "Blank"){
                active.Add(available[i]);
            }
        }
        //This way it'll all start at the top and trickle down to the bottom
        Debug.Log(active.Count);
        int count = active.Count;
        if(active.Count > 6){
            count = 6;
        }
        switch(count){
            case 6:
                playerController.wMag6Text.text = active[5];
                Debug.Log("Text: " + active[5]);
                goto case 5;
            case 5:
                playerController.wMag5Text.text = active[4];
                Debug.Log("Text: " + active[4]);
                goto case 4;
            case 4:
                playerController.wMag4Text.text = active[3];
                Debug.Log("Text: " + active[3]);
                goto case 3;
            case 3:
                playerController.wMag3Text.text = active[2];
                Debug.Log("Text: " + active[2]);
                goto case 2;
            case 2:
                playerController.wMag2Text.text = active[1];
                Debug.Log("Text: " + active[1]);
                goto case 1;
            case 1:
                playerController.wMag1Text.text = active[0];
                Debug.Log("Text: " + active[0]);
                goto case 0;
            case 0:
                break;
        }
        switch(active.Count){
            case 0:
                playerController.wMag1Text.text = "Blank";
                goto case 1;
            case 1:
                playerController.wMag2Text.text = "Blank";
                goto case 2;
            case 2:
                playerController.wMag3Text.text = "Blank";
                goto case 3;
            case 3:
                playerController.wMag4Text.text = "Blank";
                goto case 4;
            case 4:
                playerController.wMag5Text.text = "Blank";
                goto case 5;
            case 5:
                playerController.wMag6Text.text = "Blank";
                goto case 6;
            case 6:
                break;
        }


        playerController.ValidateVisibility();
    }

    int CheckName(string name){
        int result = -1;
        for(int i = 0;i < stems.Length;i++){
            if(name.ToLower().Contains(stems[i])){
                result = i;
            }
        }
        return result;
    }

    //TODO: Figure out a better way of doing turn order. Maybe three at a time instead of four and also don't reset the turn count.
    void CalculateTurnOrder(bool elementRemoved){
        List<Character> TPFulfilled = new List<Character>();
        List<Character> tempStorage = new List<Character>();
        if(elementRemoved){
            Debug.Log("This is Running");
            Debug.Log("Turn: " + turn);
            Debug.Log("TurnOrder Count: " + turnOrder.Count);
            for(int i = turn%3;i < turnOrder.Count;i++){
                Debug.Log("Re-adding " + turnOrder[i].name);
                tempStorage.Add(turnOrder[i]);
            }
            turn = 0;
        }
        turnOrder = tempStorage;
        while(turnOrder.Count < 3){
            for(int i = 0;i < allCharacters.Count;i++){
                // Debug.Log(allCharacters[i].speed[0]);
                Debug.Log(TPCount.Count + " " + i);
                TPCount[i] += (int)(allCharacters[i].speed[0] + (allCharacters[i].speed[1] * allCharacters[i].level));
                if(TPCount[i] >= 99){
                    if(turnOrder.Count + TPFulfilled.Count + 1 <= 3){
                        TPFulfilled.Add(allCharacters[i]);
                        TPCount[i] -= 99;
                        Debug.Log(allCharacters[i].name + " takes a turn. TP Remaining: " + TPCount[i]);
                    }else{
                        TPCount[i] -= (int)(allCharacters[i].speed[0] + (allCharacters[i].speed[1] * allCharacters[i].level));
                    }
                }else{
                    Debug.Log(allCharacters[i].name + " TP: " + TPCount[i]);
                }
            }
            if(TPFulfilled.Count > 1){
                Character fastest;
                while(TPFulfilled.Count > 1){
                    fastest = FindFastest(TPFulfilled);
                    turnOrder.Add(fastest);
                    TPFulfilled.Remove(fastest);
                }
            }
            if(TPFulfilled.Count == 1){
                turnOrder.Add(TPFulfilled[0]);
                TPFulfilled.Clear();
            }
        }
        for(int i = 0;i < turnOrder.Count;i++){
            Debug.Log("Turn " + (i + turn + cumulativeTurns) + ": " + turnOrder[i].name);
        }
        turnOrder1.sprite = portraits.Find(x => x.name == turnOrder[0].name + "Portrait");
        turnOrder2.sprite = portraits.Find(x => x.name == turnOrder[1].name + "Portrait");
        turnOrder3.sprite = portraits.Find(x => x.name == turnOrder[2].name + "Portrait");
        if(turnOrder[0].position == 0){ //if the character is a player
            turnOrder1Back.color = new Color((float)67/255, 1, (float)64/255, (float)100/255); //Green
        }else{
            turnOrder1Back.color = new Color(1, (float)93/255, (float)93/255, (float)100/255); //Red
        }
        if(turnOrder[1].position == 0){
            turnOrder2Back.color = new Color((float)67/255, 1, (float)64/255, (float)100/255); //Green
        }else{
            turnOrder2Back.color = new Color(1, (float)93/255, (float)93/255, (float)100/255); //Red
        }
        if(turnOrder[2].position == 0){
            turnOrder3Back.color = new Color((float)67/255, 1, (float)64/255, (float)100/255); //Green
        }else{
            turnOrder3Back.color = new Color(1, (float)93/255, (float)93/255, (float)100/255); //Red
        }
    }

    Character FindFastest(List<Character> list){
        int max = -1, index = -1;
        for(int i = 0;i < list.Count;i++){
            if((int)(list[i].speed[0] + (list[i].speed[1] * list[i].level)) > max){
                max = (int)(list[i].speed[0] + (list[i].speed[1] * list[i].level));
                index = i;
            }else if((int)(list[i].speed[0] + (list[i].speed[1] * list[i].level)) == max){
                if(list[i].position < list[index].position){
                    index = i;
                }
            }
        }
        return list[index];
    }

    void SwitchCharacter(bool recalculated){
        currentAP = maxAP;
        APBar.fillAmount = (float)currentAP/maxAP;
        APBarOverage.fillAmount = 0;
        APCount.text = "" + maxAP + "/" + maxAP;
        weaknessHit = false;
        // allCharacters.Insert(activeIndex, activeCharacter);
        if(turn % 3 == 0 && turn != 0 && !recalculated){
            Debug.Log("Recalculating Turn Order");
            CalculateTurnOrder(false);
            SwitchCharacter(true);
        }else{
            if(activeCharacter.position == 0){
                MovePortraits(activeCharacter, false);
            }
            coroutine = TurnOrderSlide(recalculated);
            StartCoroutine(coroutine);
            activeCharacter = turnOrder[turn % 3];
            activeIndex = allCharacters.FindIndex(x => x.name == activeCharacter.name);
            // allCharacters.RemoveAt(activeIndex);
            Debug.Log("Character Switched to " + activeCharacter.name);
            Debug.Log("Turn: " + turn + cumulativeTurns);
            turn++;
            if(activeCharacter.position == 0){
                SetUIData(activeCharacter);
                playerController.specialBack.gameObject.SetActive(false);
                playerController.attackBack.gameObject.SetActive(false);
                playerController.battleBack.gameObject.SetActive(true);
                MovePortraits(activeCharacter, true);
                // foreach(Button button in playerController.attackButtons){
                //     button.gameObject.SetActive(true);
                // }
            }else if(activeCharacter.position == 1){
                // Debug.Log("Loading Enemy Shenanigans goes here! Temporarily Skipping.");
                Debug.Log("Testing Things!");
                IEnemyAI tempAI = activeEnemyAI.Find(x => x.GetName() == activeCharacter.name);
                List<AttackTag> enemyAttackTags = enemyController.CalculateTurn(tempAI, activeCharacter, playerCharacters, allCharacters);
                enemyController.targetedCharacter = playerCharacters[enemyController.targetedIndex-1];
                int[] APCosts = tempAI.CalculateAPCosts(enemyAttackTags, activeCharacter);
                // List<Attack> enemyAttackChain = new List<Attack>();
                // foreach(AttackTag tag in enemyAttackTags){
                //     enemyAttackChain.Add(enemyController.enemyAttacks.Find(x => x.name == tag.name));
                //     //TODO: Finish this
                // }
                playerController.battleBack.gameObject.SetActive(false);
                playerController.attackBack.gameObject.SetActive(false);
                playerController.specialBack.gameObject.SetActive(false);
                coroutine = ExecuteEnemyTurn(enemyAttackTags, APCosts, tempAI); //TODO: make it a chaining coroutine that plays all the animations
                StartCoroutine(coroutine);
                // SwitchCharacter(false);
            }else{
                Debug.Log("Error!");
            }
        }
    }

    void MovePortraits(Character character, bool moveUp){
        Debug.Log(character.name + " Portrait Moving");
        if(moveUp){
            for(int i = 0;i < portraits.Count;i++){
                if(portraits[i].name == character.name + "Portrait"){
                    characterPortraits[i].transform.Translate(0, 100, 0);
                    break;
                }
            }
        }else{
            for(int i = 0;i < portraits.Count;i++){
                if(portraits[i].name == character.name + "Portrait"){
                    characterPortraits[i].transform.Translate(0, -100, 0);
                    break;
                }
            }
        }
    }

    void Act(int APUsed, string attackName){
        Attack attack = playerController.PullAttackData(attackName);
        if(currentAP - attack.apCost < 0){
            Debug.Log("Not Enough AP");
        }else{
            if(attack.apCost == -1){
                currentAP -= APUsed;
            }else{
                currentAP -= attack.apCost;
            }

            if(APUsed == 1){
                currentAP -= APUsed;
                playerController.NormAttack(0, activeCharacter);
                CalculateDamage(playerController.PullAttackData(activeCharacter.attacks[0].name).resistance, playerController.targetedIndex, true, playerController.PullAttackData(activeCharacter.attacks[0].name));
                if(currentAP == 0){
                    SwitchCharacter(false);
                }
            }else if(APUsed == 2){
                currentAP -= APUsed;
                playerController.NormAttack(1, activeCharacter);
                CalculateDamage(playerController.PullAttackData(activeCharacter.attacks[1].name).resistance, playerController.targetedIndex, true, playerController.PullAttackData(activeCharacter.attacks[1].name));
                if(currentAP == 0){
                    SwitchCharacter(false);
                }
            }else{
                playerController.specialBack.SetActive(false);
                playerController.attackBack.SetActive(false);
                attackTextBack.SetActive(true);
                attackText.text = attackName;
                IEnumerator tempCoroutine = CheckPlayerAnimDone(attackName);
                StartCoroutine(tempCoroutine);
                // //TODO: Finish this
                // if(playerController.PullAttackData(attackName).target[1] == 0){
                //     Debug.Log("Cast a Support Skill");
                // }else if(playerController.PullAttackData(attackName).target[1] == 1){
                //     playerController.Attack(attackName);
                //     CalculateDamage(playerController.PullAttackData(attackName).resistance, playerController.targetedIndex, true);
                // }else{
                //     Debug.Log("Error!");
                // }
            }
            if(currentAP > 7){
                APBar.fillAmount = 1;
                APBarOverage.fillAmount = (float)(currentAP-7)/maxAP;
            }else{
                APBar.fillAmount = (float)currentAP/maxAP;
                APBarOverage.fillAmount = 0;
            }
            APCount.text = "" + currentAP + "/" + maxAP;
        }
    }

    bool CalculateDamage(int element, int pos, bool targetIsEnemy, Attack attackUsed){ //TODO: Pass the attack object in
        Character temp;
        int attackMultiplier;
        if(targetIsEnemy){
            Debug.Log("Damaging " + playerController.targetedEnemy.name + " with element " + element);
            temp = enemies[playerController.targetedIndex-1];
            enemies.RemoveAt(playerController.targetedIndex-1);
        }else{
            Debug.Log("Damaging " + enemyController.targetedCharacter.name + " with element " + element);
            temp = playerCharacters[enemyController.targetedIndex-1]; //Placeholder. Replace with player damage instead.
            playerCharacters.RemoveAt(enemyController.targetedIndex-1);
        }

        //Status Effect Shenanigans
        //TODO: Crashes when adding a status effect. Fix that.
        // List<IStatusEffect> tempStatusEffects = new List<IStatusEffect>();
        // int random = UnityEngine.Random.Range(0,100);
        // if(attackUsed.status.Length > 0){ //TODO: Finish and CONVERT ALL JSONS TO NEW STRUCTURE
        //     foreach(Status status in attackUsed.status){
        //         if(random <= status.hitRate){
        //             switch(status.type){
        //                 case 0:
        //                     tempStatusEffects.Add(new Shorted());
        //                     break;
        //                 case 1:
        //                     tempStatusEffects.Add(new Poison());
        //                     break;
        //                 case 2:
        //                     tempStatusEffects.Add(new Burn());
        //                     break;
        //                 default:
        //                     break;
        //             }
        //         }
        //     }

        //     if(tempStatusEffects.Count > 0){
        //         if(targetIsEnemy){
        //             enemyStatusEffects[pos-1].AddRange(tempStatusEffects);
        //             if(enemyStatusEffects[pos-1].Count > 5){
        //                 while(enemyStatusEffects[pos-1].Count > 5){
        //                     enemyStatusEffects[pos-1].RemoveAt(0);
        //                 }
        //             }
        //         }else{
        //             playerStatusEffects[pos-1].AddRange(tempStatusEffects);
        //             if(playerStatusEffects[pos-1].Count > 5){
        //                 while(playerStatusEffects[pos-1].Count > 5){
        //                     playerStatusEffects[pos-1].RemoveAt(0);
        //                 }
        //             }
        //         }
        //     }
        // }

        //TODO: Level scaling, plateauing at a x2 multiplier when 18 levels or less below. 
        //Should calculate the multiplier before the turn starts instead of doing it on the fly with each attack.
        if(temp.level - activeCharacter.level > 18){
            attackMultiplier = (temp.level - activeCharacter.level)/9;
        }else{
            attackMultiplier = 2;
        }

        float tempFillHP = 1;
        float tempFillEP = 1;
        int damage = 0;
        int attackMag;
        bool weak = false;
        switch(element){
            case 0: //Trickster/Guaranteed Weak
                damage = 100;
                HitWeakness();
                weak = true;
                break;
            case 1: //Normal Phys Damage
                attackMag = (int)(activeCharacter.attack[0] + (int)(activeCharacter.attack[1] * activeCharacter.level));
                damage = (attackMag * attackMultiplier) - (int)(temp.defense[0] + (int)(temp.defense[1] * temp.level));
                if(damage <= 0){
                    damage = 1;
                }
                Debug.Log("Player Attack: " + attackMag);
                Debug.Log("Enemy Defense: " + (int)(temp.defense[0] + (temp.defense[1] * temp.level)));
                break;
            case 2: //Fire Damage
                attackMag = (int)(activeCharacter.magic[0] + (int)(activeCharacter.magic[1] * activeCharacter.level));
                damage = (attackMag * attackUsed.power) - (int)(temp.resistance[0] + (int)(temp.resistance[1] * temp.level));
                if(temp.resistances[0] == 0){
                    damage = (int)(damage * 1.5f);
                    HitWeakness();
                    weak = true;
                }
                if(damage <= 0){
                    damage = 1;
                }
                Debug.Log("Player Magic: " + attackMag);
                Debug.Log("Enemy Resistance: " + (int)(temp.resistance[0] + (temp.defense[1] * temp.level)));
                break;
            case 3: //Wind Damage
                attackMag = (int)(activeCharacter.magic[0] + (int)(activeCharacter.magic[1] * activeCharacter.level));
                damage = (attackMag * attackUsed.power) - (int)(temp.resistance[0] + (int)(temp.resistance[1] * temp.level));
                if(temp.resistances[1] == 0){
                    damage = (int)(damage * 1.5f);
                    HitWeakness();
                    weak = true;
                }
                if(damage <= 0){
                    damage = 1;
                }
                Debug.Log("Player Magic: " + attackMag);
                Debug.Log("Enemy Resistance: " + (int)(temp.resistance[0] + (temp.defense[1] * temp.level)));
                break;
            case 4: //Ice Damage
                attackMag = (int)(activeCharacter.magic[0] + (int)(activeCharacter.magic[1] * activeCharacter.level));
                damage = (attackMag * attackUsed.power) - (int)(temp.resistance[0] + (int)(temp.resistance[1] * temp.level));
                if(temp.resistances[2] == 0){
                    damage = (int)(damage * 1.5f);
                    HitWeakness();
                    weak = true;
                }
                if(damage <= 0){
                    damage = 1;
                }
                Debug.Log("Player Magic: " + attackMag);
                Debug.Log("Enemy Resistance: " + (int)(temp.resistance[0] + (temp.defense[1] * temp.level)));
                break;
            case 5: //Elec Damage
                attackMag = (int)(activeCharacter.magic[0] + (int)(activeCharacter.magic[1] * activeCharacter.level));
                damage = (attackMag * attackUsed.power) - (int)(temp.resistance[0] + (int)(temp.resistance[1] * temp.level));
                if(temp.resistances[3] == 0){
                    damage = (int)(damage * 1.5f);
                    HitWeakness();
                    weak = true;
                }
                if(damage <= 0){
                    damage = 1;
                }
                Debug.Log("Player Magic: " + attackMag);
                Debug.Log("Enemy Resistance: " + (int)(temp.resistance[0] + (temp.defense[1] * temp.level)));
                break;
            case 6: //Non-Elemental Magic(?)
                attackMag = (int)(activeCharacter.magic[0] + (int)(activeCharacter.magic[1] * activeCharacter.level));
                damage = (attackMag * attackUsed.power) - (int)(temp.resistance[0] + (int)(temp.resistance[1] * temp.level));
                if(damage <= 0){
                    damage = 1;
                }
                Debug.Log("Player Magic: " + attackMag);
                Debug.Log("Enemy Resistance: " + (int)(temp.resistance[0] + (temp.defense[1] * temp.level)));
                break;
            default:
                break;
        }

        allCharacters.RemoveAt(activeIndex);
        activeCharacter.currentEP -= attackUsed.epCost;
        allCharacters.Insert(activeIndex, activeCharacter);

        temp.currentHP -= damage;
        if(temp.currentHP < 0){
            temp.currentHP = 0;
        }
        tempFillHP = (float)temp.currentHP / (int)(temp.maxHP[0] + temp.maxHP[1] * (temp.level));
        Debug.Log("Dealt " + damage + " Damage! Target now has " + temp.currentHP + " HP.");
        tempFillEP = (float)activeCharacter.currentEP / (int)(activeCharacter.maxEP[0] + activeCharacter.maxEP[1] * (activeCharacter.level));
        Debug.Log("Consumed " + attackUsed.epCost + " EP. Remaining EP: " + activeCharacter.currentEP);

        if(targetIsEnemy){
            switch(pos){
                case 1:
                    enemyController.enemy1Health.fillAmount = tempFillHP;
                    break;
                case 2:
                    enemyController.enemy2Health.fillAmount = tempFillHP;
                    break;
                case 3:
                    enemyController.enemy3Health.fillAmount = tempFillHP;
                    break;
                case 4:
                    enemyController.enemy4Health.fillAmount = tempFillHP;
                    break;
                default:
                    break;
            }
            charData[pos-1][1].text = "" + activeCharacter.currentEP + "/" + (int)(activeCharacter.maxEP[0] + activeCharacter.maxEP[1] * (activeCharacter.level));
            switch(playerCharacters.FindIndex(x => x.name == activeCharacter.name)){
                case 0:
                    playerController.char1EP.fillAmount = tempFillEP;
                    break;
                case 1:
                    playerController.char2EP.fillAmount = tempFillEP;
                    break;
                case 2:
                    playerController.char3EP.fillAmount = tempFillEP;
                    break;
                case 3:
                    playerController.char4EP.fillAmount = tempFillEP;
                    break;
                default:
                    break;
            }
        }else{
            charData[pos-1][0].text = "" + temp.currentHP + "/" + (int)(temp.maxHP[0] + temp.maxHP[1] * (temp.level));
            switch(pos){
                case 1:
                    playerController.char1Health.fillAmount = tempFillHP;
                    break;
                case 2:
                    playerController.char2Health.fillAmount = tempFillHP;
                    break;
                case 3:
                    playerController.char3Health.fillAmount = tempFillHP;
                    break;
                case 4:
                    playerController.char4Health.fillAmount = tempFillHP;
                    break;
                default:
                    break;
            }
        }
        if(targetIsEnemy){
            enemies.Insert(playerController.targetedIndex-1, temp);
            StopCoroutine(coroutine);
            coroutine = ReduceHealthBar(pos, targetIsEnemy);
            StartCoroutine(coroutine);
            coroutine = DisplayDamageNumber(damage, pos, weak, targetIsEnemy, false);
            StartCoroutine(coroutine);
        }else{
            playerCharacters.Insert(enemyController.targetedIndex-1, temp);
            // StopCoroutine(coroutine);
            coroutine = ReduceHealthBar(pos, targetIsEnemy);
            StartCoroutine(coroutine);
            coroutine = DisplayDamageNumber(damage, pos, weak, targetIsEnemy, false);
            StartCoroutine(coroutine);
        }
        
        //Death Handler
        if(temp.currentHP == 0){
            if(targetIsEnemy){
                Type type = Type.GetType("AI" + temp.name);
                Component tempAI = this.gameObject.GetComponent(type);
                Destroy(tempAI);
                activeEnemyAI.Remove(activeEnemyAI.Find(x => x.GetName() == temp.name));
                enemyImgData[pos-1].gameObject.SetActive(false);
                TPCount.RemoveAt(allCharacters.FindIndex(x => x.name == temp.name && x.position == 1));
                allCharacters.Remove(allCharacters.Find(x => x.name == temp.name && x.position == 1));
                int tempIndex = turnOrder.FindIndex(x => x.name == temp.name && x.position == 1);
                turnOrder.Remove(turnOrder.Find(x => x.name == temp.name && x.position == 1));
                ValidateEnemies();
                cumulativeTurns += turn;
                activeIndex = allCharacters.FindIndex(x => x.name == activeCharacter.name && x.position == 0);
                // allCharacters.Insert(activeIndex, activeCharacter);
                if(turn % 3 != 0){
                    CalculateTurnOrder(true);
                }
                switch(tempIndex){
                    case 2:
                        turnOrder3Opacity.alpha = 0;
                        break;
                    case 1:
                        turnOrder2Opacity.alpha = 0;
                        break;
                    case 0:
                        turnOrder1Opacity.alpha = 0;
                        break;
                    default:
                        break;
                }
                bool noEnemiesLeft = true;
                foreach(Character character in allCharacters){
                    if(character.position == 1){
                        noEnemiesLeft = false;
                        break;
                    }
                }
                if(noEnemiesLeft){
                    coroutine = EndCombat();
                    StartCoroutine(coroutine);
                    return false;
                }
                // allCharacters.RemoveAt(activeIndex);
                // SwitchCharacter(false);
                for(int i = 0;i < playerController.enemies.Length;i++){
                    if(playerController.enemies[i].activeSelf){
                        playerController.ChangeTarget(i+1, enemies[i]);
                    }
                }
                // playerController.ChangeTarget();
                // SwitchCharacter(false, true);
            }else{
                charImgData[pos-1][1].color = new Color(1,0.5f,0.5f, 0.5f); //TODO: nullify the portrait
                Debug.Log(allCharacters.Find(x => x.name == temp.name && x.position == 0));
                TPCount.RemoveAt(allCharacters.FindIndex(x => x.name == temp.name && x.position == 0));
                allCharacters.Remove(allCharacters.Find(x => x.name == temp.name && x.position == 0));
                bool noPlayersLeft = true;
                foreach(Character character in allCharacters){
                    if(character.position == 0){
                        noPlayersLeft = false;
                        break;
                    }
                }
                Debug.Log(noPlayersLeft);
                if(noPlayersLeft){
                    return false;
                } 
                int tempIndex = turnOrder.FindIndex(x => x.name == temp.name && x.position == 0);
                turnOrder.Remove(turnOrder.Find(x => x.name == temp.name && x.position == 0));
                cumulativeTurns += turn;
                activeIndex = allCharacters.FindIndex(x => x.name == activeCharacter.name && x.position == 1);
                if(turn % 3 != 0){
                    CalculateTurnOrder(true);
                }
                switch(tempIndex){
                    case 2:
                        turnOrder3Opacity.alpha = 0;
                        break;
                    case 1:
                        turnOrder2Opacity.alpha = 0;
                        break;
                    case 0:
                        turnOrder1Opacity.alpha = 0;
                        break;
                    default:
                        break;
                }
                for(int i = 0;i < charImgData.Length;i++){
                    if(charImgData[i][1].color.a == 1){
                        enemyController.targetedIndex = i+1;
                        enemyController.targetedCharacter = playerCharacters[i];
                        break;
                    }
                }
                //TODO: Finish
            }
        }
        return true;
        // CalculateSupportEffects(targetIsEnemy);
    }

    public void CalculateSupportEffects(bool isEnemy, Attack supportSpell){
        Character temp;
        Debug.Log("Now Calculating");
        if(isEnemy){
            temp = enemies[enemyController.supportTargetedIndex-1];
            enemies.RemoveAt(enemyController.supportTargetedIndex-1);
        }else{
            temp = playerCharacters[playerController.targetedSupportIndex-1];
            playerCharacters.RemoveAt(playerController.targetedSupportIndex-1);
        }
        int magic = (int)(activeCharacter.magic[0] + (activeCharacter.magic[1] * activeCharacter.level));
        temp.currentHP += (magic * supportSpell.hpRegen);
        Debug.Log("Heal Amount: " + (magic * supportSpell.hpRegen));
        if(temp.currentHP > (int)(temp.maxHP[0] + (temp.maxHP[1] * temp.level))){
            temp.currentHP = (int)(temp.maxHP[0] + (temp.maxHP[1] * temp.level));
        }
        float tempFillHP = (float)temp.currentHP/((int)(temp.maxHP[0] + (temp.maxHP[1] * temp.level)));

        activeCharacter.currentEP -= supportSpell.epCost;
        float tempFillEP = (float)activeCharacter.currentEP / (int)(activeCharacter.maxEP[0] + activeCharacter.maxEP[1] * (activeCharacter.level));
        Debug.Log("Consumed " + supportSpell.epCost + " EP. Remaining EP: " + activeCharacter.currentEP);

        if(isEnemy){
            enemies.Insert(enemyController.supportTargetedIndex-1, temp);
            switch(enemyController.supportTargetedIndex){
                case 1:
                    enemyController.enemy1Health.fillAmount = tempFillHP;
                    break;
                case 2:
                    enemyController.enemy2Health.fillAmount = tempFillHP;
                    break;
                case 3:
                    enemyController.enemy3Health.fillAmount = tempFillHP;
                    break;
                case 4:
                    enemyController.enemy4Health.fillAmount = tempFillHP;
                    break;
                default:
                    break;
            }
        }else{
            playerCharacters.Insert(playerController.targetedSupportIndex-1, temp);
            charData[playerController.targetedSupportIndex-1][0].text = "" + temp.currentHP + "/" + (int)(temp.maxHP[0] + temp.maxHP[1] * (temp.level));
            switch(playerController.targetedSupportIndex){
                case 1:
                    playerController.char1Health.fillAmount = tempFillHP;
                    break;
                case 2:
                    playerController.char2Health.fillAmount = tempFillHP;
                    break;
                case 3:
                    playerController.char3Health.fillAmount = tempFillHP;
                    break;
                case 4:
                    playerController.char4Health.fillAmount = tempFillHP;
                    break;
                default:
                    break;
            }
            coroutine = DisplayDamageNumber(magic * supportSpell.hpRegen, playerController.targetedSupportIndex, false, false, true);
            StartCoroutine(coroutine);
            charData[playerCharacters.FindIndex(x => x.name == activeCharacter.name)][1].text = "" + activeCharacter.currentEP + "/" + (int)(activeCharacter.maxEP[0] + activeCharacter.maxEP[1] * (activeCharacter.level));
            switch(playerCharacters.FindIndex(x => x.name == activeCharacter.name)){
                case 0:
                    playerController.char1EP.fillAmount = tempFillEP;
                    break;
                case 1:
                    playerController.char2EP.fillAmount = tempFillEP;
                    break;
                case 2:
                    playerController.char3EP.fillAmount = tempFillEP;
                    break;
                case 3:
                    playerController.char4EP.fillAmount = tempFillEP;
                    break;
                default:
                    break;
            }
        }
    }

    public void ValidateEnemies(){

    }

    public void HitWeakness(){
        if(!weaknessHit){
            currentAP += 3;
            // if(currentAP > 7){
            //     APBar.fillAmount = 1;
            //     APBarOverage.fillAmount = (float)(currentAP-7)/maxAP;
            // }else{
            //     APBar.fillAmount = (float)currentAP/maxAP;
            //     APBarOverage.fillAmount = 0;
            // }
            weaknessHit = true;
            IEnumerator tempCoroutine = IncreaseAP();
            StartCoroutine(tempCoroutine);
        }
    }

    IEnumerator IncreaseAP(){
        if(currentAP > 7){
            while(APBar.fillAmount < 1){
                APBar.fillAmount += 0.01f;
                yield return new WaitForSeconds(0.01f);
            }
            APBar.fillAmount = 1;
            while(APBarOverage.fillAmount < (float)(currentAP-7)/maxAP){
                APBarOverage.fillAmount += 0.01f;
                yield return new WaitForSeconds(0.01f);
            }
            APBarOverage.fillAmount = (float)(currentAP-7)/maxAP;
        }else{
            while(APBar.fillAmount < (float)currentAP/maxAP){
                APBar.fillAmount += 0.01f;
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    IEnumerator ReduceHealthBar(int pos, bool isEnemy){
        if(isEnemy){
            switch(pos){
                case 1:
                    while(enemyController.enemy1HealthConsume.fillAmount > enemyController.enemy1Health.fillAmount){
                        enemyController.enemy1HealthConsume.fillAmount -= 0.01f;
                        yield return new WaitForSeconds(0.1f);
                    }
                    enemyController.enemy1HealthConsume.fillAmount = enemyController.enemy1Health.fillAmount;
                    break;
                case 2:
                    while(enemyController.enemy2HealthConsume.fillAmount > enemyController.enemy2Health.fillAmount){
                        enemyController.enemy2HealthConsume.fillAmount -= 0.01f;
                        yield return new WaitForSeconds(0.1f);
                    }
                    enemyController.enemy2HealthConsume.fillAmount = enemyController.enemy2Health.fillAmount;
                    break;
                case 3:
                    while(enemyController.enemy3HealthConsume.fillAmount > enemyController.enemy3Health.fillAmount){
                        enemyController.enemy3HealthConsume.fillAmount -= 0.01f;
                        yield return new WaitForSeconds(0.1f);
                    }
                    enemyController.enemy3HealthConsume.fillAmount = enemyController.enemy3Health.fillAmount;
                    break;
                case 4:
                    while(enemyController.enemy4HealthConsume.fillAmount > enemyController.enemy4Health.fillAmount){
                        enemyController.enemy4HealthConsume.fillAmount -= 0.01f;
                        yield return new WaitForSeconds(0.1f);
                    }
                    enemyController.enemy4HealthConsume.fillAmount = enemyController.enemy4Health.fillAmount;
                    break;
                default:
                    break;
            }
        }else{
            switch(pos){
                case 1:
                    while(playerController.char1HealthConsume.fillAmount > playerController.char1Health.fillAmount){
                        playerController.char1HealthConsume.fillAmount -= 0.01f;
                        yield return new WaitForSeconds(0.1f);
                    }
                    playerController.char1HealthConsume.fillAmount = playerController.char1Health.fillAmount;
                    break;
                case 2:
                    while(playerController.char2HealthConsume.fillAmount > playerController.char2Health.fillAmount){
                        playerController.char2HealthConsume.fillAmount -= 0.01f;
                        yield return new WaitForSeconds(0.1f);
                    }
                    playerController.char2HealthConsume.fillAmount = playerController.char2Health.fillAmount;
                    break;
                case 3:
                    while(playerController.char3HealthConsume.fillAmount > playerController.char3Health.fillAmount){
                        playerController.char3HealthConsume.fillAmount -= 0.01f;
                        yield return new WaitForSeconds(0.1f);
                    }
                    playerController.char3HealthConsume.fillAmount = playerController.char3Health.fillAmount;
                    break;
                case 4:
                    while(playerController.char4HealthConsume.fillAmount > playerController.char4Health.fillAmount){
                        playerController.char4HealthConsume.fillAmount -= 0.01f;
                        yield return new WaitForSeconds(0.1f);
                    }
                    playerController.char4HealthConsume.fillAmount = playerController.char4Health.fillAmount;
                    break;
                default:
                    break;
            }
        }
    }

    IEnumerator DisplayDamageNumber(int damage, int pos, bool weak, bool isEnemy, bool isHeal){
        // foreach(TextMeshProUGUI damageNum in playerController.enemyDamageNums){
        //     damageNum.gameObject.SetActive(false);
        // }
        // foreach(GameObject weakIndicator in playerController.weakIndicators){
        //     weakIndicator.SetActive(false);
        // }
        // foreach(TextMeshProUGUI damageNum in enemyController.playerDamageNums){
        //     damageNum.gameObject.SetActive(false);
        // }
        // foreach(GameObject weakIndicator in enemyController.weakIndicators){
        //     weakIndicator.SetActive(false);
        // }
        foreach(TextMeshProUGUI text in playerController.enemyDamageNums){
            text.color = new Color32(255,255,255,255);
        }
        foreach(TextMeshProUGUI text in enemyController.playerDamageNums){
            text.color = new Color32(255,255,255,255);
        }
        if(isHeal){
            if(isEnemy){
                playerController.enemyDamageNums[pos-1].color = new Color32(0,255,0,255);
            }else{
                enemyController.playerDamageNums[pos-1].color = new Color32(0,255,0,255);
            }
        }else{
            if(isEnemy){
                enemyController.playerDamageNums[pos-1].color = new Color32(255,255,255,255);
            }else{
                playerController.enemyDamageNums[pos-1].color = new Color32(255,255,255,255);
            }
        }
        if(isEnemy){
            playerController.enemyDamageNums[pos-1].transform.localPosition = new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(0.5f, 1.5f), 0);
            playerController.enemyDamageNums[pos-1].gameObject.SetActive(true);
            if(weak){
                playerController.weakIndicators[pos-1].SetActive(true);
            }
            playerController.enemyDamageNums[pos-1].text = "" + damage;
        }else{
            enemyController.playerDamageNums[pos-1].transform.localPosition = new Vector3(UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-25f, 100f), 0);
            enemyController.playerDamageNums[pos-1].gameObject.SetActive(true);
            Debug.Log(weak);
            if(weak){
                enemyController.weakIndicators[pos-1].SetActive(true);
            }
            enemyController.playerDamageNums[pos-1].text = "" + damage;
        }
        yield return new WaitForSeconds(0.5f);
        //TODO: Make it fade out
        // Debug.Log(playerController.enemyDamageNums[pos].color);
        // while(playerController.enemyDamageNums[pos].color.a > 0){

        // }
        if(isEnemy){
            playerController.enemyDamageNums[pos-1].gameObject.SetActive(false);
        }else{
            enemyController.playerDamageNums[pos-1].gameObject.SetActive(false);
        }
    }

    IEnumerator ExecuteEnemyTurn(List<AttackTag> attackChain, int[] APCosts, IEnemyAI tempAI){
        yield return new WaitForSeconds(1);
        for(int i = 0;i < attackChain.Count;i++){
            if(APCosts[i] == 3){
                attackTextBack.SetActive(true);
                attackText.text = attackChain[i].name;
            }
            if(currentAP == 0){
                break;
            }
            if(currentAP - APCosts[i] < 0){
                break;
            }else{
                currentAP -= APCosts[i];
                if(currentAP > 7){
                    APBar.fillAmount = 1;
                    APBarOverage.fillAmount = (float)(currentAP-7)/maxAP;
                }else{
                    APBar.fillAmount = (float)currentAP/maxAP;
                    APBarOverage.fillAmount = 0;
                }
                APCount.text = "" + currentAP + "/" + maxAP;
            }

            if(enemyController.PullAttackData(attackChain[i].name).target[1] == 0){
                //TODO: Finish this
                Debug.Log("Cast a Support Skill: " + enemyController.enemyAttacks.Find(x => x.anim == attackChain[i].name).name);
                enemyController.Support(attackChain[i].name);
                CalculateSupportEffects(true, enemyController.PullAttackData(attackChain[i].name));
                Debug.Log("Support Anim Name: " + enemyController.enemyAttacks.Find(x => x.anim == attackChain[i].name).anim);
                yield return new WaitForSeconds(0.01f);
                switch(enemyController.supportTargetedIndex){
                    case 1:
                        while(AnimatorIsPlaying(enemyController.enemyFXController.anim1)){
                            // Debug.Log("Waiting for Animation to Finish.");
                            yield return new WaitForSeconds(0.1f);
                        }
                        break;
                    case 2:
                        while(AnimatorIsPlaying(enemyController.enemyFXController.anim2)){
                            // Debug.Log("Waiting for Animation to Finish.");
                            yield return new WaitForSeconds(0.1f);
                        }
                        break;
                    case 3:
                        while(AnimatorIsPlaying(enemyController.enemyFXController.anim3)){
                            // Debug.Log("Waiting for Animation to Finish.");
                            yield return new WaitForSeconds(0.1f);
                        }
                        break;
                    case 4:
                        while(AnimatorIsPlaying(enemyController.enemyFXController.anim4)){
                            // Debug.Log("Waiting for Animation to Finish.");
                            yield return new WaitForSeconds(0.1f);
                        }
                        break;
                    default:
                        break;
                }
            }else if(enemyController.PullAttackData(attackChain[i].name).target[1] == 1){
                enemyController.Attack(attackChain[i].name);
                if(!CalculateDamage(enemyController.PullAttackData(attackChain[i].name).resistance, enemyController.targetedIndex, false, enemyController.PullAttackData(attackChain[i].name))){
                    coroutine = FadeOut();
                    StartCoroutine(coroutine);
                    yield return new WaitForSeconds(1);
                    Debug.Log("Scene Loading");
                    SceneManager.LoadScene("gameOver");
                    break;
                }
                Debug.Log("Attack Anim Name: " + enemyController.enemyAttacks.Find(x => x.name == attackChain[i].name).anim);
                yield return new WaitForSeconds(0.01f);
                switch(enemyController.targetedIndex){
                    case 1:
                        while(AnimatorIsPlaying(enemyController.playerFXController.anim1)){
                            // Debug.Log("Waiting for Animation to Finish.");
                            yield return new WaitForSeconds(0.1f);
                        }
                        break;
                    case 2:
                        while(AnimatorIsPlaying(enemyController.playerFXController.anim2)){
                            // Debug.Log("Waiting for Animation to Finish.");
                            yield return new WaitForSeconds(0.1f);
                        }
                        break;
                    case 3:
                        while(AnimatorIsPlaying(enemyController.playerFXController.anim3)){
                            // Debug.Log("Waiting for Animation to Finish.");
                            yield return new WaitForSeconds(0.1f);
                        }
                        break;
                    case 4:
                        while(AnimatorIsPlaying(enemyController.playerFXController.anim4)){
                            // Debug.Log("Waiting for Animation to Finish.");
                            yield return new WaitForSeconds(0.1f);
                        }
                        break;
                    default:
                        break;
                }
            }else{
                Debug.Log("Error!");
            }
            attackTextBack.SetActive(false);
            // yield return new WaitForSeconds(1);
        }
        if(currentAP > 0){
            List<AttackTag> fillerAttackChain = tempAI.ChooseBackup(activeCharacter, currentAP);
            coroutine = ExecuteEnemyTurn(fillerAttackChain, tempAI.CalculateAPCosts(fillerAttackChain, activeCharacter), tempAI);
            StartCoroutine(coroutine);
        }else{
            SwitchCharacter(false);
        }
    }

    IEnumerator CheckPlayerAnimDone(string attackName){
        if(playerController.PullAttackData(attackName).target[1] == 0){
            if(playerController.PullAttackData(attackName).target[0] == 0){
                playerController.targetedSupportIndex = 0;
                while(playerController.targetedSupportIndex == 0){
                    playerController.explanationBack.SetActive(true);
                    // if(characterPortraits){ //TODO: finish this

                    // }
                    yield return new WaitForSeconds(0.1f);
                }
                playerController.explanationBack.SetActive(false);
            }
            //TODO: Finish this
            playerController.Support(attackName);
            CalculateSupportEffects(false, playerController.PullAttackData(attackName));
            yield return new WaitForSeconds(0.01f);
            switch(playerController.targetedSupportIndex){
                case 1:
                    Debug.Log("Waiting for Animation to Finish.");
                    while(AnimatorIsPlaying(playerController.playerFXController.anim1)){
                        yield return new WaitForSeconds(0.1f);
                    }
                    break;
                case 2:
                    Debug.Log("Waiting for Animation to Finish.");
                    while(AnimatorIsPlaying(playerController.playerFXController.anim2)){
                        yield return new WaitForSeconds(0.1f);
                    }
                    break;
                case 3:
                    Debug.Log("Waiting for Animation to Finish.");
                    while(AnimatorIsPlaying(playerController.playerFXController.anim3)){
                        yield return new WaitForSeconds(0.1f);
                    }
                    break;
                case 4:
                    Debug.Log("Waiting for Animation to Finish.");
                    while(AnimatorIsPlaying(playerController.playerFXController.anim4)){
                        yield return new WaitForSeconds(0.1f);
                    }
                    break;
                default:
                    break;
            }
            Debug.Log("Cast a Support Skill");
        }else if(playerController.PullAttackData(attackName).target[1] == 1){
            yield return new WaitForSeconds(0.5f);
            playerController.Attack(attackName);
            CalculateDamage(playerController.PullAttackData(attackName).resistance, playerController.targetedIndex, true, playerController.PullAttackData(attackName));
            yield return new WaitForSeconds(0.01f);
            switch(playerController.targetedIndex){
                case 1:
                    Debug.Log("Waiting for Animation to Finish.");
                    while(AnimatorIsPlaying(playerController.enemyFXController.anim1)){
                        yield return new WaitForSeconds(0.1f);
                    }
                    break;
                case 2:
                    Debug.Log("Waiting for Animation to Finish.");
                    while(AnimatorIsPlaying(playerController.enemyFXController.anim2)){
                        yield return new WaitForSeconds(0.1f);
                    }
                    break;
                case 3:
                    Debug.Log("Waiting for Animation to Finish.");
                    while(AnimatorIsPlaying(playerController.enemyFXController.anim3)){
                        yield return new WaitForSeconds(0.1f);
                    }
                    break;
                case 4:
                    Debug.Log("Waiting for Animation to Finish.");
                    while(AnimatorIsPlaying(playerController.enemyFXController.anim4)){
                        yield return new WaitForSeconds(0.1f);
                    }
                    break;
                default:
                    break;
            }
        }else{
            Debug.Log("Error!");
        }
        yield return new WaitForSeconds(0.1f);
        playerController.specialBack.SetActive(true);
        playerController.attackBack.SetActive(true);
        attackTextBack.SetActive(false);
        if(currentAP == 0){
                SwitchCharacter(false);
        }
    }

    bool AnimatorIsPlaying(Animator animator){
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
    }

    IEnumerator TurnOrderSlide(bool recalculated){
        Debug.Log("Case statement reached");
        switch(turn % 3){
            case 0:
                Debug.Log("Case 0");
                if(cumulativeTurns != 0 || turn != 0){
                    turnOrder1Opacity.alpha = 0;
                    turnOrder2Opacity.alpha = 0;
                    turnOrder3Opacity.alpha = 0;
                    turnOrder1Back.transform.localPosition = new Vector3(-154, 0, 0);
                    turnOrder2Back.transform.localPosition = new Vector3(0, 0, 0);
                    turnOrder3Back.transform.localPosition = new Vector3(154, 0, 0);
                    while(turnOrder1Opacity.alpha <= 1){
                        turnOrder1Opacity.alpha += 0.1f;
                        turnOrder2Opacity.alpha += 0.1f;
                        turnOrder3Opacity.alpha += 0.1f;
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                break;
            case 1:
                Debug.Log("Case 1");
                while(turnOrder1Back.transform.localPosition.x > -308){
                    turnOrder1Back.transform.localPosition = new Vector3(turnOrder1Back.transform.localPosition.x-10, 0, 0);
                    turnOrder2Back.transform.localPosition = new Vector3(turnOrder2Back.transform.localPosition.x-10, 0, 0);
                    turnOrder3Back.transform.localPosition = new Vector3(turnOrder3Back.transform.localPosition.x-10, 0, 0);
                    yield return new WaitForSeconds(0.01f);
                }
                break;
            case 2:
                Debug.Log("Case 2");
                while(turnOrder2Back.transform.localPosition.x > -308){
                    turnOrder1Back.transform.localPosition = new Vector3(turnOrder1Back.transform.localPosition.x-10, 0, 0);
                    turnOrder2Back.transform.localPosition = new Vector3(turnOrder2Back.transform.localPosition.x-10, 0, 0);
                    turnOrder3Back.transform.localPosition = new Vector3(turnOrder3Back.transform.localPosition.x-10, 0, 0);
                    yield return new WaitForSeconds(0.01f);
                }
                break;
            default:
                break;
        }
    }

    IEnumerator EndCombat(){
        coroutine = FadeOut();
        StartCoroutine(coroutine);
        playerController.attackBack.SetActive(false);
        playerController.specialBack.SetActive(false);
        playerController.battleBack.SetActive(false);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("win");
    }

    IEnumerator PlayAudioLoop(){
        yield return new WaitForSeconds(audioSource.clip.length);
        Debug.Log("Playing Loop Audio");
        audioSource.clip = loopAudio;
        audioSource.Play();
    }

    IEnumerator FadeOut(){
        while(audioSource.volume > 0){
            audioSource.volume -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
