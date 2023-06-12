using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    RectTransform canvas;
    GameObject textPrefab;

    [SerializeField]
    GameObject projectilePrefab;
    [SerializeField]
    Transform projectileSpawnPoint;
    [SerializeField]
    AttackType attackType;

    public bool CarriesChest;

    public int HP;
    public int Gold;
    [SerializeField]
    float speed;
    [SerializeField]
    float attackDistance;
    [SerializeField]
    float stoppingDistance;
    Rigidbody rb;
    NavMeshAgent nag;
    NavMeshObstacle nob;
    Collider target;

    Vector3 navDestination;

    [SerializeField]
    int attackDamage;
    [SerializeField]
    float attackCooldown;
    float currentCooldown = 0f;

    public bool MonsterWavePack;

    

    bool isWalking = false;

    Animator anim;

    public List<StatusEffect> Effects = new List<StatusEffect>();
    Vector3 knockback = Vector3.zero;

    public void AddEffect(StatusEffect otherEffect)
    {
        var currentEffect = Effects.FirstOrDefault(x => x.effect == otherEffect.effect);
        if (currentEffect == null || otherEffect.stacking)
        {
            var copy = otherEffect.ShallowCopy();
            Effects.Add(copy);
            if (otherEffect.effect == StatusEffect.Effects.Knockback)
                //transform.position = Vector3.MoveTowards(transform.position, target.transform.position, -otherEffect.magnitude);
                knockback = (transform.position - target.transform.position).normalized * otherEffect.magnitude;
        }
        else
        {
            if (otherEffect.refreshing && currentEffect.refreshing)
                currentEffect.RefreshEffect(otherEffect);
        }
    }

    public void GetHit(int damage)
    {
        HP -= damage;
        if (canvas != null && textPrefab != null)
        {
            SpawnDamageText(damage.ToString());
        }
        if (HP <= 0)
        {
            MissionController.main.AddXP(Gold);
            MissionController.main.Monsters.Remove(this);
            canvas.transform.SetParent(null);
            if (CarriesChest)
                MissionController.main.SpawnRewardChest();
            GameObject.Destroy(gameObject);
        }

    }
    async void SpawnDamageText(string damage)
    {
        Vector3 spawnPosition = GetBottomLeftCorner(canvas) - new Vector3(Random.Range(canvas.rect.xMin, canvas.rect.xMax), Random.Range(0, canvas.rect.y), 0);
        GameObject text = Instantiate(textPrefab, spawnPosition, Quaternion.identity, canvas);
        var ts = text.GetComponent<DamageText>();
        ts.SetText(damage);
        await Task.Yield();
    }
    Vector3 GetBottomLeftCorner(RectTransform rt)
    {
        Vector3[] v = new Vector3[4];
        rt.GetWorldCorners(v);

        return new Vector3(v.Average(q => q.x), v.Average(q => q.y), v.Average(q => q.z));
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        canvas = transform.Find("Canvas").GetComponent<RectTransform>();
        textPrefab = Resources.Load<GameObject>("UIPrefabs/TextDamage");
        var co = Resources.Load<GameObject>("UIPrefabs/MonsterCanvas");


        FindTarget();
        FaceTarget(target.transform.position, 15f);
        nag = GetComponent<NavMeshAgent>();
        nob = GetComponent<NavMeshObstacle>();
        nob.radius = nag.radius;
        nag.speed = speed;
        Retarget();

        
    }

    public bool IsInBounds()
    {
        var screenCoords = Camera.main.WorldToViewportPoint(transform.position);

        return screenCoords.x > 0 && screenCoords.x < 1 && screenCoords.y > 0 && screenCoords.y < 1;
    }

    int retargetCounter = 0;
    void Update()
    {
        if (MissionController.main.Pause)
        {
            nag.speed = 0;
            return;
        }
        #region Targeting, Walking & Stopping
        if (retargetCounter++ > 60)
            Retarget();
        var targetDistance = navDestination.magnitude > 1000 ? 0f : Vector3.Distance(transform.position, navDestination);
        if (nag.enabled && targetDistance <= stoppingDistance && IsInBounds())
            StopNavigation();
        if (!nag.enabled && (targetDistance > stoppingDistance || !IsInBounds()))
        {
            StartNavigation();
            Retarget();
        }

        #endregion
        #region Speed
        var currentSpeed = speed;

        foreach (var slow in Effects.Where(x => x.effect == StatusEffect.Effects.Slow))
        {
            currentSpeed -= currentSpeed * slow.magnitude;
        }

        foreach (var freeze in Effects.Where(x => x.effect == StatusEffect.Effects.Freeze))
        {
            currentSpeed = 0;
        }
        if (currentSpeed < 0)
            currentSpeed = 0;
        if (nag.speed != currentSpeed)
            nag.speed = currentSpeed;

        #endregion
        #region Effects

        foreach (var effect in Effects.ToList())
        {
            if (!effect.ExpendDuration(Time.deltaTime))
                Effects.Remove(effect);
        }

        // apply the knockback effect:
        if (knockback.magnitude > 0.2f)
        {
            StartNavigation();
            transform.Translate(knockback * Time.deltaTime, Space.World);
        }
        // knockback energy goes by over time:
        knockback = Vector3.Lerp(knockback, Vector3.zero, 5 * Time.deltaTime);
    

        #endregion
        #region Animations & Attack
        if (targetDistance <= attackDistance)
        {
            if (currentCooldown <= 0f && !isWalking)
            {
                anim.Play(MonsterWavePack ? "Attack01" : "Attack");

                currentCooldown = attackCooldown;
            }

            currentCooldown -= Time.deltaTime;
        }
        if (!isWalking && nag.velocity.magnitude >= 0.8f)
        {
            anim.Play(MonsterWavePack ? "RunFWD" : "Run");
            isWalking = true;
        }
        if (isWalking && nag.velocity.magnitude <= 0.5f)
        {
            anim.Play(MonsterWavePack ? "IdleBattle" : "Idle");
            isWalking = false;
        }
        #endregion
    }
    void StartNavigation()
    {
        nob.enabled = false;
        nag.enabled = true;
    }
    void StopNavigation()
    {
        nag.enabled = false;
        nob.enabled = true;
    }
    void Retarget()
    {
        if (nag.enabled)
        {
            navDestination = target.ClosestPoint(transform.position);
            nag.destination = navDestination;
            retargetCounter = 0;
        }
    }

    void FindTarget()
    {
        target = GameObject.Find("Tower").GetComponent<Collider>();
    }
    private void FaceTarget(Vector3 destination, float rotationSpeed = 2f)
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 2f);
    }
    void AttackAnimationDone()
    {
        FaceTarget(target.transform.position);
        if (attackType == AttackType.Melee)
        {
            MissionController.main.GetHit(attackDamage);
        }
        if (attackType == AttackType.Ranged)
        {
            var bullet = GameObject.Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            var ps = bullet.GetComponent<MonsterProjectile>();
            if (ps != null)
            {
                ps.Damage = attackDamage;
            }
        }
    }
}
enum AttackType
{
    Melee,
    Ranged
}