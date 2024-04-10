using System;
using CIS_580.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Font;
using MLEM.Formatting;
using MLEM.Misc;
using MLEM.Textures;
using MLEM.Ui;
using MLEM.Ui.Elements;
using MLEM.Ui.Style;
using MonoGame.Extended.Screens;

namespace CIS_580;

public class MainMenu: GameScreen
{
    private new Game1 Game => (Game1) base.Game;
    public UiSystem UiSystem;
    private Texture2D _pixel;
    public bool Play;
    private Paragraph _difficulty ;
    private Paragraph _header;

    public override void LoadContent()
    {
        Play = false;
        UiSystem = new UiSystem(Game,  new UntexturedStyle(Game._spriteBatch));
        
        _pixel = new Texture2D(GraphicsDevice, 1, 1);//"Progress bar" which i used for player streak
        _pixel.SetData([Color.White]);

        #region UI

        MlemPlatform.Current = new MlemPlatform.DesktopGl<TextInputEventArgs>((w, c) => w.TextInput += c);
        
        UntexturedStyle style = new UntexturedStyle(Game._spriteBatch) {
            Font = new GenericSpriteFont(this.Content.Load<SpriteFont>("font")),
            ButtonTexture = new NinePatch(_pixel, padding: 1)
        };
        
        var box = new Panel(Anchor.AutoCenter, new Vector2(900, 9000), Vector2.Zero, setHeightBasedOnChildren: true)//Panel in which are the elements stored
        {
            DrawColor = Color.Black,
        };
        _header = new Paragraph(Anchor.TopCenter, 1, "Main menu")
        {
            Alignment = TextAlignment.Center
        };
        _difficulty = new Paragraph(Anchor.TopCenter, 1, "\nMedium")
        {
            Alignment = TextAlignment.Center,
            TextColor = Color.Yellow
            
        };
        box.AddChild(_header);
        box.AddChild(_difficulty);
        box.AddChild(new Button(Anchor.AutoCenter, new Vector2(0.5f, 90), "Play") {//Picker for difficulties
            OnPressed = element => Play = true,
            PositionOffset = new Vector2(0, 100),
            NormalColor = Color.MonoGameOrange,
            HoveredColor = Color.Orange
        });

        var difficultySelector = new Dropdown(Anchor.AutoCenter, new Vector2(0.5f, 60), "Difficulty");
        difficultySelector.AddElement("Easy", element => SetDifficulty(Difficulty.Easy,difficultySelector));
        difficultySelector.AddElement("Medium",element => SetDifficulty(Difficulty.Medium,difficultySelector));
        difficultySelector.AddElement("Hard",element => SetDifficulty(Difficulty.Hard,difficultySelector));
        difficultySelector.NormalColor = Color.Gray;
        box.AddChild(difficultySelector);
        
        UiSystem.Style = style;
        UiSystem.Add("InfoBox", box);
        
        #endregion
        
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        UiSystem.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        Game.GraphicsDevice.Clear(Color.Black);
        Game._spriteBatch.Begin();
        Game._spriteBatch.End();
        UiSystem.Draw(gameTime, Game._spriteBatch);
    }

    public MainMenu(Game game) : base(game)
    {
    }

    private void SetDifficulty(Difficulty difficulty, Dropdown selector)
    {
        Globals.Difficulty.Difficulty = difficulty;
        selector.IsOpen = false;
        _difficulty.Text =  "\n" + difficulty.ToString();

        switch (difficulty)
        {
            case Difficulty.Easy:
                _difficulty.TextColor = Color.LightGreen;
                break;
            case Difficulty.Medium:
                _difficulty.TextColor = Color.Yellow;
                break;
            case Difficulty.Hard:
                _difficulty.TextColor = Color.Red;
                break;
        }
    }
}