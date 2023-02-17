using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31.TransitionKit;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string nextSceneName;   // 次のシーンの名前

    [SerializeField]
    private int nextSceneNum;      // 次のシーン

    private int titleSceneNum = 0;
    private int gameSceneNum = 1;
    private int hardGameSceneNum = 2;

    public void SceneChange(int nextNum)
    {
        if (SceneManager.GetActiveScene().buildIndex == titleSceneNum)
        {
            if (nextSceneName == "GameScene")
            {
                nextSceneNum = gameSceneNum;
            }
            if (nextSceneName == "HardGameScene")
            {
                nextSceneNum = hardGameSceneNum;
            }
        }

        var fader = new FadeTransition()
        {
            nextScene = nextNum,
            fadeToColor = Color.white
        };
        TransitionKit.instance.transitionWithDelegate(fader);
    }

}
