using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Samin.BlocksAndWords
{
    public class BlockAdapter : MonoBehaviour
    {
        public BlockData dataContainer;
        void Awake()
        {
            dataContainer = GetComponent<BlockData>();
        }
    }
}

