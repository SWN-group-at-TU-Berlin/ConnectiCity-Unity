using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetaildScoreBoard : MonoBehaviour
{
    [SerializeField] Transform rowsContainer;
    [SerializeField] GameObject firstRow;
    [SerializeField] GameObject rowPrefab;

    GameObject lastRow;

    public void SpawnNewRow()
    {
        if(lastRow == null)
        {
            lastRow = firstRow;
        }
        GameObject newRow = Instantiate(rowPrefab, lastRow.transform.position, Quaternion.identity, rowsContainer);
        RectTransform containerRectTransform =  rowsContainer.GetComponent<RectTransform>();
        containerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, containerRectTransform.rect.height + newRow.GetComponent<RectTransform>().rect.height);
        RectTransform tmp = lastRow.GetComponent<RectTransform>();
        newRow.GetComponent<RectTransform>().anchoredPosition = new Vector2(tmp.anchoredPosition.x, lastRow.GetComponent<RectTransform>().anchoredPosition.y - newRow.GetComponent<RectTransform>().rect.height);
        lastRow = newRow;
    }
}
