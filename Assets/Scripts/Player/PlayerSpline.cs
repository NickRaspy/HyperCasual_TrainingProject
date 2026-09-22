using UnityEngine;
using UnityEngine.Splines;

namespace Butcher_TA 
{
    public class PlayerSpline : MonoBehaviour, IPlayerSpline
    {
        [SerializeField] private SplineAnimate splineAnimate;

        public void SetSplinePath(SplineContainer spline) => splineAnimate.Container = spline;

        public void StartMove() => splineAnimate.Play();

        public void ResetMove() => splineAnimate.Restart(false);

        public void StopMove() => splineAnimate.Pause();

        public void ResetStartPosition(Transform spawnpoint) => transform.SetPositionAndRotation(spawnpoint.position, spawnpoint.rotation);
    }

    public interface IPlayerSpline
    {
        void SetSplinePath(SplineContainer spline);
        void StartMove();
        void StopMove();
        void ResetMove();
        void ResetStartPosition(Transform spawnpoint);
    }
}
