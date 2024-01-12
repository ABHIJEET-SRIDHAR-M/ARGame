using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AR;
using Others;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public List<GameObject> ballPrefabs; // List of ball prefabs for each type
        
        public float spawnInterval = 2f;
        public float gameDuration = 60f;
        public TMP_Text timerText;
        public TMP_Text gameOverText;
        
        public bool gameRunning = false;

        private Dictionary<Vector3, bool> _combinations;

        private ScreenManager _screenManager;
        
        private Timer _timer;
        
        private GameObject ground;
        
        private List<BallController> activeBalls = new List<BallController>();

        protected void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        protected void OnDisable()
        {
            EnhancedTouchSupport.Disable();
            
        }

        private void OnDestroy()
        {
            Timer.Cancel(_timer);
        }

        private void Awake()
        {
            Timer.Cancel(_timer);
        }

        private void Start()
        {
            _screenManager = FindObjectOfType<ScreenManager>();
            
        }

        public void StartGame()
        {
            _screenManager.ShowScreen(Enums.UIScreen.GameMenu);
            SumScore.Reset();
            Timer.Cancel(_timer);
            _timer = Timer.Register(
                duration: gameDuration,
                onComplete: EndGame,
                onUpdate: secondsElapsed =>
                {
                    timerText.text = "Time: "+ (gameDuration - (int)secondsElapsed);
                },
                isLooped: false,
                useRealTime: false);
            
            ground = FindObjectOfType<ARPlaneDetection>().anchoredGround;
            //ground = GameObject.Find("Ground");
            gameRunning = true;
            _combinations = (GenerateVectorCombinations());
            StartCoroutine(SpawnBalls());
        }

        private void Update()
        {
            if (!gameRunning) return;
            PerformRaycast();
            RotateGround();
        }
        
        private void PerformRaycast()
        {
            var activeTouches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
            if (activeTouches.Count < 1 || activeTouches[0].phase != UnityEngine.InputSystem.TouchPhase.Began)
            {
                return;
            }
            
            int id = activeTouches[0].touchId;
            if (EventSystem.current.IsPointerOverGameObject(id))
            {
                return;
            }

            Vector2 touchPosition = activeTouches[0].screenPosition;
            
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit))
            {
                BallController ballController = hit.collider.GetComponent<BallController>();
                if (ballController != null)
                {
                    ballController.HandleClick();
                }
            }
        }

        private float _startingPosition;
        private void RotateGround()
        {
            var rotateSpeed = 150f;
            var activeTouches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
            if (activeTouches.Count < 1)
            {
                return;
            }

            var touch = activeTouches[0];

            switch (touch.phase)
            {
                case TouchPhase.None:
                    break;
                case TouchPhase.Began:
                    _startingPosition = touch.screenPosition.x;
                    break;
                case TouchPhase.Moved:
                    if (_startingPosition > touch.screenPosition.x)
                    {
                        ground.transform.Rotate(ground.transform.up, -rotateSpeed * Time.deltaTime);
                    }
                    else if (_startingPosition < touch.screenPosition.x)
                    {
                        ground.transform.Rotate(ground.transform.up, rotateSpeed * Time.deltaTime);
                    }
                    _startingPosition = touch.screenPosition.x;
                    break;
                case TouchPhase.Ended:
                    break;
                case TouchPhase.Canceled: 
                    break;
                case TouchPhase.Stationary:
                    break;
                default:
                    UnityEngine.Debug.Log("no state");
                    break;
            }
        }

        IEnumerator  SpawnBalls()
        {
            var shuffledKeys = _combinations.Keys.ToList();
            shuffledKeys = RandomizeList(shuffledKeys);
            foreach (var spawnPosition in shuffledKeys)
            {
                if (!gameRunning)
                {
                    yield break;
                }

                if (_combinations[spawnPosition])
                {
                    Debug.Log("here" + spawnPosition);
                    continue;
                }


                _combinations[spawnPosition] = true;
                
                GameObject ballPrefab = GetRandomBallPrefab(); // Get a random prefab from the list
                GameObject ball = Instantiate(ballPrefab);

                BallController ballController = ball.GetComponent<BallController>();
                ball.transform.SetParent(ground.transform);
                ball.transform.localPosition = spawnPosition;
                ballController.OnBallClicked += OnBallClick;
                ballController.spawnPosition = spawnPosition;
                activeBalls.Add(ballController);
                yield return new WaitForSeconds(spawnInterval);
            }

            if (gameRunning)
            {
                while (!PositionAvailable())
                {
                    Debug.Log("all positions filled, waiting");
                    yield return null;
                    if (!gameRunning)
                    {
                        yield break;
                    }
                }

                Debug.Log("spawning again");
                StartCoroutine(SpawnBalls());
            }

            /*if (gameRunning)
            {
                StartCoroutine(SpawnBalls());
            }*/
        }

        private bool PositionAvailable()
        {
            foreach (var combination in _combinations)
            {
                if (!combination.Value)
                {
                    return true;
                }
            }
            return false;
        }

        Dictionary<Vector3, bool> GenerateVectorCombinations()
        {   
            Dictionary<Vector3, bool> combinations = new Dictionary<Vector3, bool>();

            for (float x = -4; x <= 4; x += 1.3f)
            {
                for (float y = -4; y <= 4; y+= 1.3f)
                {
                    Vector3 combination = new Vector3(x, 0.5f, y);

                    combinations.Add(combination,false);
                }
            }

            return combinations;
        }
        
        private static System.Random random = new System.Random();
        public static List<T> RandomizeList<T>(List<T> inputList)
        {
            List<T> randomList = new List<T>(inputList);

            // Use Sort method with a custom comparer
            randomList.Sort((a, b) => random.Next(-1, 2));

            return randomList;
        }
        
        // Get a random ball prefab from the list
        GameObject GetRandomBallPrefab()
        {
            if (ballPrefabs.Count == 0)
            {
                return null;
            }

            return ballPrefabs[Random.Range(0, ballPrefabs.Count)];
        }

        void OnBallClick(BallController clickedBall)
        {
            var ballTypeValue = clickedBall.ballType;

            switch (ballTypeValue)
            {
                case Enums.BallType.Plus1Score:
                    SumScore.Add(1); 
                    break;
                case Enums.BallType.Plus2Score:
                    SumScore.Add(2); 
                    break;
                case Enums.BallType.Minus1Score:
                    SumScore.Add(-1); 
                    break;
                default:
                    Debug.Log("Unknown ball type");
                    break;
            }

            RemoveBall(clickedBall.gameObject);
        }

        public void RemoveBall(GameObject ball)
        {
            var bc = ball.GetComponent<BallController>();
            _combinations[bc.spawnPosition] = false;
            activeBalls.Remove(bc);
            Destroy(ball);
        }

        void EndGame()
        {
            Debug.Log("ending");
            gameRunning = false;
            if (SumScore.Score > SumScore.HighScore)
                SumScore.SaveHighScore();
            _screenManager.ShowScreen(Enums.UIScreen.GameOverMenu);
            gameOverText.text = $"Game Over! \nYour grade: {GetGrade()}";
            foreach (var ballController in activeBalls)
            {
                Destroy(ballController.gameObject);
            }
            activeBalls.Clear();
        }

        private string GetGrade()
        {
            string grade;

            switch (SumScore.Score)
            {
                case > 8:
                    grade = "A";
                    break;
                case > 6:
                    grade = "B";
                    break;
                case > 4:
                    grade = "C";
                    break;
                default:
                    grade = "D";
                    break;
            }

            return grade;
        }
    }
}