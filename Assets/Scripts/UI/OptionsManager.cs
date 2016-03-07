using UnityEngine;
using UnityEngine.UI;

namespace PixelView.Time_.UI
{
    public class OptionsManager : MonoBehaviour
    {
        [System.Serializable]
        public struct UIReference
        {
            public Slider VolumeSlider;

            public Slider SensitivitySlider;

            public InputField PlayerNameInput;

            public RectTransform PlayerColorGrid;
        }


        public GameObject PlayerColorGridItemPrefab;

        public UIReference UI;


        private void Start()
        {
            foreach (var color in GameSettings.Instance.SelectableColors)
            {
                var item = Instantiate(PlayerColorGridItemPrefab, Vector3.zero, Quaternion.identity) as GameObject;

                item.transform.SetParent(UI.PlayerColorGrid, false);

                item.GetComponentInChildren<Image>().color = color;

                var toggle = item.GetComponentInChildren<Toggle>();
                toggle.isOn = false;
                toggle.group = UI.PlayerColorGrid.GetComponent<ToggleGroup>();
                toggle.onValueChanged.AddListener(value => 
                {
                    if (value)
                        GameSettings.Instance.PlayerColorIndex = item.transform.GetSiblingIndex();
                });
            }
            UI.PlayerColorGrid.GetChild(GameSettings.Instance.PlayerColorIndex).GetComponentInChildren<Toggle>().isOn = true;

            UI.VolumeSlider.value = GameSettings.Instance.Volume;
            UI.VolumeSlider.onValueChanged.AddListener(value => GameSettings.Instance.Volume = value);

            UI.SensitivitySlider.value = GameSettings.Instance.Sensitivity;
            UI.SensitivitySlider.onValueChanged.AddListener(value => GameSettings.Instance.Sensitivity = value);

            UI.PlayerNameInput.text = GameSettings.Instance.PlayerName;
            UI.PlayerNameInput.onEndEdit.AddListener(value => GameSettings.Instance.PlayerName = value);
        }
    }
}