using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrafficInfoTab : Tab
{
    [SerializeField] TextMeshProUGUI subcatNum;
    [SerializeField] TextMeshProUGUI currentBudget;
    [SerializeField] TextMeshProUGUI cost;
    [SerializeField] TextMeshProUGUI newBudget;
    [SerializeField] Button buildButton;

    public void UpdateTexts(bool canHostPT, string _subcatNum, string _currentBudget, string _cost, string _newBudget)
    {
        PlayOpenTab(true);
        buildButton.interactable = canHostPT;
        subcatNum.text = _subcatNum;
        currentBudget.text = _currentBudget;
        cost.text = _cost;
        newBudget.text = _newBudget;
    }
}
