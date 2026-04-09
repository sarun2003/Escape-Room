namespace EscapeRoom
{
    public enum ElectricalPuzzleState
    {
        Locked,     // Panel is sealed — player can't interact yet
        Active,     // Player is solving the wire puzzle
        Solved,     // All wires matched correctly — triggering reward
        Failed,     // A wrong wire was connected — triggering penalty
        Unlocked    // Puzzle complete, panel inert
    }
}
