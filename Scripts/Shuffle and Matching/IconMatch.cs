using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/*
 * Iconmatch stores the icon and their respective string to be matched
 * it return the string to the game manager at the beginning of the level/game
 */

namespace Samin.BlocksAndWords
{
    public class IconMatch : MonoBehaviour
    {
        public string Word
        {
            get
            {
                return this.word;
            }
            set
            {
                this.word = value;
            }
        }
        [SerializeField] string word;
        [SerializeField] Color matchedColor;

        public string GetWord()
        {
            return word.ToUpper();
        }

        [SerializeField] Sprite iconSprite;
        [SerializeField] SpriteRenderer imageRenderer;

        [SerializeField] Transform background;
        [SerializeField] Transform effects;
        SpriteRenderer backRenderer;

        public enum IconState
        {
            unmatched,
            matched
        }

        public IconState stateOfIcon;
        [SerializeField] Transform tickMark;

        void Start()
        {
            imageRenderer = GetComponent<SpriteRenderer>();
            backRenderer = background.GetComponent<SpriteRenderer>();
            imageRenderer.sprite = iconSprite;
            imageRenderer.color = Color.grey;
            backRenderer.color = Color.grey;
            stateOfIcon = IconState.unmatched;
            tickMark.gameObject.SetActive(false);
            effects.gameObject.SetActive(false);
        }

        public void ChangeState()
        {
            stateOfIcon = IconState.matched;
        }

        private void Update()
        {
            /*
            if(stateOfIcon == IconState.matched)
            {
                imageRenderer.color = Color.white;
                backRenderer.color = Color.white;
                tickMark.gameObject.SetActive(true);
                OnMatchTickAction();
                effects.gameObject.SetActive(true);
            }
            else
            {
                imageRenderer.color = Color.grey;
                backRenderer.color = Color.grey;
            }
            */
        }

        public void OnMatchIconAction()
        {
            if (stateOfIcon == IconState.matched)
            {
                imageRenderer.color = Color.white;
                backRenderer.color = Color.white;
                tickMark.gameObject.SetActive(true);
                OnMatchTickAction();
                effects.gameObject.SetActive(true);
            }
            else
            {
                imageRenderer.color = Color.grey;
                backRenderer.color = Color.grey;
            }
        }

        void OnMatchTickAction()
        {
            Vector3 punchPosition = new Vector3(tickMark.transform.position.x, tickMark.transform.position.y + 0.1f, tickMark.transform.position.z);
            tickMark.transform.DOPunchPosition(punchPosition, 0.2f);
        }
    }
}

