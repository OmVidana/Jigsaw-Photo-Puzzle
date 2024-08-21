using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Game
{
    public class PuzzleManager : MonoBehaviour
    {
        // UI
        private VisualElement _root;
        private Button _goBackButton;
        private Label _stopWatch;
        private static Button _startButton;
    
        //PlayerPref
        private static string _difficulty;
        private static string _stopWatchTime;
        
        //Game
        public GameObject board;
        public List<Sprite> puzzles;
        [Range(0f, 1.0f)] public float puzzleTransparency;
        private static GameObject _piecesContainer;
        private static Stopwatch _stopwatch;
        private static int _piecesPlaced;
        private static int _totalPieces;
        private string _piecesPath;
        private byte _cols;
        private byte _rows;
        private bool _gameEnded;
        
        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _difficulty = PlayerPrefs.GetString("Difficulty");
            _stopWatchTime = PlayerPrefs.GetString("StopWatch");
            PuzzleGenerator();
        }

        private void OnEnable()
        {
            _goBackButton = _root.Q<Button>("GoBack");
            _goBackButton.clicked += OnGoBackButtonClicked;
        
            _startButton = _root.Q<Button>("Start");
            _startButton.clicked += OnStartButtonClicked;

            _stopWatch = _root.Q<Label>("Stopwatch");
            _stopWatch.text = $"{_stopWatchTime}";

            _stopwatch = new Stopwatch();
        }

        void Update()
        {
            if (_stopwatch.IsRunning)
            {
                _stopWatch.text = $"{_stopwatch.Elapsed.Minutes:00}:{_stopwatch.Elapsed.Seconds:00}:{_stopwatch.Elapsed.Milliseconds / 10:00}";
            }
        }
        
        //UI Toolkit Subscriptions
        private void OnGoBackButtonClicked()
        {
            if (_stopwatch.IsRunning){
                _stopwatch.Stop();
                _stopWatchTime = $"{_stopwatch.Elapsed.Minutes:00}:{_stopwatch.Elapsed.Seconds:00}:{_stopwatch.Elapsed.Milliseconds / 10:00}";
                PlayerPrefs.SetString("StopWatch", _stopWatchTime);
            }
            SceneManager.LoadScene(0);
            Debug.Log($"Puzzle incomplete during {_stopWatchTime} seconds on {_difficulty} difficulty.");

        }
    
        private void OnStartButtonClicked()
        {
            if (!_stopwatch.IsRunning && !_gameEnded) {
                _piecesContainer.SetActive(true);
                _stopwatch.Start();
                _startButton.text = "Pause!";
            }
            else
            {
                _stopwatch.Stop();
                _piecesContainer.SetActive(false);
                _startButton.text = "Continue!";
            }
        }
    
        //Game Functions
        private void PuzzleGenerator()
        {
            SpriteRenderer boardSprite = board.GetComponent<SpriteRenderer>();
            var boardColor = boardSprite.color;
            boardColor.a = puzzleTransparency;
            boardSprite.color = boardColor;
            
            switch (_difficulty)
            {
                case "Easy":
                    boardSprite.sprite = puzzles[0];
                    _cols = 3;
                    _rows = 2;
                    _piecesPath = "Backgrounds/1/Pieces";
                    break;
                case "Medium":
                    boardSprite.sprite = puzzles[1];
                    _cols = 5;
                    _rows = 4;
                    _piecesPath = "Backgrounds/2/Pieces";
                    break;
                case "Hard":
                    boardSprite.sprite = puzzles[2];
                    _cols = 10;
                    _rows = 10;
                    _piecesPath = "Backgrounds/3/Pieces";
                    break;
            }
            
            Sprite[] sprites = Resources.LoadAll<Sprite>(_piecesPath);
            if (sprites.Length == 0)
            {
                throw new Exception($"No Pieces found in {_piecesPath}");
            }
            
            _piecesPlaced = 0;
            _totalPieces = _cols * _rows;
            _piecesContainer = new GameObject("PiecesContainer");
            
            Vector3 boardSize = board.GetComponent<SpriteRenderer>().bounds.size;

            float pieceWidth = boardSize.x / _cols;
            float pieceHeight = boardSize.y / _rows;

            int index = 1;
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    GameObject pieceHolder = new GameObject($"P_{index - 1}");
                    pieceHolder.transform.SetParent(board.transform);

                    float posX = -boardSize.x / 2 + (col * pieceWidth) + (pieceWidth / 2);
                    float posY = boardSize.y / 2 - (row * pieceHeight) - (pieceHeight / 2);

                    pieceHolder.transform.localPosition = new Vector3(posX, posY, 0);
                    pieceHolder.AddComponent<PuzzleHolder>();
                        
                    GeneratePiece(sprites[index - 1], index).transform.SetParent(_piecesContainer.transform);
                    index++;
                }
            }
            
            _piecesContainer.SetActive(false);
        }
    
        private GameObject GeneratePiece(Sprite sprite, int layer)
        {
            GameObject puzzlePiece = new GameObject(sprite.name);
            SpriteRenderer spriteRenderer = puzzlePiece.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            puzzlePiece.AddComponent<BoxCollider2D>();
            puzzlePiece.AddComponent<PuzzlePiece>();
    
            float randomX = Random.Range(8f, 14.5f);
            float randomY = Random.Range(-5.5f, 5.5f);
            puzzlePiece.transform.position = new Vector3(randomX, randomY, 5.01f + layer * 0.01f);
            return puzzlePiece;
        }
        
        public static void PiecePlaced()
        {
            _piecesPlaced++;
            if (_piecesPlaced >= _totalPieces)
            {
                CompletePuzzle();
            }
        }
        
        private static void CompletePuzzle()
        {
            _stopwatch.Stop();
            _stopWatchTime = $"{_stopwatch.Elapsed.Minutes:00}:{_stopwatch.Elapsed.Seconds:00}:{_stopwatch.Elapsed.Milliseconds / 10:00}";
            _startButton.text = "Completed!";
            _startButton.SetEnabled(false);
            PlayerPrefs.SetString("StopWatch", _stopWatchTime);
            Debug.Log($"Puzzle completed in {_stopWatchTime} seconds on {_difficulty} difficulty.");
        }
    }
}
