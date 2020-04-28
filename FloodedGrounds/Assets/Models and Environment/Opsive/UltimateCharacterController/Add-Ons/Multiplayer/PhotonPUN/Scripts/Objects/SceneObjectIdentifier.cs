/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using Opsive.UltimateCharacterController.Objects;
using Photon.Pun;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Objects
{
    /// <summary>
    /// A lightweight class that can be uniquely identified within the scene for the PUN addon.
    /// </summary>
    public class SceneObjectIdentifier : ObjectIdentifier
    {
        private bool m_Registered;

        /// <summary>
        /// Registers itself as a Scene Object Identifier.
        /// </summary>
        private void Awake()
        {
            if (GetComponent<PhotonView>() == null) {
                Utility.PunUtility.RegisterSceneObjectIdentifier(this);
                m_Registered = true;
            }
        }

        /// <summary>
        /// Unregisters itself as a Scene Object Identifier.
        /// </summary>
        private void OnDestroy()
        {
            if (m_Registered) {
                Utility.PunUtility.UnregisterSceneObjectIdentifier(this);
                m_Registered = false;
            }
        }
    }
}