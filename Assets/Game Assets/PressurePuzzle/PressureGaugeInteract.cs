using UnityEngine;

public class PressureGaugeInteract : ObjectProperties
{

    private PressurePuzzleManager puzzleManager;
 
    public override void Start()
    {
        base.Start();
        puzzleManager = FindObjectOfType<PressurePuzzleManager>();
    }
 
    // Call this from player interaction tool when it confirms a left-click
    // on an object whose ObjectProperties.m_hovered == true.
    public void OnInteract()
    {
        puzzleManager.OnPlayerInteract();
    }

 
}
