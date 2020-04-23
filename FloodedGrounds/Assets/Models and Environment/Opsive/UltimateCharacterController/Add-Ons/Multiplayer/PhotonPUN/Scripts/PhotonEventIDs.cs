/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun
{
    /// <summary>
    /// Specifies the events used by the Ultimate Character Controller / Photon PUN multiplayer addon.
    /// </summary>
    public class PhotonEventIDs
    {
        public const byte PlayerInstantiation = 155; // A player has joined the room and been instantiated.
        public const byte RemotePlayerInstantiationComplete = 156; // A remote player has finished being instantiated.
        public const byte StateChange = 157; // A state on the character was changed.
        public const byte ObjectInstantiation = 158; // An object was instantiated by the NetworkObjectPool.
        public const byte ObjectDestruction = 159; // An object was destroyed by the NetworkObjectPool.
    }
}