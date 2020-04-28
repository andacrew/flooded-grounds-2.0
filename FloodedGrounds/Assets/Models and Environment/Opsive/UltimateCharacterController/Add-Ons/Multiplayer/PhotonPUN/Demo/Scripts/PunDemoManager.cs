/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Opsive.UltimateCharacterController.Demo;
using Opsive.UltimateCharacterController.Events;
using Photon.Realtime;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Demo
{
    /// <summary>
    /// Initializes the demo manager to the local player.
    /// </summary>
    public class PunDemoManager : DemoManager
    {
        /// <summary>
        /// Initializes the default values.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            EventHandler.RegisterEvent<Player, GameObject>("OnPlayerEnteredRoom", OnPlayerEnteredRoom);
        }

        /// <summary>
        /// Prevent the base class from initializing the character.
        /// </summary>
        protected override void Start() { }

        /// <summary>
        /// A player has entered the room. Initialize the Demo manager to the local player.
        /// </summary>
        /// <param name="player">The Photon Player that entered the room.</param>
        /// <param name="character">The character that the player controls.</param>
        private void OnPlayerEnteredRoom(Player player, GameObject character)
        {
            if (player.IsLocal) {
#if FIRST_PERSON_CONTROLLER && THIRD_PERSON_CONTROLLER
                m_DefaultFirstPersonStart = PlayerPrefs.GetInt("START_PERSPECTIVE", m_DefaultFirstPersonStart ? 1 : 0) == 1;
#endif
                InitializeCharacter(character, true, false);
            }
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