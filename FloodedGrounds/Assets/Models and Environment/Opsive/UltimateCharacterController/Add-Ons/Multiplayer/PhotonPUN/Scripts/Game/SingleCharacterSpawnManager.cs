/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Game
{
    /// <summary>
    /// Manages the character instantiation within a PUN room.
    /// </summary>
    public class SingleCharacterSpawnManager : SpawnManagerBase
    {
        [Tooltip("A reference to the character that PUN should spawn. This character must be setup using the PUN Multiplayer Manager.")]
        [SerializeField] protected GameObject m_MCharacter;
        [SerializeField] protected GameObject m_HCharacter;


        public GameObject MCharacter { get { return m_MCharacter; } set { m_MCharacter = value; } }
        public GameObject HCharacter { get { return m_HCharacter; } set { m_HCharacter = value; } }


        /// <summary>
        /// Abstract method that allows for a character to be spawned based on the game logic.
        /// </summary>
        /// <param name="newPlayer">The player that entered the room.</param>
        /// <returns>The character prefab that should spawn.</returns>
        protected override GameObject GetCharacterPrefab(Player newPlayer)
        {
            
            // Return the same character for all other instances.
            return m_HCharacter;

        }
    }
}