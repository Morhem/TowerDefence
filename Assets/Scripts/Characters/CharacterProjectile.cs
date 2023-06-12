using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterProjectile : Character
{
    protected float currentBurstCooldown = 0f;
    protected int currentBurstAmount = 1;


    void Update()
    {
        if (MissionController.main.Pause)
            return;
        if (ongoingCooldown <= 0f)
        {
            var target = BaseTowerAttack.FindTarget(MissionController.main.transform.position, float.MaxValue, Stats.Targeting);
            if (target != null)
            {
                if (currentBurstAmount > 0)
                {
                    if (currentBurstCooldown <= 0)
                    {
                        transform.LookAt(target.transform.position);

                        Attack();
                        Stats.Target = target;
                        var bullet = GameObject.Instantiate(ProjectilePrefab, transform.position, Quaternion.identity);
                        bullet.name = $"Bullet{Random.Range(0, 1000)}";
                        var ps = bullet.GetComponent<BaseTowerAttack>();
                        if (ps != null)
                        {
                            ps.SetStats(Stats);
                            ps.UpdateEffects(OnhitEffects);
                        }
                        currentBurstAmount--;
                        currentBurstCooldown = Stats.BurstCooldown;
                    }
                }
                else
                {
                    ongoingCooldown = Stats.CurrentCooldown;
                    currentBurstCooldown = Stats.BurstCooldown;
                    currentBurstAmount = Stats.CurrentBurst;
                }
            }
        }

        ongoingCooldown -= Time.deltaTime;
        currentBurstCooldown -= Time.deltaTime;

    }

}
