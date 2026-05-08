using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Hotbar.Procedural
{
    public class Room : ProceduralObject
    {
        [Header("[Wall]")]
        public List<Wall> wallList = new List<Wall>();

        public void AddWallInfo(Wall wall)
        {
            if(wallList == null)
                wallList = new List<Wall>();

            if (wallList.Exists(x => x == wall))
            {
                Debug.LogError($"[Room.AddWall] => the object {wall.name} is exist in pool");
                return;
            }

            wallList.Add(wall);
        }
    }
}

