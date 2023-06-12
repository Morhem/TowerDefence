using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardPanelScript : MonoBehaviour
{
    [System.NonSerialized]
    public List<Character> characters = new List<Character>();
    [SerializeField]
    GameObject cardPrefab;
    void Start()
    {
        var cardPanel = transform.Find("CardPanel");
        foreach (var character in characters)
        {
            var card = GameObject.Instantiate(cardPrefab, cardPanel).GetComponent<CharacterCard>();
            card.character = character;
            card.count = Random.Range(1, 11);
        }
    }

    public void Done()
    {
        GameObject.Destroy(gameObject);
    }

}
