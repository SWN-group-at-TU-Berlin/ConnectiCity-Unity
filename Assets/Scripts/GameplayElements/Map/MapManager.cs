using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    #region singleton
    public static MapManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        if (light != null)
        {
            lightIntensity = light.intensity;
        }
        DeactivateSubcatchmentsChildren();
    }
    #endregion

    [SerializeField] List<Subcatchment> _subcatchments;
    [SerializeField] int GR100 = 2;
    [SerializeField] int GR75 = 4;
    [SerializeField] int GR50 = 4;
    [SerializeField] int GR25 = 2;
    [SerializeField] int RB1 = 6;
    [SerializeField] int RB2 = 6;
    [SerializeField] Light light;
    [Range(0.1f, 1f)]
    [SerializeField] float lightFatctor;

    float lightIntensity;

    private void Start()
    {
        //setup benefits in each subcatchement

        /*//foreach subcatchment
        foreach (Subcatchment subcat in _subcatchments)
        {
            //set subcatchment benefit from dictionary
            subcat.SubcatchmentBenefit = (int)DataReader.Instance.SubcatchmentsBenefits[subcat.SubcatchmentNumber];
        }*/
        RandomBGIInitialization();
    }

    private void DeactivateSubcatchmentsChildren()
    {
        foreach (Subcatchment subcat in _subcatchments)
        {
            foreach (Transform child in subcat.transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public Subcatchment GetSubcatchment(int subcatchmentNumber)
    {
        Subcatchment subcat = Array.Find(_subcatchments.ToArray(), s => s.SubcatchmentNumber == subcatchmentNumber);
        return subcat;
    }

    public List<Subcatchment> GetSubcatchments()
    {
        return _subcatchments;
    }

    public Subcatchment[] GetHousesSubcatchments()
    {
        Subcatchment[] houses = Array.FindAll(_subcatchments.ToArray(), s => s.Usage == AreaUsage.Residential);
        return houses;
    }

    public int GetNumberOfHousesSubcatchmentsBuilt()
    {
        int builtHouseSubcats = 0;
        foreach (Subcatchment subcat in GetHousesSubcatchments())
        {
            if (subcat.IsBuilt)
            {
                builtHouseSubcats++;
            }
        }
        return builtHouseSubcats;
    }

    public Subcatchment[] GetBusinessSubcatchments()
    {
        Subcatchment[] businesses = Array.FindAll(_subcatchments.ToArray(), s => s.Usage == AreaUsage.Commercial);
        return businesses;
    }

    public int GetNumberOfBusinessSubcatchmentsBuilt()
    {
        int builtHouseSubcats = 0;
        foreach (Subcatchment subcat in GetBusinessSubcatchments())
        {
            if (subcat.IsBuilt)
            {
                builtHouseSubcats++;
            }
        }
        return builtHouseSubcats;
    }

    public Subcatchment[] GetBuiltSubcatchments()
    {
        Subcatchment[] builtSubcat = Array.FindAll(_subcatchments.ToArray(), s => s.IsBuilt);
        return builtSubcat;
    }

    public void HighlightBuildableSubcatchments(Subcatchment[] subcatchments)
    {
        foreach (Subcatchment subcat in subcatchments)
        {
            subcat.HighlightSubcatchment();
        }
        //lower light intensiti for cooler visual effect
        if (subcatchments.Length > 0)
        {
            light.intensity = lightIntensity * lightFatctor;
        }
    }

    public void DehighlightBuildableSubcatchments()
    {
        foreach (Subcatchment subcat in _subcatchments)
        {
            if (subcat.IsHighlighted)
            {
                subcat.DehighlightSubcatchment();
            }
        }
        light.intensity = lightIntensity;
    }

    public IEnumerator FadeOffLights()
    {
        float lowLightIntesntiy = lightIntensity * lightFatctor;
        while (light.intensity > lowLightIntesntiy)
        {
            light.intensity -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public IEnumerator FadeInLights()
    {
        while (light.intensity < 1)
        {
            light.intensity += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void ReactivateAllSubcatchments()
    {
        foreach (Subcatchment subcat in _subcatchments)
        {
            if (!subcat.Active)
            {
                subcat.SetSubcatchmentActive(true);
            }
        }
    }

    public int GetNumberOfBGIsBuilt()
    {
        int bgisNum = 0;
        foreach (Subcatchment subcat in _subcatchments)
        {
            bgisNum += subcat.BGIHosted.Count;
        }

        return bgisNum;
    }

    /*Gives each subcatchment a random gr coverage and rb type*/
    void RandomBGIInitialization()
    {
        RandomInitializationLoop(GR100, BuildStatus.GR100);
        RandomInitializationLoop(GR75, BuildStatus.GR75);
        RandomInitializationLoop(GR50, BuildStatus.GR50);
        RandomInitializationLoop(GR25, BuildStatus.GR25);
        RandomInitializationLoop(RB1, BuildStatus.RB1);
        RandomInitializationLoop(RB2, BuildStatus.RB2);
    }

    private void RandomInitializationLoop(int BGIOccurrencies, BuildStatus BGIToInitialize)
    {
        for (int i = 0; i < BGIOccurrencies; i++)
        {
            Subcatchment BGISubcat = _subcatchments[UnityEngine.Random.Range(0, _subcatchments.Count)];
            if (BGIToInitialize.ToString().Contains("GR"))
            {
                while (!BGISubcat.GRPercentage.Equals(BuildStatus.Unbuild))
                {
                    BGISubcat = _subcatchments[UnityEngine.Random.Range(0, _subcatchments.Count)];
                }
                BGISubcat.GRPercentage = BGIToInitialize;
            }
            else if (BGIToInitialize.ToString().Contains("RB"))
            {
                while (!BGISubcat.RBType.Equals(BuildStatus.Unbuild))
                {
                    BGISubcat = _subcatchments[UnityEngine.Random.Range(0, _subcatchments.Count)];
                }
                BGISubcat.RBType = BGIToInitialize;
            }
        }
    }
}
