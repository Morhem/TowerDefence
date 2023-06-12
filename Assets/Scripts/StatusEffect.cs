using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : MonoBehaviour
{
    public enum Effects
    {
        Slow,
        Freeze,
        Knockback,
        Burn,
        DamageAmplification
    }
    public Effects effect;
    public float duration;

    public float magnitude;
    public bool isPeriodic;
    public bool stacking;
    public bool refreshing;
    public float cooldown;
    protected float currentCooldown = 0;

    public StatusEffect ShallowCopy()
    {
        return (StatusEffect)this.MemberwiseClone();
    }

    public void RefreshEffect(StatusEffect otherEffect)
    {
        duration = Mathf.Max(duration, otherEffect.duration);
        magnitude = Mathf.Max(magnitude, otherEffect.magnitude);
    }
    public StatusEffect Clone()
    {
        var result = new StatusEffect
        {
            effect = effect,
            duration = duration,
            magnitude = magnitude,
            isPeriodic = isPeriodic,
            stacking = stacking,
            cooldown = cooldown
        };
        return result;
    }
    public bool ExpendDuration(float time)
    {
        duration -= time;
        return duration > 0;
    }
}
