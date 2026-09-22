using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Butcher_TA
{
    public class PlayerEffects : MonoBehaviour, IPlayerEffects
    {
        [SerializeField] private List<ParticleSystem> particles;
        [SerializeField] private Animator halo;
        [SerializeField] private TMP_Text pointEffect;

        private SpriteRenderer haloSpriteRenderer;
        private Animator pointEffectAnimator;

        private void Start()
        {
            haloSpriteRenderer = halo.GetComponent<SpriteRenderer>();
            pointEffectAnimator = pointEffect.GetComponent<Animator>();
        }

        public void UseEffect(Material particle, Color color, int points, bool isGood)
        {
            haloSpriteRenderer.color = color;
            halo.gameObject.SetActive(false);
            halo.gameObject.SetActive(true);
            halo.Play("Splash");

            foreach (ParticleSystem p in particles)
            {
                if (particle != null)
                {
                    p.GetComponent<ParticleSystemRenderer>().material = particle;
                }
                p.Play();
            }

            pointEffect.text = points.ToString("+0;-0;0");
            pointEffectAnimator.Play(isGood ? "Gain" : "Loss", 0, 0f);
        }
    }

    public interface IPlayerEffects
    {
        void UseEffect(Material particle, Color color, int points, bool isGood);
    }
}
