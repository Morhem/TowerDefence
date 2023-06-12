using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCard : MonoBehaviour
{
    [System.NonSerialized]
    public Character character;
    [System.NonSerialized]
    public int count;
    Image portrait;
    Image background;
    void Start()
    {
        var charName = transform.Find("Name").GetComponent<TMPro.TMP_Text>();
        var countText = transform.Find("Count").GetComponent<TMPro.TMP_Text>();
        background = GetComponentsInChildren<Image>()[0];
        portrait = GetComponentsInChildren<Image>()[1];

        var csb = character.gameObject.GetComponent<CharacterStatBlock>();

        charName.text = csb.characterName;
        countText.text = count.ToString();
        background.color = character.CharacterColor;
        portrait.sprite = character.CharacterPortrait;
    }
}
