using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradePanelScript : MonoBehaviour
{
    public Dictionary<AbilityCardScriptableObject, Character> upgradePool = new Dictionary<AbilityCardScriptableObject, Character>();
    public int upgradeCount;
    public bool isChest;
    List<GameObject> existingButtons = new List<GameObject>();

    public GameObject buttonPrefab;

    private void Start()
    {
        MissionController.main.upgradeInProgress = true;
        buttonPrefab = MissionController.main.upgradeButtonPrefab;
        CreateButtons();
    }
    public void CreateButtons()
    {
        foreach (Transform tran in transform)
        {
            GameObject.Destroy(tran.gameObject);
        }
        foreach (var button in existingButtons)
            GameObject.Destroy(button);
        existingButtons.Clear();
        var upgradePoolTMP = upgradePool.ToList();
        for (int i = 0; i < upgradeCount; i++)
        {
            if (!upgradePoolTMP.Any())
                break;
            

            var buttonIndex = Random.Range(0, upgradePoolTMP.Count());
            if (isChest && upgradePoolTMP.Any(x => x.Key.chestOnly))
            {
                buttonIndex = upgradePoolTMP.IndexOf(upgradePoolTMP.First(x => x.Key.chestOnly));
            }

            var kvp = upgradePoolTMP[buttonIndex];
            var abilityCard = kvp.Key;
            var tower = kvp.Value;
            var button = GameObject.Instantiate(buttonPrefab, transform);
            existingButtons.Add(button);
            var bs = button.GetComponent<UpgradeButton>();
            bs.character = tower;
            bs.Ability = abilityCard;
            //if (bs.singularUpgrade)
            upgradePoolTMP.RemoveAt(buttonIndex);
        }
    }

}
