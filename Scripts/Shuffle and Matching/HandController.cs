using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Samin.BlocksAndWords
{
    public class HandController : MonoBehaviour
    {
        [SerializeField] Ease moveEase = Ease.Linear;
        Vector3 primaryScale;
        void Start()
        {
            primaryScale = transform.localScale;
        }


        void Update()
        {
            if (TutorialManager.tutorialSingleton.reachedAtDestination)
            {
                if(transform.localScale == primaryScale)
                {
                    transform.DOScale(primaryScale * 1.3f, 0.4f);
                }
                
                if(transform.localScale == primaryScale * 1.3f)
                {
                    transform.DOScale(primaryScale, 0.4f);
                }
                
            }
        }

        public void AnimateWhenOverTarget()
        {
            Sequence animSequence = DOTween.Sequence().SetId("DoTweenAnim1");
            animSequence.Append(gameObject.transform.DOScale(primaryScale * 1.4f, 0.3f));
            animSequence.Append(gameObject.transform.DOScale(primaryScale, 0.3f));
            animSequence.SetLoops(-1);
        }

        public void ScaleWhenOverTarget()
        {
            gameObject.transform.DOScale(primaryScale * Mathf.Sin(Time.fixedTime * Mathf.PI), 0.5f);
        }
    }
}

