using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DetaildScoreBoard : MonoBehaviour
{
    [SerializeField] Transform rowsContainer;
    [SerializeField] GameObject firstRow;
    [SerializeField] GameObject rowPrefab;
    [SerializeField] TextMeshProUGUI totalTxt;
    [SerializeField] TextMeshProUGUI roundTxt;

    GameObject lastRow;

    public void UpdateTotalText(string total)
    {
        totalTxt.text = total;
    }

    public void UpdateRoundText(string round)
    {
        roundTxt.text = round;
    }

    public void SetBoardData(RoundSnapshot snap)
    {
        UpdateRoundText(RoundManager.Instance.CurrentRound.ToString());
        UpdateTotalText(snap.total.ToString());

        ClearRowsContainer();
        foreach (GameStat stat in snap.Stats)
        {
            SpawnNewRow(stat.name, stat.value, stat.socialScore.ToString(), stat.environmentalScore.ToString(), stat.economicScore.ToString());
        }
    }

    public void SpawnNewRow(string gStat, string val, string sScore, string envScore, string ecoScore)
    {
        if (lastRow == null)
        {
            firstRow.GetComponent<ScoreRow>().UpdateRowText(gStat, val, sScore, envScore, ecoScore, true);
            lastRow = firstRow;
        }
        else
        {

            GameObject newRow = Instantiate(rowPrefab, lastRow.transform.position, Quaternion.identity, rowsContainer);
            RectTransform containerRectTransform = rowsContainer.GetComponent<RectTransform>();
            containerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, containerRectTransform.rect.height + newRow.GetComponent<RectTransform>().rect.height);
            RectTransform tmp = lastRow.GetComponent<RectTransform>();
            newRow.GetComponent<RectTransform>().anchoredPosition = new Vector2(tmp.anchoredPosition.x, lastRow.GetComponent<RectTransform>().anchoredPosition.y - newRow.GetComponent<RectTransform>().rect.height);
            newRow.GetComponent<ScoreRow>().UpdateRowText(gStat, val, sScore, envScore, ecoScore, !lastRow.GetComponent<ScoreRow>().Palette1On);
            lastRow = newRow;
        }
    }

    void ClearRowsContainer()
    {
        foreach(Transform row in rowsContainer)
        {
            if (!row.gameObject.Equals(firstRow))
            {
                Destroy(row.gameObject);
            }
        }
        lastRow = null;
        rowsContainer.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, firstRow.GetComponent<RectTransform>().rect.height);
    }

    public void Appear()
    {
        Animator anim = GetComponent<Animator>();
        if (anim.isActiveAndEnabled)
        {
            anim.Play("Appear", 0, 0f);
        }
        else
        {
            anim.enabled = true;
            anim.Play("Appear", 0, 0f);
        }
    }

    public void Disappear()
    {
        Animator anim = GetComponent<Animator>();
        if (anim.isActiveAndEnabled)
        {
            anim.Play("Disappear", 0, 0f);
        }
        else
        {
            anim.enabled = true;
            anim.Play("Disappear", 0, 0f);
        }
    }
}
