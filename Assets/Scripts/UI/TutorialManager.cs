using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] List<TutorialPanel> Transforms;
    [SerializeField] RectTransform TutorialPanel;
    [SerializeField] RectTransform TutorialPanelMask;
    TutorialPanel panelShown;

    private void Awake()
    {
        panelShown = Transforms[0];
    }

    public void SwithcTutorialPanels()
    {
        if (Transforms[0].Equals(panelShown))
        {
            TutorialPanel.anchoredPosition = new Vector2(Transforms[1].parent.posX, Transforms[1].parent.posY);
            TutorialPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Transforms[1].parent.height);
            TutorialPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Transforms[1].parent.width);
            panelShown = Transforms[1];
        }
        else
        {
            TutorialPanel.anchoredPosition = new Vector2(Transforms[0].parent.posX, Transforms[0].parent.posY);
            TutorialPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Transforms[0].parent.height);
            TutorialPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Transforms[0].parent.width);
            panelShown = Transforms[0];
        }
    }
}

[System.Serializable]
public class TutorialPanel
{
    public RectTransformData parent;
    public RectTransformData child;
}

[System.Serializable]
public class RectTransformData
{
    public float posX;
    public float posY;
    public float width;
    public float height;
}
