using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Butcher_TA
{
    public class PlayerScore : MonoBehaviour, IPlayerScore
    {
        [SerializeField] private List<Outfit> outfits;
        [SerializeField] private Image sliderFill;
        [SerializeField] private Slider slider;
        [SerializeField] private Text stateText;
        [SerializeField] private List<Color> stateColors;
        [SerializeField] private List<string> states;

        private int currentOutfitIndex;

        public void OnScoreChange(int score)
        {
            slider.value = score;
        }

        public void ChangeOutfit(int score)
        {
            int index = GetNewOutfitIndex(score);
            for (int i = 0; i < outfits.Count; i++)
            {
                outfits[i].outfitModel.SetActive(i == index);
            }

            currentOutfitIndex = index;
            sliderFill.color = stateColors[index];
            stateText.text = states[index];
            stateText.color = stateColors[index];
        }

        public int GetCurrentOutfitIndex()
        {
            return currentOutfitIndex;
        }

        public int GetNewOutfitIndex(int score)
        {
            return Mathf.Max(0, outfits.FindLastIndex(outfit => score >= outfit.minimalScore));
        }
    }

    public interface IPlayerScore
    {
        void OnScoreChange(int score);
        void ChangeOutfit(int score);
        int GetCurrentOutfitIndex();
        int GetNewOutfitIndex(int score);
    }
}
