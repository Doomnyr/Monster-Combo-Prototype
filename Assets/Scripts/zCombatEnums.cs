public enum CombatTeam
 { 
    Player, 
    Enemy
    }

[System.Serializable]
public struct GridPosition
{
    public int Column; // 0 or 1
    public int Row;    // 0, 1, or 2

    public GridPosition(int column, int row)
    {
        Column = column;
        Row = row;
    }
}