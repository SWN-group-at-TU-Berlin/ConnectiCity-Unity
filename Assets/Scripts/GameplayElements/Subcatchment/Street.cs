using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Street : MonoBehaviour
{
    [SerializeField] int _streetNumber;
    #region getter
    public int StreetNumber { get { return _streetNumber; } }
    #endregion
    Renderer objRenderer;

    private void Start()
    {
        objRenderer = GetComponent<Renderer>();
    }

    public void SetStreetColor(Color color)
    {
        Material tmp = objRenderer.material;
        tmp.SetColor("_EmissionColor", color);
    }
}
