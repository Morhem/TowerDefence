
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityCardScriptableObject", menuName = "ScriptableObjects/AbilityCard")]
public class AbilityCardScriptableObject : ScriptableObject
{
    public string ID;
    [SerializeField]
    public string title;
    public bool singularUpgrade = false;
    public bool chestOnly = false;

    public bool isPublic;
    public Perk perk;
    public float magnitude1;
    public float magnitude2;


    public override string ToString()
    {
        return perk.ToString();
    }
}

