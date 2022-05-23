using System;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusEffect{
    Effects EvaluateEffects(Character target);
    int GetDuration();
    int GetType();
}