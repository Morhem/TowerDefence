using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterStatBlock : MonoBehaviour
{
    public GameObject Target;
    public GameObject[] IgnoreTargets = new GameObject[] { };

    public string characterName;

    public int CharacterMetaLevel;

    public int Damage;
    public int CriticalChance;
    public int CriticalDamagePercent;
    public float Cooldown;
    public float ProjectileSpeed;
    public float AOERadius;
    public int RicochetAmount;
    public float RicochetRange;
    public float TTL;
    public int Bounces;
    public int Burst;
    public float BurstCooldown;
    public Targeting Targeting;

    public string SaveToString()
    {
        var result = $"{characterName};{CharacterMetaLevel};{Damage};{CriticalChance};{CriticalDamagePercent};{Cooldown};{ProjectileSpeed};{AOERadius};{RicochetAmount};{RicochetRange};{TTL};{Bounces};{Burst};{BurstCooldown};{(int)Targeting}";
        result = result.Replace(',', '.').Replace(';', ',');
        return result;
    }

    public string FillFromString(string sheetText)
    {
        var charInfo = sheetText.Split(",");
        characterName = charInfo[0];
        CharacterMetaLevel = int.Parse(charInfo[1]);
        Damage = int.Parse(charInfo[2]);
        CriticalChance = int.Parse(charInfo[3]);
        CriticalDamagePercent = int.Parse(charInfo[4]);
        Cooldown = float.Parse(charInfo[5].Replace('.', ','));
        ProjectileSpeed = float.Parse(charInfo[6].Replace('.', ','));
        AOERadius = float.Parse(charInfo[7].Replace('.', ','));
        RicochetAmount = int.Parse(charInfo[8]);
        RicochetRange = float.Parse(charInfo[9].Replace('.', ','));
        TTL = float.Parse(charInfo[10].Replace('.', ','));
        Bounces = int.Parse(charInfo[11]);
        Burst = int.Parse(charInfo[12]);
        BurstCooldown = float.Parse(charInfo[13].Replace('.', ','));
        Targeting = (Targeting)int.Parse(charInfo[14]);

        return characterName;
    }

    public string FillFromCSB(CharacterStatBlock csb)
    {
        characterName = csb.characterName;
        CharacterMetaLevel = csb.CharacterMetaLevel;
        Damage = csb.Damage;
        CriticalChance = csb.CriticalChance;
        CriticalDamagePercent = csb.CriticalDamagePercent;
        Cooldown = csb.Cooldown;
        ProjectileSpeed = csb.ProjectileSpeed;
        AOERadius = csb.AOERadius;
        RicochetAmount = csb.RicochetAmount;
        RicochetRange = csb.RicochetRange;
        TTL = csb.TTL;
        Bounces = csb.Bounces;
        Burst = csb.Burst;
        BurstCooldown = csb.BurstCooldown;
        Targeting = csb.Targeting;

        return characterName;
    }
    

    public int CurrentDamage
    {
        get
        {
            return Damage + (int)Upgrades.Where(x => x.perk == Perk.DamageFlat).Sum(x => x.magnitude1) + (int)Upgrades.Where(x => x.perk == Perk.DamagePercent).Sum(x => x.magnitude1 * Damage);
        }
    }
    public int CurrentCriticalChance
    {
        get
        {
            return CriticalChance + (int)Upgrades.Where(x => x.perk == Perk.CriticalChance).Sum(x => x.magnitude1);
        }
    }
    public int CurrentCriticalDamagePercent
    {
        get
        {
            return CriticalDamagePercent + (int)Upgrades.Where(x => x.perk == Perk.CriticalDamagePercent).Sum(x => x.magnitude1);
        }
    }

    public float CurrentCooldown { get { return Upgrades.Where(x => x.perk == Perk.AttackInterval).Aggregate(Cooldown,
                    (current, next) =>
                        current - current * next.magnitude1);
        } }
    public float CurrentProjectileSpeed { get { return ProjectileSpeed + Upgrades.Where(x => x.perk == Perk.BulletSpeed).Sum(x => x.magnitude1 * ProjectileSpeed); } }
    public float CurrentAOERadius { get { return AOERadius + Upgrades.Where(x => x.perk == Perk.RadiusAOE).Sum(x => x.magnitude1 * AOERadius); } }
    public int CurrentRicochetAmount { get { return RicochetAmount + (int)Upgrades.Where(x => x.perk == Perk.RicochetAmount).Sum(x => x.magnitude1); } }
    public float CurrentRicochetRange { get { return RicochetRange; } }
    public float CurrentTTL { get { return TTL + Upgrades.Where(x => x.perk == Perk.TTL).Sum(x => x.magnitude1); } }
    public int CurrentBounces { get { return Bounces + (int)Upgrades.Where(x => x.perk == Perk.BounceAmount).Sum(x => x.magnitude1); } }
    public int CurrentBurst { get { return Burst + (int)Upgrades.Where(x => x.perk == Perk.BurstIncrease).Sum(x => x.magnitude1); } }

    List<Upgrade> Upgrades = new List<Upgrade>();

    public virtual void Upgrade(Upgrade upgrade)
    {
        Upgrades.Add(upgrade);
    }

}
