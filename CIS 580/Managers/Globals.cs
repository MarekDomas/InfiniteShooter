using CIS_580.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CIS_580;

public class Globals
{
    public const int ScreenWidth = 1330;
    public const int ScreenHeight = 840;
    public static GraphicsDeviceManager GraphicsDevice;
    public static DifficultyObj Difficulty = new DifficultyObj(Enums.Difficulty.Medium);
}