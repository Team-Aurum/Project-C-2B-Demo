using System.Collections;
using System.Collections.Generic;

public class Shorted: IStatusEffect{
    public int duration = 3;
    int type = 0;
    public Shorted(){

    }

    public Effects EvaluateEffects(Character target){
        Effects effects = new Effects();
        int EP = (int)(target.maxEP[0] + (target.maxEP[1] * target.level));
        effects.EPChanges = (EP/10) * -1;
        return effects;
    }

    public int GetDuration(){
        return duration;
    }

    public int GetType(){
        return type;
    }
}