using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRicochet : ProjectileGuided
{
    public int RicochetAmount;
    public float RicochetRange;
    public List<GameObject> excludeTargets = new List<GameObject>();
    public Targeting targeting;
    private void Start()
    {
        
    }

    public override void SetStats(CharacterStatBlock stats)
    {
        RicochetAmount = stats.CurrentRicochetAmount;
        RicochetRange = stats.CurrentRicochetRange;
        base.SetStats(stats);
    }
    void Ricochet()
    {
        var nTarget = FindTarget(transform.position, RicochetRange, targeting, excludeTargets.ToArray());
        var nBullet = GameObject.Instantiate(this, transform.position, transform.rotation);
        var ps = nBullet.GetComponent<BaseTowerAttack>();
        ps.SetStats(new CharacterStatBlock
        {
            Damage = Damage,
            Target = Target,
            RicochetAmount = RicochetAmount--,
            RicochetRange = RicochetRange
        });
        ps.UpdateEffects(OnhitEffects);
    }
    protected override void Destruction()
    {
        if (RicochetAmount-- > 0)
        {
            Ricochet();
            
        }
        base.Destruction();
    }
}
