using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradePanelScript : MonoBehaviour
{
    public Dictionary<UpgradeButton, Character> upgradePool = new Dictionary<UpgradeButton, Character>();
    public int upgradeCount;
    List<GameObject> existingButtons = new List<GameObject>();

    private void Start()
    {
        MissionController.main.upgradeInProgress = true;
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
            var kvp = upgradePoolTMP[buttonIndex];
            var upButton = kvp.Key;
            var tower = kvp.Value;
            var button = GameObject.Instantiate(upButton.gameObject);
            existingButtons.Add(button);
            var bs = button.GetComponent<UpgradeButton>();
            bs.tower = tower;
            //if (bs.singularUpgrade)
            upgradePoolTMP.RemoveAt(buttonIndex);
        }
    }

}
