using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public Sprite characterSprite;
    [System.NonSerialized]
    public Color backgroundColor;

    [System.NonSerialized]
    public Character character;

    public AbilityCardScriptableObject Ability;


    public void SaveSO()
    {
        //AbilityCardScriptableObject example = ScriptableObject.CreateInstance<AbilityCardScriptableObject>();

        //example.ID = this.ID;
        //example.title = this.title;
        //example.singularUpgrade = this.singularUpgrade;
        //example.chestOnly = this.chestOnly;
        //example.characterSprite = this.characterSprite;
        //example.backgroundColor = this.backgroundColor;
        //example.isPublic = this.isPublic;
        //example.perk = this.perk;
        //example.magnitude1 = this.magnitude1;
        //example.magnitude2 = this.magnitude2;

        //var folder = this.ID.Split(' ')[0];

        //string path = $"Assets/Resources/UIPrefabs/Abilities/{folder}/{gameObject.name}.asset";
        //AssetDatabase.CreateAsset(example, path);
        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();

    }




    private void Start()
    {
        var text = GetComponentInChildren<Text>();
        var background = GetComponent<Image>();
        var abilityIcon = transform.Find("AbilityIcon").GetComponent<Image>();
        var characterIcon = transform.Find("CharacterIcon").GetComponent<Image>();
        text.text = Ability.title;
        background.color = character?.CharacterColor ?? Color.gray;
        abilityIcon.sprite = MissionController.main.icons[Ability.perk];

        var btnComponent = GetComponent<Button>();
        btnComponent.onClick.AddListener(Tap);

        if (character != null)
            characterIcon.sprite = character.CharacterPortrait;
        else
            GameObject.Destroy(characterIcon.gameObject);
    }
    public void Tap()
    {
        var upgrade = new Upgrade { perk = Ability.perk, magnitude1 = Ability.magnitude1, magnitude2 = Ability.magnitude2, id = Ability.ID, character = character };

        MissionController.main.Upgrade(upgrade, character, Ability.isPublic);

        

        var panel = GameObject.Find("PanelUpgrades(Clone)");
        GameObject.Destroy(panel);
        MissionController.main.upgradeInProgress = false;
        MissionController.main.Pause = false;
        MissionController.main.CheckPendingUpgrades();
    }


}

public class Upgrade
{
    public string id;
    public Perk perk;
    public float magnitude1;
    public float magnitude2;
    public Character character;
}

public enum Perk
{
    DamagePercent,
    DamageFlat,
    Range,
    RadiusAOE,
    BounceAmount,
    RicochetAmount,
    BulletSpeed,
    TTL,
    AttackInterval,
    CriticalChance,
    PuddleFire,
    PuddlePoison,
    UndeadPact,
    ChakramAugument,
    Multishot,
    BurstIncrease,
    XPMultiplier,
    CriticalDamagePercent,
    EffectSlow,
    AdditionalMiniboss
}
