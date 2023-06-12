using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseTowerAttack : MonoBehaviour
{
    private StatusEffect[] onhitEffects;
    [System.NonSerialized]
    public int Damage;
    [System.NonSerialized]
    public int CriticalChance;
    [System.NonSerialized]
    public int CriticalDamagePercent;
    protected bool Destroyed = false;
    public bool ScaleWithAOE;
    protected StatusEffect[] OnhitEffects 
    { 
        get => onhitEffects; 
        set => onhitEffects = value; 
    }

    public static GameObject FindTarget(Vector3 position, float range, Targeting targeting, params GameObject[] exclude)
    {
        var possibleTargets = MissionController.main.Monsters.Where(x=> !exclude.Contains(x.gameObject) && Vector3.Distance(x.transform.position, position) <= range && x.IsInBounds()).ToList();
        Monster target;
        switch (targeting)
        {
            case Targeting.ClosestEnemy:
                target = possibleTargets.OrderBy(x => Vector3.Distance(x.transform.position, position)).FirstOrDefault();
                break;
            case Targeting.RandomEnemy:
                target = possibleTargets.OrderBy(x => Random.Range(0, 100)).FirstOrDefault();
                break;
            case Targeting.RandomDirection:
                target = possibleTargets.OrderBy(x => Random.Range(0, 100)).FirstOrDefault();
                break;
            default:
                target = possibleTargets.OrderBy(x => Random.Range(0, 100)).FirstOrDefault();
                break;
        }

        return target?.gameObject ?? null;
    }

    public virtual void SetStats(CharacterStatBlock stats)
    {
        if (ScaleWithAOE)
            transform.localScale = new Vector3(stats.CurrentAOERadius, 1, stats.CurrentAOERadius);
        Damage = stats.CurrentDamage;
        CriticalChance = stats.CurrentCriticalChance;
        CriticalDamagePercent = stats.CurrentCriticalDamagePercent;
    }
    public void UpdateEffects(IEnumerable<StatusEffect> effects)
    {
        OnhitEffects = effects.ToArray();
    }
    public virtual bool HitMonster(Monster monster)
    {
        if (Destroyed)
            return false;
        if (monster != null)
        {
            var criticalRoll = Random.Range(1, 101);
            var critical = criticalRoll < CriticalChance;
            var damage = Damage;
            var critDamage = Damage * CriticalDamagePercent / 100;
            if (critical)
                damage += critDamage;

            monster.GetHit(damage);
            foreach (var effect in OnhitEffects)
                monster.AddEffect(effect);
            return true;
        }
        return false;
    }
    protected virtual void Destruction()
    {
        GameObject.Destroy(gameObject);
        Destroyed = true;
    }
}
public enum Targeting
{
    ClosestEnemy,
    RandomEnemy,
    RandomDirection
}