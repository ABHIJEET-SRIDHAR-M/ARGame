using System.Collections.Generic;
using AR;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = System.Object;

namespace Managers
{
    public class CustomInputManager : MonoBehaviour
    {
        public List<Button> buttons;
        public Slider slider;
        private PlacementManager _placementManager;

        private const float SliderRange = 180f;
        private const float SliderStart = 0;

        // Mapping button names to method names
        private readonly Dictionary<string, string> _buttonMethodMap = new Dictionary<string, string>
        {
            { "ButtonUp", "OnUp" },
            { "ButtonDown", "OnDown" },
            { "ButtonLeft", "OnRight" },
            { "ButtonRight", "OnLeft" },
            { "ButtonRNext", "OnRNext" },
            { "ButtonPNext", "StartGame" },
            { "ButtonScan", "OnStartScanning" },
            { "ButtonRestart", "StartGame" },
            // Add more mappings as needed
        };

        private void Start()
        {
            _placementManager = FindObjectOfType<PlacementManager>();
            // Attach methods to button click events
            foreach (var button in buttons)
            {
                var bstate = button.gameObject.activeInHierarchy;
                //button.gameObject.SetActive(true);
                button.onClick.AddListener(() => ButtonClicked(button));
                //button.gameObject.SetActive(bstate);
            }

            // Attach method to slider value changed event
            //slider.gameObject.SetActive(true);
            slider.minValue = -SliderRange;
            slider.maxValue = SliderRange;
            slider.value = SliderStart;
            slider.wholeNumbers = true;
            slider.onValueChanged.AddListener(SliderValueChanged);
            
            //slider.gameObject.SetActive(sstate);
        }

        private void ButtonClicked(Button button)
        {
            // Get the method name from the dictionary
            if (_buttonMethodMap.TryGetValue(button.name, out string methodName))
            {
                InvokeMethodByName(methodName);
            }
            else
            {
                Debug.LogWarning($"No method mapped for button {button.name}");
            }
        }

        private void InvokeMethodByName(string methodName)
        {
            object caller = this;
            // Use reflection to dynamically invoke the method
            if (methodName.Equals("OnUp") || methodName.Equals("OnDown") || methodName.Equals("OnLeft") || methodName.Equals("OnRight"))
            {
                caller = _placementManager;
            }
            caller.GetType().GetMethod(methodName)?.Invoke(caller, null);

        }

        private void SliderValueChanged(float value)
        {
            // Invoke a method from AnotherClass when the slider value changes
            _placementManager.OnSliderValueChanged(value);
        }

        public void OnRNext()
        {
            FindObjectOfType<ScreenManager>().ShowScreen(Enums.UIScreen.PlacementMenu);
            
        }
        public void OnStartScanning()
        {
            FindObjectOfType<ScreenManager>().ShowScreen(Enums.UIScreen.ScanMenu);
            FindObjectOfType<ARPlaneDetection>().OnPlaneDetection();
            
        }
        
        public void StartGame()
        {
            FindObjectOfType<ScreenManager>().showScore = true;
            var gm = FindObjectOfType<GameManager>();
            gm.StartGame();
        }
    }
}