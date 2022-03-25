using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/*
 * 
 * the box object contains 2 image, selected one and unselected one
 * the current image will store the current one
 * the image is associated with 2 enum state, selected and not selected
 * all information is contained in block adapter
 * when a box is tapped, it's state and image is changed
 * image is chaned in update(?)
 * where to change the image?  ----> block controller
 * where to change the state?  ----> block controller
 * the box must retun its containing letter
 * returning letter method is in blockdata
 */

namespace Samin.BlocksAndWords
{
    public class BlockController : MonoBehaviour
    {
        [SerializeField] Sprite selectImage;
        [SerializeField] Sprite letterImage;
        [SerializeField] Sprite matchedImage;

        [SerializeField] Color matchedColor;
        [SerializeField] Color regularColor;
        [SerializeField] Color originalColor;
        [SerializeField] Transform lineThrough;

        [SerializeField] Image currentImage;
        [SerializeField] SpriteRenderer imageRenderer;
        public BlockAdapter adapter;
        public Vector3 oldScale;

        [SerializeField] float matchScale;
        [SerializeField] float matchTimer;
        public float shakeDuration;
        public float strength;
        public int vibration;
        public float randomness;
        public float elasticity;

        public enum BlockState
        {
            selected,
            notSelected,
            matched
        }

        public enum MatchState
        {
            shake,
            enlarge,
            all
        }

        public BlockState state;
        public MatchState mState;
        public bool clicked;

        void Awake()
        {
            adapter = GetComponent<BlockAdapter>();
            currentImage = GetComponent<Image>();
            imageRenderer = GetComponent<SpriteRenderer>();
            state = BlockState.notSelected;
            //matchedColor = Color.white;
            lineThrough.gameObject.SetActive(false);
            oldScale = transform.localScale;
            //CheckCurrentImage();
            if(GridManager.gridSingleton.remainder == 0)
            {
                BuildImageFirstTime(GridManager.gridSingleton.remainder);
            }
        }



        /*
         * the state is changed when the card is touched
         * the previous state must be recalled
         */
        public void ChangeState()
        {
            if(state == BlockState.selected)
            {
                state = BlockState.notSelected;
            }
            else if(state == BlockState.notSelected)
            {
                state = BlockState.selected;
            }

        }

        public void SetMatchedState()
        {
            state = BlockState.matched;
        }

        public BlockState CheckState()
        {
            return this.state;
        }

        public void CheckCurrentImage()
        {
            if (state == BlockState.notSelected)
            {
                currentImage.sprite = letterImage;
                imageRenderer.color = originalColor;
                /*
                if(GridManager.gridSingleton.remainder == 2)  //level1
                {
                    //imageRenderer.color = Color.yellow;
                    SetColor("#E9D767");
                }
                else if(GridManager.gridSingleton.remainder == 0) //level2
                {
                    //imageRenderer.color = Color.green;
                    SetColor("#1BE11B");
                }
                else if(GridManager.gridSingleton.remainder == 1)
                {
                    //imageRenderer.color = Color.blue;
                    SetColor("#3EC8FF");
                }
                */
            }
            else if (state == BlockState.selected)
            {
                currentImage.sprite = selectImage;
                imageRenderer.color = Color.white;
            }
            else if(state == BlockState.matched)
            {
                currentImage.sprite = letterImage;
                StartCoroutine(DoPunchColorOnMatch(0.2f));
                //imageRenderer.color = matchedColor;  //change color of sprite renderer, not the sprite color
                lineThrough.gameObject.SetActive(true);
            }
        }

        public void BuildImageFirstTime(int remainder)
        {
            currentImage.sprite = letterImage;
            if (remainder == 2)  //level1
            {
                //imageRenderer.color = Color.yellow;
                SetColor("#E9D767");
            }
            else if (remainder == 0) //level2
            {
                //imageRenderer.color = Color.green;
                SetColor("#1BE11B");
            }
            else if (remainder == 1)
            {
                //imageRenderer.color = Color.blue;
                SetColor("#3EC8FF");
            }

        }

        void SetColor(string hexa)
        {
            Color newColor;
            ColorUtility.TryParseHtmlString(hexa, out newColor);
            imageRenderer.color = newColor;
            regularColor = newColor;
            originalColor = newColor;
        }

        [SerializeField] public TextMeshProUGUI textAlphabet;
        [SerializeField] public Text textAlphabetLow;

        public void SetBlockText()
        {
            //textAlphabet = GetComponent<TextMeshProUGUI>();
            string letter = adapter.dataContainer.Character.ToString();
            textAlphabet.SetText(letter);
            //textAlphabetLow.text = letter;
        }

        [SerializeField] ParticleSystem onMatchEffect;

        public void OnMatchAction()
        {
            if(onMatchEffect != null)
            {
                onMatchEffect.Play();
            }
            if(mState == MatchState.enlarge)
            {
                MatchEnlarge();
            }
            else if(mState == MatchState.shake)
            {
                MatchShake();
            }
            else
            {
                MatchEnlarge();
                MatchShake();
            }
            
        }

        IEnumerator DoPunchColorOnMatch(float duration)
        {
            imageRenderer.DOColor(Color.white, duration);
            while(imageRenderer.color != Color.white)
            {
                yield return null;
            }
            imageRenderer.DOColor(matchedColor, duration);
            while(imageRenderer.color != matchedColor)
            {
                yield return null;
            }
        }

        void MatchEnlarge()
        {
            transform.DOPunchScale(oldScale * matchScale, matchTimer, vibrato: vibration, elasticity: elasticity);
        }

        void MatchShake()
        {
            transform.DOShakePosition(shakeDuration, strength, vibration, randomness, false, true);
        }

        public void UpdateBlockCondition()
        {
            CheckCurrentImage();
            imageRenderer.sprite = currentImage.sprite;
        }

        void Update()
        {
            //UpdateBlockCondition();
        }
    }
}

