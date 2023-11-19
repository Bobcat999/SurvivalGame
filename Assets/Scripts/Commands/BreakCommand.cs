using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Commands
{
    public class BreakCommand
    {
        public Item tool;
        public BlockTile tile;
        public Vector3Int tilePos;
        public float breakTime;//in seconds
        public float timeOfStart;
        public bool dropContens;

        public BreakCommand(Item tool, BlockTile tile, Vector3Int tilePos) { 
            
            this.tool = tool;
            this.tile = tile;
            this.tilePos = tilePos;

            timeOfStart = Time.time;
            dropContens = tile.breakType == tool.breakingType;

            //determine the time it should take to mine;
            breakTime = tile.breakTime / ((dropContens)? tool.breakingSpeed : 1);
            
        }

        public bool IsFinished()
        {
            return timeOfStart + breakTime < Time.time;
        }

        public float GetProgress()
        {
            if (IsFinished())
            {
                return 1;
            }
            return (Time.time - timeOfStart) / breakTime;
        }
    }
}