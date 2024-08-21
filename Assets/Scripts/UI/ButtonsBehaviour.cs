using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI
{
    public class ButtonsBehaviour : MonoBehaviour
    {
        private VisualElement _root;
        private DropdownField _difficultyDropdown;
        private Button _startButton;
        private Button _exitButton;
    
        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
        }
    
        private void OnEnable()
        {
            _difficultyDropdown = _root.Q<DropdownField>("SelectDifficult");
            _difficultyDropdown.choices = new List<string> { "Easy", "Medium", "Hard" };
            _difficultyDropdown.value = null;
            _difficultyDropdown.RegisterValueChangedCallback(evt => OnDropdownValueChanges(evt.newValue));
        
            _startButton = _root.Q<Button>("Play");
            _startButton.clicked += OnStartButtonClicked;
            _startButton.SetEnabled(false);
        
            _exitButton = _root.Q<Button>("Exit");
            _exitButton.clicked += OnExitButtonClicked;
        }

        private void OnDropdownValueChanges(string newValue)
        {
            PlayerPrefs.SetString("Difficulty", newValue);
            PlayerPrefs.SetString("StopWatch", "00:00:00");
            _startButton.SetEnabled(!string.IsNullOrEmpty(newValue));
        }
        private void OnStartButtonClicked()
        {
            SceneManager.LoadScene(1);
        }
    
        private void OnExitButtonClicked()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}
