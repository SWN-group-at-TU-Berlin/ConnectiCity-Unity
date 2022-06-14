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

    public bool IsBuilt { get; set; }
    public bool IsHighlighted { get; set; }

    Outline outline;

    int BGIHosted { get; set; }
    bool CanHostBGI { get; set; }
    bool IsSelected { get; set; }
    bool IsHovered { get; set; }
    bool Active { get; set; }

    InputProvider input;

    private void Awake()
    {
        defaultMaterial = GetComponent<MeshRenderer>().material;
        highlightSelectionColor = GetComponent<Outline>().OutlineColor;
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

    public void BuildInfrastructureOnSubcatchment()
    {
        transform.GetChild(0).gameObject.SetActive(true);
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
        IsHighlighted = true;
        GetComponent<MeshRenderer>().material = _highlightedMaterial;
    }

    public void DehighlightSubcatchment()
    {
        IsHighlighted = false;
        GetComponent<MeshRenderer>().material = defaultMaterial;
    }

    public void HostNewBGI()
    {

    }
}
