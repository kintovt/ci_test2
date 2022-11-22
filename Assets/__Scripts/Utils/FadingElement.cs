using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class FadeDuration
{
    public float Delay = 0.0f;
    public float In = 1.0f;
    public float Stay = 3.0f;
    public float Out = 1.0f;

    public float TotalTime
    {
        get
        {
            return Delay + In + Stay + Out;
        }
    }
}

public class FadingElement : MonoBehaviour {

    public UnityEvent OnFadingInCompleteAction;
    public UnityEvent OnStayingCompleteAction;
    public UnityEvent OnFadingOutCompleteAction;
    public Graphic Graphic;
    public CanvasGroup CanvasGroup;
    public bool Faded;
    public bool AutoStart;
    public bool AutoPlay;
    public bool Blocked;

    [SerializeField]
    public FadeDuration FadeDuration;

    private YieldInstruction fadeInstruction = new YieldInstruction();

    void Start () {

        Color c = Color.white;
        if (Graphic != null)
            c = Graphic.color;
        else
        {
            if (this.GetComponent<Graphic>() != null)
                Graphic = this.GetComponent<Graphic>();
        }

        if (Faded)
        {
            c.a = 0;
            if (Graphic != null) Graphic.color = c;
            if (CanvasGroup != null) CanvasGroup.alpha = 0;
        }
        else
        {
            c.a = 1;
            if (Graphic != null) Graphic.color = c;
            if (CanvasGroup != null) CanvasGroup.alpha = 1;
        }
        if (AutoStart && !Blocked)
            FadeIn(FadeDuration.In);
    }

    public void FadeIn(float value)
    {
        if (gameObject.activeInHierarchy && !Blocked) StartCoroutine(FadingIn(value));
    }

    public void Stay()
    {
        if (gameObject.activeInHierarchy && !Blocked) StartCoroutine(Staying());
    }

    public void FadeOut()
    {
        if (gameObject.activeInHierarchy && !Blocked) StartCoroutine(FadingOut());
    }

    IEnumerator Staying()
    {
        yield return new WaitForSeconds(FadeDuration.Stay);
        OnStayingCompleteAction.Invoke();
        if (AutoPlay)
            FadeOut();
    }

    IEnumerator FadingIn(float value)
    {
        yield return new WaitForSeconds(FadeDuration.Delay);
        Color c = Color.white;
        float elapsedTime = 0.0f;
        if (Graphic != null)
            c = Graphic.color;
        else
            c.a = CanvasGroup.alpha;

        while (elapsedTime < value)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsedTime / FadeDuration.In);

            if (Graphic != null) Graphic.color = c;
            if (CanvasGroup != null) CanvasGroup.alpha = c.a;
        }
        OnFadingInCompleteAction.Invoke();
        if (AutoPlay) Stay();
    }

    IEnumerator FadingOut()
    {
        Color c = Color.white;
        float elapsedTime = 0.0f;
        if (Graphic != null)
            c = Graphic.color;
        else
            c.a = CanvasGroup.alpha;

        while (elapsedTime < FadeDuration.Out)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime;
            c.a = 1.0f - Mathf.Clamp01(elapsedTime / FadeDuration.Out);

            if (Graphic != null) Graphic.color = c;
            if (CanvasGroup != null) CanvasGroup.alpha = c.a;
        }
        OnFadingOutCompleteAction.Invoke();
    }
}
