using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class Test1 : MonoBehaviour
{

    public GameObject ground;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateGround();
    }
    protected void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    protected void OnDisable()
    {
        EnhancedTouchSupport.Disable();
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
                else if (_startingPosition < touch.screenPosition
                             .x)
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

}
