using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using DG.Tweening;
using UnityEngine.SceneManagement;

/*
 * the game manager records the icons and their respective strings
 * game manager has a string-builder
 * it checks if any touch or tap on a block
 * if any block is tapped ---->
 *     change its state and image from block controller
 *     return the character it it is selected
 *     (store the character in the string builder and update it) -----> canceled
 * there could be such situation when a match is created but all the letters in the match is not in selected state
 *     the game manager has to check if there is any sequential match in the blocks in a row
 *     game manager iteartes from first block to the last block
 *     if any block have the same first letter to any of the match, start to check the next sequential letters of icon words
 *     if multiple icon words has same starting letter, check all of them one adter another
 * it continuously check if the string builder matches with any of the strings
 * if matches, underline the blocks(do something)
 */


namespace Samin.BlocksAndWords
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager GameManagerSingleton;

        public GameManager(int rows, int cols, int emptyBlocks, int level)
        {
            GridManager.gridSingleton.gameObject.SetActive(true);
            GridManager.gridSingleton.SetGridDimention(rows, cols, emptyBoxes);
            this.level = level;
        }

        [SerializeField] Transform iconParent;
        [SerializeField] List<IconMatch> icons;
        public List<string> iconWords;
        public List<string> unmatchedIconWords;
        public List<string> matchedIconWords;
        [SerializeField] List<char> firstLetterList;

        [SerializeField] BlockController currentBlock;
        [SerializeField] BlockController previousBlock;
        [SerializeField] List<BlockController> activeBlocks;

        [SerializeField] Transform gridHolder;
        [SerializeField] List<GameObject> gridHolderChilds;
        public List<GameObject> matchedBlocks;
        public List<GameObject> unmatchedBlocks;
        public List<GameObject> currentMatch;

        public List<char> gridLetterCache;
        public List<char> unmatchedGridLetterCache;

        [SerializeField] int emptyBoxes;
        [SerializeField] List<int> emptyBoxIndices = new List<int>();

        public bool readyToCheckResult;

        [SerializeField] Text levelText;
        public int level;
        public float scalingDuration;
        public float descalingDuration;
        public float blockScaling;

        public bool endLevel = false;

        [SerializeField] Transform oneEffect;
        [SerializeField] Transform twoEffect;

        [SerializeField] Ease moveEase = Ease.Linear;
        void Start()
        {
            oneEffect.gameObject.SetActive(false);
            twoEffect.gameObject.SetActive(false);
            endLevel = false;


            level = LevelManager.level;

            //levelText.text = "Level " + level;
            levelText.text = "Level " + PlayerPrefs.GetInt("level").ToString();
            currentString = new StringBuilder();
            GameManagerSingleton = this;
            iconWords = new List<string>();
            unmatchedIconWords = new List<string>();
            matchedIconWords = new List<string>(); 
            SetIconsToGameManager();
            StoreIconWords();  //readyToBuildGrid
            gridLetterCache = new List<char>();
            unmatchedGridLetterCache = new List<char>();

            GridManager.gridSingleton.MainMethod();


            matchedBlocks = new List<GameObject>();
            unmatchedBlocks = new List<GameObject>();
            currentMatch = new List<GameObject>();

            emptyBoxes = GridManager.gridSingleton.emptyBlocks;

            firstLetterList = new List<char>();
            firstLetterList = StoreTheBlocksAndCharacter();
            gridHolderChilds = new List<GameObject>();
            UpdateGridHolderChildSequence();
            //UpdateGridLetters();
            UpdateGridLettersUnmatched();

            ShuffleManager.Initializer(gridHolderChilds[0]);

            if (CheckForMatchWithIcons())
            {
                // do something when the current formated word matches with the icon image word
               // Debug.Log("new match found");
            }
        }

        public void SetIconsToGameManager()
        {
            icons = new List<IconMatch>();
            for (int i = 0; i < iconParent.childCount; i++)
            {
                if (iconParent.GetChild(i).gameObject.activeInHierarchy)
                {
                    icons.Add(iconParent.GetChild(i).GetComponent<IconMatch>());
                }
            }
        }

        public void StoreIconWords()
        {
            if(icons.Count > 0)
            {
                for (int i = 0; i < icons.Count; i++)
                {
                    iconWords.Add(icons[i].GetWord());
                    unmatchedIconWords.Add(icons[i].GetWord());
                }
                readyToBuildGrid = true;
            }
            else
            {
                throw new System.Exception("Set the icon and their word in the scene");
            }
            
        }

        public bool readyToBuildGrid;

        [SerializeField] string currentCharacter;
        public StringBuilder currentString;

        bool CheckForMatchWithIconsOld()
        {
            for(int i =0; i<iconWords.Count; i++)
            {
                if (currentString.ToString().Equals(iconWords[i]))
                {
                    return true;
                }
            }
            return false;
        }

        List<char> StoreTheBlocksAndCharacter()
        {
            if (GridManager.gridSingleton.gridLetterList.Count > 0)
            {
                //gridLetterCache = GridManager.gridSingleton.gridLetterList;
                List<GameObject> gridBlocks = new List<GameObject>();
                for (int i = 0; i < gridHolder.childCount; i++)
                {
                    gridBlocks.Add(gridHolder.GetChild(i).gameObject);
                }

                unmatchedBlocks = gridBlocks;

                for (int i = 0; i < gridBlocks.Count; i++)
                {
                    BlockController currentBlockContrl = gridBlocks[i].GetComponent<BlockController>();
                    if (char.IsLetter(currentBlockContrl.adapter.dataContainer.GetLetter()))
                    {
                        gridLetterCache.Add(currentBlockContrl.adapter.dataContainer.GetLetter());
                    }
                    else
                    {
                        emptyBoxIndices.Add(i);
                    }
                }

                //unmatchedGridLetterCache = gridLetterCache; // unmatchedgridletter is set from unmatchedblocks

                //gridLetterCache = GridManager.gridSingleton.SetGridLetterBackFromBlocks(gridBlocks);
                List<char> firstLetterList = new List<char>();
                for (int i = 0; i < iconWords.Count; i++)
                {
                    firstLetterList.Add(iconWords[i][0]);
                }
                return firstLetterList;
            }
            else
            {
                throw new System.Exception(" gridList has no letter. Set the list in gridManager at first");
            }
        }

        void UpdateGridLetters()   //controls gridLettercache
        {
            gridLetterCache.Clear();
           // Debug.Log("grid holder has " + gridHolder.childCount + " children");
            for (int i = 0; i < gridHolder.childCount; i++)
            {
                BlockController currentBlockContrl = gridHolder.GetChild(i).gameObject.GetComponent<BlockController>();  // should be taken grom gridHolderChild
                if (char.IsLetter(currentBlockContrl.adapter.dataContainer.GetLetter()))
                {
                    gridLetterCache.Add(currentBlockContrl.adapter.dataContainer.GetLetter());
                }
    
            }
        }

        void UpdateGridLettersUnmatched()  //controls unmatched grid letter, collect from unmatched blocks, contains the empty one as well, synchronous with unmatched blocks list
        {
            unmatchedGridLetterCache.Clear();
            for (int i = 0; i < unmatchedBlocks.Count; i++)
            {
                BlockController currentBlockContrl = unmatchedBlocks[i].gameObject.GetComponent<BlockController>();  // should be taken grom gridHolderChild
                if (char.IsLetter(currentBlockContrl.adapter.dataContainer.GetLetter()))
                {
                    unmatchedGridLetterCache.Add(currentBlockContrl.adapter.dataContainer.GetLetter());
                }
                else
                {
                    unmatchedGridLetterCache.Add( ' ' );
                }
            }
        }

        void UpdateGridHolderChildSequence()
        {
            gridHolderChilds.Clear();
            for (int i = 0; i < gridHolder.childCount; i++)
            {
                gridHolderChilds.Add(gridHolder.GetChild(i).gameObject);
            }
        }

        void UpdateEmptyBlocks()
        {
            emptyBoxIndices.Clear();
            for (int i = 0; i < unmatchedBlocks.Count; i++)
            {
                BlockController currentBlockContrl = unmatchedBlocks[i].GetComponent<BlockController>();
                if (!char.IsLetter(currentBlockContrl.adapter.dataContainer.GetLetter()))
                {
                    emptyBoxIndices.Add(i);
                }
            }
        }



        /*
         * take a list of first letters of all icon words
         * start checking all the grid blocks
         * start iteration
         *   if any block matches with any of the first letter
         *      check the whole word matches with the subsequent letter by taking the substring of a particular length
         *         if matches return true
         *   otherwise return false
         * continue iteration
         */

        
        bool CheckForMatchWithIcons()
        {
            if(GridManager.gridSingleton.gridLetterList.Count > 0)
            {
                //Debug.Log("start checking");
                for (int j = 0; j < unmatchedGridLetterCache.Count; j++)
                {
                    for (int k = 0; k < firstLetterList.Count; k++)
                    {
                        if (firstLetterList[k].Equals(unmatchedGridLetterCache[j]))
                        {
                            //Debug.Log("searching for the letter " + firstLetterList[k].ToString() + unmatchedGridLetterCache[j]);
                            // check if the subsequent letter matches the iconword[j]
                            string nextString = BuildStringFromCharacter(unmatchedIconWords[k].Length, j, unmatchedGridLetterCache);
                           // Debug.Log("string to be checked " + nextString + " for " + unmatchedIconWords[k]);
                            if (unmatchedIconWords[k].Equals(nextString))
                            {
                                if (readyToCheckResult & ShuffleManager.repos)  //repos is necessary
                                {
                                    List<GameObject> tempCheck = GetTheMatchedBlocks2(unmatchedIconWords[k].Length, j, nextString, unmatchedGridLetterCache[j]);
                                    for (int p = 1; p < tempCheck.Count; p++)
                                    {
                                        if ((tempCheck[p - 1].transform.position.y - tempCheck[p].transform.position.y) > 0.01f)
                                        {
                                           // Debug.Log(nextString + " they are not on the same row");
                                            //Debug.Log(tempCheck[p - 1].ToString() + tempCheck[p].ToString());
                                            //Debug.Log(tempCheck[p - 1].transform.position.y - tempCheck[p].transform.position.y);
                                            return false;
                                        }
                                    }

                                    for(int a = 1; a < tempCheck.Count; a++)
                                    {
                                        if(!char.IsLetter(tempCheck[a].GetComponent<BlockController>().adapter.dataContainer.GetLetter()))
                                        {
                                            //Debug.Log("there is empty block in between match");
                                            return false;
                                        }
                                    }

                                    IconMatch iconScript = GetIconScriptFromWord(unmatchedIconWords[k]);
                                    iconScript.ChangeState();
                                    iconScript.OnMatchIconAction();

                                    // start index of unmatched letter list and block list can not be same, as letter list has not empty item
                                    currentMatch.Clear();
                                    currentMatch.AddRange(GetTheMatchedBlocks2(unmatchedIconWords[k].Length, j, nextString, unmatchedGridLetterCache[j]));
                                    matchedBlocks.AddRange(currentMatch);

                                    StartCoroutine(MatchedBlockCOR(currentMatch));

                                    matchedIconWords.Add(unmatchedIconWords[k]);
                                    unmatchedIconWords.Remove(unmatchedIconWords[k]);  //when one item removed, subsequent item's index are changed
                                    firstLetterList.Remove(firstLetterList[k]);
                                    // unmatched blocks are changed, matched box can have error
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                                
                            }
                        }
                    }
                }
                return false;
            }
            else
            {
                throw new System.Exception(" gridList has no letter. Set the list in gridManager at first");
            }

        }

        IEnumerator MatchedBlockCOR(List<GameObject> matchedBlocks)
        {
            for (int i = 0; i < matchedBlocks.Count; i++)
            {
                //matchedBlocks[i].GetComponent<BlockController>().UpdateBlockCondition();
                matchedBlocks[i].GetComponent<BlockController>().OnMatchAction();
                yield return null;
            }
        }

        string BuildStringFromCharacter(int length, int startIndex, List<char> charCache)
        {
            StringBuilder stringB = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                if(startIndex + i < charCache.Count)
                {
                    //if(i == 0) {  Debug.Log(charCache[startIndex + i].ToString()) ; }
                    
                    if(char.IsLetter(charCache[startIndex + i]))
                    {
                        stringB.Append(charCache[startIndex + i]);  //unmatchedgridletter cache
                    }
                    else
                    {
                        //stringB.Append(charCache[startIndex + i]);
                    }
                    
                    //stringB.Append(charCache[startIndex + i]);
                }
                
            }
            //Debug.Log(stringB.ToString());
            return stringB.ToString();
        }

        List<GameObject> GetTheMatchedBlocks(int length, int startIndex, string word, char firstLetter)
        {
            List<GameObject> listB = new List<GameObject>();
            int emptyBoxCounter = 0;
            Debug.Log("start index " + startIndex);
            for (int i = 0; i < emptyBoxIndices.Count; i++)
            {
                if(emptyBoxIndices[i] <= startIndex + emptyBoxes )
                {
                    emptyBoxCounter++;
                }
            }
            
            //Debug.Log("number of empty boxes " + emptyBoxCounter + " for " + word + " starting with " + firstLetter);
            //Debug.Log("the first blocks to load " + unmatchedBlocks[startIndex + emptyBoxCounter]);

            for (int i = 0; i < length; i++)
            {
                if (startIndex + emptyBoxCounter + i < unmatchedBlocks.Count)
                {
                    /*
                    if(!char.IsLetter(unmatchedBlocks[startIndex + emptyBoxCounter + i].GetComponent<BlockController>().adapter.dataContainer.GetLetter()))
                    {
                        listB.Add(unmatchedBlocks[startIndex + emptyBoxCounter + i]);
                    }
                    else
                    {
                        listB.Add(unmatchedBlocks[startIndex + emptyBoxCounter + i + 1]);
                    }
                    */
                    listB.Add(unmatchedBlocks[startIndex + emptyBoxCounter + i]);   // get matchedbox from unmatched box
                }
            }
            return listB;
        }

        List<GameObject> GetTheMatchedBlocks2(int length, int startIndex, string word, char firstLetter)
        {
            List<GameObject> listB = new List<GameObject>();
            for (int i = 0; i < length; i++)
            {
                if (startIndex + i < unmatchedBlocks.Count)
                {
                    listB.Add(unmatchedBlocks[startIndex + i]);
                }
            }
            return listB;
        }

        IconMatch GetIconScriptFromWord(string word)
        {
            for (int i = 0; i < iconParent.childCount; i++)
            {
                IconMatch iconScript = iconParent.GetChild(i).GetComponent<IconMatch>();
                if (word.Equals(iconScript.GetWord()))
                {
                    return iconScript;
                }
            }
            return null;
        }

        IEnumerator ChangeSelectedBlockScale(GameObject hitBlock)
        {
            BlockController hitBlockController = hitBlock.GetComponent<BlockController>();
            Vector3 oldScale = hitBlock.transform.localScale;
            hitBlock.transform.DOScale(oldScale * blockScaling, scalingDuration);
            yield return new WaitForSeconds(scalingDuration);
            hitBlock.transform.DOScale(oldScale, descalingDuration);
            yield return new WaitForSeconds(descalingDuration);
            if(hitBlock.transform.localScale == oldScale)
            {
                hitBlockController.clicked = true;
            }
            else
            {
                hitBlock.transform.localScale = oldScale;
                hitBlockController.clicked = true;
            }
        }

        void ChangeToOriginalScale(BlockController previousBlock)
        {
            previousBlock.transform.localScale = previousBlock.oldScale;
        }

        #region BothBlockAnimation
        /*
        bool CheckIfScaleReset(BlockController previousBlock, BlockController currentBlock)
        {
            if(previousBlock.oldScale == previousBlock.transform.localScale && currentBlock.oldScale == currentBlock.transform.localScale)
            {
                Debug.Log("scale for both is reset");
                return true;
            }
            return false;
        }

        public bool CheckIfBothSelected(BlockController previousBlock, BlockController currentBlock)
        {
            if (CheckIfScaleReset(previousBlock, currentBlock))
            {
                if (previousBlock.CheckState() == BlockController.BlockState.selected && currentBlock.CheckState() == BlockController.BlockState.selected)
                {
                    if (previousBlock.clicked && currentBlock.clicked)
                    {
                        previousBlock.clicked = false;
                        currentBlock.clicked = false;
                        Debug.Log(previousBlock.ToString() + currentBlock.ToString() + "both are selected");

                        return true;
                    }
                    else
                    {
                        Debug.Log("both are not selected");
                        Debug.Log(previousBlock.clicked.ToString() + currentBlock.clicked.ToString());
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
            
        }
        */
        #endregion

        bool CheckIfBothSelected2(BlockController previousBlock, BlockController currentBlock)
        {
            if (previousBlock.CheckState() == BlockController.BlockState.selected && currentBlock.CheckState() == BlockController.BlockState.selected)
            {
                previousBlock.clicked = false;
                currentBlock.clicked = false;
                //Debug.Log(previousBlock.ToString() + currentBlock.ToString() + "both are selected");
                return true;
            }
            else
            {
                //.Log("both are not selected");
            }
            return false;
        }

        [SerializeField] bool readyToSwap;

        IEnumerator SwappingOperation()  // need some yield function with this
        {
            ShuffleManager.UpliftBlocksBeforeShuffle(previousBlock, currentBlock);
            
            ShuffleManager.Shuffle(previousBlock, currentBlock);
            
            ShuffleManager.LowerAfterShuffle(previousBlock, currentBlock);
            StartCoroutine(ShuffleManager.UnselectAfterShuffle(previousBlock, currentBlock));  //repos true

            while(previousBlock.state != BlockController.BlockState.notSelected && currentBlock.state != BlockController.BlockState.notSelected)
            {
                yield return null;
            }

            previousBlock.UpdateBlockCondition();
            currentBlock.UpdateBlockCondition();
            UpdateGridHolderChildSequence();
            UpdateEmptyBlocks();
            UpdateGridLettersUnmatched();

            readyToSwap = false;
            readyToCheckResult = true;
        }
        void Update()
        {
            if (GameManager.GameManagerSingleton.unmatchedIconWords.Count > 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    // keep the main camera always in orthographic(box) projection
                    RaycastHit2D hit;
                    hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.zero);
                   // Debug.DrawLine(transform.position, hit.point, Color.red, 1f);

                    if (hit)
                    {
                        GameObject hitBlock = hit.transform.gameObject;
                        if (unmatchedBlocks.Contains(hitBlock))
                        {
                            BlockController hitBlockController = hitBlock.GetComponent<BlockController>();
                            if (hitBlockController != null)
                            {
                                if(currentBlock != null)
                                {
                                    previousBlock = currentBlock;
                                    ChangeToOriginalScale(previousBlock);
                                }
                                
                                currentBlock = hitBlockController;
                                hitBlockController.ChangeState();
                                //hitBlockController.CheckCurrentImage();
                                hitBlockController.UpdateBlockCondition();
                                StartCoroutine(ChangeSelectedBlockScale(hitBlock));

                                if (previousBlock != null)
                                {
                                    readyToSwap = true;
                                    #region FirstBlockAnimation
                                    if (CheckIfBothSelected2(previousBlock, currentBlock))
                                    {
                                        readyToCheckResult = false;
                                        ShuffleManager.repos = false;
                                        StartCoroutine(SwappingOperation());  //checkresult true, swap false, repose true
                                    }
                                    #endregion
                                }

                                //previousBlock = currentBlock;
                            }
                            else
                            {
                                //Debug.LogError("The block under touch has not BlockController Script. Attach the script to it");
                            }
                        }
                        else
                        {
                           // Debug.Log("this is not an unmatchedBlock");
                        }
                    }
                    else
                    {
                       // Debug.Log("no collider at the touch position");
                    }
                }
                else
                {
                    //Debug.Log("no blocks are touched");
                }
                #region BothBlockAnimation
                /*
                if (previousBlock != null & currentBlock != null && readyToSwap)
                {

                    if (CheckIfBothSelected(previousBlock, currentBlock))
                    {
                        readyToCheckResult = false;
                        ShuffleManager.repos = false;
                        SwappingOperation();  //checkresult true, swap false
                    }
                    if (!readyToSwap)
                    {
                        //previousBlock = currentBlock;
                        //currentBlock = null;
                    }

                }
                */
                #endregion
                if (CheckForMatchWithIcons())
                {
                    // do something when the current formated word matches with the icon image word
                    for (int i = 0; i < matchedBlocks.Count; i++)
                    {
                        BlockController currentBC = matchedBlocks[i].GetComponent<BlockController>();
                        if (currentBC.state != BlockController.BlockState.matched)
                        {
                            currentBC.SetMatchedState();
                            currentBC.UpdateBlockCondition();
                        }
                    }
                    //Debug.Log("new match found");
                    unmatchedBlocks.RemoveAll(blocks => blocks.GetComponent<BlockController>().state == BlockController.BlockState.matched);
                    //UpdateGridLetters();
                    UpdateEmptyBlocks();
                    UpdateGridLettersUnmatched();
                }
            }
            else
            {
                //level++;
                StartCoroutine(OnLevelEndAction());
            }

            if (endLevel)
            {
                nextButton.gameObject.SetActive(true);
            }
            else
            {
                nextButton.gameObject.SetActive(false);
            }

        }

        IEnumerator OnLevelEndAction()
        {
            yield return new WaitForSeconds(0.5f);
            oneEffect.gameObject.SetActive(true);
            twoEffect.gameObject.SetActive(true);
            endLevel = true;
        }

        public void PlayAgain()
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentScene);
        }

        [SerializeField] Button nextButton;

        /*
        public void NextLevel()
        {
            GameManagerSingleton.level++;
            //Debug.Log(level);
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            //Debug.Log(currentScene);
            SceneManager.LoadScene(currentScene);
        }
        */

    }
}

