using Common;
using UnityEngine;

namespace UI
{
    public class ScreenManager : MonoBehaviour
    {

        public enum UIScreen
        {
            MainMenu,
            PositionMenu,
            RotationMenu,
            // Add more screen names as needed
        }

        private Transform[] screens;
        public UIScreen CurrentScreen { get; private set; } = UIScreen.MainMenu;

        void Start()
        {
            SetupUIScreens();
        }

        private void SetupUIScreens()
        {
            // Get all the first-level children of the canvas
            screens = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                screens[i] = transform.GetChild(i);
                //screens[i].gameObject.SetActive(false);
            }

            // Show the initial screen
            //ShowScreen(UIScreen.MainMenu);
        }

        // Function to transition to a specific screen by enum
        public void ShowScreen(UIScreen screen)
        {
            int index = (int)screen;

            if (index >= 0 && index < screens.Length)
            {
                // Hide the current screen
                screens[(int)CurrentScreen].gameObject.SetActive(false);

                // Show the new screen
                screens[index].gameObject.SetActive(true);

                // Update the current screen
                CurrentScreen = screen;
            }
        }
        
        public void ForceScreen(UIScreen screen)
        {
            int index = (int)screen;

            if (index >= 0 && index < screens.Length)
            {
                foreach (var screen1 in screens)
                {
                    screen1.gameObject.SetActive(false);
                }

                // Show the new screen
                screens[index].gameObject.SetActive(true);

                // Update the current screen
                CurrentScreen = screen;
            }
        }



    }
}
