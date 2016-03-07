using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace PixelView.Time_.UI
{
    [System.Serializable]
    public struct Score
    {
        public int Rank;

        public string Username;

        public int Value;
    }


    public class LeaderboardsClient : MonoBehaviour
    {
        #region LoadPageResponse

        [System.Serializable]
        struct LoadPageResponse
        {
            public bool PrevPage;

            public bool NextPage;

            public Score[] Scores;            
        }

        #endregion

        #region PageLoadedEvent

        [System.Serializable]
        public class PageLoadedEvent : UnityEvent<Score[]>
        {
        }

        #endregion


        public string BaseUrl = "localhost";

        public int Count = 10;

        [HideInInspector]
        public int Page;

        [HideInInspector]
        public bool CanLoadNextPage;

        [HideInInspector]
        public bool CanLoadPrevPage;

        [HideInInspector]
        public Score[] Scores;

        public PageLoadedEvent OnPageLoaded;        


        private void Start()
        {
            LoadPage(0);
        }

        public void LoadPage(int page)
        {
            StartCoroutine(LoadPageCoroutine(page));
        }

        public void LoadPrevPage()
        {
            if (CanLoadPrevPage)
            {
                LoadPage(Page - 1);
            }
        }

        public void LoadNextPage()
        {
            if (CanLoadNextPage)
            {
                LoadPage(Page + 1);
            }
        }        

        private IEnumerator LoadPageCoroutine(int page)
        {
            var url = string.Format(BaseUrl + "/get-scores.php?page={0}&count={1}", page, Count);
            var www = new WWW(url);

            yield return www;

            try
            {
                var loadPageResponse = JsonUtility.FromJson<LoadPageResponse>(www.text);

                Page = page;
                CanLoadPrevPage = loadPageResponse.PrevPage;
                CanLoadNextPage = loadPageResponse.NextPage;
                Scores = loadPageResponse.Scores;

                OnPageLoaded.Invoke(Scores);
            }
            catch
            {
                Page = 0;
                CanLoadPrevPage = false;
                CanLoadNextPage = false;
                Scores = null;
            }            
        }
    }
}