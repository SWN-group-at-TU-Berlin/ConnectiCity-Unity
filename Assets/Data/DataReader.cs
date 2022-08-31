using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataReader : MonoBehaviour
{
    #region singleton
    private static DataReader instance;
    public static DataReader Instance { get { return instance; } }
    #endregion

    [SerializeField] TextAsset rainCostsTables;
    [SerializeField] TextAsset runoffReductionTables;
    [SerializeField] TextAsset buildingCostsTable;
    [SerializeField] TextAsset actionPointsTable;
    [SerializeField] TextAsset incomeBenefitTable;
    [SerializeField] TextAsset citizenSatisfactionBenefitTable;
    [SerializeField] TextAsset citizenNumberBenefitTable;
    [SerializeField] int rainIntensity;
    [SerializeField] int subcat;
    [SerializeField] BuildStatus buildStatus;

    //Rain costs dictionaries stored by rain levels
    Dictionary<int, Dictionary<SubcatchmentKey, float>> rainCostsDictionaries;
    #region getter
    public Dictionary<int, Dictionary<SubcatchmentKey, float>> RainCostsDictionaries { get { return rainCostsDictionaries; } }
    #endregion

    //Runoff reduction dictionaries stored by rain levels
    Dictionary<int, Dictionary<SubcatchmentKey, float>> runoffReductionPercentagesDictionaries;
    #region getter
    public Dictionary<int, Dictionary<SubcatchmentKey, float>> RunoffReductionPercentagesDictionaries { get { return runoffReductionPercentagesDictionaries; } }
    #endregion

    Dictionary<Benefit, Dictionary<SubcatchmentKey, float>> benefits;
    #region getter
    public Dictionary<Benefit, Dictionary<SubcatchmentKey, float>> Benefits { get { return benefits; } }
    #endregion

    Dictionary<SubcatchmentKey, float> buildingCosts;
    #region getter
    public Dictionary<SubcatchmentKey, float> BuildingCosts { get { return buildingCosts; } }
    #endregion

    Dictionary<SubcatchmentKey, float> actionPointsCosts;
    #region getter
    public Dictionary<SubcatchmentKey, float> ActionPointsCosts { get { return actionPointsCosts; } }
    #endregion

    Dictionary<int, float> subcatchmentsAreas;
    #region getter
    public Dictionary<int, float> SubcatchmentsAreas { get { return subcatchmentsAreas; } }
    #endregion

    private void Awake()
    {
        //SINGLETON
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //Dictionaries initialization 
        rainCostsDictionaries = new Dictionary<int, Dictionary<SubcatchmentKey, float>>();
        runoffReductionPercentagesDictionaries = new Dictionary<int, Dictionary<SubcatchmentKey, float>>();
        buildingCosts = new Dictionary<SubcatchmentKey, float>();
        subcatchmentsAreas = new Dictionary<int, float>();
        benefits = new Dictionary<Benefit, Dictionary<SubcatchmentKey, float>>();

         //Dictionaries population
        PopulateDictionaryFromRainLevelTables(rainCostsDictionaries, rainCostsTables);
        PopulateDictionaryFromRainLevelTables(runoffReductionPercentagesDictionaries, runoffReductionTables);
        buildingCosts = PopulateDictionaryFromSubcatchmentsTables(buildingCostsTable);
        actionPointsCosts = PopulateDictionaryFromSubcatchmentsTables(actionPointsTable);
        Dictionary<SubcatchmentKey, float> incomeBenefits = PopulateDictionaryFromSubcatchmentsTables(incomeBenefitTable);
        Dictionary<SubcatchmentKey, float> citizenSatisfactionBenefits = PopulateDictionaryFromSubcatchmentsTables(citizenSatisfactionBenefitTable);
        Dictionary<SubcatchmentKey, float> citizenNumberBenefits = PopulateDictionaryFromSubcatchmentsTables(citizenNumberBenefitTable);
        benefits.Add(Benefit.income, incomeBenefits);
        benefits.Add(Benefit.citizenNumber, citizenNumberBenefits);
        benefits.Add(Benefit.citizenSatisfaction, citizenSatisfactionBenefits);
        /*TO TEST BUILDING COSTS POPULATION*/
        foreach (Benefit benefit in Enum.GetValues(typeof(Benefit)))
        {
            Dictionary<SubcatchmentKey, float> b = benefits[benefit];
            for (int i = 1; i <= 12; i++)
            {
                string subcatCosts = "Subcat " + i;
                foreach (BuildStatus stat in Enum.GetValues(typeof(BuildStatus)))
                {
                    if (b.ContainsKey(new SubcatchmentKey(i, stat)))
                    {
                        subcatCosts += " | " + stat + ": " + b[new SubcatchmentKey(i, stat)] + " //";
                    }
                }
                Debug.Log(subcatCosts);
            }
        }
    }

    private Dictionary<SubcatchmentKey, float> PopulateDictionaryFromSubcatchmentsTables(TextAsset table)
    {
        //Set up arrays to generate rain level table dictionaries
        //split data into 3 different strings contained in one array, one per rain level
        string buildCostData = table.text; //the csv file needs a specific separator for the rain level = '/'
        string[] buildCostsDataTable;
        string[] buildCostsDataTablesLables;
        SetUpDataTablesIntoArray(buildCostData, out buildCostsDataTable, out buildCostsDataTablesLables);

        /*Generate one dictionary per rain level data tables*/
        Dictionary<SubcatchmentKey, float> dictionaryToPopulate = new Dictionary<SubcatchmentKey, float>();
        return dictionaryToPopulate = GenerateDictionaryFromTable(buildCostsDataTable, buildCostsDataTablesLables);
    }

    private void PopulateDictionaryFromRainLevelTables(Dictionary<int, Dictionary<SubcatchmentKey, float>> dictionaryToPopulate, TextAsset table)
    {
        //Set up arrays to generate rain level table dictionaries
        //split data into 3 different strings contained in one array, one per rain level
        string[] dataPerRainLevel = table.text.Split('/'); //the csv file needs a specific separator for the rain level = '/'
        string[] dataRainLevel1DataTable, dataRainLevel2DataTable, dataRainLevel3DataTable;
        string[] rainDataTablesLables1, rainDataTablesLables2, rainDataTablesLables3;
        SetUpDataTablesIntoArray(dataPerRainLevel[1], out dataRainLevel1DataTable, out rainDataTablesLables1);
        SetUpDataTablesIntoArray(dataPerRainLevel[2], out dataRainLevel2DataTable, out rainDataTablesLables2);
        SetUpDataTablesIntoArray(dataPerRainLevel[3], out dataRainLevel3DataTable, out rainDataTablesLables3);

        /*Generate one dictionary per rain level data tables*/
        Dictionary<SubcatchmentKey, float> rainLevel1TableDictionary = GenerateDictionaryFromTable(dataRainLevel1DataTable, rainDataTablesLables1);
        Dictionary<SubcatchmentKey, float> rainLevel2TableDictionary = GenerateDictionaryFromTable(dataRainLevel2DataTable, rainDataTablesLables2);
        Dictionary<SubcatchmentKey, float> rainLevel3TableDictionary = GenerateDictionaryFromTable(dataRainLevel3DataTable, rainDataTablesLables3);

        /*Storing rain level data tables dictionaries into a rain level dictionary*/
        dictionaryToPopulate.Add(1, rainLevel1TableDictionary);
        dictionaryToPopulate.Add(2, rainLevel2TableDictionary);
        dictionaryToPopulate.Add(3, rainLevel3TableDictionary);
    }

    private void SetUpDataTablesIntoArray(string dataPerRainLevel, out string[] dataRainLevel1, out string[] lables)
    {
        /*SETTING UP THE ARRAYS*/
        //Split each string into a row array using "\n" as separator
        dataRainLevel1 = dataPerRainLevel.Split('\n');

        //take lables from one of the arrays
        lables = dataRainLevel1[1].Split(',');

        //clean the arays from anything that isn't a number
        List<string> rainLevel1List = new List<string>(dataRainLevel1);

        string[] termsToClean = { "Subcatchment", ",,," };

        dataRainLevel1 = CleanParsedData(dataRainLevel1, rainLevel1List, termsToClean);
    }

    public float GetRainEventCost(int rainInt, SubcatchmentKey subcatKey)
    {
        float cost = rainCostsDictionaries[rainInt][subcatKey];
        Debug.Log("Rain cost for rain level: " + rainIntensity + " | subcat: " + subcat + " | build status: " + buildStatus.ToString() + " = " + cost);
        return cost;
    }

    public float GetRunoffReductionPercentage(int rainInt, SubcatchmentKey subcatKey)
    {
        float runoff = runoffReductionPercentagesDictionaries[rainInt][subcatKey];
        Debug.Log("Runoff reduction for rain level: " + rainIntensity + " | subcat: " + subcat + " | build status: " + buildStatus.ToString() + " = " + runoff);
        return runoff;
    }

    public float GetSubcatArea(int subcatInt)
    {
        float area = subcatchmentsAreas[subcatInt];
        Debug.Log("Area of subcat: " + subcat + " | area:" + " = " + area);
        return area;
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
            for (int i = 1; i < lables.Length; i++)
            {
                //skips the first column if it is area table 
                if (!lables[i].Equals("Area"))
                {

                    //instantiate a new subcatchmentKey key
                    //initialize key with x[0] as subcatchment number and labels array [i] as build status
                    string lable = lables[i].Trim('\r');
                    BuildStatus status = ConvertStringToStatus(lable);

                    SubcatchmentKey newKey = new SubcatchmentKey(int.Parse(singleRowCells[0]), status);

                    //store a new element into the dictionary as [key][x[i]]
                    
                    //singleRowCells[i] = singleRowCells[i].Replace(".", ",");
                    
                    newDictionary.Add(newKey, float.Parse(singleRowCells[i]));
                }
                else if (subcatchmentsAreas.Count < 12)
                {
                    //if the lable is "Area", store it in the area dictionary
                    string adjustedArea = singleRowCells[i].Replace(".", ",");
                    subcatchmentsAreas.Add(int.Parse(singleRowCells[0]), float.Parse(singleRowCells[i]));
                }
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
        if (toClean[toClean.Length - 1].Equals(""))
        {
            toCleanClone.Remove(toClean[toClean.Length - 1]);
        }
        return toCleanClone.ToArray();
    }
}

[Serializable]
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


