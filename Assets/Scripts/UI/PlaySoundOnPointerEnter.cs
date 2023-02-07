using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaySoundOnPointerEnter : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] string SoundToPlay;

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.Play(SoundToPlay);
    }
}
