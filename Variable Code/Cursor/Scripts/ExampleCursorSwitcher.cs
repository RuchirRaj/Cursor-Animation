using UnityEngine;

namespace VariableCode.Cursor
{
    public class ExampleCursorSwitcher : MonoBehaviour
    {
        public CursorId firstId;
        public CursorId secondId;

        public KeyCode switchKey;

        private int _current;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(switchKey))
            {
                if (_current == 0)
                {
                    _current = 1;
                    CursorManager.EnsureCreation().SetActiveCursor(secondId);
                }
                else
                {
                    _current = 0;
                    CursorManager.EnsureCreation().SetActiveCursor(firstId);
                }
            }
        }
    }
}
