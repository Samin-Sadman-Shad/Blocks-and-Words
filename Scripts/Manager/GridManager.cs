using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Samin.BlocksAndWords
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] int rows ;
        public int cols ;
        [SerializeField] float tileSize = 1;
        public int emptyBlocks;
        [SerializeField] float offset;

        [SerializeField] GameObject referenceBlock;
        public List<GameObject> referenceBlockList;

        public static GridManager gridSingleton;
        public int remainder;

        public void SetGridDimention(int rows, int cols, int emptyBoxes)
        {
            gridSingleton.rows = rows;
            gridSingleton.cols = cols;
            gridSingleton.emptyBlocks = emptyBoxes;
        }

        [SerializeField] List<char> allLetterList;
        public List<char> gridLetterList;
        [SerializeField] List<string> wordList;

        void Awake()
        {
            gridSingleton = this;
        }

        public void MainMethod()
        {
            allLetterList = new List<char>();
            allLetterList = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().ToList();
            referenceBlockList = new List<GameObject>();
            /*
            if (GameManager.GameManagerSingleton.readyToBuildGrid)
            {
                SetWordsToList();
                StoreLetterToList();
                GenerateGrid();
            }
            */
            SetWordsToList();
            StoreLetterToList();
            GenerateGrid();
            gridSingleton.remainder = BootControl.mainLevel % 3;
            Debug.Log("level no is " + BootControl.mainLevel);
            //Debug.Log("remainder is " + remainder);
            SetFirstColorOfBlock(gridSingleton.remainder);
        }

        public void MainMethodTutorial()
        {
            allLetterList = new List<char>();
            allLetterList = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().ToList();
            referenceBlockList = new List<GameObject>();
            /*
            if (GameManager.GameManagerSingleton.readyToBuildGrid)
            {
                SetWordsToList();
                StoreLetterToList();
                GenerateGrid();
            }
            */
            SetWordsToListTutorial();
            StoreLetterToList();
            charFromWords = new List<char>();
            GetCharFromWords();
            GenerateGridForTutorial();
            UpdateGridForTorial();
        }

        void SetFirstColorOfBlock(int remainder)
        {
            if(referenceBlockList != null && referenceBlockList.Count > 0)
            {
                for (int i = 0; i < referenceBlockList.Count; i++)
                {
                    var block = referenceBlockList[i].transform.GetComponent<BlockController>();
                    block.BuildImageFirstTime(gridSingleton.remainder);
                }
            }
        }

        int GetGridSize()
        {
            return rows * cols;
        }

        char[] BreakOneWord(string word)
        {
            word = word.ToUpper();
            return word.ToCharArray();
        }

        void SetWordsToList()
        {
            if(GameManager.GameManagerSingleton.iconWords.Count != 0)
            {
                List<string> tempWordList = GameManager.GameManagerSingleton.iconWords;
                wordList = new List<string>();
                for (int i = 0; i < tempWordList.Count; i++)
                {
                    wordList.Add(tempWordList[i]);
                }
            }
            else
            {
                throw new System.Exception("no iconMatch is set to GameManager");
            }
            
        }

        void SetWordsToListTutorial()
        {
            if (TutorialManager.tutorialSingleton.iconWords.Count != 0)
            {
                List<string> tempWordList = TutorialManager.tutorialSingleton.iconWords;
                wordList = new List<string>();
                for (int i = 0; i < tempWordList.Count; i++)
                {
                    wordList.Add(tempWordList[i]);
                }
            }
            else
            {
                throw new System.Exception("no iconMatch is set to GameManager");
            }
        }

        void StoreLetterToList()   // got some problem
        {
            List<char> charFromWords = new List<char>();
            for (int i = 0; i < wordList.Count; i++)
            {
                charFromWords.AddRange(BreakOneWord(wordList[i]));
            }
            //Debug.Log("character count from icon words are " + charFromWords.Count);

            int gridSize = GetGridSize();
            int lettersRemaining = gridSize - charFromWords.Count - emptyBlocks;

            //Debug.Log("total grid size " + gridSize);
            //Debug.Log("remaining number of letter " + lettersRemaining);

            gridLetterList = new List<char>();
            gridLetterList.AddRange(charFromWords);
            int incrementor = 0;
            System.Random random = new System.Random();  // random object must be out of loop, other wise generate same value in every iteration
            while (incrementor < lettersRemaining)
            {
                int randomIndex = random.Next(allLetterList.Count);
                //Debug.Log("index got randomly " + randomIndex);
                gridLetterList.Add(allLetterList[randomIndex]);
                incrementor++;
            }

            //Debug.Log("letter in grid boxes " + gridLetterList.Count);
        }

        void Shuffle<T>(List<T> list)
        {
            System.Random random = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                int k = random.Next(n);
                n--;
                T temp = list[k];
                list[k] = list[n];
                list[n] = temp;
            }
        }

        void ShuffleBlockPositionWithBlocks(List<GameObject> list)
        {
            System.Random random = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                int k = random.Next(n);
                n--;
                GameObject tempObject = list[k];
                Transform tempChild = transform.GetChild(k);
                Vector3 tempPos = tempObject.transform.position;

                GameObject lastObject = list[n];
                Transform lastChild = transform.GetChild(n);
                Vector3 lastPos = lastObject.transform.position;

                lastObject.transform.position = tempPos;
                list[k] = lastObject;
                lastChild.SetSiblingIndex(k);

                tempObject.transform.position = lastPos;
                list[n] = tempObject;
                tempChild.SetSiblingIndex(n);
            }
        }

        void ShuffleBlocksWithGridList(List<char> list, List<GameObject> blocks)
        {
            System.Random random = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                int k = random.Next(n);
                n--;
                GameObject tempObject = blocks[k];
                Vector3 tempPos = tempObject.transform.position;
                GameObject lastObject = blocks[n];
                Vector3 lastPos = lastObject.transform.position;
                lastObject.transform.position = tempPos;
                tempObject.transform.position = lastPos;

                char temp = list[k];
                list[k] = list[n];
                list[n] = temp;
            }
        }

        public List<char> SetGridLetterBackFromBlocks(List<GameObject> list)
        {
            int p = 0;
            for (int i = 0; i < list.Count; i++)
            {
                GameObject gb = list[i];
                BlockController bc = gb.GetComponent<BlockController>();
                if(char.IsLetter( bc.adapter.dataContainer.GetLetter()))
                {
                    //Debug.Log(bc.adapter.dataContainer.GetLetter());
                    gridLetterList[p] = bc.adapter.dataContainer.GetLetter();
                    p++;
                }
                else
                {
                   //Debug.Log("this block is empty");
                   // p--;
                }
                // Debug.Log(p);
            }
            return gridLetterList;
        }

        int gridBlockIndex = 0;
        void GenerateGrid()
        {
            GameObject referenceTile = Instantiate(referenceBlock);
            referenceTile.SetActive(true);
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    //generate the tile from prefab folder (block)
                    GameObject tile = Instantiate(referenceTile, transform);
                    BlockController tileBlockController = tile.GetComponent<BlockController>();
                    if(tileBlockController != null)
                    {
                        if (gridBlockIndex < gridLetterList.Count)
                        {
                            //Debug.Log(gridLetterList[gridBlockIndex]);
                            //tileBlockController.adapter.dataContainer.Character = gridLetterList[gridBlockIndex];
                            tileBlockController.adapter.dataContainer.SetLetter(gridLetterList[gridBlockIndex]);
                            //tileBlockController.textAlphabetLow.text = " ";
                            tileBlockController.textAlphabet.text = " ";
                            tileBlockController.SetBlockText();
                            tile.name = tileBlockController.adapter.dataContainer.GetLetter().ToString();
                            if (!char.IsLetter(tileBlockController.adapter.dataContainer.GetLetter()))
                            {
                                tile.name = "empty";
                            }
                            gridBlockIndex++;
                        }
                        else
                        {
                            // for the blocks which will remain empty
                        }
                    }
                    else
                    {
                        Debug.LogWarning("This referenceTile " + tile + " has not BlockControllerScript");
                    }
                   
                    
                    float posX = col * tileSize;
                    float posY = row * -tileSize + transform.position.y;

                    tile.transform.position = new Vector3(posX, posY, transform.position.z);
                    referenceBlockList.Add(tile);
                }
            }
            Destroy(referenceTile);

            ShuffleBlockPositionWithBlocks(referenceBlockList);
            //SetGridLetterBackFromBlocks(referenceBlockList);
            //Shuffle(gridLetterList);
            //ShuffleBlocksWithGridList(gridLetterList, referenceBlockList);

            float gridWidth = cols * tileSize;
            float gridHeight = rows * tileSize;
            //pivot point for tile is in the centre
            transform.position = new Vector3(-gridWidth / 2 + tileSize / 2, gridHeight / 2 - tileSize / 2 + transform.position.y, transform.position.z);
        }

        void GenerateGridForTutorial()
        {
            GameObject referenceTile = Instantiate(referenceBlock);
            referenceTile.SetActive(true);
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    //generate the tile from prefab folder (block)
                    GameObject tile = Instantiate(referenceTile, transform);
                    BlockController tileBlockController = tile.GetComponent<BlockController>();
                    if (tileBlockController != null)
                    {
                        if (gridBlockIndex < gridLetterList.Count)
                        {
                            tileBlockController.adapter.dataContainer.SetLetter(gridLetterList[gridBlockIndex]);
                            tileBlockController.textAlphabet.text = " ";
                            tileBlockController.SetBlockText();
                            tile.name = tileBlockController.adapter.dataContainer.GetLetter().ToString();
                            if (!char.IsLetter(tileBlockController.adapter.dataContainer.GetLetter()))
                            {
                                tile.name = "empty";
                            }
                            gridBlockIndex++;
                        }
                        else
                        {
                            
                        }
                    }
                    else
                    {
                        Debug.LogWarning("This referenceTile " + tile + " has not BlockControllerScript");
                    }


                    float posX = col * tileSize;
                    float posY = row * -tileSize + transform.position.y;

                    tile.transform.position = new Vector3(posX, posY, transform.position.z);
                    referenceBlockList.Add(tile);
                }
            }
            Destroy(referenceTile);

            //ShuffleBlockPositionWithBlocks(referenceBlockList);
            
            float gridWidth = cols * tileSize;
            float gridHeight = rows * tileSize;
            //pivot point for tile is in the centre
            transform.position = new Vector3(-gridWidth / 2 + tileSize / 2, gridHeight / 2 - tileSize / 2 + transform.position.y, transform.position.z);
           // transform.position = new Vector3(-gridWidth / 2 + tileSize / 2, transform.position.y, transform.position.z);
        }

        void UpdateGridForTorial()
        {
            ShuffleManager.SwapForTutorial(referenceBlockList[1].GetComponent<BlockController>(), referenceBlockList[6].GetComponent<BlockController>());
            ShuffleManager.SwapForTutorial(referenceBlockList[3].GetComponent<BlockController>(), referenceBlockList[10].GetComponent<BlockController>());
            ShuffleManager.SwapForTutorial(referenceBlockList[4].GetComponent<BlockController>(), referenceBlockList[11].GetComponent<BlockController>());
        }
        
        public void GetCharFromWords()
        {
             //charFromWords = new List<char>();
            for (int i = 0; i < wordList.Count; i++)
            {
                charFromWords.AddRange(BreakOneWord(wordList[i]));
            }
            //Debug.Log("char from words are set");
            //return charFromWords;
        }

        [SerializeField]  List<char> charFromWords;

        public List<GameObject> GetBlocksOfIconMatch()
        {
            List<GameObject> blocksofIcon = new List<GameObject>();
            /*
            charFromWords = new List<char>();
            for (int i = 0; i < wordList.Count; i++)
            {
                charFromWords.AddRange(BreakOneWord(wordList[i]));
            }
            */
            for (int j = 0; j < charFromWords.Count; j++)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform currentChild = transform.GetChild(i);
                    if (charFromWords[j] == ((currentChild.GetComponent<BlockController>().adapter.dataContainer.GetLetter())))
                    {
                        blocksofIcon.Add(currentChild.gameObject);
                        break;
                        //charFromWords.Remove(currentChild.GetComponent<BlockController>().adapter.dataContainer.GetLetter());  //
                    }
                }

                   
            }

            return blocksofIcon;
        }

    }
}

