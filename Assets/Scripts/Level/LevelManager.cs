using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ButchersGames
{
    public class LevelManager : MonoBehaviour
    {
        private const string CompleteLevelCountKey = "Complete Lvl Count";
        private const string LastLevelIndexKey = "Last Level Index";
        private const string CurrentAttemptKey = "Current Attempt";

        private static LevelManager instance;

        [SerializeField] private LevelsList levels;
        [SerializeField] private bool editorMode;

        public static LevelManager Default => instance;
        public static int CurrentLevel => instance != null && instance.editorMode
            ? instance.CurrentLevelIndex + 1
            : CompleteLevelCount + 1;
        public static int CompleteLevelCount
        {
            get => PlayerPrefs.GetInt(CompleteLevelCountKey);
            set => PlayerPrefs.SetInt(CompleteLevelCountKey, value);
        }

        public static int LastLevelIndex
        {
            get => PlayerPrefs.GetInt(LastLevelIndexKey);
            set => PlayerPrefs.SetInt(LastLevelIndexKey, value);
        }

        public static int CurrentAttempt
        {
            get => PlayerPrefs.GetInt(CurrentAttemptKey);
            set => PlayerPrefs.SetInt(CurrentAttemptKey, value);
        }

        public int CurrentLevelIndex;
        public List<Level> Levels => levels.lvls;
        public Level CurrentLevelInstance { get; private set; }
        public event Action OnLevelStarted;

        private void Awake()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        public void Init()
        {
            if (CurrentLevelInstance != null)
            {
                return;
            }

#if !UNITY_EDITOR
            editorMode = false;
#endif

            SelectLevel(editorMode ? CurrentLevelIndex : LastLevelIndex);
        }

        public void StartLevel()
        {
            OnLevelStarted?.Invoke();
        }

        public void RestartLevel()
        {
            CurrentAttempt++;
            SelectLevel(CurrentLevelIndex);
        }

        public void NextLevel()
        {
            if (!editorMode)
            {
                CompleteLevelCount++;
                CurrentAttempt = 0;
            }

            SelectLevel(CurrentLevelIndex + 1);
        }

        public void PrevLevel()
        {
            SelectLevel(CurrentLevelIndex - 1);
        }

        public void SelectLevel(int levelIndex)
        {
            if (levels == null || levels.lvls == null || levels.lvls.Count == 0)
            {
                Debug.LogError("Level list is empty.", this);
                return;
            }

            int count = levels.lvls.Count;
            int correctedIndex = ((levelIndex % count) + count) % count;

            if (!editorMode && levels.randomizedLvls && levelIndex >= count && count > 1)
            {
                correctedIndex = UnityEngine.Random.Range(0, count - 1);
                if (correctedIndex >= CurrentLevelIndex)
                {
                    correctedIndex++;
                }
            }

            Level prefab = levels.lvls[correctedIndex];
            if (prefab == null)
            {
                Debug.LogError($"Level prefab at index {correctedIndex} is missing.", this);
                return;
            }

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (Application.isPlaying)
                {
                    child.SetActive(false);
                    Destroy(child);
                }
                else
                {
                    DestroyImmediate(child);
                }
            }

#if UNITY_EDITOR
            CurrentLevelInstance = Application.isPlaying
                ? Instantiate(prefab, transform)
                : (Level)PrefabUtility.InstantiatePrefab(prefab, transform);
#else
            CurrentLevelInstance = Instantiate(prefab, transform);
#endif

            CurrentLevelIndex = correctedIndex;
            if (Application.isPlaying && !editorMode)
            {
                LastLevelIndex = correctedIndex;
            }
        }
    }
}
