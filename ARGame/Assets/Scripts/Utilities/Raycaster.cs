using Others;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Utilities
{
    public class Raycaster : MonoBehaviour
    {

        public bool allowRotate = false;
        // Start is called before the first frame update
        void Start()
        {

        }
        
        protected void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        protected void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }

        // Update is called once per frame
        void Update()
        {
            
            
            var activeTouches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
            Debug.Log(activeTouches.Count);
            if (activeTouches.Count < 1 || activeTouches[0].phase != TouchPhase.Began)
            {
                return;
            }
            int id = activeTouches[0].touchId;
            if (EventSystem.current.IsPointerOverGameObject(id))
            {
                return;
            }

            // Get the touch position
            Vector2 touchPosition = activeTouches[0].screenPosition;

            // Raycast from the touch position
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit))
            {
                // Print the name of the hit GameObject
                BallController ballController = hit.collider.GetComponent<BallController>();
                if (ballController != null)
                {
                    ballController.HandleClick();
                }
            }

        }

        public void SelectTile()
        {
            
        }
    }
}
