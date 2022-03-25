using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Samin.BlocksAndWords
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] List<GameObject> blocksList;
        [SerializeField] GameObject currentBlock;
        [SerializeField] GameObject previousBlock;
        [SerializeField] GameObject nextBlock;
        [SerializeField] Transform handCursor;
        [SerializeField] Transform iconParent;
        [SerializeField] Transform cursorPointer;
        [SerializeField] Transform nextButton;

        public List<string> iconWords;
        public List<IconMatch> icons;

        public int currentIndex;
        public int wordCounter;
        [SerializeField] int counter;
        [SerializeField] List<int> currentIndexList;

        public static TutorialManager tutorialSingleton;

        [SerializeField] Transform grid;
        public List<GameObject> gridChilds;
        [SerializeField] List<GameObject> currentMatched;

        [SerializeField] Text levelText;
        /*
         * it must contain the blocks that to be matched
         * the blocks must be in ordered list
         * there will be a counter in update
         * the counter records the block index from the block list
         * the block is the target destination for the handIcon
         * if the handicon has not been reached, it will move towards the target
         * when it reaches the target, movement will be stopped and the counter will be updated
         * for new counter, we get e new target
         */
        [SerializeField] Ease moveEase = Ease.Linear;
        void Start()
        {
            tutorialSingleton = this;
            
            iconWords = new List<string>();
            SetIconsToGameManager();
            StoreIconWords();
            grid = GridManager.gridSingleton.gameObject.transform;
            GridManager.gridSingleton.MainMethodTutorial();
            StoreGridBlocks();

            blocksList = GridManager.gridSingleton.GetBlocksOfIconMatch();
            currentIndex = 0;
            wordCounter = 0;
            counter = 0;
            currentIndexList = new List<int>();
            currentMatched = new List<GameObject>();
            InitialTask();
            levelText.text = "Level " + 1;
            //currentBlock = blocksList[currentIndex];
        }

        public void SetIconsToGameManager()
        {
            icons = new List<IconMatch>();
            for (int i = 0; i < iconParent.childCount; i++)
            {
                icons.Add(iconParent.GetChild(i).GetComponent<IconMatch>());
            }
        }

        public void StoreIconWords()
        {
            if (icons.Count > 0)
            {
                for (int i = 0; i < icons.Count; i++)
                {
                    iconWords.Add(icons[i].GetWord());
                    //Debug.Log(icons[i].GetWord());
                }
                //GameManager.GameManagerSingleton.readyToBuildGrid = true;
            }
            else
            {
                throw new System.Exception("Set the icon and their word in the scene");
            }

        }

        void StoreGridBlocks()
        {
            for (int i = 0; i < grid.childCount; i++)
            {
                gridChilds.Add(grid.GetChild(i).gameObject);
            }
        }

        [SerializeField] Vector3 TouchPosition;

        void InitialTask()
        {
            currentIndexList.Add(1);
            currentIndexList.Add(6);
            currentIndexList.Add(5);
            currentIndexList.Add(12);
        }

       [SerializeField] bool swapped = true;
       [SerializeField] bool matched = true;
        public bool endLevel = false;
        private void Update()
        {
            
            if(wordCounter < iconWords.Count && counter < 4)
            {
                matched = true;
                swapped = true;

                currentIndex = currentIndexList[counter];
                nextBlock = gridChilds[currentIndex];
                cursorPointer = gridChilds[currentIndex].transform.Find("cursorPointer");
                if (Vector3.Distance(handCursor.transform.position, cursorPointer.position) > 0.47)
                {
                    MoveHandToDestination(cursorPointer);
                }
                else
                {
                    reachedAtDestination = true;
                }
                if (reachedAtDestination)
                {
                    SelectTargetBlock(currentIndex);  // targetblockselected true
                }
                
                if (targetBlockSelected)
                {
                    reachedAtDestination = false;
                    targetBlockSelected = false;
                    swapped = false;
                    counter++;
                }

                if (previousBlock != null && previousBlock != currentBlock && !swapped)
                {
                    if (CheckIfBothBlockSelected(previousBlock, currentBlock))
                    {
                        BlockController pB = previousBlock.GetComponent<BlockController>();
                        BlockController cB = currentBlock.GetComponent<BlockController>();
                        StartCoroutine(Swapping(pB, cB));
                        matched = false;
                    }
                }

                if (swapped && previousBlock != null && !matched)
                {
                    matched = true;
                    AddCurrentMatch();
                    IconMatch iconScript = icons[wordCounter];
                    iconScript.ChangeState();
                    iconScript.OnMatchIconAction();

                    for (int i = 0; i < currentMatched.Count; i++)
                    {
                        BlockController currentBC = currentMatched[i].GetComponent<BlockController>();
                        if (currentBC.state != BlockController.BlockState.matched)
                        {
                            StartCoroutine(BlockMatchCOR(currentBC));
                        }
                    }
                    currentMatched.Clear();
                    wordCounter++;
                }
            }
            else
            {
                endLevel = true;
                if (Vector3.Distance(handCursor.transform.position, nextButton.position) > 0.67)
                {
                    MoveHandToDestination(nextButton);
                }
                else
                {
                    reachedAtDestination = true;
                }
            }
        }

        IEnumerator BlockMatchCOR(BlockController currentBC)
        {
            yield return new WaitForSeconds(0.3f);
            currentBC.SetMatchedState();
            currentBC.UpdateBlockCondition();
            currentBC.OnMatchAction();
        }

        void AddCurrentMatch()
        {
            if(wordCounter == 0)
            {
                currentMatched.Add(gridChilds[0]);
                currentMatched.Add(gridChilds[1]);
                currentMatched.Add(gridChilds[2]);
            }
            else if(wordCounter == 1)
            {
                currentMatched.Add(gridChilds[10]);
                currentMatched.Add(gridChilds[11]);
                currentMatched.Add(gridChilds[12]);
            }
        }

        [SerializeField] bool targetBlockSelected = false;
        void SelectTargetBlock(int index)
        {
            if (Input.GetMouseButtonDown(0))
            {
                TouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                TouchPosition.z = gridChilds[index].transform.position.z;
                if (Vector3.Distance(gridChilds[index].transform.position, TouchPosition) < 0.2)
                {
                    gridChilds[index].GetComponent<BlockController>().ChangeState();
                    if(currentBlock != null)
                    {
                        previousBlock = currentBlock;
                    }
                    
                    currentBlock = gridChilds[index];
                    currentBlock.GetComponent<BlockController>().UpdateBlockCondition();
                    targetBlockSelected = true;
                }
            }
        }

        bool CheckIfBothBlockSelected(GameObject p, GameObject c)
        {
            BlockController previousBlock = p.GetComponent<BlockController>();
            BlockController currentBlock = c.GetComponent<BlockController>();
            if (previousBlock.CheckState() == BlockController.BlockState.selected && currentBlock.CheckState() == BlockController.BlockState.selected)
            {
                previousBlock.clicked = false;
                currentBlock.clicked = false;
                //Debug.Log(previousBlock.ToString() + currentBlock.ToString() + "both are selected");
                return true;
            }

            return false;
        }


        public bool reachedAtDestination = false;
        void MoveHandToDestination(Transform destinationBlock)
        {
            Vector3 direction = destinationBlock.position - handCursor.transform.position;
            handCursor.gameObject.transform.Translate(direction.x * Time.deltaTime * 5, direction.y * Time.deltaTime * 5, 0f);
        }

        IEnumerator Swapping(BlockController previousBlock, BlockController currentBlock)
        {
            if (!swapped)
            {
                swapped = true;
                //ShuffleManager.UpliftBlocksBeforeShuffle(previousBlock, currentBlock);
                ShuffleManager.ShuffleForTutorial(previousBlock, currentBlock);
                //ShuffleManager.LowerAfterShuffle(previousBlock, currentBlock);
                StartCoroutine(ShuffleManager.UnselectAfterShuffle(previousBlock, currentBlock));

                while (previousBlock.state != BlockController.BlockState.notSelected && currentBlock.state != BlockController.BlockState.notSelected)
                {
                    yield return null;
                }

                previousBlock.UpdateBlockCondition();
                currentBlock.UpdateBlockCondition();
            }
        }

    }
}

