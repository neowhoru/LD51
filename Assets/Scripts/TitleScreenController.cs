using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    public Animator fadeAnimator;

    public string targetSceneToLoad = "Level1";
    // Start is called before the first frame update
    void Start()
    {
        if (fadeAnimator != null)
        {
            fadeAnimator.gameObject.SetActive(true);
        }

        Invoke(nameof(FadeInMenu),1 );
    }

    public void FadeInMenu()
    {
        if (fadeAnimator != null)
        {
            fadeAnimator.Play("FadeIn");
        }
    }

    public void OnStartButtonClicked()
    {
        fadeAnimator.Play("FadeOut");
        Invoke(nameof(StartGame),2);
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync(targetSceneToLoad);
    }
}
