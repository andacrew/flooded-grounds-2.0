using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KirinAudioPlayer : MonoBehaviour
{
    public static KirinAudioPlayer instance = null;
    public GameObject kirinVoiceAudioSource;
    public GameObject kirinMovesAudioSource;
    public GameObject kirinFootstepsAudioSource;
    public GameObject kirinEffectsAudioSource;

    private AudioSource kirinVoice;
    private AudioSource kirinMoves;
    private AudioSource kirinFootsteps;
    private AudioSource kirinEffects;
    //private AudioSource audioData;

    [Header("Idle Ready")]
    public AudioClip[] idlePantVoice;
    public AudioClip[] idlePantArmor;
    public AudioClip[] idlePantScales;
    [Header("Idle Caution")]
    public AudioClip[] idleCautionVoice;
    public AudioClip[] idleCautionArmor;
    public AudioClip[] idleCautionScales;
    [Header("Idle Cleaning")]
    public AudioClip[] idleCleaningVoice;
    public AudioClip[] idleCleaningArmor;
    public AudioClip[] idleCleaningScales;
    [Header("Walk Forward")]
    public AudioClip[] walkForwardFeet;
    public AudioClip[] walkForwardClaw;
    public AudioClip[] walkForwardArmor;
    [Header("Run Forward")]
    public AudioClip[] runForwardFeet;
    public AudioClip[] runForwardClaw;
    public AudioClip[] runForwardArmor;
    [Header("Jump")]
    public AudioClip[] jumpVoiceStart;
    public AudioClip[] jumpVoiceLand;
    public AudioClip[] jumpStartFeet;
    public AudioClip[] jumpLandFeet;
    public AudioClip[] jumpStartClaw;
    public AudioClip[] jumpLandClaw;
    public AudioClip[] jumpStartArmor;
    public AudioClip[] jumpLandArmor;

    [Header("Handgun Attack")]
    public AudioClip[] handgunVoice;
    public AudioClip[] handgunFeet;
    public AudioClip[] handgunClaws;
    public AudioClip[] handgunArmor;
    public AudioClip[] handgunScales;
    public AudioClip[] handgunShot;
    public AudioClip[] handgunCharge;
    public AudioClip[] handgunDischarge;
    [Header("Slash Attack")]
    public AudioClip[] slashVoice;
    public AudioClip[] slashFeet;
    public AudioClip[] slashClaws;
    public AudioClip[] slashArmor;
    [Header("Hit")]
    public AudioClip[] hitReact;
    public AudioClip hitReactImpact;


    public AudioClip[] growls;
    public AudioClip hitRoar;



    void Awake()
     {
        //Check if instance already exists
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        kirinVoice = kirinVoiceAudioSource.GetComponent<AudioSource>();
        kirinMoves = kirinMovesAudioSource.GetComponent<AudioSource>();
        kirinFootsteps = kirinFootstepsAudioSource.GetComponent<AudioSource>();
        kirinEffects = kirinEffectsAudioSource.GetComponent<AudioSource>();

    }

    #region Trigger audio effects
    public void PlayAudioIdlePant()
    {
        kirinFootsteps.Stop();
        kirinVoice.Stop();
        kirinMoves.Stop();

        kirinVoice.clip = idlePantVoice[Random.Range(0, idlePantVoice.Length)];
        kirinVoice.PlayOneShot(kirinVoice.clip);

        kirinMoves.clip = idlePantArmor[Random.Range(0, idlePantArmor.Length)];
        kirinMoves.PlayOneShot(kirinMoves.clip);

        //kirinMoves.Stop();
        //kirinMoves.clip = idlePantScales[Random.Range(0, idlePantScales.Length)];
        //kirinMoves.PlayOneShot(kirinMoves.clip);
    }

    public void PlayAudioIdleCaution()
    {
        kirinFootsteps.Stop();
        kirinVoice.Stop();
        kirinMoves.Stop();

        kirinVoice.clip = idleCautionVoice[Random.Range(0, idleCautionVoice.Length)];
        kirinVoice.PlayOneShot(kirinVoice.clip);

        kirinMoves.clip = idleCautionArmor[Random.Range(0, idleCautionArmor.Length)];
        kirinMoves.PlayOneShot(kirinMoves.clip);

        //kirinMoves.clip = idleCautionScales[Random.Range(0, idleCautionScales.Length)];
        //kirinMoves.PlayOneShot(kirinMoves.clip);
    }

    public void PlayAudioIdleClean()
    {
        kirinFootsteps.Stop();
        kirinVoice.Stop();
        kirinMoves.Stop();

        kirinVoice.clip = idleCleaningVoice[Random.Range(0, idleCleaningVoice.Length)];
        kirinVoice.PlayOneShot(kirinVoice.clip);

        kirinMoves.clip = idleCleaningArmor[Random.Range(0, idleCleaningArmor.Length)];
        kirinMoves.PlayOneShot(kirinMoves.clip);

        //kirinMoves.clip = idleCleaningScales[Random.Range(0, idleCleaningScales.Length)];
        //kirinMoves.PlayOneShot(kirinMoves.clip);
    }

    public void PlayAudioWalkForwardFootSteps()
    {
        kirinFootsteps.Stop();
        kirinVoice.Stop();
        kirinMoves.Stop();

        kirinFootsteps.clip = walkForwardFeet[Random.Range(0, walkForwardFeet.Length)];
        kirinFootsteps.PlayOneShot(kirinFootsteps.clip);

        kirinFootsteps.clip = walkForwardClaw[Random.Range(0, walkForwardClaw.Length)];
        kirinFootsteps.PlayOneShot(kirinFootsteps.clip);

        kirinMoves.clip = walkForwardArmor[Random.Range(0, walkForwardArmor.Length)];
        kirinMoves.PlayOneShot(kirinMoves.clip);

    }

    public void PlayAudioRunForwardFootSteps()
    {

        kirinFootsteps.Stop();
        kirinVoice.Stop();
        kirinMoves.Stop();

        kirinFootsteps.clip = runForwardFeet[Random.Range(0, runForwardFeet.Length)];
        kirinFootsteps.PlayOneShot(kirinFootsteps.clip);

        kirinFootsteps.clip = runForwardClaw[Random.Range(0, runForwardClaw.Length)];
        kirinFootsteps.PlayOneShot(kirinFootsteps.clip);

        kirinMoves.clip = runForwardArmor[Random.Range(0, runForwardArmor.Length)];
        kirinMoves.PlayOneShot(kirinMoves.clip);

    }


    public void PlayAudioHandgunAttack()
    {
        kirinFootsteps.Stop();
        kirinVoice.Stop();
        kirinMoves.Stop();
        kirinEffects.Stop();

        kirinVoice.clip = handgunVoice[Random.Range(0, handgunVoice.Length)];
        kirinVoice.PlayOneShot(kirinVoice.clip);

        kirinFootsteps.clip = handgunFeet[Random.Range(0, handgunFeet.Length)];
        kirinFootsteps.PlayOneShot(kirinFootsteps.clip);

        kirinFootsteps.clip = handgunClaws[Random.Range(0, handgunClaws.Length)];
        kirinFootsteps.PlayOneShot(kirinFootsteps.clip);

        kirinMoves.clip = handgunArmor[Random.Range(0, handgunArmor.Length)];
        kirinMoves.PlayOneShot(kirinMoves.clip);
    }

    public void PlayAudioHandgunShot()
    {
        kirinEffects.clip = handgunShot[Random.Range(0, handgunShot.Length)];
        kirinEffects.PlayOneShot(kirinEffects.clip);

    }


    public void PlayAudioHandgunCharge()
    {
        kirinEffects.clip = handgunCharge[Random.Range(0, handgunCharge.Length)];
        kirinEffects.PlayOneShot(kirinEffects.clip);

    }

    public void PlayAudioHandgunDischarge()
    {
        kirinEffects.clip = handgunDischarge[Random.Range(0, handgunDischarge.Length)];
        kirinEffects.PlayOneShot(kirinEffects.clip);
    }

    public void PlayAudioSlash()
    {
        kirinFootsteps.Stop();
        kirinVoice.Stop();
        kirinMoves.Stop();
        kirinEffects.Stop();

        kirinVoice.clip = slashVoice[Random.Range(0, slashVoice.Length)];
        kirinVoice.PlayOneShot(kirinVoice.clip);

        kirinFootsteps.clip = slashFeet[Random.Range(0, slashFeet.Length)];
        kirinFootsteps.PlayOneShot(kirinFootsteps.clip);

        kirinFootsteps.clip = slashClaws[Random.Range(0, slashClaws.Length)];
        kirinFootsteps.PlayOneShot(kirinFootsteps.clip);

        kirinMoves.clip = slashArmor[Random.Range(0, slashArmor.Length)];
        kirinMoves.PlayOneShot(kirinMoves.clip);
    }

    public void PlayAudioJump()
    {
        kirinFootsteps.Stop();
        kirinVoice.Stop();
        kirinMoves.Stop();
        kirinEffects.Stop();

        kirinVoice.clip = jumpVoiceStart[Random.Range(0, jumpVoiceStart.Length)];
        kirinVoice.PlayOneShot(kirinVoice.clip);

        kirinFootsteps.clip = jumpStartFeet[Random.Range(0, jumpStartFeet.Length)];
        kirinFootsteps.PlayOneShot(kirinFootsteps.clip);

        kirinFootsteps.clip = jumpStartClaw[Random.Range(0, jumpStartClaw.Length)];
        kirinFootsteps.PlayOneShot(kirinFootsteps.clip);

        kirinMoves.clip = jumpStartArmor[Random.Range(0, jumpStartArmor.Length)];
        kirinMoves.PlayOneShot(kirinMoves.clip);
    }

    public void PlayAudioJumpLand()
    {
        kirinFootsteps.Stop();
        kirinVoice.Stop();
        kirinMoves.Stop();
        kirinEffects.Stop();

        kirinVoice.clip = jumpVoiceLand[Random.Range(0, jumpVoiceLand.Length)];
        kirinVoice.PlayOneShot(kirinVoice.clip);

        kirinFootsteps.clip = jumpLandFeet[Random.Range(0, jumpLandFeet.Length)];
        kirinFootsteps.PlayOneShot(kirinFootsteps.clip);

        kirinFootsteps.clip = jumpLandClaw[Random.Range(0, jumpLandClaw.Length)];
        kirinFootsteps.PlayOneShot(kirinFootsteps.clip);

        kirinMoves.clip = jumpLandArmor[Random.Range(0, jumpLandArmor.Length)];
        kirinMoves.PlayOneShot(kirinMoves.clip);

    }



    #endregion

}
