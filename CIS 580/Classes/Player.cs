namespace CIS_580;

public class Player
{
    private int score;
    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            shot = true;
            this.Streak++;
            ScoreIncremented = true;
        }
    }

    private int hp;
    public int HP
    {
        get { return hp; }
        set { hp = value; }
    }

    public bool ScoreIncremented = false;
    
    private bool shot;
    public bool Shot
    {
        get { return shot; }
        set { shot = value; }
    }

    private bool isAlive;
    public bool IsAlive
    {
        get { return isAlive;}
        set { isAlive = value; }
    }

    private int streak;
    public int Streak
    {
        get { return streak; }
        set { streak = value; }
    }

    private int maxStreak = 0;
    public int MaxStreak
    {
        get { return maxStreak; }
        set { maxStreak = value; }
    }

    public Player()
    {
        Score = 0;
        HP = 3;
        IsAlive = true;
    }
}