using TMPro;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

namespace XRMultiplayer
{
    public class PlayerSlot : MonoBehaviour
    {
        public TMP_Text playerSlotName;
        public TMP_Text playerInitial;
        public Image playerIconImage;
        public UISlotType uiType;

        [Header("Mic Button")]
        public Image voiceChatFillImage;
        [SerializeField] Button m_MicButton;
        [SerializeField] Image m_PlayerVoiceIcon;
        [SerializeField] Image m_SquelchedIcon;
        [SerializeField] Sprite[] micIcons;
        [Header("Text Box")]
        [SerializeField] Button m_Textbox;
        XRINetworkPlayer m_Player;
        internal ulong playerID = 0;

        public enum UISlotType
        {
            RoomInfo,
            GradeInfo
        }

        public void Setup(XRINetworkPlayer player)
        {
            // Checks what Type of Room the uiType is set with currently, using RoomInfo or GradeInfo as the two room types
            
            m_Player = player;
            m_Player.onColorUpdated += UpdateColor;
            m_Player.onNameUpdated += UpdateName;
            m_Player.squelched.Subscribe(UpdateSquelchedState);
            // If it is RoomInfo it starts the mic scripts and detects it in the Log
            if (uiType == UISlotType.RoomInfo)
            {
                m_Player.selfMuted.OnValueChanged += UpdateSelfMutedState;
                m_MicButton.onClick.AddListener(Squelch);
                m_SquelchedIcon.enabled = false;
                Debug.Log("Room Info UI detected");
            }
            // If it is the GradeInfo it sets the host of the session to become the only person able to see the grade menu
            else if (uiType == UISlotType.GradeInfo)
            {
                Debug.Log("Grade Info UI detected");
                GameObject gradeInfo = GameObject.Find("Grade Info UI");
                Debug.Log(gradeInfo);
                gradeInfo.SetActive(false);
                if (m_Player.IsSessionOwner == true)
                {
                    gradeInfo.SetActive(true);
                }
            }
            if (m_Player.IsLocalPlayer)
            {
                m_MicButton.interactable = false;
            }

            if (m_Player.selfMuted.Value)
            {
                m_PlayerVoiceIcon.sprite = micIcons[1];
            }
        }

        void OnDestroy()
        {
            m_Player.onColorUpdated -= UpdateColor;
            m_Player.onNameUpdated -= UpdateName;
            if (uiType == UISlotType.RoomInfo)
            {
                m_Player.selfMuted.OnValueChanged -= UpdateSelfMutedState;
                m_MicButton.onClick.RemoveListener(Squelch);
            }
            m_Player.squelched.Unsubscribe(UpdateSquelchedState);
        }

        void UpdateColor(Color newColor)
        {
            playerIconImage.color = newColor;
        }

        void UpdateName(string newName)
        {
            if (!newName.IsNullOrEmpty())
            {
                string playerName = newName;
                if (m_Player.IsLocalPlayer)
                {
                    playerName += " (You)";
                }
                else if (m_Player.IsOwnedByServer)
                {
                    playerName += " (Host)";
                }
                playerSlotName.text = playerName;
                playerInitial.text = newName.Substring(0, 1);
            }
        }
        void Awake()
        {
            // Initial startup for the gameObject room 
            GameObject root = gameObject.transform.root.gameObject;
            if (root.name.Contains("Room Info UI"))
            {
                Debug.Log("This is Room Info UI");
            }
            else if (root.name.Contains("Grade Info UI"))
            {
                Debug.Log("This is Grade Info UI");
            }
        }

        #region Muting
        public void Squelch()
        {
            m_Player.ToggleSquelch();
        }

        void UpdateSelfMutedState(bool old, bool current)
        {
            m_PlayerVoiceIcon.sprite = micIcons[current ? 1 : 0];
        }

        void UpdateSquelchedState(bool squelched)
        {
            m_SquelchedIcon.enabled = squelched;
        }
        #endregion
    }
}
