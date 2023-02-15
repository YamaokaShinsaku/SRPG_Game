using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31.TransitionKit;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string nextSceneName;   // 次のシーンの名前

    private int nextSceneNum;      // 次のシーン

    private int titleSceneNum = 0;
    private int gameSceneNum = 1;

    public void SceneChange(string nextScene)
    {
        if (SceneManager.GetActiveScene().buildIndex == titleSceneNum)
        {
            if (nextSceneName == "GameScene")
            {
                nextSceneNum = gameSceneNum;
            }
            //nextSceneNum = gameSceneNum;
        }

        var fader = new FadeTransition()
        {
            nextScene = nextSceneNum,
            fadeToColor = Color.white
        };
        TransitionKit.instance.transitionWithDelegate(fader);
    }

}
