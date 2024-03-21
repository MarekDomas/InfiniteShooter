namespace CIS_580;

public class Player
{
    private int score;
    public int Score
    {
        get { return score; }
        set { score = value; }
    }

    private int hp;
    public int HP
    {
        get { return hp; }
        set { hp = value; }
    }

    private bool isAlive;

    public bool IsAlive
    {
        get { return isAlive;}
        set { isAlive = value; }
    }

    public Player()
    {
        Score = 0;
        HP = 3;
        IsAlive = true;
    }
}