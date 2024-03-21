using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;

namespace CIS_580;

public class Game1 : Game
{
    public GraphicsDeviceManager _graphics;
    public SpriteBatch _spriteBatch;
    private GameWindow _gameWindow;
    private PlayScreen _playScreen;
    private MainMenu _mainMenu;
    private Vector2 _mousePos;

    private readonly ScreenManager _screenManager;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _gameWindow = Window;
        _screenManager = new ScreenManager();
        Components.Add(_screenManager);
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _gameWindow.Title = "krek";
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 1330;
        _graphics.PreferredBackBufferHeight = 840;
        _graphics.ApplyChanges();
        _playScreen= new PlayScreen(this);
        _mainMenu = new MainMenu(this);
        StateManager.MenuScreen = true;
        StateManager.PlayScreen = false;
        MainMenu();
        base.Initialize();
    }

    protected override void LoadContent()
    {        
        // TODO: use this.Content to load your game content here
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        // TODO: Add your update logic here
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape) ||Keyboard.GetState().IsKeyDown(Keys.Q) )
            Exit();

        if (Keyboard.GetState().IsKeyDown(Keys.E) && !StateManager.PlayScreen)
        {
            Play();
        }

        
        base.Update(gameTime);
    }

    private double timer2 = 0;
    private int counter = 0;
    private bool isClicked = false;
    protected override void Draw(GameTime gameTime)
    {

        GraphicsDevice.Clear(Color.CornflowerBlue);

        base.Draw(gameTime);
    }

    private void MainMenu()
    {
        _screenManager.LoadScreen(_mainMenu, new FadeTransition(GraphicsDevice, Color.Black));
        StateManager.MenuScreen = true;
        StateManager.PlayScreen = false;
    }

    private void Play()
    {
        _screenManager.LoadScreen(_playScreen, new FadeTransition(GraphicsDevice, Color.Black));
        StateManager.PlayScreen = true;
        StateManager.MenuScreen = false;
    }

}