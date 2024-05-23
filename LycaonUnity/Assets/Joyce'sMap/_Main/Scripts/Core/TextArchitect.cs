using System.Collections;
using UnityEngine;
using TMPro;
public class TextArchitect : MonoBehaviour
{
    private TextMeshProUGUI tmpro_ui;
    private TextMeshPro tmpro_world;
    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;

    public string CurrentText => tmpro.text;
    public string TargetText { get; private set; } = "";
    public string PreText { get; private set; } = "";
    private int PreTextLenght = 0;

    public string FullTargetText => PreText + TargetText;

    public enum BuildMethod { Instant, Typewriter, Fade }
    public BuildMethod buildMethod = BuildMethod.Typewriter;

    public Color TextColor { get { return tmpro.color; } set { tmpro.color = value; } }

    public float Speed { get { return BaseSpeed * SpeedMultiplier; } set { SpeedMultiplier = value; } }
    private const float BaseSpeed = 1;
    private float SpeedMultiplier = 1;

    private int CharacterPerCycle { get { return Speed <= 2f ? CharacterMupltiplier : Speed <= 2.5f ? CharacterMupltiplier * 2 : CharacterMupltiplier * 3; } }
    private int CharacterMupltiplier = 1;


    public bool HurryUp = false;

    public TextArchitect(TextMeshProUGUI tmpro_ui)
    {
        this.tmpro_ui = tmpro_ui;
    }
    public TextArchitect(TextMeshPro tmpro_world)
    {
        this.tmpro_world = tmpro_world;
    }


    public Coroutine Build(string Text)
    {
        PreText = "";
        TargetText = Text;
        Stop();

        BuildProces = tmpro.StartCoroutine(Building());
        return BuildProces;
    }
    public Coroutine Append(string Text)
    {
        PreText = tmpro.text;
        TargetText = Text;
        Stop();

        BuildProces = tmpro.StartCoroutine(Building());
        return BuildProces;
    }

    private Coroutine BuildProces = null;
    public bool IsBuilding => BuildProces != null;

    public void Stop()
    {
        if (!IsBuilding)
            return;
        tmpro.StopCoroutine(BuildProces);
        BuildProces = null;
    }

    IEnumerator Building()
    {
        Prepare();

        switch (buildMethod)
        {
            case BuildMethod.Typewriter:
                yield return Build_Typewriter();
                break;
            case BuildMethod.Fade:
                yield return Build_Fade();
                break;
        }
        OnComplete();
    }

    private void OnComplete()
    {
        BuildProces = null;
    }

    private void Prepare()
    {
        switch (buildMethod)
        {
            case BuildMethod.Instant:
                Prepare_Instant();
                break;
            case BuildMethod.Typewriter:
                Prepare_Typewriter();
                break;
            case BuildMethod.Fade:
                Prepare_Fade();
                break;
        }
    }

    private void Prepare_Instant()
    {
        tmpro.color = tmpro.color;
        tmpro.text = FullTargetText;
        tmpro.ForceMeshUpdate();
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
    }
    private void Prepare_Typewriter()
    {
        tmpro.color = tmpro.color;
        tmpro.maxVisibleCharacters = 0;
        tmpro.text = PreText;

        if (!string.IsNullOrEmpty(PreText))
        {
            tmpro.ForceMeshUpdate();
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        }

        tmpro.text += TargetText;
        tmpro.ForceMeshUpdate();
    }
    private void Prepare_Fade()
    {

    }




    private IEnumerable Build_Typewriter()
    {
        while (tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            tmpro.maxVisibleCharacters += HurryUp ? CharacterPerCycle * 5 : CharacterPerCycle;

            yield return new WaitForSeconds(0.015f / Speed);
        }
    }

    private IEnumerable Build_Fade()
    {
        yield return null;
    }
}

