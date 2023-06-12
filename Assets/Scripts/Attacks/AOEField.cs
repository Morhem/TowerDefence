using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AOEField : BaseTowerAttack
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void SetStats(CharacterStatBlock stats)
    {
        base.SetStats(stats);
    }
    void Update()
    {
        if (MissionController.main.Pause)
            return;
        foreach (var monster in colliderList.ToList())
        if (monster == null || !monster.activeSelf)
                colliderList.Remove(monster);
    }
    public void HitEveryone()
    {
        foreach (var collider in colliderList)
        {
            if (collider.tag == "Monster")
            {
                var ms = collider.GetComponent<Monster>();
                HitMonster(ms);
            }
        }
    }
    public List<GameObject> colliderList = new List<GameObject>();

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Monster" && !colliderList.Contains(collider.gameObject))
        {
            colliderList.Add(collider.gameObject);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (colliderList.Contains(collider.gameObject))
        {
            colliderList.Remove(collider.gameObject);
        }
    }
}
