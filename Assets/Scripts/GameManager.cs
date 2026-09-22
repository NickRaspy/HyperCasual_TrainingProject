using System.Collections.Generic;
using ButchersGames;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Butcher_TA
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public PlayerBehavior player;

        [Header("Audio")]
        public AudioSource source;
        [SerializeField] private AudioClip winSFX;
        [SerializeField] private AudioClip loseSFX;

        [Header("UI")]
        [SerializeField] private Canvas gameUI;
        [SerializeField] private Canvas menuUI;
        [SerializeField] private Canvas winUI;
        [SerializeField] private Canvas loseUI;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text summaryScoreText;
        [SerializeField] private TMP_Text getScoreButtonText;
        [SerializeField] private TMP_Text getMultiScoreButtonText;
        [SerializeField] private List<Text> levelNameTexts;

        private int score;
        private int summaryScore;
        private Level currentLevel;
        private bool rewardClaimed;

        public bool IsPlaying { get; private set; }
        public UnityEvent<int> OnScoreChange { get; } = new UnityEvent<int>();
        public int ScoreMultiplier { get; set; } = 1;

        public int Score
        {
            get => score;
            set
            {
                int newScore = Mathf.Max(0, value);
                if (score == newScore)
                {
                    return;
                }

                score = newScore;
                UpdateScoreUI();
                OnScoreChange.Invoke(score);

                if (IsPlaying && score == 0)
                {
                    EndLevel(false);
                }
            }
        }

        public int SummaryScore
        {
            get => summaryScore;
            set
            {
                summaryScore = Mathf.Max(0, value);
                if (summaryScoreText != null)
                {
                    summaryScoreText.text = summaryScore.ToString();
                }
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        private void Start()
        {
            SummaryScore = 0;
            PrepareGame();
        }

        private void Update()
        {
            if (!IsPlaying && menuUI.gameObject.activeSelf && Input.GetMouseButtonDown(0))
            {
                StartGame();
            }
        }

        public void PrepareGame()
        {
            IsPlaying = false;
            rewardClaimed = false;
            ScoreMultiplier = 1;
            menuUI.gameObject.SetActive(true);
            gameUI.gameObject.SetActive(false);
            winUI.gameObject.SetActive(false);
            loseUI.gameObject.SetActive(false);

            LevelManager.Default.Init();
            currentLevel = LevelManager.Default.CurrentLevelInstance;
            if (currentLevel == null)
            {
                return;
            }

            player.SetMoveState(false);
            player.ResetStartPosition(currentLevel.playerSpawnPoint);
            player.SetSplinePath(currentLevel.spline);
            player.ResetMove();
            player.ResetControls();
            Score = 40;
            UpdateScoreUI();
            player.RefreshScore(Score);
            player.SetModelAnimatorBoolValue("isWalking", false);
            player.PlayModelAnimation("Idle");
            UpdateLevelNameTexts();
        }

        public void StartGame()
        {
            if (IsPlaying || currentLevel == null || !menuUI.gameObject.activeSelf)
            {
                return;
            }

            menuUI.gameObject.SetActive(false);
            gameUI.gameObject.SetActive(true);
            IsPlaying = true;
            player.SetMoveState(true);
            player.SetModelAnimatorBoolValue("isWalking", true);
            LevelManager.Default.StartLevel();
        }

        public void EndLevel(bool didWin)
        {
            if (!IsPlaying)
            {
                return;
            }

            IsPlaying = false;
            gameUI.gameObject.SetActive(false);
            player.SetMoveState(false);
            player.SetModelAnimatorBoolValue("isWalking", false);
            player.PlayModelAnimation(didWin ? "Dance" : "Anger");

            if (source != null)
            {
                AudioClip clip = didWin ? winSFX : loseSFX;
                if (clip != null)
                {
                    source.PlayOneShot(clip);
                }
            }

            if (didWin)
            {
                SetGetScoreButtonText(false);
                winUI.gameObject.SetActive(true);
            }
            else
            {
                loseUI.gameObject.SetActive(true);
            }
        }

        public void SetSummaryScore(bool isMultiplied)
        {
            if (rewardClaimed || !winUI.gameObject.activeSelf)
            {
                return;
            }

            rewardClaimed = true;
            SummaryScore += Score * (isMultiplied ? Mathf.Max(1, ScoreMultiplier) : 1);
        }

        public void SetGetScoreButtonText(bool isMultiplied)
        {
            if (isMultiplied)
            {
                getMultiScoreButtonText.text = (Score * Mathf.Max(1, ScoreMultiplier)).ToString();
            }
            else
            {
                getScoreButtonText.text = Score.ToString();
            }
        }

        private void UpdateScoreUI()
        {
            if (scoreText != null)
            {
                scoreText.text = score.ToString();
            }
        }

        private void UpdateLevelNameTexts()
        {
            foreach (Text text in levelNameTexts)
            {
                if (text != null)
                {
                    text.text = $"Уровень {LevelManager.CurrentLevel}";
                }
            }
        }
    }
}
