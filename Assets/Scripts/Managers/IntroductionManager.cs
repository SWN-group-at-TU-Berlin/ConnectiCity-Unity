using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroductionManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textBox;
    [SerializeField] GameObject nextButton;
    [SerializeField] GameObject endButton;
    [SerializeField] GameObject transitionPanel;
    [SerializeField] List<string> introductionTexts;

    int currentText;

    private void Start()
    {
        currentText = 0;
        NextTxt();
    }

    public void NextTxt()
    {
        if (currentText < introductionTexts.Count - 1)
        {
            textBox.text = introductionTexts[currentText];
            currentText++;
        }
        else
        {
            textBox.text = introductionTexts[currentText];
            nextButton.SetActive(false);
            endButton.SetActive(true);
        }
    }

    public void Play()
    {
        float fadeInTime = transitionPanel.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length;
        transitionPanel.SetActive(true);
        StartCoroutine(NextScene(fadeInTime));
    }

    private IEnumerator NextScene(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
