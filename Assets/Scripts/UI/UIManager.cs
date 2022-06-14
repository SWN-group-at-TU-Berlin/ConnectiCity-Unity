using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    #region singleton
    public static UIManager Instance { get { return _instance; } }
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

        DefaultButtons = new List<GameObject>();
        DefaultButtons.Add(houseButtonDefault);
        DefaultButtons.Add(businessButtonDefault);
        DefaultButtons.Add(GRButtonDefault);
        DefaultButtons.Add(PPButtonDefault);
        DefaultButtons.Add(RBButtonDefault);
    }
    #endregion

    [Header("Buttons References")]
    [SerializeField] GameObject houseButtonDefault;
    [SerializeField] GameObject houseButtonDown;
    [SerializeField] GameObject businessButtonDefault;
    [SerializeField] GameObject businessButtonDown;
    [SerializeField] GameObject GRButtonDefault;
    [SerializeField] GameObject GRButtonDown;
    [SerializeField] GameObject RBButtonDefault;
    [SerializeField] GameObject RBButtonDown;
    [SerializeField] GameObject PPButtonDefault;
    [SerializeField] GameObject PPButtonDown;

    List<GameObject> DefaultButtons;
    bool houseButtonPressed = false;
    bool businessButtonPressed = false;
    bool GRButtonPressed = false;
    bool PPButtonPressed = false;
    bool RBButtonPressed = false;

    void ActivateDefaultButtons(GameObject pressedButton)
    {
        foreach (GameObject button in DefaultButtons)
        {
            if (!button.Equals(pressedButton))
            {
                button.GetComponent<Button>().enabled = true;
                button.GetComponent<Button>().interactable = true;
            }
        }
    }

    void DeactivateDefaultButtons(GameObject pressedButton)
    {
        foreach (GameObject button in DefaultButtons)
        {
            if (!button.Equals(pressedButton))
            {
                button.GetComponent<Button>().enabled = false;
                button.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void BusinessButtonPressed()
    {
        businessButtonPressed = !businessButtonPressed;
        businessButtonDown.SetActive(businessButtonPressed);
        businessButtonDefault.SetActive(!businessButtonPressed);
        if (businessButtonDefault.activeInHierarchy)
        {
            ActivateDefaultButtons(businessButtonDefault);
        }
        else
        {
            DeactivateDefaultButtons(businessButtonDefault);
        }
    }

    public void HouseButtonPressed()
    {
        houseButtonPressed = !houseButtonPressed;
        houseButtonDown.SetActive(houseButtonPressed);
        houseButtonDefault.SetActive(!houseButtonPressed);
        if (houseButtonDefault.activeInHierarchy)
        {
            ActivateDefaultButtons(houseButtonDefault);
        }
        else
        {
            DeactivateDefaultButtons(houseButtonDefault);
        }
    }
}
