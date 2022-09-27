using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RainDistribution : MonoBehaviour
{
    /*This class is supposed to be attached to the RainDistribution UI element*/
    [SerializeField] RectTransform maxDistribution;
    [SerializeField] Transform rounds;

    private List<Vector3> distributionPositions;
    private bool showingDistribution;

    private void Awake()
    {
        distributionPositions = new List<Vector3>();
    }

    private void Start()
    {
        InitializeGraph();
    }


    public void InitializeGraph()
    {
        //Set up sliders values
        InitializeSliderValues();

        //Get all the positions for line renderer
        InitializeLineRendererPositions();
    }

    private void InitializeSliderValues()
    {
        float maxHeight = maxDistribution.rect.height;
        float maxRainDistribution = RainEventsManager.Instance.RainPerRound[10];
        //rounds tracker
        int i = 1;
        foreach (Transform round in rounds)
        {
            //recover slider component
            Slider slider = round.GetChild(0).GetComponent<Slider>();
            RectTransform sliderRect = round.GetChild(0).GetComponent<RectTransform>();

            //set min val to 0
            slider.minValue = 0;

            //set slider height to corrispondent range in rain distribution 
            float roundRainRange = RainEventsManager.Instance.RainPerRound[i]/maxRainDistribution;
            float newSliderHeight = maxHeight * roundRainRange;
            sliderRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSliderHeight);
            ColorBlock tmpClorBlock = slider.colors;
            tmpClorBlock.colorMultiplier = 5;
            slider.colors = tmpClorBlock;

            //set slider to max value
            slider.value = slider.maxValue;

            //increase round tracker
            i++;
        }
    }

    private void InitializeLineRendererPositions()
    {
        foreach (Transform round in rounds)
        {
            foreach (Transform sliderElement in round.GetChild(0))
            {
                if (sliderElement.name.Contains("Handle"))
                {
                    distributionPositions.Add(sliderElement.GetChild(0).position);
                }
            }
        }
    }

}
