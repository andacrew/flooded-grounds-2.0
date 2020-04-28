/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Photon.Realtime;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Game
{
    /// <summary>
    /// Manages the character instantiation within a PUN room.
    /// </summary>
    public class FGSpawner : SpawnManagerBase
    {
        [Tooltip("A reference to the character that PUN should spawn. This character must be setup using the PUN Multiplayer Manager.")]
        [SerializeField] protected GameObject H_Character;
        [SerializeField] protected GameObject M_Character;

        public GameObject humanCharacter { get { return H_Character; } set { H_Character = value; } }
        public GameObject MonsterCharacter { get {return M_Character;}  set {M_Character = value;}}
        /// <summary>
        /// Abstract method that allows for a character to be spawned based on the game logic.
        /// </summary>
        /// <param name="newPlayer">The player that entered the room.</param>
        /// <returns>The character prefab that should spawn.</returns>
        protected override GameObject GetCharacterPrefab(Player newPlayer)
        {
            // Return the same character for all instances.
            return  H_Character;
        }
    }
}