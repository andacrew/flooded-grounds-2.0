using Assets.All_Scripts.Network.Request;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootRaycast : MonoBehaviour
{
    private struct gunStats
    {
        public float fireRate;
        public float range;
        public int damage;

        public gunStats(float fireRate, float range, int damage)
        {
            this.fireRate = fireRate;
            this.range = range;
            this.damage = damage;
        }
    }

    public ParticleSystem monsterBlood;
    private Animator anim;

    // For fire rates 
    public float nextFire;
    private Dictionary<string, gunStats> gun_stats = new Dictionary<string, gunStats>();
    private string equip;

    public ManagementHUD hud;

    // Testing for shotgun spread raycast 
    float variance = 1.0f;  // This much variance 
    float distance = 10.0f; // at this distance
    List<Vector3> shotgunRays = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        gun_stats.Add("Pistol", new gunStats(0.35f, 200f, 5));
        gun_stats.Add("AK-47", new gunStats(0.10f, 200f, 5));
        gun_stats.Add("Shotgun", new gunStats(1.0f, 50f, 1));

        // This script is intended to be attached to gun as component
        // equip = this.tag;

        anim = GameObject.FindGameObjectWithTag("Bog_lord").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawRay(this.transform.position, this.transform.forward * gun_stats[equip].range, Color.magenta);
    }

    public void Shoot()
    {
        // List of particle positions and angles to send to server 
        List<Vector3> particlePos = new List<Vector3>();
        List<Vector3> particleAngle = new List<Vector3>();
        RaycastHit hit;

        equip = hud.gunEquipped;

        bool playAnimation = false;

        if (hud.ammoIn > 0 && Time.time > nextFire)
        {
            // Update the time when our player can fire next
            nextFire = Time.time + gun_stats[equip].fireRate;

            //Update HUD
            hud.AmmoCounter(1);

            // Conditions for if statement 
            // 1. Input returns true when left click down and false when released 
            // 2. Send out raycast straight forward from player camera 
            // 3. Check if raycast hit Bog lord
            // 4. Applying pistol fire rate
            if ((equip == "Pistol" | equip == "AK-47") && Physics.Raycast(this.transform.position, this.transform.forward, out hit, gun_stats[equip].range) && hit.collider.gameObject.tag == "Bog_lord")
            {
                // Trigger Bog_lord isHit animation 
                playAnimation = true;

                // spawn monsterBlood particle effect, then destroy clone gameObject
                var rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
                Destroy(Instantiate(monsterBlood.gameObject, hit.point, rot), 2f);

                particlePos.Add(hit.point);
                particleAngle.Add(rot.eulerAngles);

                RequestHit requestHit = new RequestHit();
                requestHit.setData(Constants.MONSTER, gun_stats[equip].damage, 1, particlePos, particleAngle);
                requestHit.send();
                Main.GetConnectionManager().send(requestHit);

                Debug.Log("Bog lord has been shot with pistol/ak-47!");
            }

            if (equip == "Shotgun")
            {
                int raysHit = 0;

                for (var i = 0; i < 30; i++)
                {
                    Vector3 v3Offset = transform.up * Random.Range(0.0f, variance);
                    v3Offset = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), transform.forward) * v3Offset;
                    Vector3 v3Hit = transform.forward * distance + v3Offset;

                    // shotgunRays.Add(v3Hit);
                    // will add up shotgun spread rays that hit Bog lord to apply the total as damage to Bog lord
                    if (Physics.Raycast(this.transform.position, v3Hit, out hit, gun_stats[equip].range) && hit.collider.gameObject.tag == "Bog_lord")
                    {
                        // spawn monsterBlood particle effect, then destroy clone gameObject
                        var rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
                        Destroy(Instantiate(monsterBlood.gameObject, hit.point, rot), 2f);

                        playAnimation = true;
                        particlePos.Add(hit.point);
                        particleAngle.Add(rot.eulerAngles);
                        raysHit += 1;
                    }
                }

                RequestHit requestHit = new RequestHit();
                requestHit.setData(Constants.MONSTER, raysHit, raysHit, particlePos, particleAngle);
                requestHit.send();
                Main.GetConnectionManager().send(requestHit);

                //Instantiate(monsterBlood, hit.collider.gameObject.transform)
                // applyDmgBog(raysHit);
                Debug.Log(raysHit);
                Debug.Log("Bog lord has been shot with shotgun!");
            }

            if (playAnimation)
            {
                // Needs to be fixed. Monster gets stuck in isHit animation
                //anim.SetBool("isHit", true);
            }
        }
    }

}