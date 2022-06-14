using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager _instance;
    #region singleton
    public RoundManager Instance { get { return _instance; } }
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

    int _currentRound;
    #region getter
    public int CurrentRound { get { return _currentRound; } }
    #endregion

    private RoundPhase phase
    #region getters setters
    {
        get { return this.phase; }
        set { this.phase = value; }
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
