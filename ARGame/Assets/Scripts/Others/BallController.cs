using System;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Others
{
    public class BallController : MonoBehaviour
    {
        public Enums.BallType ballType = Enums.BallType.Plus1Score;
        private bool isClicked = false;
        
        private float _bounceSpeed = 1f;

        private float bounceRange = 10f;

        private float initialY = 0;
        
        private float _spawnTime;

        private float _maxAliveTime = 15f;
        public Vector3 spawnPosition;

        // Event to notify GameManager when the ball is clicked
        public delegate void BallClickedEventHandler(BallController clickedBall);

        public event BallClickedEventHandler OnBallClicked;
        private Timer _timer;
        
        private void Awake()
        {
            Timer.Cancel(_timer);
        }

        private void OnDestroy()
        {
            Timer.Cancel(_timer);
        }

        private void Start()
        {
            initialY = transform.parent.position.y;
            _spawnTime = Time.time;
            _timer = Timer.Register(
                duration: _maxAliveTime,
                onComplete: OnTimeEnd,
                onUpdate: secondsElapsed =>
                {
                    Bounce(bounceRange, transform.parent.position.y + (0.3f));
                    /*if (gameObject)
                    {
                        Bounce(bounceRange, transform.parent.position.y + (0.3f));
                    }*/
                },
                isLooped: false,
                useRealTime: false);
        }

        private void Update()
        {
            //Bounce(bounceRange, transform.parent.position.y + (0.3f));
        }

        private void OnTimeEnd()
        {
            Debug.Log("time has ended for " + name );
            var gm = FindObjectOfType<GameManager>();
            gm.RemoveBall(gameObject);
        }

        void Bounce(float height, float ground)
        {
            // Bounce the ball up and down with specified jump height and ground level
            float sineValue = Mathf.Abs(Mathf.Sin((Time.time - _spawnTime) * _bounceSpeed));
            float newY = ground + sineValue * height;
            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
        }

        
        public void HandleClick()
        {
            if (!isClicked)
            {
                Debug.Log("here");
                isClicked = true;
                // Invoke the event to notify GameManager
                OnBallClicked?.Invoke(this);
            }
        }
    }
}
