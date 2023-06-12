using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    public GameObject characterPrefab;
    public bool unlocked;
    Image background;
    Image portrait;
    CharacterScreenScript css;

    // Start is called before the first frame update
    void Start()
    {
        var text = GetComponentInChildren<TMPro.TMP_Text>();
        var images = GetComponentsInChildren<Image>();
        var character = characterPrefab.GetComponent<Character>();
        var csb = characterPrefab.GetComponent<CharacterStatBlock>();
        background = images[0];
        portrait = images[1];
        int level = CharacterScreenScript.GetCharacterLevel(csb.characterName);
        css = GameObject.Find("CharactersTab").GetComponent<CharacterScreenScript>();
        text.text = $"{csb.characterName} ({level})";
        portrait.sprite = character.CharacterPortrait;
    }

    public void Toggle()
    {
        css.SelectCharacter(characterPrefab);
    }
}
