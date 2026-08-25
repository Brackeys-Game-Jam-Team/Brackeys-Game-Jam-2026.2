public class Player
{
    public int Id { get; }
    public int Score { get; set; }
    public bool IsHuman { get; }

    public Player(int id, bool isHuman)
    {
        Id = id;
        IsHuman = isHuman;
        Score = 0;
    }

    public override string ToString() => $"Player {Id}{(IsHuman ? " (You)" : "")}";
}