using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiarTetris
{
    public class BlockInstanceManager : MonoBehaviour
    {
        [SerializeField]
        Vector3 farPlace = new Vector3(1000, 1000, 1000);

        [SerializeField]
        Block tetrisBlockPrefab;

        [SerializeField]
        Transform blockParent;
        public Transform BlockParent => blockParent;

        Queue<Block> blockQueue = new Queue<Block>();

        public Block StartUseBlock()
        {
            Block block;
            if (blockQueue.Count == 0)
            {
                block = Instantiate(tetrisBlockPrefab);
                block.SetParent(blockParent);
            }
            else
            {
                block = blockQueue.Dequeue();
                block.gameObject.SetActive(true);
            }

            return block;
        }

        public void EndUseBlock(Block block)
        {
            block.transform.position = farPlace;
            block.gameObject.SetActive(false);
            blockQueue.Enqueue(block);
        }
    }
}