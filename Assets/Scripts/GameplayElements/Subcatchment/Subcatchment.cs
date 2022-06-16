using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subcatchment : MonoBehaviour
{
    [Header("Subcatchment settings")]
    [SerializeField] int _subcatchmentNumber;
    #region getter
    public int SubcatchmentNumber { get { return _subcatchmentNumber; } }
    #endregion

    [SerializeField] AreaUsage _usage;
    #region getter
    public AreaUsage Usage { get { return _usage; } }
    #endregion

    [SerializeField] AreaSize _size;
    #region getter
    public AreaSize Size { get { return _size; } }
    #endregion

    [Header("Highlighted Materials")]
    [SerializeField] Material _highlightedMaterial;

    Material defaultMaterial;
    Color highlightSelectionColor;


    bool _isBuilt;
    #region getter
    public bool IsBuilt { get { return _isBuilt; } }
    #endregion
    bool _isHighlighted;
    #region getter
    public bool IsHighlighted { get { return _isHighlighted; } }
    #endregion

    Outline outline;

    List<InfrastructureType> BGIHosted { get; set; }
    bool IsSelected { get; set; } // still don't know if this will be useful
    bool IsHovered { get; set; }
    bool Active { get; set; } //getting deactivated after rainfall event

    InputProvider input;

    private void Awake()
    {
        defaultMaterial = GetComponent<MeshRenderer>().material;
        highlightSelectionColor = GetComponent<Outline>().OutlineColor;
        BGIHosted = new List<InfrastructureType>();
    }

    private void OnEnable()
    {
        outline = GetComponent<Outline>();
        input = new InputProvider();
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(input.MousePosition());
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.tag.Equals("Subcatchment") && hit.transform.gameObject.name.Equals("SC" + SubcatchmentNumber) && IsHighlighted)
            {
                if (!IsHovered)
                {
                    MouseHoveringOnSubcatchment();
                }

                if (input.MouseLeftButton())
                {
                    InfrastructureBuilder.Instance.BuildInfrastructure(GetComponent<Subcatchment>());
                }
            }
            else
            {
                if (IsHovered)
                {
                    IsHovered = false;
                    outline.OutlineColor = Color.white;
                    outline.enabled = IsHovered;
                }
            }

        }
        else
        {
            if (IsHovered)
            {
                IsHovered = false;
                outline.OutlineColor = Color.white;
                outline.enabled = IsHovered;
            }
        }
    }

    void RequestInfrastructureConstruction()
    {

    }

    public void BuildInfrastructureOnSubcatchment(InfrastructureType infrastructure)
    {
        if (infrastructure.Equals(InfrastructureType.Business) || infrastructure.Equals(InfrastructureType.House))
        {
            transform.GetChild(0).gameObject.SetActive(true);
            _isBuilt = true;
        }
        else
        {
            switch (infrastructure)
            {
                case InfrastructureType.GR:
                    {
                        Debug.Log("GR built");
                        break;
                    }
                case InfrastructureType.RB:
                    {
                        Debug.Log("RB built");
                        break;
                    }
                case InfrastructureType.PP:
                    {
                        Debug.Log("PP built");
                        break;
                    }
            }
            BGIHosted.Add(infrastructure);
        }
    }

    public bool CanHostBGI(InfrastructureType BGI)
    {
        bool canHostBGI = false;
        if(BGI.Equals(InfrastructureType.House) || BGI.Equals(InfrastructureType.Business))
        {
            canHostBGI = false;
        } else if(BGIHosted.Count >= 2)
        {
            canHostBGI = false;
        } else if(BGIHosted.Count == 0)
        {
            canHostBGI = true;
        } else if(!BGIHosted[0].Equals(BGI))
        {
            canHostBGI = true;
        } 
        return canHostBGI;
    }

    void MouseHoveringOnSubcatchment()
    {
        IsHovered = true;
        outline.enabled = IsHovered;
        outline.OutlineColor = highlightSelectionColor;
    }

    private void OnMouseEnter()
    {
        Debug.Log("Mouse on subcat: " + this.SubcatchmentNumber);
    }

    private void OnMouseOver()
    {
        Debug.Log("Mouse on subcat: " + this.SubcatchmentNumber);
    }

    public void HighlightSubcatchment()
    {
        _isHighlighted = true;
        GetComponent<MeshRenderer>().material = _highlightedMaterial;
    }

    public void DehighlightSubcatchment()
    {
        _isHighlighted = false;
        GetComponent<MeshRenderer>().material = defaultMaterial;
    }

    public void HostNewBGI()
    {

    }
}
