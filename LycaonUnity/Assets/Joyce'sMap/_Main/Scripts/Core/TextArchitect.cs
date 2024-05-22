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

    public enum BuildMethod { Instant, Typewriter, Fade}
    public BuildMethod buildMethod = BuildMethod.Typewriter;

    public Color TextColor { get { return tmpro.color; } set { tmpro.color = value; } }

    public float Speed { get { return BaseSpeed * SpeedMultiplier; } set { SpeedMultiplier = value; } }
    private const float BaseSpeed = 1;
    private float SpeedMultiplier = 1;

    private int CharacterMupltiplier = 1;

    public bool HurryUp = false;
}
