using UnityEngine;
using System;
using System.Collections.Generic;

namespace Assets.All_Scripts.Network.Request
{
    class RequestHit : NetworkRequest
    {
        private string hitPlayer;
        private int damage;
        private int numParticles;
        private List<Vector3> particlesPositions;
        private List<Vector3> particleAngles;

        public RequestHit()
        {
            request_id = Constants.CMSG_HIT;
        }

        public void setData(string hitPlayer, int damage, int numParticles, List<Vector3> particlesPositions, List<Vector3> particleAngles)
        {
            this.hitPlayer = hitPlayer;
            this.damage = damage;
            this.numParticles = numParticles;
            this.particlesPositions = particlesPositions;
            this.particleAngles = particleAngles;
        }

        public void send()
        {
            packet = new GamePacket(request_id);
            //Add the character code of the hit player
            packet.addShort16((short)Constants.CharacterToID[hitPlayer]);
            //Add the damage dealt
            packet.addShort16((short)damage);
            //Add the number of particles
            packet.addShort16((short)numParticles);

            for (int i = 0; i < numParticles; i++)
            {
                //Add all of the positions
                packet.addFloat32(particlesPositions[i].x);
                packet.addFloat32(particlesPositions[i].y);
                packet.addFloat32(particlesPositions[i].z);

                //Add all of the angles
                packet.addFloat32(particleAngles[i].x);
                packet.addFloat32(particleAngles[i].y);
                packet.addFloat32(particleAngles[i].z);
            }
        }
    }
}
