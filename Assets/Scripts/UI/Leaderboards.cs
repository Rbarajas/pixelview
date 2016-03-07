using UnityEngine;

namespace PixelView.Time_.UI
{
    public class Leaderboards : MonoBehaviour
    {
        [System.Serializable]
        public struct UI_
        {
            public LeaderboardsEntry[] Entries;
        }


        public UI_ UI;


        public void Populate(Score[] scores)
        {
            for (var entryIndex = 0; entryIndex < UI.Entries.Length; ++entryIndex)
            {
                var entry = UI.Entries[entryIndex];

                entry.gameObject.SetActive(entryIndex < scores.Length);

                if (entry.gameObject.activeSelf)
                {
                    var score = scores[entryIndex];
                    entry.Populate(score.Rank, score.Username, score.Value);
                }
            }
        }
    }
}