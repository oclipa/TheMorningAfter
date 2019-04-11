using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MaidController : MonoBehaviour
{
    MaidInstructionsProvider maidInstructions;

    private void Start()
    {
        this.maidInstructions = GetComponentInChildren<MaidInstructionsProvider>();
    }

    /// <summary>
    /// Ye Olde Easter Egg
    /// </summary>
    public void Swear()
    {
        this.maidInstructions.Swear();
    }
}
