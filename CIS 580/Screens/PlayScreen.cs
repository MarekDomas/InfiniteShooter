using System;
using System.Collections.Generic;
using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Content;
using MonoGame.Extended.Screens;
using nkast.Aether.Physics2D.Dynamics;

namespace CIS_580;

public class PlayScreen: GameScreen
{
    private new Game1 Game => (Game1) base.Game;

    private SpriteFont _spriteFont;
    private Player _player ;
    private World _world;
    private Texture2D _background;
    private float _delay;
    
    private double _timer;
    private readonly List<Ball> _spawnedBalls = new List<Ball>();
    
    private Texture2D _ballTexture;
    private Vector2 _textSize;
    
    private SoundEffect _shoot;
    public SoundEffect Music;
    private SoundEffectInstance _musicInst;
    private Camera camera;
    private Texture2D _blankTexture;
    
    private static Vector2 msPos;
    private static Vector2 hpPos;
    
    public PlayScreen(Game game) : base(game)
    {
        msPos = new Vector2(Game._graphics.PreferredBackBufferWidth - 300,50);
        hpPos = new Vector2(Game._graphics.PreferredBackBufferWidth - 150, 10);
    }

    public override void Initialize()
    {
        _player = new Player();
        _world = new World(new Vector2(0, 0));
        _delay = 0.7f;
        camera = new Camera(GraphicsDevice);
        camera.Debug.IsVisible = false;
        camera.Position = new Vector2(Globals.ScreenWidth/2,Globals.ScreenHeight/2);//position camera to the centre of screen
        Mouse.SetCursor(MouseCursor.Crosshair);
        Console.WriteLine(Globals.Difficulty.Difficulty);
        base.Initialize();
    }

    public override void LoadContent()
    {
        Game._spriteBatch = new SpriteBatch(GraphicsDevice);
        _ballTexture = Content.Load<Texture2D>("ball");
        _spriteFont = Content.Load<SpriteFont>("font");
        _shoot = Content.Load<SoundEffect>("gun-gunshot-02");
        Music = Content.Load<SoundEffect>("music");
        _background = Content.Load<Texture2D>("bakground");
        _blankTexture = new Texture2D(GraphicsDevice, 1, 1);//"Progress bar" which i used for player streak
        _blankTexture.SetData([Color.White]);
        _musicInst = Music.CreateInstance();
        _musicInst.Play();
        this.camera.LoadContent();
        base.LoadContent();
    }
    
    bool hasSoundPlayed;
    MouseState mouseState = Mouse.GetState();

    private double musicTimer;
    private double streakTimer;
    public override void Update(GameTime gameTime)
    {

        if (StateManager.PlayScreen)
        {

            if (_player.IsAlive)
            {
                mouseState = Mouse.GetState();
                
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (!hasSoundPlayed)
                    {
                        _shoot.Play(); 
                        hasSoundPlayed = true; 
                    }
                }
                else
                {
                    hasSoundPlayed = false;
                }

                float timeStep = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _world.Step(timeStep);//Updating physics
                
                Ball newball =  SpawnBall(gameTime,_delay);//Spawns ball each interval

                if (newball is not null)
                {
                    _spawnedBalls.Add(newball);
                }
                 
                foreach (Ball ball in _spawnedBalls)
                {
                    ball.Update(_player,gameTime);
                }

                #region Streak
                    
                //Manages score streaks
                if (StateManager.PlayerHasWinStreak)
                {
                    streakTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (streakTimer >= 2 && !StateManager.PlayerHasWinStreak)
                {
                    _player.ScoreIncremented = false;
                    _player.Streak = 0;
                    streakTimer = 0;
                    StateManager.PlayerHasWinStreak = false;
                }
                
                if (_player.Streak > 2 && streakTimer <= 10)
                {
                    StateManager.PlayerHasWinStreak = true;
                }
                else
                {
                    StateManager.PlayerHasWinStreak = false;
                }
                
                if (StateManager.PlayerHasWinStreak)
                {
                    if (_player.Shot)
                    {
                        _barWidth = 100;
                        if (_player.Streak > _player.MaxStreak)
                            _player.MaxStreak = _player.Streak;
                        
                        _player.Shot = false;
                    }

                    _barWidth -= 100 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (_barWidth <= 0)
                    {
                        _player.ScoreIncremented = false;
                        _player.Streak = 0;
                        streakTimer = 0;
                        StateManager.PlayerHasWinStreak = false;
                    }
                }
                
                #endregion
                
                //Plays song after it ends
                musicTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (musicTimer > Music.Duration.TotalSeconds)
                {
                    _musicInst = Music.CreateInstance();
                    _musicInst.Play();
                    musicTimer = 0;
                }
            }
        }
        else
        {
            _musicInst.Stop();
        }
        
        camera.Update(gameTime);
    }

    private double timer2;
    private int counter;
    private bool isClicked = false;
    private float _barWidth = 75;

    public override void Draw(GameTime gameTime)
    {
        if (StateManager.PlayScreen)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Game._spriteBatch.Begin(camera,samplerState: SamplerState.PointClamp); //samplerState: SamplerState.PointClamp
            Game._spriteBatch.Draw(_background,new Vector2(0,0),null,Color.White,0,new Vector2(0f,0f),new Vector2(Game._graphics.PreferredBackBufferWidth / (float)_background.Width,Game._graphics.PreferredBackBufferHeight / (float)_background.Height),SpriteEffects.None,1);
            if (_player.IsAlive)
            {
                
                 foreach (Ball ball in _spawnedBalls)
                 {
                     if (ball.IsAlive)
                     {
                         ball.Draw(Game._spriteBatch,gameTime);
                         Game._spriteBatch.Draw(_ballTexture, ball.Position, null, Color.White,0,new Vector2(0,0),ball.Radius/32,SpriteEffects.None,1);
                     }
                     else
                     {
                         ball.Die(Game._spriteBatch,gameTime);
                     }
                 }
                
                 //Timer that increments each second and is showed on the screen
                 timer2 += gameTime.ElapsedGameTime.TotalSeconds;
                 if (timer2 >= 1.0)
                 {
                     counter++;

                     timer2 = 0;
                 }

                 if (_player.HP <= 0)
                 {
                     _player.IsAlive = false;
                 }
                 
                 Game._spriteBatch.DrawString(_spriteFont, "Score: " + _player.Score, new Vector2(10, 10), Color.Black,0,new Vector2(0,0),new Vector2(0.5f,0.5f),SpriteEffects.None,1);
                 Game._spriteBatch.DrawString(_spriteFont, "Time: " + counter,new Vector2(10, 60),Color.Black,0,new Vector2(0,0),new Vector2(0.5f,0.5f),SpriteEffects.None,1);
                 Game._spriteBatch.DrawString(_spriteFont,"Max streak: " + _player.MaxStreak + "X",msPos,Color.Black,0,new Vector2(0,0),new Vector2(0.5f,0.5f),SpriteEffects.None,1);
                 Game._spriteBatch.DrawString(_spriteFont,"Hp: " + _player.HP,hpPos,Color.DarkRed,0,new Vector2(0,0),new Vector2(0.5f,0.5f),SpriteEffects.None,1);

                 if (StateManager.PlayerHasWinStreak)
                 {
                    Game._spriteBatch.DrawString(_spriteFont, _player.Streak + "X",new Vector2(Mouse.GetState().Position.X + 10,Mouse.GetState().Position.Y - 50),Color.DarkRed,0,new Vector2(0,0),new Vector2(0.35f,0.35f),SpriteEffects.None,1);
                    Game._spriteBatch.Draw(_blankTexture,new Vector2(Mouse.GetState().Position.X + 10,Mouse.GetState().Position.Y - 20),null,Color.Black,0,new Vector2(0,0),new Vector2(_barWidth,15),SpriteEffects.None,1);
                 }
            }
            else//Death screen
            {
                Game._spriteBatch.DrawString(_spriteFont,"GAME OVER!", new Vector2(Game._graphics.PreferredBackBufferWidth / 2 - 200,
                    Game._graphics.PreferredBackBufferHeight / 2 - 100),Color.DarkRed,0,new Vector2(0,0),1,SpriteEffects.None,1);
                Game._spriteBatch.DrawString(_spriteFont,"Max streak: " + _player.MaxStreak + "X", msPos,Color.Black,0,new Vector2(0,0),1,SpriteEffects.None,1);
                Game._spriteBatch.DrawString(_spriteFont,"Total score: " + _player.Score,new Vector2(Game._graphics.PreferredBackBufferWidth / 2 - 200,
                    Game._graphics.PreferredBackBufferHeight / 2 + 100) ,Color.White,0,new Vector2(0,0),1,SpriteEffects.None,1);
                Music.Dispose();
            }
            Game._spriteBatch.End();
        }
    }

    public override void UnloadContent()
    {
        foreach (Ball ball in _spawnedBalls)
        {
            ball.DeathParticle.Dispose();
        }
        base.UnloadContent();
    }
    
    public bool CompareVectors(Vector2 vector1, Vector2 vector2, float tolerance)
    {
        // Check if the absolute difference between each component is within the tolerance
        if (Math.Abs(vector1.X - vector2.X) <= tolerance &&
            Math.Abs(vector1.Y - vector2.Y) <= tolerance)
        {
            return true;
        }

        return false;
    }
    
    private Ball SpawnBall(GameTime time,float delay)//Spawns ball with random velocity,position, radius and density
    {
        _timer += time.ElapsedGameTime.TotalSeconds;
        if (_timer >= delay)
        {
            //Console.WriteLine("test");
            Random rand = new Random();
            float posX = 0f;
            float posY = 0f;
            Ball newBall = new Ball(Game.GraphicsDevice);
            newBall.Velocity = new Vector2(0,rand.Next(Globals.Difficulty.Velocity.Item1,Globals.Difficulty.Velocity.Item2));
            newBall.Velocity.Normalize();
            newBall.Velocity *= 50;
            newBall.Texture = _ballTexture;
            
            newBall.Radius = rand.Next(Globals.Difficulty.Radius.Item1,Globals.Difficulty.Radius.Item2);
            posX = rand.Next(0,GraphicsDevice.Viewport.Width - (int)newBall.Radius);
            posY = rand.Next(0,GraphicsDevice.Viewport.Height - (int)newBall.Radius);
            newBall.Position = new Vector2( posX,-posY);
            newBall.Body = _world.CreateCircle(newBall.Radius,rand.Next(Globals.Difficulty.Density.Item1,Globals.Difficulty.Density.Item2),newBall.Position,BodyType.Dynamic);
            newBall.Body.Tag = "Ball";
            //world.ContactManager.OnBroadphaseCollision += Ball;
            newBall.Body.OnCollision += (fixtureA, fixtureB, contact) =>
            {
                if ((string)fixtureB.Body.Tag == "Ball")
                {
                    //newBall.IsAlive = false;
                    //player.Score++;
                }
                //newBall.Velocity *= -1;

                return false;
            };
            newBall.Body.LinearVelocity = newBall.Velocity;
            newBall.Tolerance = newBall.Radius;
            newBall.LoadContent(Content);
            newBall.DeathParticle = newBall.CreateParticle(GraphicsDevice,Color.White,0.3);
            _spawnedBalls.Add(newBall);
            _timer = 0;
            return newBall;
        }

        return null;

    }
}