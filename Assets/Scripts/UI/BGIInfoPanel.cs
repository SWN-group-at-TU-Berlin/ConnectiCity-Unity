using UnityEngine;

public class BGIInfoPanel : MonoBehaviour
{
    [SerializeField] GlossaryOpener glossary;

    public void OpenGlossary()
    {
        if (!glossary.IsOpen())
        {
            glossary.OpenClose();
        }
    }
}
