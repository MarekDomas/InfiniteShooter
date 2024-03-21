using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using MonoGame.Penumbra.DesktopGL;
using Penumbra;

namespace CIS_580;

public class PlayScreen: GameScreen
{
    private new Game1 Game => (Game1) base.Game;

    private SpriteFont _spriteFont;
    private Player _player ;
    private World _world;
    private Texture2D _background;
    private float _delay;
    
    private double _timer = 0;
    private readonly List<Ball> _spawnedBalls = new List<Ball>();
    
    private Texture2D _ballTexture;
    private Vector2 _textSize;
    
    private SoundEffect _shoot;
    public SoundEffect Music;
    private SoundEffectInstance _musicInst;
    
    
    public PlayScreen(Game game) : base(game)
    {

    }

    public override void Initialize()
    {
        _player = new Player();
        _world = new World(new Vector2(0, 0));
        _delay = 0.7f;

        Mouse.SetCursor(MouseCursor.Crosshair);   
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
        _musicInst = Music.CreateInstance();
        _musicInst.Play();
        base.LoadContent();
    }
    
     bool hasSoundPlayed = false;
    MouseState mouseState = Mouse.GetState();

    private double musicTimer = 0;
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
                _world.Step(timeStep);
                
                Ball newball =  SpawnBall(gameTime,_delay);

                if (newball != null)
                {
                    _spawnedBalls.Add(newball);
                }
                 
                foreach (Ball ball in _spawnedBalls)
                {
                    ball.Move(_player,gameTime);
                }
                
                musicTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (musicTimer > Music.Duration.TotalSeconds)
                {
                    _musicInst.Play();
                    musicTimer = 0;
                }
            }
        }
        else
        {
            _musicInst.Stop();
        }

    }

    private double timer2 = 0;
    private int counter = 0;
    private bool isClicked = false;
    public override void Draw(GameTime gameTime)
    {
        if (StateManager.PlayScreen)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Game._spriteBatch.Begin(); //samplerState: SamplerState.PointClamp
            Game._spriteBatch.Draw(_background,new Vector2(0,0),null,Color.White,0,new Vector2(0f,0f),new Vector2((float)Game._graphics.PreferredBackBufferWidth / (float)_background.Width,(float)Game._graphics.PreferredBackBufferHeight / (float)_background.Height),SpriteEffects.None,1);
            if (_player.IsAlive)
            {
                
                 foreach (Ball ball in _spawnedBalls)
                 {
                     if (ball.IsAlive)
                     {
                         Game._spriteBatch.Draw(_ballTexture, ball.Position, null, Color.White,0,new Vector2(0,0),ball.Radius/32,SpriteEffects.None,1);
                     }
                     else
                     {
                         
                         ball.Die(Game._spriteBatch,gameTime);
                     }
                 }
                
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
                 
                 Game._spriteBatch.DrawString(_spriteFont, "Score: " + _player.Score.ToString(), new Vector2(10, 10), Color.Black,0,new Vector2(0,0),new Vector2(0.5f,0.5f),SpriteEffects.None,1);
                 Game._spriteBatch.DrawString(_spriteFont, "Time: " + counter.ToString(),new Vector2(10, 60),Color.Black,0,new Vector2(0,0),new Vector2(0.5f,0.5f),SpriteEffects.None,1);
                 Game._spriteBatch.DrawString(_spriteFont,"Hp: " + _player.HP.ToString(),new Vector2(Game._graphics.PreferredBackBufferWidth - 150,10),Color.DarkRed,0,new Vector2(0,0),new Vector2(0.5f,0.5f),SpriteEffects.None,1);
            }
            else
            {
                Game._spriteBatch.DrawString(_spriteFont,"GAME OVER!", new Vector2(Game._graphics.PreferredBackBufferWidth / 2 - 200,Game._graphics.PreferredBackBufferHeight / 2 - 100 ),Color.DarkRed,0,new Vector2(0,0),1,SpriteEffects.None,1);
                Game._spriteBatch.DrawString(_spriteFont,"Total score: " + _player.Score, new Vector2(Game._graphics.PreferredBackBufferWidth / 2 - 200,Game._graphics.PreferredBackBufferHeight / 2 ),Color.White,0,new Vector2(0,0),1,SpriteEffects.None,1);
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

    private void initBalls(Ball[] balls)
    {
        // Give the ball a random velocity
        Random rand = new System.Random();
        float posX = 0f;
        float posY = 0f;
        for (int i = 0; i < balls.Length; i++)
        {
            Ball newBall = new Ball();
            posX = rand.Next(0,GraphicsDevice.Viewport.Width);
            posY = rand.Next(0,GraphicsDevice.Viewport.Height);
            newBall.Position = new Vector2( posX,-posY);

            newBall.Velocity = new Vector2(0,rand.Next(1,3));
            newBall.Velocity.Normalize();
            newBall.Velocity *= 50;
            newBall.Texture = _ballTexture;

            newBall.Radius = 32;
            newBall.Body = _world.CreateCircle(newBall.Radius,rand.Next(0,2),newBall.Position,BodyType.Dynamic);
            newBall.Body.Tag = "Ball";
            //world.ContactManager.OnBroadphaseCollision += Ball;
            newBall.Body.OnCollision += (Fixture fixtureA, Fixture fixtureB, Contact contact) =>
            {
                if (fixtureB.Body.Tag == "Ball")
                {
                    //newBall.IsAlive = false;
                    //player.Score++;
                }
                //newBall.Velocity *= -1;

                return false;
            };
            newBall.Body.LinearVelocity = newBall.Velocity;
            
            balls[i] = newBall;
        }

    }

    private Ball SpawnBall(GameTime time,float delay)
    {
        _timer += time.ElapsedGameTime.TotalSeconds;
        if (_timer >= delay)
        {
            //Console.WriteLine("test");
            Random rand = new System.Random();
            float posX = 0f;
            float posY = 0f;
            Ball newBall = new Ball();
            newBall.Velocity = new Vector2(0,rand.Next(3,6));
            newBall.Velocity.Normalize();
            newBall.Velocity *= 50;
            newBall.Texture = _ballTexture;
            
            newBall.Radius = rand.Next(20,49);
            posX = rand.Next(0,GraphicsDevice.Viewport.Width - (int)newBall.Radius);
            posY = rand.Next(0,GraphicsDevice.Viewport.Height - (int)newBall.Radius);
            newBall.Position = new Vector2( posX,-posY);
            newBall.Body = _world.CreateCircle(newBall.Radius,rand.Next(0,2),newBall.Position,BodyType.Dynamic);
            newBall.Body.Tag = "Ball";
            //world.ContactManager.OnBroadphaseCollision += Ball;
            newBall.Body.OnCollision += (Fixture fixtureA, Fixture fixtureB, Contact contact) =>
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
            newBall.DeathParticle = newBall.CreateParticle(GraphicsDevice);
            _spawnedBalls.Add(newBall);
            
            
            _timer = 0;
            return newBall;
        }

        return null;

    }
}