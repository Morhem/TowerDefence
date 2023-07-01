using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{
    [SerializeField]
    GameObject imagePrefab;

    List<GameObject> columnPanels = new List<GameObject>();
    List<TMPro.TMP_Text> characterNames = new List<TMPro.TMP_Text>();


    void Start()
    {
        MissionController.main.Pause = true;
        foreach (Transform child in transform)
        {
            columnPanels.Add(child.gameObject);
            var text = child.Find("CharacterName").GetComponent<TMPro.TMP_Text>();
            characterNames.Add(text);
        }
        SetUpgradeImage(null, characterNames[0], columnPanels[0]);
        for (int i = 0; i < MissionController.main.characters.Count; i++)
        {
            var character = MissionController.main.characters[i];
            SetUpgradeImage(character, characterNames[i + 1], columnPanels[i + 1]);
        }
        
    }

    void SetUpgradeImage(Character character, TMPro.TMP_Text textComponent, GameObject panel)
    {
        textComponent.text = character == null ? "All" : character.CharacterName;
        foreach (var upgrade in MissionController.main.upgrades.Where(x => x.character == character))
        {
            var sprite = MissionController.main.icons[upgrade.perk];
            var image = GameObject.Instantiate(imagePrefab, panel.transform).GetComponent<Image>();
            image.sprite = sprite;
        }
    }

    private void OnDestroy()
    {
        MissionController.main.Pause = false;
    }
}
