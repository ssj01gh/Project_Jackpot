using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStepSoundScript : MonoBehaviour
{
    [SerializeField]
    private PlayerScript Player;
    public void PlayPlayerStepSound()
    {
        if (Player == null)
            return;

        Player.PlayPlayerStepSound();
    }
}
