using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.TextureAtlases;


namespace CIS_580;

public class Ball
{
    private Vector2 position;
    public Vector2 Position
    {
        get { return position; }
        set { position = value; }
    }
    
    private Vector2 velocity;
    public Vector2 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }
    
    private Texture2D texture;
    public Texture2D Texture
    {
        get { return texture; }
        set { texture = value; }
    }
    private bool isAlive = true;
    public bool IsAlive
    {
        get { return isAlive; }
        set { isAlive = value; }
    }
    
    private double tolerance = 0;

    public double Tolerance
    {
        get { return tolerance; }
        set { tolerance = value; }
    }
    
    private Vector2 ballCenter;
    public Vector2 BallCenter
    {
        get { return ballCenter; }
        set { ballCenter = new Vector2(position.X + 32, position.Y + 32);}
    }

    private float radius;
    public float Radius
    {
        get { return radius; }
        set { radius = value; }
    }

    private Body body;
    public Body Body
    {
        get { return body; }
        set { body = value; }
    }

    private ParticleEffect deathParticle;

    public ParticleEffect DeathParticle
    {
        get { return deathParticle; }
        set { deathParticle = value; }
    }
    
    public Ball(GraphicsDevice graphicsDevice)
    {
        trailParticle = CreateParticle(graphicsDevice, Color.MonoGameOrange,1);
    }
    
    private bool clicked = false;
    private ContentManager _contentManager;
    private SoundEffect _sfx;

    public void LoadContent(ContentManager contentManager)
    {
        _contentManager = contentManager;
        _sfx = _contentManager.Load<SoundEffect>("explosion");
    }

    public void Update(Player player, GameTime time)
    {
        
        Vector2 ballCenter = new Vector2(position.X + radius, position.Y + radius);
        if (isAlive)
        {
            position = body.Position;

            if (position.Y >= Globals.ScreenHeight)//After it reaches bottom of the screen it dies and deducts score
            {
                player.HP--;
                isAlive = false;
            }

            if (Vector2.Distance(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), ballCenter) < tolerance && Mouse.GetState().LeftButton == ButtonState.Pressed )//Dies when the player click it
            {
                if (!clicked)
                {
                    player.Score++;
                    _sfx.Play();
                    isAlive = false;
                } 
                //clicked = true;
            }
            
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                clicked = true;
            }

            if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                clicked = false;
            }

            DeathParticle.Position = new Vector2(position.X + radius,position.Y + radius);
        }

    }

    public void Draw(SpriteBatch spriteBatch,GameTime time)//Draws trail particle
    {
        spriteBatch.Draw(trailParticle);
        trailParticle.Update((float)time.ElapsedGameTime.TotalSeconds);
        trailParticle.Position = new Vector2(position.X + radius ,position.Y + radius);
    }

    private ParticleEffect trailParticle;
    
    public double timer = 0;
    public void Die(SpriteBatch spriteBatch,GameTime time)//Plays death particle for half a second
    {
        timer += time.ElapsedGameTime.TotalSeconds;
        if (timer <= 0.5)
        {
            spriteBatch.Draw(DeathParticle);
            DeathParticle.Update((float)time.ElapsedGameTime.TotalSeconds);
        }
    }

    public ParticleEffect CreateParticle(GraphicsDevice graphicsDevice,Color color,double lifetime)
    {
        Texture2D newTexture = new Texture2D(graphicsDevice,1,1) ;
        ParticleEffect newParticle = new ParticleEffect();
        
        newTexture.SetData(new[] { color });

        TextureRegion2D textureRegion = new TextureRegion2D(newTexture);
        newParticle = new ParticleEffect(autoTrigger: false)
        {
            Position = new Vector2(-1000,-1000),
            Emitters = new List<ParticleEmitter>
            {
                new ParticleEmitter(textureRegion, 1000, TimeSpan.FromSeconds(lifetime),
                    Profile.Circle(this.radius,Profile.CircleRadiation.Out))
                {
                    Parameters = new ParticleReleaseParameters
                    {
                        Speed = new Range<float>(50f, 50f),
                        Quantity = 3,
                        Rotation = new Range<float>(-1f, 1f),
                        Scale = new Range<float>(3.0f, 4.0f)
                    },
                    Modifiers =
                    {
                        new AgeModifier
                        {
                            Interpolators =
                            {
                                new ColorInterpolator
                                {
                                    StartValue = new HslColor(0.33f, 0.5f, 0.5f),
                                    EndValue = new HslColor(0.5f, 0.9f, 1.0f)
                                }
                            }
                        },
                        new RotationModifier {RotationRate = -2.1f},
                        new RectangleContainerModifier {Width = 50, Height = 50},
                        new LinearGravityModifier {Direction = -Vector2.UnitY, Strength = 30f},
                    }
                }
            }
        };
        
        return newParticle;
    }
    
}