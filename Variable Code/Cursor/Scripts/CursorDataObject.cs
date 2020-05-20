using System;
using System.Collections.Generic;
using UnityEngine;

namespace VariableCode.Cursor
{
    [Serializable]
    public class CursorDataClass
    {
        /// <summary>
        /// Sequence of cursors to be shown as 
        /// </summary>
        public List<Texture2D> image;
        /// <summary>
        /// The pixel position of pointer tip of each of the cursor 
        /// </summary>
        public Vector2         hotSpot;
        /// <summary>
        /// Time each cursor is show before changing to next
        /// </summary>
        public float timePerFrame = 1;
    }

    [CreateAssetMenu(fileName = "Cursor Data", menuName = "Variable Code/Cursor Data")]
    public class CursorDataObject : ScriptableObject
    {
        public List<CursorDataClass> data;

        private void Reset()
        {
            data = new List<CursorDataClass>(10);
        }
    }
}