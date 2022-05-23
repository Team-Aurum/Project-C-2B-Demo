using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFXController
{
    public Animator anim1;
    public Animator anim2;
    public Animator anim3;
    public Animator anim4;

    public SpriteRenderer anim1Sprite;
    public SpriteRenderer anim2Sprite;
    public SpriteRenderer anim3Sprite;
    public SpriteRenderer anim4Sprite;

    public PlayerFXController(){}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Attack(string attackName, int position, int resistance){
        switch(position){
            case 1:
                ValidateColor(anim1Sprite, resistance);
                anim1.Play(attackName, 0, 0);
                break;
            case 2:
                ValidateColor(anim2Sprite, resistance);
                anim2.Play(attackName, 0, 0);
                break;
            case 3:
                ValidateColor(anim3Sprite, resistance);
                anim3.Play(attackName, 0, 0);
                break;
            case 4:
                ValidateColor(anim4Sprite, resistance);
                anim4.Play(attackName, 0, 0);
                break;
            default:
                break;
        }
    }

    public void ValidateColor(SpriteRenderer sprite, int resistance){
        switch(resistance){
                case 0:
                case 1:
                    sprite.color = new Color(1,1,1);
                    break;
                case 2:
                    sprite.color = new Color(1,0,0);
                    break;
                case 3:
                    sprite.color = new Color(0,1,0);
                    break;
                case 4:
                    sprite.color = new Color(0,0,1);
                    break;
                case 5:
                    sprite.color = new Color(0,1,1);
                    break;
                case 6:
                case 7:
                    sprite.color = new Color(1,1,1);
                    break;
                case 8:
                    sprite.color = new Color(0,1,0);
                    break;
                default:
                    break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
