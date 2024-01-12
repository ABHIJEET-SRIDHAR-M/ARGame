using System;
using AR;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

namespace Managers
{
    public class PlacementManager : MonoBehaviour
    {

        private const float HMoveStep = .02f;
        private const float VMoveStep = .02f;
        
        private const int HMoveCount = 5;
        private const int VMoveCount = 5;
        
        private int _currentHCount;
        private int _currentVCount;
        
        public void OnUp()
        {
            if (_currentVCount >= VMoveCount) return;

            var pd = FindObjectOfType<ARPlaneDetection>();
            if (!pd) return;
            var ground = pd.anchoredGround;
            if (!ground) return;
            var pos = ground.transform.position + ground.transform.forward*VMoveStep;
            ground.transform.position = pos;
            _currentVCount++;
        }
        public void OnDown()
        {
            if (_currentVCount <= -VMoveCount) return;
            var pd = FindObjectOfType<ARPlaneDetection>();
            if (!pd) return;
            var ground = pd.anchoredGround;
            if (!ground) return;
            var pos = ground.transform.position - ground.transform.forward*VMoveStep;
            ground.transform.position = pos;
            _currentVCount--;
        }
        public void OnRight()
        {
            if (_currentHCount <= -HMoveCount) return;
            var pd = FindObjectOfType<ARPlaneDetection>();
            if (!pd) return;
            var ground = pd.anchoredGround;
            if (!ground) return;
            var pos = ground.transform.position - ground.transform.right*HMoveStep;
            ground.transform.position = pos;
            _currentHCount--;
        }
        public void OnLeft()
        {
            if (_currentHCount >= HMoveCount) return;
            var pd = FindObjectOfType<ARPlaneDetection>();
            if (!pd) return;
            var ground = pd.anchoredGround;
            if (!ground) return;
            var pos = ground.transform.position + ground.transform.right*HMoveStep;
            ground.transform.position = pos;
            _currentHCount++;
        }

        public void OnSliderValueChanged(float value)
        {
            var pd = FindObjectOfType<ARPlaneDetection>();
            if (!pd) return;
            var ground = pd.anchoredGround;
            if (!ground) return;
            var rot = ground.transform.rotation.eulerAngles;
            rot.y = value;
            ground.transform.rotation = Quaternion.Euler(rot);
        }
    }
}
