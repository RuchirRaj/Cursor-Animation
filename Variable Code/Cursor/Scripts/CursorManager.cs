using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace VariableCode.Cursor
{
    public enum CursorReferenceType
    {
        Class,
        ScriptableObject
    }
    [Serializable]
    public struct CursorId
    {
        //Should we look up the objects(ScriptableObject) list or data(Class) list
        public CursorReferenceType referenceType;
        //Index of the scriptable object in the current list
        public int objectIndex;
        //List of the class CursorDataClass in the list
        public int classIndex;
        //Index of the desired image in the CursorDataClass(Just set it to 0 if unsure)
        public int imageIndex;
    }
    public class CursorManager : MonoBehaviour
    {
        private static CursorManager _manager;

        public static CursorManager Manager
        {
            get => _manager;
            private set => _manager = value;
        }
        
        // You can either provide reference to an scriptable object stored in your project or define a new class in the inspector 
        public List<CursorDataClass> data;
        public List<CursorDataObject> objects;

        public CursorId currentId;
        public bool initialized;

        [SerializeField] private float currentTime;
        [SerializeField] private float timePerFrame;


        private void Awake()
        {
            if (Manager)
                Manager = this;
        }

        /// <summary>
        /// Ensure the creation of manager
        /// </summary>
        /// <returns>The global Cursor manager</returns>
        public static CursorManager EnsureCreation()
        {
            if (!_manager)
            {
                _manager = FindObjectOfType<CursorManager>();
                if (!_manager)
                {
                    GameObject managerGo = new GameObject("Cursor Manager");
                    _manager = managerGo.AddComponent<CursorManager>();
                    _manager.Reset();
                }
            }
            return _manager;
        }

        private void Reset()
        {
            data = new List<CursorDataClass>();
            objects = new List<CursorDataObject>();
            initialized = true;
        }

        /// <summary>
        /// Sets the active cursor style to according to the Id provided
        /// </summary>
        /// <param name="cursorId"></param>
        public void SetActiveCursor(CursorId cursorId)
        {
            initialized = data != null;
            if (!IsValidId(cursorId)) return;
            currentTime = 0;
            currentId = cursorId;
            UnityEngine.Cursor.SetCursor(GetTexture(currentId), GetHotspot(currentId), CursorMode.Auto);
        }

        private void Update()
        {
            if(!initialized) return;
            currentTime += Time.deltaTime;
            timePerFrame = GetTimePerFrame(currentId);
            if (timePerFrame < currentTime)
            {
                while (timePerFrame < currentTime)
                    currentTime -= timePerFrame;
                
                if (currentId.imageIndex >= GetFrameCount(currentId) - 1)
                {
                    currentId.imageIndex = 0;
                }
                else
                {
                    currentId.imageIndex++;
                }
            }
            UnityEngine.Cursor.SetCursor(GetTexture(currentId), GetHotspot(currentId), CursorMode.Auto);
        }

        public int GetFrameCount(CursorId cursorId)
        {
            int frame = 0;

            if (IsValidId(cursorId))
            {
                if (cursorId.referenceType == CursorReferenceType.Class)
                {
                    frame = data[cursorId.classIndex].image.Count;
                }
                else
                {
                    frame = objects[cursorId.objectIndex].data[cursorId.classIndex].image.Count;
                }
            }
            
            return frame;
        }

        /// <summary>
        /// Returns the time per frame for each Cursor before moving to next index
        /// </summary>
        /// <param name="cursorId"></param>
        /// <returns></returns>
        public float GetTimePerFrame(CursorId cursorId)
        {
            timePerFrame = 1;
            if (IsValidId(cursorId))
            {
                if (cursorId.referenceType == CursorReferenceType.Class)
                {
                    timePerFrame = data[cursorId.classIndex].timePerFrame;
                }
                else
                {
                    timePerFrame = objects[cursorId.objectIndex].data[cursorId.classIndex].timePerFrame;
                }
            }

            return timePerFrame;
        }
        
        /// <summary>
        /// Returns the respective cursor texture based on the Id provided
        /// </summary>
        /// <param name="cursorId"></param>
        /// <returns></returns>
        public Texture2D GetTexture(CursorId cursorId)
        {
            if (IsValidId(cursorId))
            {
                if (cursorId.referenceType == CursorReferenceType.Class)
                {
                    return data[cursorId.classIndex].image[cursorId.imageIndex];
                }
                else
                {
                    return objects[cursorId.objectIndex].data[cursorId.classIndex].image[cursorId.imageIndex];
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the hotSpot for the Cursor texture as specified in inpector
        /// </summary>
        /// <param name="cursorId"></param>
        /// <returns></returns>
        public Vector2 GetHotspot(CursorId cursorId)
        {
            Vector2 hotspot = Vector2.zero;
            if (IsValidId(cursorId))
            {
                if (cursorId.referenceType == CursorReferenceType.Class)
                {
                    hotspot = data[cursorId.classIndex].hotSpot;
                }
                else
                {
                    hotspot = objects[cursorId.objectIndex].data[cursorId.classIndex].hotSpot;
                }
            }

            return hotspot;
        }

        /// <summary>
        /// Checks if the Id is valid
        /// </summary>
        /// <param name="cursorId"></param>
        /// <returns></returns>
        public bool IsValidId(CursorId cursorId)
        {
            if (!initialized) return false;
            bool valid = true;

            if (cursorId.referenceType == CursorReferenceType.ScriptableObject)
            {
                if (objects.Count <= cursorId.objectIndex)
                {
                    valid = false;
                }
                if (valid && objects[cursorId.objectIndex].data.Count <= cursorId.classIndex)
                {
                    valid = false;
                }
                if (valid && objects[cursorId.objectIndex].data[cursorId.classIndex].image.Count <= cursorId.imageIndex)
                {
                    valid = false;
                }
            }
            else
            {
                if (data[cursorId.classIndex].image.Count <= cursorId.imageIndex)
                {
                    valid = false;
                }
                if (valid && data.Count <= cursorId.classIndex)
                {
                    valid = false;
                }
            }

            return valid;
        }
        
    }
}