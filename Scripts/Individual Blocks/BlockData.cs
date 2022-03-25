using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * block data acts as a container of every blocks, it encapsulate the data of the blocks
 */

namespace Samin.BlocksAndWords
{
    public class BlockData : MonoBehaviour
    {
        public char Character
        {
            get
            {
                return character;
            }
            set
            {
                character = value;
            }
        }
        [SerializeField] char character;
        
        public char GetLetter()
        {
            return character;
        }

        public void SetLetter(char letter)
        {
            character = letter;
        }
        
    }
}

