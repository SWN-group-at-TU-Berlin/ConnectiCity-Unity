using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataReader : MonoBehaviour
{
    [SerializeField] TextAsset data;
    [SerializeField] int rainIntensity;
    [SerializeField] int subcat;
    [SerializeField] BuildStatus buildStatus;

    Dictionary<int, Dictionary<SubcatchmentKey, float>> rainCosts;

    private void Awake()
    {
        rainCosts = new Dictionary<int, Dictionary<SubcatchmentKey, float>>();
    }

    // Start is called before the first frame update
    void Start()
    {
        /*SETTING UP THE ARRAYS*/
        //split data into 3 different strings contained in one array, one per rain level
        string[] dataPerRainLevel = data.text.Split('/'); //the csv file needs a specific separator for the rain level = '/'

        //Split each string into a row array using "\n" as separator
        string[] dataRainLevel1 = dataPerRainLevel[1].Split('\n');
        string[] dataRainLevel2 = dataPerRainLevel[2].Split('\n');
        string[] dataRainLevel3 = dataPerRainLevel[3].Split('\n');

        //take lables from one of the arrays
        string[] lables = dataRainLevel1[1].Split(',');

        //clean the arays from anything that isn't a number
        List<string> rainLevel1List = new List<string>(dataRainLevel1);
        List<string> rainLevel2List = new List<string>(dataRainLevel2);
        List<string> rainLevel3List = new List<string>(dataRainLevel3);

        string[] termsToClean = { "Subcatchment", ",,," };

        dataRainLevel1 = CleanParsedData(dataRainLevel1, rainLevel1List, termsToClean);
        dataRainLevel2 = CleanParsedData(dataRainLevel2, rainLevel2List, termsToClean);
        dataRainLevel3 = CleanParsedData(dataRainLevel3, rainLevel3List, termsToClean);

        /*SETTING UP THE FINAL DICTIONARIES*/
        Dictionary<SubcatchmentKey, float> rainLevel1Table = GenerateDictionaryFromTable(dataRainLevel1, lables);
        Dictionary<SubcatchmentKey, float> rainLevel2Table = GenerateDictionaryFromTable(dataRainLevel2, lables);
        Dictionary<SubcatchmentKey, float> rainLevel3Table = GenerateDictionaryFromTable(dataRainLevel3, lables);

        rainCosts.Add(1, rainLevel1Table);
        rainCosts.Add(2, rainLevel2Table);
        rainCosts.Add(3, rainLevel3Table);
        
    }

    public void GetRainEventCost()
    {
        float cost = rainCosts[rainIntensity][new SubcatchmentKey(subcat, buildStatus)];
        Debug.Log("Rain cost for rain level: " + rainIntensity + " | subcat: " + subcat + " | build status: " + buildStatus.ToString() + " = " + cost);
    }

    private Dictionary<SubcatchmentKey, float> GenerateDictionaryFromTable(string[] table, string[] lables)
    {
        //instatiate a new dictionary
        Dictionary<SubcatchmentKey, float> newDictionary = new Dictionary<SubcatchmentKey, float>();
        
        //for each row in the array
        foreach (string row in table)
        {
            //split the row into single data units using the "," separator and store them into an array x (name is irrelevant) 
            string[] singleRowCells = row.Split(',');
            
            //scroll throught each element of the lables array with a specific index i (name is irrelevant), starting from i = 1 -> avoids the "Subcatchment" lable
            for (int i = 2; i < lables.Length; i++)
            {
                //instantiate a new subcatchmentKey key
                //initialize key with x[0] as subcatchment number and labels array [i] as build status
                string lable = lables[i].Trim('\r');
                BuildStatus status = ConvertStringToStatus(lable);
                
                SubcatchmentKey newKey = new SubcatchmentKey(int.Parse(singleRowCells[0]), status);
                
                //store a new element into the dictionary as [key][x[i]]
                newDictionary.Add(newKey, int.Parse(singleRowCells[i]));
            }

        }
        return newDictionary;
    }

    public BuildStatus ConvertStringToStatus(string sStatus)
    {
        BuildStatus convertedSTatus = BuildStatus.Unbuild;
        foreach (BuildStatus status in Enum.GetValues(typeof(BuildStatus)))
        {
            if (sStatus.Equals(status.ToString()))
            {
                convertedSTatus = status;
            }
        }
        return convertedSTatus;
    }

    private string[] CleanParsedData(string[] toClean, List<string> toCleanClone, string[] termsToClean)
    {
        foreach (string term in termsToClean)
        {
            foreach (string s in toClean)
            {
                if (s.Contains(term))
                {
                    toCleanClone.Remove(s);
                }
            }
        }
        toCleanClone.Remove(toClean[toClean.Length - 1]);
        return toCleanClone.ToArray();
    }
}

public struct SubcatchmentKey
{
    private int subcatchmentNumber;
    private BuildStatus buildStatus;
    public SubcatchmentKey(int subcatNnumber, BuildStatus status)
    {
        subcatchmentNumber = subcatNnumber;
        buildStatus = status;
    }
}

public enum BuildStatus
{
    Unbuild,
    Built,
    PP,
    RB1,
    RB2,
    GR25,
    GR50,
    GR75,
    GR100,
    RB1_PP,
    RB2_PP,
    GR25_PP,
    GR50_PP,
    GR75_PP,
    GR100_PP,
    GR25_RB1,
    GR50_RB1,
    GR75_RB1,
    GR100_RB1,
    GR25_RB2,
    GR50_RB2,
    GR75_RB2,
    GR100_RB2,
}
