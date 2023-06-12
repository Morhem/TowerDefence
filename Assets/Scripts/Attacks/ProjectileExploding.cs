using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileExploding : ProjectileGuided
{
    public float particleLifeMin;
    public float particleLifeMax;
    public float explosionRadius;
    bool exploded = false;

    public override void SetStats(CharacterStatBlock stats)
    {
        explosionRadius = stats.CurrentAOERadius;
        base.SetStats(stats);
    }

    protected override void OnTriggerEnter(Collider other)
    {

    var ms = other.GetComponent<Monster>();
        if (ms != null && !exploded)
        {
            if (explosionPrefab == null)
            {
                ms.GetHit(Damage);
            }
            else
            {
                ExplosionScript.CreateExplosion(transform.position, Damage, explosionRadius, explosionPrefab);
            }
            exploded = true;
            GameObject.Destroy(gameObject);
        }
    }
}
