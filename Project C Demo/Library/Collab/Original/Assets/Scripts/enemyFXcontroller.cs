using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemyFXcontroller : MonoBehaviour{
    public Button attack;
    public Animator anim;

    // Start is called before the first frame update
    void Start(){
        attack = GameObject.Find("Attack").GetComponent<Button>();
        attack.onClick.AddListener(Attack);
        anim = GameObject.Find("Anim1").GetComponent<Animator>();
        anim.Play("Rest", 0, 0);
    }

    void Attack(){
        Debug.Log("Attacked");
        //anim.Play("Slash", 0, 0);
        anim.Play("Stab", 0, 0);
    }

    // Update is called once per frame
    void Update(){

    }
}
