using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CostsManager : MonoBehaviour
{
    public static CostsManager _instance;
    #region singleton
    public static CostsManager Instance { get { return _instance; } }
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
    }
    #endregion

    public List<BasicInfrastructureStats> InfrastructuresStats;
    public List<BasicBGIStats> BGIStats;

    public BasicInfrastructureStats GetInfrastructureStats(string infrastructureName) {
        BasicInfrastructureStats infrastructureStats = Array.Find(InfrastructuresStats.ToArray(), stats => stats.Name.Equals(infrastructureName));
        return infrastructureStats;
    }
    public BasicBGIStats GetBGIStats(string BGIName)
    {
        BasicBGIStats infrastructureStats = Array.Find(BGIStats.ToArray(), stats => stats.Name.Equals(BGIName));
        return infrastructureStats;
    }

}
