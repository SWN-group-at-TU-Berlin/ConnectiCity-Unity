using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    #region singleton
    private static TrafficManager instance;
    #region getter
    public static TrafficManager Instance { get { return instance; } }
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion


    [SerializeField] TrafficModelController trafficModel;
    
    [Header("Traffic statistics")]
    //source: gooogle sheet "residents+jobs+traffic" -> tab "Traffic"
    [SerializeField] float avgVehicleSize = 4.5f;
    [SerializeField] float maxCarsVelocity = 50f; //in km/h
    [SerializeField] float maxTrafficIntensity = 300f; //value refers to the values of the py traffic model
    [SerializeField] Gradient gradientExample; //value refers to the values of the py traffic model
    [SerializeField] AnimationCurve avgVehicleEmission;

    //float will be swapped with a custom class for Street like Subcatchment
    Dictionary<int, float> trafficData;
    Dictionary<int, float> streetsFullCapacity;
    Dictionary<int, float> streetsLengths;
    private bool _v2vCommunication = false;
    #region getter
    public bool V2VCommunication { get { return _v2vCommunication; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        trafficData = new Dictionary<int, float>();
        streetsFullCapacity = new Dictionary<int, float>();
        streetsLengths = new Dictionary<int, float>();

        streetsFullCapacity = DataReader.Instance.StreetsFullCapacity;
        streetsLengths = DataReader.Instance.StreetsLength;
    }

    public void UpdateTrafficData()
    {
        //Gathering input
        string areasInput = "";
        string floodInput = "";

        //flood input
        if (RainEventsManager.Instance.FlashFloodCheck())
        {
            floodInput = "1";
            if (_v2vCommunication)
            {
                floodInput = "2";
            }
        }
        else
        {
            floodInput = "0";
        }

        //areas Input
        foreach (Subcatchment subcat in MapManager.Instance.GetSubcatchments())
        {
            //areas input e.g. 0,1,2,0,0,0,0,0,1,1,1,0
            string singleSubcatInput = "";
            if (subcat.IsBuilt)
            {
                singleSubcatInput = "1";
                if (subcat.PublicTransport)
                {
                    singleSubcatInput = "2";
                }
            }
            else
            {
                singleSubcatInput = "0";
            }

            if (subcat.SubcatchmentNumber != 12)
            {
                singleSubcatInput += ",";
            }

            areasInput += singleSubcatInput;
        }

        StartCoroutine(WaitForTrafficData(areasInput, floodInput));

    }

    IEnumerator WaitForTrafficData(string areasInput, string floodInput)
    {
        trafficModel.CalculateTrafficData(areasInput, floodInput);
        yield return new WaitUntil(() => trafficModel.ResultsReady);
        trafficData = trafficModel.TrafficData;
    }

    public int GetCarNumberOnStreet(int streetNum)
    {
        float streetLenght = streetsLengths[streetNum];
        //maximum possible distance between cars wen traffic is low (assuming only 1 car)
        float maxDist = streetLenght - avgVehicleSize;
        float currentCarsDistance = maxDist * Mathf.Abs(trafficData[streetNum] / streetsFullCapacity[streetNum] - 1);
        float totalSpaceCarOccupies = currentCarsDistance + avgVehicleSize;
        int numberOfCars = Mathf.CeilToInt(streetLenght / totalSpaceCarOccupies);
        return numberOfCars;
    }

    public float GetCarsVelocityOnStreet(int streetNum)
    {
        float velocity = maxCarsVelocity / Mathf.Abs(trafficData[streetNum] / streetsFullCapacity[streetNum] - 1);
        return velocity;
    }

    public float GetEmissionOnStreet(int streetNum)
    {
        float avgEmissionPerVehicle = avgVehicleEmission.Evaluate(GetCarsVelocityOnStreet(streetNum));
        return avgEmissionPerVehicle;
    }

    public bool StreetCongested(int streetNum)
    {
        bool streetCongested = false;
        if(trafficData[streetNum] > maxTrafficIntensity)
        {
            streetCongested = true;
        }
        return streetCongested;
    }
}