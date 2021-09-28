using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayTextTrigger : Trigger
{
    private TextMeshProUGUI displayText;
    private IEnumerator fade = null;

    public string text;
    public Color color = Color.white;
    public float duration = 3f;
    public float decayTime = 1f;


    void Start()
    {
        displayText = GameObject.FindGameObjectWithTag("UIDisplayText").GetComponent<TextMeshProUGUI>();
    }

    public override void TriggerAction()
    {
        if (!(DestroyThis && fade != null)) //Don't pick up multiple times if destroyThis = true
        {
            displayText.text = text;
            displayText.color = color;
            displayText.CrossFadeAlpha(1f, 0f, true);

            fade = Fade();
            StartCoroutine(fade);
        }
    }

    private IEnumerator Fade()
    {
        yield return new WaitForSeconds(duration);

        //Fade out
        displayText.CrossFadeAlpha(0f, decayTime, true);

        //Destroy self
        if (DestroyThis)
        {
            isTriggered = true;
        }
    }

}