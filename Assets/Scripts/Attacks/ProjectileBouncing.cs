using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBouncing : BaseTowerAttack
{
    Targeting targeting = Targeting.ClosestEnemy;
    GameObject Target;
    List<GameObject> ignoreTargets = new List<GameObject>();
    public bool  DestroyOnMonsterHit;

    public GameObject explosionPrefab;

    float explosionRadius;

    float TTL;
    float speed;

    float lifetime = 0f;
    int bounces = 0;

    int RicochetAmount;
    float RicochetRange;

    Rigidbody rb;
    bool isPaused = false;
    Vector3 savedVelocity;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Launch();
    }
    void Launch()
    {
        transform.LookAt(Target.transform);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        rb.velocity = (transform.forward * speed);
    }
    public override void SetStats(CharacterStatBlock stats)
    {
        speed = stats.CurrentProjectileSpeed;
        explosionRadius = stats.CurrentAOERadius;
        TTL = stats.CurrentTTL;
        bounces = stats.CurrentBounces;
        RicochetAmount = stats.CurrentRicochetAmount;
        RicochetRange = stats.CurrentRicochetRange;
        Target = stats.Target;
        ignoreTargets.AddRange(stats.IgnoreTargets);
        base.SetStats(stats);
    }

    private void Update()
    {
        if (MissionController.main.Pause)
        {
            if (!isPaused)
            {
                isPaused = true;
                OnPauseGame();
            }
            return;
        }
        else
        {
            if (isPaused)
            {
                isPaused = false;
                OnResumeGame();
            }
        }
        if (lifetime > TTL)
            Destruction();
        lifetime += Time.deltaTime;
        Bounce();
    }
    bool bouncing = false;
    void Bounce()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        if ((screenPosition.y > Screen.height) || (screenPosition.y < 0f) || (screenPosition.x > Screen.width) || (screenPosition.x < 0f))
        {
            if (bounces > 0)
            {
                bouncing = true;

                var reflectAxis = (screenPosition.x > Screen.width) || (screenPosition.x < 0f) ? Vector3.right : Vector3.forward;

                screenPosition.x = Mathf.Clamp(screenPosition.x, 0f, Screen.width);
                screenPosition.y = Mathf.Clamp(screenPosition.y, 0f, Screen.height);
                Vector3 newWorldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
                transform.position = new Vector3(newWorldPosition.x, transform.position.y, newWorldPosition.z);
                rb.velocity = Vector3.Reflect(rb.velocity, reflectAxis);
                bounces--;
                bouncing = false;
            }
            else
                if (!bouncing)
                Destruction();
        }


    }
    bool Ricochet()
    {
        var nTarget = FindTarget(transform.position, RicochetRange, targeting, ignoreTargets.ToArray());
        if (nTarget != null)
        {
            Target = nTarget;
            Launch();
            return true;
        }
        return false;
    }

    void OnPauseGame()
    {
        savedVelocity = rb.velocity;
        rb.isKinematic = true;
    }
    void OnResumeGame()
    {
        rb.isKinematic = false;
        rb.AddForce(savedVelocity, ForceMode.VelocityChange);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")// && !ignoreTargets.Contains(other.gameObject))
        {

            var ms = other.GetComponent<Monster>();
            var hit = HitMonster(ms);
            if (hit)
            {
                ignoreTargets.Add(other.gameObject);
                var ric = RicochetAmount-- > 0;
                bool ricSuccess = false;
                if (ric)
                    ricSuccess = Ricochet();

                if (explosionPrefab != null)
                {
                    ExplosionScript.CreateExplosion(transform.position, Damage, explosionRadius, explosionPrefab);
                }

                if (DestroyOnMonsterHit && (!ricSuccess || !ric))
                {
                    Destruction();
                }
            }

        }

    }

    protected override void Destruction()
    {
        base.Destruction();
    }
}
