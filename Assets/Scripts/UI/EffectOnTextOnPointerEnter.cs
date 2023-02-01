using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EffectOnTextOnPointerEnter : MonoBehaviour
{
    [SerializeField] TMP_Text TextComponent;
    [SerializeField] FontStyles effect;

    public void ApplyEffect()
    {
        TextComponent.fontStyle = effect;
    }

    public void DeactivateEffect()
    {
        TextComponent.fontStyle = FontStyles.Normal;
    }
}
