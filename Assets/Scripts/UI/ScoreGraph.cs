using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreGraph : MonoBehaviour
{

    private void Start()
    {
        float totalScore = 0;
        Dictionary<int, float> testScoreDic = new Dictionary<int, float>();
        for (int i = 0; i < 10; i++)
        {
            totalScore += Random.Range(0, 3);
            testScoreDic.Add(i + 1, totalScore);
        }
        UpdateGraph(testScoreDic, 30);
    }

    void UpdateGraph(Dictionary<int, float> graphValues, float topScore)
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
                        float maxHeight = barVisual.rect.height;
                        float newBarHeight = maxHeight * scoreProportion;
                        barVisual.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newBarHeight);
                        element.GetChild(0).GetComponent<TextMeshProUGUI>().text = value.ToString();
                    }
                }
                else //Set the new size to 0
                {
                    barVisual.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
                }

                //Set the round text to corrispondent round
                if (element.name.Equals("Round"))
                {
                    element.GetComponent<TextMeshProUGUI>().text = i.ToString();
                }
            }
            i++;
        }
    }
}

