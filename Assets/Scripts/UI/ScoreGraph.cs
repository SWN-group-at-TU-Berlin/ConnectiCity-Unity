using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreGraph : MonoBehaviour
{
    [SerializeField] float MaxBarHeight = 400f;

    public void UpdateGraph(Dictionary<int, float> graphValues, float topScore)
    {
        Transform graphElementsContainer = transform.GetChild(0);
        int i = 1;
        //scroll throught all objs in "RoundsLine"
        foreach (Transform graphBar in graphElementsContainer)
        {
            //scroll throught "GraphBar" elements
            foreach (Transform element in graphBar)
            {
                //Recover the component to controll the bar image size
                RectTransform barVisual = element.GetComponent<RectTransform>();
                if (graphValues.ContainsKey(i))
                {
                    float value = graphValues[i];
                    //Calculate new size of the bare confronting it with topScore and set it
                    if (element.name.Equals("BarVisual"))
                    {
                        float scoreProportion = value / topScore;
                        float newBarHeight = MaxBarHeight * scoreProportion;
                        barVisual.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newBarHeight);
                        element.GetChild(0).GetComponent<TextMeshProUGUI>().text = value.ToString();
                    }
                }
                else //Set the new size to 0
                {
                    if (element.name.Equals("BarVisual"))
                    {
                        element.GetChild(0).GetComponent<TextMeshProUGUI>().text = "0";
                    }
                }
            }
            i++;
        }
    }
}

