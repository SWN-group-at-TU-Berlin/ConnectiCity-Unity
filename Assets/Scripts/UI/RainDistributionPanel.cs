using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RainDistributionPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentRainDistribution;

    private void OnEnable()
    {
        currentRainDistribution.text = RainEventsManager.Instance.RainPerRound[RoundManager.Instance.CurrentRound].ToString();
    }
}
