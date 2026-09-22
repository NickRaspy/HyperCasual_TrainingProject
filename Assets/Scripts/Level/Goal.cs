using System.Collections;
using UnityEngine;

namespace Butcher_TA
{
    public class Goal : MonoBehaviour
    {
        [SerializeField] private AudioClip clip;
        private bool triggered;
        private void OnTriggerEnter(Collider other)
        {
            if (triggered || !other.CompareTag("Player") || GameManager.instance == null || !GameManager.instance.IsPlaying)
            {
                return;
            }

            triggered = true;
            GameManager.instance.player.ChangeMove(false);
            StartCoroutine(Wait());
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && GameManager.instance != null && GameManager.instance.IsPlaying)
            {
                GameManager.instance.player.ChangeMove(true);
            }
        }
        IEnumerator Wait()
        {
            yield return new WaitForSeconds(0.5f);
            transform.parent.GetComponent<Animator>().Play("GoalOpen");
            if (clip != null && GameManager.instance.source != null)
            {
                GameManager.instance.source.PlayOneShot(clip);
            }
        }
    }
}
