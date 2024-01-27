#if NODECANVAS
using UnityEngine;

namespace Pearl.NodeCanvas
{
    public class SettingLabelFSMManager : MonoBehaviour
    {
        [SerializeField]
        public string newValue = "";
        [SerializeField]
        public bool gameManager = false;
        [SerializeField]
        public bool checkTransitions = false;
        [SerializeField, ConditionalField("!@gameManager")]
        public PearlFSMOwner FSM = null;

        private void Reset()
        {
            FSM = GetComponent<PearlFSMOwner>();
        }

        public void Execute()
        {
            if (gameManager)
            {
                if (checkTransitions)
                {
                    GameManager.CheckTransitionsAfterChangeLabel(newValue);
                }
                else
                {
                    GameManager.ChangeLabel(newValue);
                }
            }
            else if (FSM != null)
            {
                if (checkTransitions)
                {
                    FSM.CheckForceTransitionsAfterChangeLabel(newValue);
                }
                else
                {
                    FSM.ChangeLabel(newValue);
                }
            }
        }
    }
}

#endif
