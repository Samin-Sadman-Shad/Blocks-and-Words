using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * shuffle manager checks when to shuffle
 *      it must be informed which two blocks to be interchanged
 *      it will received two blocks as input
 * it has methods to execute shuffle
 *      it interchanges the position of the blocks 
 */

namespace Samin.BlocksAndWords
{
    public class ShuffleManager : MonoBehaviour
    {
        [SerializeField] Animator shuffleAnimator;
        public static bool isSwappingCompleted;

        public static float moveDuration = 0.2f;
        [SerializeField] Ease moveEase = Ease.Linear;

        public static float zPos;
        public static float zPos2;
        static float  targetZPos1;
        static float targetZPos2;

        public static bool uplifted;
        public static bool shuffled;
        public static bool repos;

        public static void Initializer(GameObject block)
        {
            zPos = block.transform.localPosition.z;
            zPos2 = block.GetComponent<RectTransform>().position.z;
            targetZPos1 = zPos2 - 1.5f;
            targetZPos2 = zPos2 - 1f;
        }

        public static void UpliftBlocksBeforeShuffle(BlockController previousBlock, BlockController currentBlock)
        {
            if(previousBlock.gameObject != currentBlock.gameObject)
            {
                previousBlock.GetComponent<RectTransform>().DOMoveZ(targetZPos1, 0.1f);
                currentBlock.GetComponent<RectTransform>().DOMoveZ(targetZPos2, 0.1f);
            }
        }

        public static void Shuffle(BlockController previousBlock, BlockController currentBlock)
        {
            Vector3 tempPos = previousBlock.gameObject.GetComponent<RectTransform>().position;
            Transform tempT = previousBlock.transform;
            GameObject tempG = tempT.gameObject;
            int tempIndex = tempT.GetSiblingIndex();
            int tempIndexG = GameManager.GameManagerSingleton.unmatchedBlocks.IndexOf(tempG);

            Vector3 currentPos = currentBlock.gameObject.GetComponent<RectTransform>().position;
            Transform currentT = currentBlock.transform;
            GameObject currentG = currentT.gameObject;
            int currentIndex = currentT.GetSiblingIndex();
            int currentIndexG = GameManager.GameManagerSingleton.unmatchedBlocks.IndexOf(currentG);

            //currentBlock.gameObject.transform.position = tempPos;
            currentBlock.gameObject.GetComponent<RectTransform>().DOMove(tempPos, moveDuration);
            currentT.SetSiblingIndex(tempIndex);
            GameManager.GameManagerSingleton.unmatchedBlocks[tempIndexG] = currentG;

            //previousBlock.gameObject.transform.position = currentPos;
            previousBlock.gameObject.GetComponent<RectTransform>().DOMove(currentPos, moveDuration);
            tempT.SetSiblingIndex(currentIndex);
            GameManager.GameManagerSingleton.unmatchedBlocks[currentIndexG] = tempG;
            shuffled = true;
        }

        public static void ShuffleForTutorial(BlockController previousBlock, BlockController currentBlock)
        {
            //Vector3 tempPos = previousBlock.gameObject.GetComponent<RectTransform>().position;
            Vector3 tempPos = previousBlock.transform.position;
            Transform tempT = previousBlock.transform;
            GameObject tempG = tempT.gameObject;
            int tempIndex = tempT.GetSiblingIndex();
            int tempIndexG = TutorialManager.tutorialSingleton.gridChilds.IndexOf(tempG);

            //Vector3 currentPos = currentBlock.gameObject.GetComponent<RectTransform>().position;
            Vector3 currentPos = currentBlock.transform.position;
            Transform currentT = currentBlock.transform;
            GameObject currentG = currentT.gameObject;
            int currentIndex = currentT.GetSiblingIndex();
            int currentIndexG = TutorialManager.tutorialSingleton.gridChilds.IndexOf(currentG);

            //currentBlock.gameObject.GetComponent<RectTransform>().DOMove(tempPos, moveDuration);
            //previousBlock.gameObject.GetComponent<RectTransform>().DOMove(currentPos, moveDuration);

            previousBlock.gameObject.transform.DOMove(currentPos, moveDuration);
            //previousBlock.gameObject.transform.position = currentPos;
            tempT.SetSiblingIndex(currentIndex);
            TutorialManager.tutorialSingleton.gridChilds[currentIndexG] = tempG;

            currentBlock.gameObject.transform.DOMove(tempPos, moveDuration);
            //currentBlock.gameObject.transform.position = tempPos;
            currentT.SetSiblingIndex(tempIndex);
            TutorialManager.tutorialSingleton.gridChilds[tempIndexG] = currentG;

            //Debug.Log("swapped in shuffle manager");

            shuffled = true;
        }

        public static void SwapForTutorial(BlockController previousBlock, BlockController currentBlock)
        {
            Vector3 tempPos = previousBlock.gameObject.GetComponent<RectTransform>().position;
            Transform tempT = previousBlock.transform;
            GameObject tempG = tempT.gameObject;
            int tempIndex = tempT.GetSiblingIndex();
            int tempIndexG = GridManager.gridSingleton.referenceBlockList.IndexOf(tempG);

            Vector3 currentPos = currentBlock.gameObject.GetComponent<RectTransform>().position;
            Transform currentT = currentBlock.transform;
            GameObject currentG = currentT.gameObject;
            int currentIndex = currentT.GetSiblingIndex();
            int currentIndexG = GridManager.gridSingleton.referenceBlockList.IndexOf(currentG);

            //currentBlock.gameObject.transform.position = tempPos;
            //currentBlock.gameObject.GetComponent<RectTransform>().DOMove(tempPos, moveDuration);
            currentBlock.gameObject.GetComponent<RectTransform>().position = tempPos;
            currentT.SetSiblingIndex(tempIndex);
            GridManager.gridSingleton.referenceBlockList[tempIndexG] = currentG;

            //previousBlock.gameObject.transform.position = currentPos;
            //previousBlock.gameObject.GetComponent<RectTransform>().DOMove(currentPos, moveDuration);
            previousBlock.gameObject.GetComponent<RectTransform>().position = currentPos;
            tempT.SetSiblingIndex(currentIndex);
            GridManager.gridSingleton.referenceBlockList[currentIndexG] = tempG;
        }

        public static void LowerAfterShuffle(BlockController previousBlock, BlockController currentBlock)
        {
            if (shuffled)
            {
                shuffled = false;
                previousBlock.GetComponent<RectTransform>().DOMoveZ(zPos2, 0.1f);
                currentBlock.GetComponent<RectTransform>().DOMoveZ(zPos2, 0.1f);
            }

        }

        public static IEnumerator UnselectAfterShuffle(BlockController previousBlock, BlockController currentBlock)
        {
            yield return new WaitForSeconds(moveDuration);
            //previousBlock.UnselectBlock();
            //currentBlock.UnselectBlock();
            previousBlock.ChangeState();
            currentBlock.ChangeState();
            repos = true;
        }
    }
}

