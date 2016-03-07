using UnityEngine;
using UnityEngine.UI;

namespace PixelView.Time_.UI
{
    public class LeaderboardsEntry : MonoBehaviour
    {
        [System.Serializable]
        public struct UI_
        {
            public Text RankText;

            public Text UsernameText;

            public Text ScoreText;
        }


        public UI_ UI;


        public void Populate(int rank, string username, int score)
        {
            UI.RankText.text = rank.ToString();
            UI.UsernameText.text = username;
            UI.ScoreText.text = score.ToString();
        }
    }
}