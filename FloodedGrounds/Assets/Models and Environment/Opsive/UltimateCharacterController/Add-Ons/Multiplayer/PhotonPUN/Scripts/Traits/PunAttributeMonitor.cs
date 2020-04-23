/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Opsive.UltimateCharacterController.Events;
using Opsive.UltimateCharacterController.Traits;
using Opsive.UltimateCharacterController.Utility;
using Photon.Pun;
using Photon.Realtime;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Traits
{
    /// <summary>
    /// The PunAttributeMonitor will ensure the attribute values are synchronized when a new player joins the room.
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(AttributeManager))]
    public class PunAttributeMonitor : MonoBehaviour
    {
        private AttributeManager m_AttributeManager;
        private PhotonView m_PhotonView;

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        private void Awake()
        {
            m_AttributeManager = gameObject.GetCachedComponent<AttributeManager>();
            m_PhotonView = gameObject.GetCachedComponent<PhotonView>();
        }

        /// <summary>
        /// Register for any interested events.
        /// </summary>
        private void Start()
        {
            if (m_PhotonView.IsMine) {
                EventHandler.RegisterEvent<Player, GameObject>("OnPlayerEnteredRoom", OnPlayerEnteredRoom);
            }
        }

        /// <summary>
        /// A player has entered the room. Ensure the joining player is in sync with the current game state.
        /// </summary>
        /// <param name="player">The Photon Player that entered the room.</param>
        /// <param name="character">The character that the player controls.</param>
        private void OnPlayerEnteredRoom(Player player, GameObject character)
        {
            var attributes = m_AttributeManager.Attributes;
            if (attributes == null) {
                return;
            }

            for (int i = 0; i < attributes.Length; ++i) {
                m_PhotonView.RPC("UpdateAttributeRPC", player, attributes[i].Name, attributes[i].Value, attributes[i].MinValue, attributes[i].MaxValue, attributes[i].AutoUpdateAmount,
                                                               attributes[i].AutoUpdateInterval, attributes[i].AutoUpdateStartDelay, (int)attributes[i].AutoUpdateValueType);
            }
        }

        /// <summary>
        /// Updates the attribute values for the specified attribute.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <param name="minValue">The min value of the attribute.</param>
        /// <param name="maxValue">The max value of the attribute.</param>
        /// <param name="autoUpdateAmount">The amount to change the value with each auto update.</param>
        /// <param name="autoUpdateInterval">The amount of time to wait in between auto update loops.</param>
        /// <param name="autoUpdateStartDelay">The amount of time between a value change and when the auto updater should start.</param>
        /// <param name="autoUpdateValueType">Describes how the attribute should update the value</param>
        [PunRPC]
        private void UpdateAttributeRPC(string name, float value, float minValue, float maxValue, float autoUpdateAmount, float autoUpdateInterval, float autoUpdateStartDelay, int autoUpdateValueType)
        {
            var attribute = m_AttributeManager.GetAttribute(name);
            if (attribute == null) {
                return;
            }

            attribute.Value = value;
            attribute.MinValue = minValue;
            attribute.MaxValue = maxValue;
            attribute.AutoUpdateAmount = autoUpdateAmount;
            attribute.AutoUpdateInterval = autoUpdateInterval;
            attribute.AutoUpdateStartDelay = autoUpdateStartDelay;
            attribute.AutoUpdateValueType = (Attribute.AutoUpdateValue)autoUpdateValueType;
        }

        /// <summary>
        /// The object has been destroyed.
        /// </summary>
        private void OnDestroy()
        {
            EventHandler.UnregisterEvent<Player, GameObject>("OnPlayerEnteredRoom", OnPlayerEnteredRoom);
        }
    }
}