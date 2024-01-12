
using UnityEngine;

namespace UI
{
    public class ScreenManager : MonoBehaviour
    {
        
        
        private Transform[] screens;
        public Enums.UIScreen CurrentScreen { get; private set; } = Enums.UIScreen.MainMenu;

        public bool showScore = false;

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
                screens[i].gameObject.SetActive(false);
            }

            // Show the initial screen
            ShowScreen(Enums.UIScreen.MainMenu);
        }

        // Function to transition to a specific screen by enum
        public void ShowScreen(Enums.UIScreen screen)
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
            screens[(int)Enums.UIScreen.ScoreMenu].gameObject.SetActive(showScore);
        }
        
    }
}
