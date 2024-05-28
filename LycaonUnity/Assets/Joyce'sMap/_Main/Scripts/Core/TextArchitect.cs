using System.Collections;
using UnityEngine;
using TMPro;
using UnityEditor.Experimental.GraphView;
using System.Globalization;
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
        HurryUp = false;
    }

    public void ForceComplete()
    {
        switch (buildMethod)
        {
            case BuildMethod.Typewriter:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
                break;
            case BuildMethod.Fade:
                tmpro.ForceMeshUpdate();
                break;
        }
        Stop();
        OnComplete();
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
        tmpro.text = PreText;
        if (PreText != "")
        {
            tmpro.ForceMeshUpdate();
            PreTextLenght = tmpro.textInfo.characterCount;
        }
        else
            PreTextLenght = 0;

        tmpro.text += TargetText;
        tmpro.maxVisibleCharacters = int.MaxValue;
        tmpro.ForceMeshUpdate();

        TMP_TextInfo textInfo = tmpro.textInfo;
        Color ColorVisable = new Color(TextColor.r, TextColor.g, TextColor.b, 1);
        Color ColorHidden = new Color(TextColor.r, TextColor.g, TextColor.b, 0);
        Color32[] VertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;

        for (int i = 0; i < textInfo.characterCount; i++) 
        {
            TMP_CharacterInfo CharInfo = textInfo.characterInfo[i];

            if (!CharInfo.isVisible)
                continue;

            if (i < PreTextLenght)
            {
                for (int v = 0; v < 4; v++)
                    VertexColors[CharInfo.vertexIndex + v] = ColorVisable;
            }
            else 
            {
                for (int v = 0; v < 4; v++)
                    VertexColors[CharInfo.vertexIndex + v] = ColorHidden;
            }
        }
        tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
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
        int Minrange = PreTextLenght;
        int Maxrange = Minrange + 1;
        byte AlphaThreshold = 15;

        TMP_TextInfo TextInfo = tmpro.textInfo;
        Color32[] VertexColors = TextInfo.meshInfo[TextInfo.characterInfo[0].materialReferenceIndex].colors32;
        float[] Alphas = new float[TextInfo.characterCount];

        while (true) 
        {
            float FadeSpeed = ((HurryUp ? CharacterPerCycle * 5 : CharacterPerCycle) * Speed) * 4f;
            for (int i = Minrange; 1 < Maxrange; i++)
            {
                TMP_CharacterInfo CharInfo = TextInfo.characterInfo[i];

                if (!CharInfo.isVisible)
                    continue;

                int VertexIndex = TextInfo.characterInfo[i].vertexIndex;
                Alphas[i] = Mathf.MoveTowards(Alphas[i], 255, FadeSpeed);

                for (int v = 0; v < 4; v++)
                {
                    VertexColors[CharInfo.vertexIndex + v].a = (byte)Alphas[i];
                }

                if (Alphas[i] >= 255)
                    Minrange++;

            }
            tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            bool LastCharacterIsInvisable = !TextInfo.characterInfo[Maxrange - 1].isVisible;
            if (Alphas[Maxrange - 1] > AlphaThreshold || LastCharacterIsInvisable) 
            {
                if (Maxrange < TextInfo.characterCount)
                    Maxrange++;
                else if (Alphas[Maxrange - 1] >= 255 || LastCharacterIsInvisable)
                    break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}

