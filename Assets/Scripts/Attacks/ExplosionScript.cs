using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    [SerializeField]
    protected ParticleSystem Explosion;

    [SerializeField]
    protected GameObject AOEMarkerPrefab;

    public float SizeMin;
    public float SizeMax;
    public int Damage;
    public float Range;

    public void Explode()
    {
        var main = Explosion.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(SizeMin, SizeMax);
        var targets = Physics.OverlapSphere(transform.position, Range).ToArray();

        var marker = GameObject.Instantiate(AOEMarkerPrefab, transform.position, Quaternion.identity);
        marker.transform.localScale = new Vector3(Range * 2, Range * 2, Range * 2);

        foreach (var target in targets)
        {
            if (Range == 0f)
                break;
            var other = target.gameObject;
            //var distance = Vector2.Distance(transform.position, target.ClosestPoint(transform.position));


            if (other.tag == "Monster")
            {
                var es = other.GetComponent<Monster>();
                if (es != null)
                {
                    es.GetHit(Damage);
                }
            }

        }

        Explosion.Play();
    }

    public static void CreateExplosion(Vector3 position, int damage, float damageRange, GameObject explosionPrefab)
    {
        var explosionObject = GameObject.Instantiate(explosionPrefab, position, Quaternion.identity);
        var es = explosionObject.GetComponent<ExplosionScript>();
        es.Damage = damage;
        es.Range = damageRange;
        es.SizeMin = 3f;
        es.SizeMax = 3f;
        es.Explode();

    }

}
