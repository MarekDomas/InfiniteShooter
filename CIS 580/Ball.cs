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
    
    
    public Ball(Vector2 position, Vector2 velocity, Texture2D texture)
    {
        this.position = position;
        this.velocity = velocity;
        this.texture = texture;
        
        body.OnCollision += (Fixture fixtureA, Fixture fixtureB, Contact contact) =>
        {
            if (fixtureA.Body.Tag == "Ball")
            {
                isAlive = false;
            }

            return true;
        };
    }

    public Ball()
    {
        
    }
    
    private bool clicked = false;
    private ContentManager _contentManager;
    private SoundEffect _sfx;

    public void LoadContent(ContentManager contentManager)
    {
        _contentManager = contentManager;
        _sfx = _contentManager.Load<SoundEffect>("explosion");
    }

    public void Move(Player player, GameTime time)
    {
        
        Vector2 ballCenter = new Vector2(position.X + radius, position.Y + radius);
        if (isAlive)
        {
            //position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //body.Awake = true;
            //body.LinearVelocity = velocity;
            //body.Position = new Vector2(10, body.Position.Y);
            position = body.Position;
            // if (Collides(balls))
            // {
            //     isAlive = false;
            // }

            if (position.Y >= 840)
            {
                player.HP--;
                isAlive = false;
            }

            if (Vector2.Distance(new Vector2(Mouse.GetState().X, Mouse.GetState().Y), ballCenter) < tolerance && Mouse.GetState().LeftButton == ButtonState.Pressed )
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

    public ParticleEffect CreateParticle(GraphicsDevice graphicsDevice)
    {
        Texture2D newTexture = new Texture2D(graphicsDevice,1,1) ;
        ParticleEffect newParticle = new ParticleEffect();
        
        newTexture.SetData(new[] { Color.White });

        TextureRegion2D textureRegion = new TextureRegion2D(newTexture);
        newParticle = new ParticleEffect(autoTrigger: false)
        {
            Position = new Vector2(Mouse.GetState().Position.X,Mouse.GetState().Position.Y),
            Emitters = new List<ParticleEmitter>
            {
                new ParticleEmitter(textureRegion, 500, TimeSpan.FromSeconds(0.3),
                    Profile.Circle(30,Profile.CircleRadiation.Out))
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

    public double timer = 0;
    public void Die(SpriteBatch _spriteBatch,GameTime time)
    {
        timer += time.ElapsedGameTime.TotalSeconds;
        if (timer <= 0.5)
        {
            _spriteBatch.Draw(DeathParticle);
            DeathParticle.Update((float)time.ElapsedGameTime.TotalSeconds);
        }
    }
    
}