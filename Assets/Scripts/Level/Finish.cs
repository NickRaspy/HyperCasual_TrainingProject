using UnityEngine;

namespace Butcher_TA
{
    public class Finish : MonoBehaviour
    {
        [SerializeField] private AudioClip clip;
        [SerializeField] private bool isFinal;
        [SerializeField] private int minimumGap;
        private bool triggered;
        private void OnTriggerEnter(Collider other)
        {
            if (triggered || !other.CompareTag("Player") || GameManager.instance == null || !GameManager.instance.IsPlaying)
            {
                return;
            }

            triggered = true;
            if (GameManager.instance.Score >= minimumGap && !isFinal)
            {
                transform.parent.GetComponent<Animator>().Play("DoorOpen");
                if (clip != null && GameManager.instance.source != null)
                {
                    GameManager.instance.source.PlayOneShot(clip);
                }
            }
            else
            {
                GameManager.instance.EndLevel(true);
            }
        }
    }
}
