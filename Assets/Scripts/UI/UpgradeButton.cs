using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public string ID;
    [SerializeField]
    string title;
    public bool singularUpgrade = false;
    public bool chestOnly = false;
    public Sprite characterSprite;
    [System.NonSerialized]
    public Color backgroundColor;

    public bool isPublic;
    public Perk perk;
    public float magnitude1;
    public float magnitude2;




    [System.NonSerialized]
    public Character tower;

    private void Start()
    {
        var panel = GameObject.Find("ButtonPanel");


        var button = GameObject.Instantiate(MissionController.main.upgradeButtonPrefab, panel.transform);


        var text = button.GetComponentInChildren<Text>();
        var background = button.GetComponent<Image>();
        var abilityIcon = button.transform.Find("AbilityIcon").GetComponent<Image>();
        var characterIcon = button.transform.Find("CharacterIcon").GetComponent<Image>();
        text.text = title;
        background.color = tower?.CharacterColor ?? Color.gray;
        abilityIcon.sprite = MissionController.main.icons[perk];

        var btnComponent = button.GetComponent<Button>();
        btnComponent.onClick.AddListener(Tap);

        if (tower != null)
            characterIcon.sprite = tower.CharacterPortrait;
        else
            GameObject.Destroy(characterIcon.gameObject);
    }
    public void Tap()
    {
        var upgrade = new Upgrade { perk = perk, magnitude1 = magnitude1, magnitude2 = magnitude2, id = ID };

        MissionController.main.Upgrade(upgrade, tower, isPublic);

        

        var panel = GameObject.Find("PanelUpgrades(Clone)");
        GameObject.Destroy(panel);
        MissionController.main.upgradeInProgress = false;
        MissionController.main.CheckPendingUpgrades();
        MissionController.main.Pause = false;
    }


}

public class Upgrade
{
    public string id;
    public Perk perk;
    public float magnitude1;
    public float magnitude2;
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
