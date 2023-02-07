using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioOptionsSlider : MonoBehaviour
{
    [SerializeField] bool effectsSlider = false;
    Slider slider;
    Toggle toggle;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        toggle = GetComponent<Toggle>();
    }

    public void UpdateSlider()
    {
        if (effectsSlider)
        {
            AudioOptions.Instance.effectsVolume = slider.value;
        }
        else
        {
            AudioOptions.Instance.musicVolume = slider.value;
        }
    }

    public void UpdateToggle()
    {
        AudioOptions.Instance.voiceEffects = toggle.isOn;
    }
}
