using UnityEngine;
using UnityEngine.Splines;

namespace Butcher_TA
{
    public class PlayerBehavior : MonoBehaviour
    {
        private IPlayerMovement playerMovement;
        private IPlayerEffects playerEffects;
        private IPlayerScore playerScore;
        private IPlayerAnimation playerAnimation;
        private IPlayerSpline playerSpline;

        private void Awake()
        {
            playerMovement = GetComponent<IPlayerMovement>();
            playerEffects = GetComponent<IPlayerEffects>();
            playerScore = GetComponent<IPlayerScore>();
            playerAnimation = GetComponent<IPlayerAnimation>();
            playerSpline = GetComponent<IPlayerSpline>();
        }

        private void Start()
        {
            GameManager.instance.OnScoreChange.AddListener(OnScoreChanged);
        }

        private void OnDestroy()
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.OnScoreChange.RemoveListener(OnScoreChanged);
            }
        }

        private void OnScoreChanged(int score)
        {
            playerScore.OnScoreChange(score);
            if (score > 0)
            {
                ChangeOutfit(score, GameManager.instance.IsPlaying);
            }
        }

        public void SetMoveState(bool isMoving)
        {
            playerMovement.CanMove = isMoving;
            if (isMoving)
            {
                playerSpline.StartMove();
            }
            else
            {
                playerSpline.StopMove();
            }
        }

        public void ChangeMove(bool isMoving)
        {
            playerMovement.CanMove = isMoving;
        }

        public void ResetControls()
        {
            playerMovement.ResetPosition();
        }

        public void RefreshScore(int score)
        {
            playerScore.OnScoreChange(score);
            playerScore.ChangeOutfit(score);
            int outfitIndex = playerScore.GetCurrentOutfitIndex();
            playerAnimation.SetModelAnimatorIntegerValue("walkState", outfitIndex > 1 ? 1 : 0);
        }

        public void ChangeOutfit(int score, bool mustAnimate)
        {
            int previousIndex = playerScore.GetCurrentOutfitIndex();
            int newIndex = playerScore.GetNewOutfitIndex(score);
            if (newIndex == previousIndex)
            {
                return;
            }

            playerScore.ChangeOutfit(score);
            playerAnimation.SetModelAnimatorIntegerValue("walkState", newIndex > 1 ? 1 : 0);
            if (mustAnimate)
            {
                playerAnimation.PlayModelAnimation(previousIndex < newIndex ? "Spin" : "Stepped");
            }
        }

        public void UseEffect(Material particle, Color color, int points, bool isGood)
        {
            playerEffects.UseEffect(particle, color, points, isGood);
        }

        public void PlayModelAnimation(string name)
        {
            playerAnimation.PlayModelAnimation(name);
        }

        public void SetModelAnimatorBoolValue(string name, bool value)
        {
            playerAnimation.SetModelAnimatorBoolValue(name, value);
        }

        public void SetSplinePath(SplineContainer spline)
        {
            playerSpline.SetSplinePath(spline);
        }

        public void ResetMove()
        {
            playerSpline.ResetMove();
        }

        public void ResetStartPosition(Transform spawnpoint)
        {
            playerSpline.ResetStartPosition(spawnpoint);
        }
    }
}
