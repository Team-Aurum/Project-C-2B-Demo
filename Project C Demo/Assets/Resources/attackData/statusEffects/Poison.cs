using System.Collections;
using System.Collections.Generic;

public class Poison: IStatusEffect{
    public int duration = 3;
    int type = 1;
    public Poison(){

    }

    public Effects EvaluateEffects(Character target){
        Effects effects = new Effects();
        int HP = (int)(target.maxHP[0] + (target.maxHP[1] * target.level));
        effects.damage = (HP/10) * -1;
        return effects;
    }

    public int GetDuration(){
        return duration;
    }

    public int GetType(){
        return type;
    }
}