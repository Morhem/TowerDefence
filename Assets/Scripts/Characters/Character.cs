using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [SerializeField]
    public Color CharacterColor;
    [SerializeField]
    public Sprite CharacterPortrait;
    [SerializeField]
    public bool UnlockedByDefault;
    [System.NonSerialized]
    public bool Unlocked;
    Animator anim;

    public GameObject ProjectilePrefab;
    public UpgradeButton[] PossibleUpgrades;
    public CharacterStatBlock Stats;
    protected float ongoingCooldown = 0f;
    protected int Level = 0;
    public List<StatusEffect> OnhitEffects = new List<StatusEffect>();

    protected virtual void Start()
    {
        Stats = GetComponent<CharacterStatBlock>();
        anim = GetComponent<Animator>();
        ongoingCooldown = 0;
    }

    protected void Attack()
    {
        anim.Play("Attack");
    }
    protected void Idle()
    {
        anim.Play("Idle");
    }
    protected void Taunt()
    {
        anim.Play("Taunt");
    }
    protected void Cheer()
    {
        anim.Play("Cheer");
    }

    public virtual void Upgrade(Upgrade upgrade)
    {
        switch (upgrade.perk)
        {
            case Perk.EffectSlow:
                OnhitEffects.Add(new StatusEffect { effect = StatusEffect.Effects.Slow, duration = upgrade.magnitude2, magnitude = upgrade.magnitude1, refreshing = true });
                break;
            default:
                Stats.Upgrade(upgrade);
                break;
        }
        
    }
}
