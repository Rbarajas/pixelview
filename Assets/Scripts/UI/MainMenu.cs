using System.Collections.Generic;
using UnityEngine;

namespace PixelView.Time_.UI
{
    public class MainMenu : MonoBehaviour
    {
        public GameObject InitiallyOpen;


        private Stack<GameObject> m_Open;


        private void Awake()
        {
            m_Open = new Stack<GameObject>();

            DontDestroyOnLoad(gameObject);
            if (FindObjectsOfType<MainMenu>().Length > 1)
                Destroy(gameObject);
        }

        private void Start()
        {
            Open(InitiallyOpen);
        }

        private void OnLevelWasLoaded()
        {
            var changedToLobby = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Main Menu";

            GetComponent<Canvas>().enabled = changedToLobby;

            Cursor.visible = changedToLobby;
        }

        public void Open(GameObject menu)
        {
            if (m_Open.Count > 0)
            {
                m_Open.Peek().SetActive(false);
            }

            if (menu != null)
            {
                menu.SetActive(true);

                m_Open.Push(menu);
            }
        }

        public void Replace(GameObject menu)
        {
            while (m_Open.Count > 0)
            {
                m_Open.Pop().SetActive(false);
            }

            Open(menu);
        }

        public void Back()
        {
            if (m_Open.Count > 0)
            {
                m_Open.Pop().SetActive(false);
            }

            if (m_Open.Count > 0)
            {
                m_Open.Peek().SetActive(true);
            }
            else
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }
}