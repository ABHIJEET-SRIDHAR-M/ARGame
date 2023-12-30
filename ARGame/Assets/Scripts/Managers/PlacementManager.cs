using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace Managers
{
    public class PlacementManager : MonoBehaviour
    {

        public Slider horizontalPositionSlider;
        public Slider rotationSlider;
        public Slider verticalPositionSlider;

        public TMP_Text text;
    
        private const float HPSliderRange = .1f;
        private const float HPSliderStart = 0f;
        
        private const float VPSliderRange = .1f;
        private const float VPSliderStart = 0f;
        
        private const float RSliderRange = 1f;
        private const float RSliderStart = 0f;

        private GameObject _propInSlider;
        
        private ScreenManager _screenManager;

        public void SetProp(ARAnchor anchor)
        {
            _propInSlider = anchor.gameObject;
            SetupSlider();
        }
        private void Start()
        {
            _screenManager = FindObjectOfType<ScreenManager>();

            SetupSlider();
            
            _screenManager.ForceScreen(ScreenManager.UIScreen.MainMenu);
        }

        private Quaternion GetPrefabRotation(Quaternion originalAngle)
        {
            var rot = originalAngle.eulerAngles;
            rot = new Vector3(rot.x, rot.y + 180, rot.z);
            return Quaternion.Euler(rot);
        }
    
        private void SetupSlider()
        {
            switch (_screenManager.CurrentScreen)
            {
                case ScreenManager.UIScreen.PositionMenu:
                    horizontalPositionSlider.minValue = -HPSliderRange;
                    horizontalPositionSlider.maxValue = HPSliderRange;
                    horizontalPositionSlider.value = HPSliderStart;
                    horizontalPositionSlider.onValueChanged.AddListener(OnHPSliderValueChanged);
        
                    verticalPositionSlider.minValue = -VPSliderRange;
                    verticalPositionSlider.maxValue = VPSliderRange;
                    verticalPositionSlider.value = VPSliderStart;
                    verticalPositionSlider.onValueChanged.AddListener(OnVPSliderValueChanged);
                    break;
                case ScreenManager.UIScreen.RotationMenu:
                    rotationSlider.minValue = -RSliderRange;
                    rotationSlider.maxValue = RSliderRange;
                    rotationSlider.value = RSliderStart;
                    rotationSlider.onValueChanged.AddListener(OnRSliderValueChanged);
                    break;
                case ScreenManager.UIScreen.MainMenu:
                    break;
                default:
                    Debug.Log("None");
                    break;
            }
        }
        
        
        
        private void OnHPSliderValueChanged(float value)
        {
            var scale = 10;
            var val = value * scale;
            if (_propInSlider)
            {
                var pos = _propInSlider.transform.position;
                pos.x = val;
                _propInSlider.transform.position = pos;

            }

            text.text = "Value: " + value;

        }
    
        private void OnVPSliderValueChanged(float value)
        {
            var scale = 10;
            var val = value * scale;
            if (_propInSlider)
            {
                var pos = _propInSlider.transform.position;
                pos.z = val;
                _propInSlider.transform.position = pos;

            }
            text.text = "Value: " + value;
        }
        
        private void OnRSliderValueChanged(float value)
        {
            var scale = 10;
            var val = value * scale;
            if (_propInSlider)
            {
                var pos = _propInSlider.transform.position;
                pos.z = val;
                _propInSlider.transform.position = pos;

            }
            text.text = "Value: " + value;
        }
    }
}
