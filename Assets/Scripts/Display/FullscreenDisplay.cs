using UnityEngine;
using System.Collections;

public class FullscreenDisplay : MonoBehaviour {

    static FullscreenDisplay instance;

    static string upperPain = "_UpperPain";
    static string lowerPain = "_LowerPain";

    SafeCoroutine upperPainCoroutine;
    SafeCoroutine lowerPainCoroutine;

    public static FullscreenDisplay Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] protected Renderer displayRenderer;
    Material displayMaterial;

    void Awake()
    {
        instance = this;
        displayRenderer.material = new Material(displayRenderer.material);
        displayMaterial = displayRenderer.material;
    }

    public void TriggerUpperPain()
    {
        KillCoroutine(upperPainCoroutine);
        upperPainCoroutine = this.StartSafeCoroutine(PainDisplay(upperPain));
    }

    public void TriggerLowerPain()
    {
        KillCoroutine(lowerPainCoroutine);
        lowerPainCoroutine = this.StartSafeCoroutine(PainDisplay(lowerPain));
    }

    void KillCoroutine(SafeCoroutine target)
    {
        if (target != null && target.IsRunning)
        {
            target.Stop();
        }
    }

    IEnumerator PainDisplay(string targetProperty)
    {
        float startValue = displayMaterial.GetFloat(targetProperty);
        float fadeIn = .1f;
        float fadeOut = .5f;
        for (float i = 0; i < 1; i += Time.deltaTime / fadeIn)
        {
            displayMaterial.SetFloat(targetProperty, Mathf.Lerp(startValue, 1, i));
            yield return null;
        }
        for (float i = 0; i < 1; i += Time.deltaTime / fadeOut)
        {
            displayMaterial.SetFloat(targetProperty, Mathf.Lerp(1, 0, i));
            yield return null;
        }
        displayMaterial.SetFloat(targetProperty, 0);
    }
	
}
