using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Screens;

namespace CIS_580;

public class MainMenu: GameScreen
{
    private new Game1 Game => (Game1) base.Game;
    
    private SpriteFont spriteFont;

    public override void LoadContent()
    {
        spriteFont = Content.Load<SpriteFont>("font");
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        //_position = Vector2.Lerp(_position, Mouse.GetState().Position.ToVector2(), 1f * gameTime.GetElapsedSeconds());
    }

    public override void Draw(GameTime gameTime)
    {
        Game.GraphicsDevice.Clear(Color.CornflowerBlue);
        Game._spriteBatch.Begin();
        //Game._spriteBatch.Draw(_logo, _position, Color.White);
        Game._spriteBatch.DrawString(spriteFont,"Menu!",new Vector2(10,10),Color.Black);
        Game._spriteBatch.End();
    }

    public MainMenu(Game game) : base(game)
    {
    }
}