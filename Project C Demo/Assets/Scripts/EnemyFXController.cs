using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyFXController
{
    //public Button attack;
    public Animator anim1;
    public Animator anim2;
    public Animator anim3;
    public Animator anim4;

    public EnemyFXController(){}

    // Start is called before the first frame update
    void Start(){
        Debug.Log("EnemyFXController Started");
        //attack = GameObject.Find("Attack").GetComponent<Button>();
        //anim1.Play("Rest", 0, 0);
        //anim2.Play("Rest", 0, 0);
        //anim3.Play("Rest", 0, 0);
        //anim4.Play("Rest", 0, 0);
    }

    public void Attack(string attackName, int position, int resistance){
        ValidateColor(position, resistance);
        switch(position){
            case 1:
                anim1.Play(attackName, 0, 0);
                break;
            case 2:
                anim2.Play(attackName, 0, 0);
                break;
            case 3:
                anim3.Play(attackName, 0, 0);
                break;
            case 4:
                anim4.Play(attackName, 0, 0);
                break;
            default:
                break;
        }
    }

    public void ValidateColor(int position, int resistance){

    }

    // Update is called once per frame
    void Update(){

    }
}
