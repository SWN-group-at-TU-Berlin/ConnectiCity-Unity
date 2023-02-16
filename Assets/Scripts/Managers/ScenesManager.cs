using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    [SerializeField] string trackToPlayOnStartInThisScene;
    [SerializeField] GameObject transitionPanel;

    private void Start()
    {
        StartCoroutine(AudioManager.Instance.PlayTrack(trackToPlayOnStartInThisScene));
    }
    public void PlayAgain()
    {
        transitionPanel.GetComponent<Animator>().Play("FadeIn", 0, 0f);
        AudioManager.Instance.StopTrack();
        float animLenght = 0;
        foreach (AnimationClip anim in transitionPanel.GetComponent<Animator>().runtimeAnimatorController.animationClips)
        {
            if (anim.name.Equals("FadeIn"))
            {
                animLenght = anim.length;
            }
        }
        StartCoroutine(ReloadMainScene(animLenght));
    }

    public void NextScene()
    {
        AudioManager.Instance.StopTrack();
        transitionPanel.GetComponent<Animator>().Play("FadeIn", 0, 0f);
        float animLenght = 0;
        foreach (AnimationClip anim in transitionPanel.GetComponent<Animator>().runtimeAnimatorController.animationClips)
        {
            if (anim.name.Equals("FadeIn"))
            {
                animLenght = anim.length;
            }
        }
        StartCoroutine(LoadNextScene(animLenght));
    }

    private IEnumerator ReloadMainScene(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator LoadNextScene(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public Scene CurrentScene()
    {
        return SceneManager.GetActiveScene();
    }
}
