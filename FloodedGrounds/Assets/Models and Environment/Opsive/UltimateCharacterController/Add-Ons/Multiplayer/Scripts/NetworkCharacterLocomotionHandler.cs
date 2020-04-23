/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Opsive.UltimateCharacterController.Character;
using Opsive.UltimateCharacterController.Networking;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.Character
{
    /// <summary>
    /// Enables the UltimateCharacterLocomotionHandler to be aware of the character on the network.
    /// </summary>
    public class NetworkCharacterLocomotionHandler : UltimateCharacterLocomotionHandler
    {
        [Tooltip("Should the camera be attached to the local player?")]
        [SerializeField] protected bool m_AttachCamera = true;

        private INetworkInfo m_NetworkInfo;

        /// <summary>
        /// Initializes the default values.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            m_NetworkInfo = GetComponent<INetworkInfo>();
        }

        /// <summary>
        /// Determines if the handler should be enabled.
        /// </summary>
        private void Start()
        {
            if (m_NetworkInfo.IsLocalPlayer()) {
                if (m_AttachCamera) {
                    var camera = Utility.UnityEngineUtility.FindCamera(gameObject);
                    if (camera != null) {
                        camera.GetComponent<Camera.CameraController>().Character = gameObject;
                    }
                }
            } else {
                // Non-local players will not be controlled with player input.
                enabled = false;
                m_CharacterLocomotion.enabled = false;
            }
        }

        /// <summary>
        /// The character has respawned.
        /// </summary>
        protected override void OnRespawn()
        {
            if (!m_NetworkInfo.IsLocalPlayer()) {
                return;
            }

            base.OnRespawn();
        }
    }
}