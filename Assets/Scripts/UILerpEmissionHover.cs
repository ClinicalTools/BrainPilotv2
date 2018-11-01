using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILerpEmissionHover : UIInteractState
{
    protected Color startingColor;
    public Color hoverColor = Color.red;
    public Color clickColor = Color.green;

    public float transitionTime = 0.1f;

    public override void Load()
    {
        base.Load();
        startingColor = uiElement.meshRenderer.material.GetColor("_EmissionColor");
        SwitchColor(hoverColor);
    }

    public override void Activate()
    {
        base.Activate();
        SwitchColor(clickColor);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        SwitchColor(hoverColor);
    }

    public override void Remove()
    {
        SwitchColor(startingColor);

        Invoke("base.Remove", transitionTime);
    }

    void SwitchColor(Color nextColor)
    {
        StopAllCoroutines();
        StartCoroutine(LerpToColor(nextColor));
    }

    IEnumerator LerpToColor(Color nextColor)
    {
        float timeElapsed = 0;
        Color lastColor = uiElement.meshRenderer.material.GetColor("_EmissionColor");
        while (timeElapsed <= transitionTime)
        {
            float t = timeElapsed / transitionTime;
            Color color = Color.Lerp(lastColor, nextColor, t);

            uiElement.meshRenderer.material.SetColor("_EmissionColor", color);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

    }


}
