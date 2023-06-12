using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_Text textElement;
    [SerializeField]
    float fadeoutTime;
    void Start()
    {
        textElement.CrossFadeAlpha(0, fadeoutTime, false);
    }
    public void SetText(string text)
    {
        textElement.text = text;
    }
    void Update()
    {
        fadeoutTime -= Time.deltaTime;
        if (fadeoutTime <= 0)
        {
            if (transform.root.name == "Canvas")
            {
                GameObject.Destroy(transform.root.gameObject);
            }
            else
                GameObject.Destroy(gameObject);
        }
    }
}
