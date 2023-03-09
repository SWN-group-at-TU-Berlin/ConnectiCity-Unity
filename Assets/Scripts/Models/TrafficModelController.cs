using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Linq;
using TMPro;

public class TrafficModelController : MonoBehaviour
{
    public string _results;
    int osSystemVariable = 0;
    Dictionary<int, float> trafficData;
    #region getter
    public Dictionary<int, float> TrafficData { get { return trafficData; } }
    #endregion
    bool _internalResultsReady = false;
    bool _resultsReady = false;
    #region getter
    public bool ResultsReady { get { return _resultsReady; } }
    #endregion

    private void Start()
    {
        osSystemVariable = GetOperatingSystem();
        trafficData = new Dictionary<int, float>();
    }

    public static int GetOperatingSystem()
    {
        int osSystemVariable = 0;

        if (Application.platform.Equals(RuntimePlatform.WindowsPlayer) ||
            Application.platform.Equals(RuntimePlatform.WindowsEditor))
        {
            osSystemVariable = 1;
        }
        else if (Application.platform.Equals(RuntimePlatform.OSXPlayer) ||
            Application.platform.Equals(RuntimePlatform.OSXEditor))
        {
            osSystemVariable = 2;
        }
        else if (Application.platform.Equals(RuntimePlatform.LinuxPlayer) ||
            Application.platform.Equals(RuntimePlatform.LinuxEditor))
        {
            osSystemVariable = 3;
        }
        return osSystemVariable;
    }


    public void ExecProcess(string areas, string flooding)
    {
        // 1) Create Process Info
        var psi = new ProcessStartInfo();

        string m_path = null;
        if (osSystemVariable == 1)
        {
            m_path = System.IO.Directory.GetParent(Application.dataPath).FullName;
            string[] FileNamePath = { m_path, @"ConnectiCity-TrafficModel-W\venv\Scripts\python" };
            psi.FileName = string.Join(@"\", FileNamePath);
        }
        else if (osSystemVariable == 2)
        {
            m_path = System.IO.Directory.GetParent(Application.dataPath).FullName;

            //path manupulation to delete the .app folder from m_path
            if (Application.platform.Equals(RuntimePlatform.OSXPlayer))
            {
                string[] pathToClean = m_path.Split('/');
                List<string> arrayPathToClean = new List<string>(pathToClean);
                arrayPathToClean.RemoveAt(arrayPathToClean.Count - 1);
                string[] cleanArrayPath = arrayPathToClean.ToArray();
                m_path = string.Join("/", cleanArrayPath);
            }

            string[] FileNamePath = { m_path, "ConnectiCity-TrafficModel/venv/bin/python3" };
            psi.FileName = string.Join("/", FileNamePath);
        }
        else if (osSystemVariable == 3)
        {
            m_path = System.IO.Directory.GetParent(Application.dataPath).FullName;
            string[] FileNamePath = { m_path, "ConnectiCity-TrafficModel/venv/bin/python3.8" };
            psi.FileName = string.Join("/", FileNamePath);
        }


        // 2) Provide Script and arg

        string[] ScriptPath = { m_path, @"ConnectiCity-TrafficModel-W\main.py" };
        var script = string.Join(@"\", ScriptPath);
        if (osSystemVariable != 1)
        {
            string[] tmp = { m_path, "ConnectiCity-TrafficModel/main.py" };
            ScriptPath = tmp;
            script = string.Join("/", ScriptPath);

        }

        psi.Arguments = $"{script} --areas {areas} --flooding {flooding}";

        // 3) Process configuration
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;

        // 4) Execute process and get output
        var errors = "";
        _results = "";

        using (var process = Process.Start(psi))
        {
            errors = process.StandardError.ReadToEnd();
            _results = process.StandardOutput.ReadToEnd();
        }

        UnityEngine.Debug.Log("ERRORS:");
        UnityEngine.Debug.Log(errors);
        UnityEngine.Debug.Log("Results:");
        UnityEngine.Debug.Log(_results);

        //yield return new WaitUntil(() => !_results.Equals(""));
        if (!_results.Equals(""))
        {
            _internalResultsReady = true;
        }
    }

    public void CalculateTrafficData(string areas, string flood)
    {
        StartCoroutine(GetTrafficData_Coroutine(areas, flood));
    }

    private IEnumerator GetTrafficData_Coroutine(string areas, string flood)
    {
        _internalResultsReady = false;
        _resultsReady = false;

        //checking if areas input is correct
        bool areasChecks = AreasInputCheck(areas);

        //checking if flood input is correc
        bool floodCehcks = FloodInputChekc(flood);

        //flood should be in between 0 and 2
        if (areasChecks && floodCehcks)
        {
            ExecProcess(areas, flood);
            yield return new WaitUntil(() => _internalResultsReady);
            string results = _results;

            trafficData = ParseResults(results);
            _resultsReady = true;

        }
    }

    private static Dictionary<int, float> ParseResults(string results)
    {
        //output (results) e.g. [70, 269, 147, 130, 268, 150, 86, 53, 55, 312, 117, 242, 118]
        //trim square brackets adn white spaces
        results = results.Trim('[');
        results = results.Trim(']');
        results = results.Trim();

        //isolate each result int array
        string[] splittedResults = results.Split(',');

        //populate dictionary with reference to specifi edge number
        int i = 1;
        Dictionary<int, float> tmpResults = new Dictionary<int, float>();
        foreach (string result in splittedResults)
        {
            string _result = result.Trim(']');
            float numResult = float.Parse(_result);
            tmpResults.Add(i, numResult);
            i++;
        }

        return tmpResults;
    }

    private static bool FloodInputChekc(string flood)
    {
        bool floodCehcks = false;
        if (flood.Equals("0") || flood.Equals("1") || flood.Equals("2"))
        {
            floodCehcks = true;
        }
        else
        {
            UnityEngine.Debug.LogWarning("flood input in traffic model is not correct");
        }

        return floodCehcks;
    }

    private static bool AreasInputCheck(string areas)
    {
        //areas input e.g. 0,1,2,0,0,0,0,0,1,1,1,0
        int commas = areas.Count(x => x.Equals(','));
        int zeros = areas.Count(x => x.Equals('0'));
        int ones = areas.Count(x => x.Equals('1'));
        int twos = areas.Count(x => x.Equals('2'));
        bool areasChecks = false;
        if (commas == 11 && (zeros + ones + twos) == 12)
        {
            areasChecks = true;
        }
        else
        {
            UnityEngine.Debug.LogWarning("areas input in traffic model is not correct - INPUT:" + "\n" + areas);
        }

        return areasChecks;
    }
}
