using System;
using CIS_580.Enums;

namespace CIS_580;

public class DifficultyObj
{
    private (int, int) velocity;
    public (int, int) Velocity
    {
        get => velocity;
        set => velocity = value;
    }

    private (int, int) radius;
    public (int, int) Radius
    {
        get => radius;
        set => radius = value;
    }

    private (int, int) density;
    public (int, int) Density
    {
        get => density;
        set => density = value;
    }

    private Difficulty _difficulty;
    public Difficulty Difficulty
    {
        get => _difficulty;
        set
        {
            _difficulty = value;
            changeDifficulty(value);
        }
    }

    public DifficultyObj(Difficulty difficulty)
    {
        Difficulty = difficulty;
        changeDifficulty(difficulty);
    }

    private void changeDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                Velocity = (1, 4);
                Radius = (35, 55);
                density = (1, 3);
                break;
            case Difficulty.Medium:
                Velocity = (3, 6);
                Radius = (20, 49);
                density = (1, 3);
                break;
            case Difficulty.Hard:
                Velocity = (7, 11);
                Radius = (15, 32);
                density = (1, 3);
                break;
        }
    }
}