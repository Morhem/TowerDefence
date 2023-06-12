using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterPersistent : Character
{
    [SerializeField]
    bool projectilesAsChildren;
    List<BaseTowerAttack> projectiles = new List<BaseTowerAttack>();
    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < Stats.CurrentBurst; i++)
        {
            CreateProjectile(true, i);
        }

        
    }
    
    void CreateProjectile(bool setStats, int orbitIndex)
    {
        if (MissionController.main.Pause)
            return;
        var proj = GameObject.Instantiate(ProjectilePrefab, MissionController.main.transform.position, Quaternion.identity).GetComponent<BaseTowerAttack>();
        projectiles.Add(proj);
        if (projectilesAsChildren)
            proj.transform.SetParent(transform);
        if (setStats)
        {
            proj.SetStats(Stats);
            proj.UpdateEffects(OnhitEffects);
            if (proj is ProjectileOrbiting)
                SetProjectilePosition(proj, orbitIndex);
        }
    }

    void SetProjectilePosition(BaseTowerAttack proj, int orbitIndex)
    {
        var relativeAngle = (projectiles.Count() == 0 ? 0 : (360f / Stats.CurrentBurst)) * orbitIndex;
        //vector = Quaternion.AngleAxis(-45, Vector3.up) * vector;
        proj.transform.position = (Quaternion.AngleAxis(relativeAngle, Vector3.up) * Vector3.forward * Stats.CurrentAOERadius);
    }

    public override void Upgrade(Upgrade upgrade)
    {
        base.Upgrade(upgrade);
        if (upgrade.perk == Perk.BurstIncrease)
            for (int i = 0; i < upgrade.magnitude1; i++)
            {
                CreateProjectile(false, i);
            }
        int orbitIndex = 0;
        foreach (var proj in projectiles)
        {
            proj.SetStats(Stats);
            proj.UpdateEffects(OnhitEffects);
            if (proj is ProjectileOrbiting)
                SetProjectilePosition(proj, orbitIndex);

            orbitIndex++;
        }

    }

    void LateUpdate()
    {
        if (MissionController.main.Pause)
            return;
        if (projectiles.Any(x => !x.gameObject.activeSelf))
        {
            foreach (var proj in projectiles)
            {
                GameObject.Destroy(proj);
            }
            projectiles.Clear();
            for (int i = 0; i < Stats.CurrentBurst; i++)
            {
                CreateProjectile(true, i);
            }
        }
        
        if (ongoingCooldown <= 0f)
        {
            foreach (var proj in projectiles)
            {
                if (proj is AOEField)
                    (proj as AOEField).HitEveryone();
            }

            ongoingCooldown = Stats.CurrentCooldown;
        }

        ongoingCooldown -= Time.deltaTime;
    }
}
