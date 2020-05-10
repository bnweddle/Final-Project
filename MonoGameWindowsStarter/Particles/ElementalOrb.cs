using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elemancy.Parallax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Elemancy
{
    public class ElementalOrb
    {
        Game game;

        /// <summary>
        /// Bounding Circle for Elemental Orb
        /// </summary>
        public BoundingCircle Bounds;

        /// <summary>
        /// Active Element Type of Player
        /// </summary>
        Element curElement;

        Player player;

        TrackingPlayer playerT;

        // Color used for particles and base orb depending on element type
        Color curColor;

        // Texture for base 'Elemental Power' Orb
        Texture2D baseOrb;

        // Texture for 'Elemental Power' Orb Particles
        Texture2D particle;

        // Sound FX for 'Elemental Power' Orb
        SoundEffect orbSFX;

        // Timer for 'Elemental Power' Orbs
        TimeSpan timer = new TimeSpan(0);

        // Timer for 
        TimeSpan activeTimer = new TimeSpan(0);

        // Position for 'Elemental Power' Orbs
        Vector2 Position { get; set; } = Vector2.Zero;

        // Velocity for 'Elemental Power' Orbs
        Vector2 Velocity { get; set; } = Vector2.Zero;

        const double DURATION = 750;

        // Speed Variable for the Elemental Orb
        float speedVar = .5f;

        Random random = new Random();

        // Elemental Particle System
        public ParticleSystem elementalOrbParticleSystem;

        public enum ActiveState
        { 
            Idle = 0,
            ToActivate = 1,
            Active = 2,
        }

        public ActiveState State;

        public ElementalOrb(Game game, Player player)
        {
            this.game = game;
            this.player = player;
            playerT = new TrackingPlayer(player, 1.0f);
        }

        public void LoadContent(ContentManager content)
        {
            baseOrb = content.Load<Texture2D>("baseElementalOrb");
            particle = content.Load<Texture2D>("particle");
            orbSFX = content.Load<SoundEffect>("orbSFX");
            elementalOrbParticleSystem = newElementalOrbParticleSystem(game.GraphicsDevice, 0, Element.None, particle);
        }

        public void Initialize()
        {
            State = ActiveState.Idle;
        }

        public void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime;
            if(State == ActiveState.ToActivate)
            {
                // Play SFX here
                orbSFX.Play(.1f, 0f, 0f);
                State = ActiveState.Active;
                activeTimer = new TimeSpan(0);
                elementalOrbParticleSystem = newElementalOrbParticleSystem(game.GraphicsDevice, 5000, curElement, particle);
            }
            else if(State == ActiveState.Active)
            {
                Position += (float)gameTime.ElapsedGameTime.TotalMilliseconds * Velocity * speedVar;
                Bounds.X = Position.X;
                Bounds.Y = Position.Y;
                elementalOrbParticleSystem.Update(gameTime);
                if (timer.TotalMilliseconds - activeTimer.TotalMilliseconds > DURATION)
                {
                    State = ActiveState.Idle;
                    elementalOrbParticleSystem = newElementalOrbParticleSystem(game.GraphicsDevice, 0, Element.None, particle);
                }
            }

            if (State == ActiveState.Active)
            {
                //System.Diagnostics.Debug.WriteLine($"Elemental Orb Active - Element: {curElement}");
                //System.Diagnostics.Debug.WriteLine($"Elemental Orb Active - X Position: {Position.X}");
                //System.Diagnostics.Debug.WriteLine($"Elemental Orb Active - X Bounds: {Bounds.X}");
                //System.Diagnostics.Debug.WriteLine($"Elemental Orb Active - Color: {curColor}");
                //System.Diagnostics.Debug.WriteLine($"Elemental Orb Active - Particle Color: {elementalOrbParticleSystem.particles[0].Color}");
                //System.Diagnostics.Debug.WriteLine($"Elemental Orb Active - Particle Life: {elementalOrbParticleSystem.particles[0].Life}");
                //for (int i = 0; i < 20; i++)
                //    System.Diagnostics.Debug.WriteLine($"Elemental Orb Active - Particle's X Position: {elementalOrbParticleSystem.particles[i].Position.X}");
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (State == ActiveState.Active)
            {
                spriteBatch.Draw(baseOrb, Bounds, curColor);
                elementalOrbParticleSystem.Draw(gameTime, playerT.Transform);             
            }
        }

        public void Attack(Vector2 position, Vector2 velocity, Element element)
        {
            Bounds.Radius = 20;
            Bounds.X = position.X + Bounds.Radius + 50;
            Bounds.Y = position.Y + Bounds.Radius + 30;
            Position = new Vector2(Bounds.X, Bounds.Y);
            Velocity = velocity;
            curElement = element;
            switch (curElement)
            {
                case Element.None:
                    curColor = Color.Transparent;
                    break;
                case Element.Lightning:
                    curColor = Color.White;
                    break;
                case Element.Fire:
                    curColor = Color.OrangeRed;
                    break;
                case Element.Water:
                    curColor = Color.Blue;
                    break;
            }
            State = ActiveState.ToActivate;
            timer = new TimeSpan(0);
        }

        public ParticleSystem newElementalOrbParticleSystem(GraphicsDevice graphicsDevice, int size, Element curElement, Texture2D elementParticle)
        {
            elementalOrbParticleSystem = new ParticleSystem(game, size, elementParticle);
            //if (size != 0)
            //{
            elementalOrbParticleSystem.Emitter = Position;
            elementalOrbParticleSystem.SpawnPerFrame = 50;
            // Set the SpawnParticle method
            elementalOrbParticleSystem.SpawnParticle = (ref Particle particle) =>
            {
                Vector2 particlePosition = randomRadiusPosition(Bounds.Radius);
                particle.Position = particlePosition;
                Vector2 particleVelocity = (particlePosition - Position) * 2;
                particle.Velocity = particleVelocity;
                Vector2 particleAcceleration = 0.1f * new Vector2((float)-random.NextDouble(), (float)-random.NextDouble());
                particle.Acceleration = particleAcceleration;
                particle.Color = curColor;
                particle.Scale = .5f;
                particle.Life = (float)(random.Next(7500) / 10000) + .75f;
            };

            // Set the UpdateParticle method
            elementalOrbParticleSystem.UpdateParticle = (float deltaT, ref Particle particle) =>
            {
                particle.Velocity += deltaT * particle.Acceleration;
                particle.Position += deltaT * particle.Velocity;
                particle.Scale -= deltaT;
                particle.Life -= 3 * deltaT;
            };
            //}

            return elementalOrbParticleSystem;
        }

        Vector2 randomRadiusPosition(float radius)
        {
            Vector2 result = new Vector2();
            result.X = MathHelper.Lerp(Bounds.X - radius, Bounds.X, (float)random.NextDouble());
            result.Y = MathHelper.Lerp(Bounds.Y - 1.1f * radius, Bounds.Y + 0.7f * radius, (float)random.NextDouble());
            return result;
        }
    }
}
