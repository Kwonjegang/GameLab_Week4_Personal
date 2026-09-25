using TMPro;
using UnityEngine;

[ExecuteAlways]
public class TMPOutlineAnimator : MonoBehaviour
{
    public TMP_Text text;

    [Range(0f, 1f)]
    public float outlineThickness = 0f;

    void Update()
    {
        if (text == null)
            text = GetComponent<TMP_Text>();

        if (text != null)
            text.outlineWidth = outlineThickness;
    }
}
