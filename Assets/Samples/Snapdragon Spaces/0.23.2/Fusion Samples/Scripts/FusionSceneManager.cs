/******************************************************************************
 * File: FusionSceneManager.cs
 * Copyright (c)2024 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/


using UnityEngine;
using UnityEngine.SceneManagement;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class FusionSceneManager : MonoBehaviour
    {
        public void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void LoadSceneByID(int sceneId)
        {
            SceneManager.LoadScene(sceneId);
        }

        public void LoadSceneByName(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void ToggleObjectVisibility(GameObject objectToToggle)
        {
            objectToToggle.SetActive(!objectToToggle.activeSelf);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
