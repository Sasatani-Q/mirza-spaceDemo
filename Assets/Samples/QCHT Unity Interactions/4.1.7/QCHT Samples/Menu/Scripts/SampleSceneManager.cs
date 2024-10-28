// /******************************************************************************
//  * File: SampleSceneManager.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace QCHT.Samples.Menu
{
    public class SampleSceneManager : MonoBehaviour
    {
        [SerializeField]
        private SampleSettings startSample;

        [SerializeField]
        private Canvas _menuCanvas;

        private SampleSettings _currentSampleToLoad;
        private SampleSettings _currentSample;
        private Scene _currentScene;

        public UnityEvent OnSampleLoaded = new UnityEvent();

        public IEnumerator Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            if (startSample)
                LoadSample(startSample);

            // Fix when loading scene 
            // Avoid clicking on a UI element just after the scene loading
            // XRIT seems to set activatethisframe without considering the previous trigger state in input system
            // It occurs when XRBaseController when is created
            // TODO : Investigate this 
            _menuCanvas.gameObject.SetActive(false);
            yield return null;
            _menuCanvas.gameObject.SetActive(true);
            // End fix
        }

        public void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void SwitchToScene(string name)
        {
            SceneManager.LoadScene(name);
        }

        #region Sample Loading

        /// <summary>
        /// Loads a sample scene and unload the current sample scene if exists.
        /// </summary>
        public void LoadSample(SampleSettings sample)
        {
            if (_currentSampleToLoad || sample.SceneName.Equals(_currentScene.name))
                return;

            // Unload current scene if exists
            if (_currentScene.IsValid() && _currentScene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(_currentScene);
                _currentSample = null;
            }

            // Load the new sample scene by name
            _currentSampleToLoad = sample;
            SceneManager.LoadScene(sample.SceneName, LoadSceneMode.Additive);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            if (!_currentSampleToLoad || scene.name != _currentSampleToLoad.SceneName)
                return;

            _currentScene = scene;
            _currentSample = _currentSampleToLoad;
            _currentSampleToLoad = null;

            OnSampleLoaded?.Invoke();
        }

        #endregion
    }
}