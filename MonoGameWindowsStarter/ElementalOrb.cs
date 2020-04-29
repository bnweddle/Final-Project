using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
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

        // Particle for 'Elemental Power' Orbs
        Texture2D particle;

        // Timer for 'Elemental Power' Orbs
        TimeSpan timer = new TimeSpan(0);

        // Timer for 
        TimeSpan activeTimer = new TimeSpan(0);

        // Position for 'Elemental Power' Orbs
        Vector2 Position { get; set; } = Vector2.Zero;

        // Velocity for 'Elemental Power' Orbs
        Vector2 Velocity { get; set; } = Vector2.Zero;

        const double DURATION = 2000;

        Random random = new Random();

        // Elemental Particle System
        public ParticleSystem elementalOrbParticleSystem;

        public enum State
        { 
            Idle = 0,
            ToActivate = 1,
            Active = 2,
        }

        public State state;

        public ElementalOrb(Game game)
        {
            this.game = game;
        }

        public void LoadContent(ContentManager content)
        {

            particle = content.Load<Texture2D>("particle");
            elementalOrbParticleSystem = newElementalOrbParticleSystem(game.GraphicsDevice, 0, Element.None, particle);
        }

        public void Initialize()
        {
            state = State.Idle;
        }

        public void Update(GameTime gameTime, Element element)
        {
            timer += gameTime.ElapsedGameTime;
            if(state == State.ToActivate)
            {
                // Play SFX here

                state = State.Active;
                activeTimer = new TimeSpan(0);
                elementalOrbParticleSystem = newElementalOrbParticleSystem(game.GraphicsDevice, 5000, curElement, particle);
            }
            else if(state == State.Active)
            {
                Position += (float)gameTime.ElapsedGameTime.TotalMilliseconds * Velocity;
                elementalOrbParticleSystem.Update(gameTime);
                if (timer.TotalMilliseconds - activeTimer.TotalMilliseconds > DURATION)
                {
                    state = State.Idle;
                    elementalOrbParticleSystem = newElementalOrbParticleSystem(game.GraphicsDevice, 0, Element.None, particle);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (state == State.Active)
                elementalOrbParticleSystem.Draw(gameTime);
        }

        public void Attack(Vector2 position, Vector2 velocity, Element element)
        {
            Bounds.Radius = 20;
            Bounds.X = position.X + Bounds.Radius;
            Bounds.Y = position.Y + Bounds.Radius;
            Position = new Vector2(Bounds.X, Bounds.Y);
            Velocity = velocity;
            curElement = element;
            state = State.ToActivate;
            timer = new TimeSpan(0);
            System.Diagnostics.Debug.WriteLine("Elemental Orb Activated.");
        }

        public ParticleSystem newElementalOrbParticleSystem(GraphicsDevice graphicsDevice, int size, Element curElement, Texture2D elementParticle)
        {
            elementalOrbParticleSystem = new ParticleSystem(game, size, elementParticle);
            if (size != 0)
            {
                elementalOrbParticleSystem.Emitter = new Vector2(100, 100);
                // Set the SpawnParticle method
                elementalOrbParticleSystem.SpawnParticle = (ref Particle particle) =>
                {
                    Vector2 particlePosition = new Vector2(
                        MathHelper.Lerp(Position.X - 20, Position.X + 20, (float)random.NextDouble()), // X between this X +-20
                        MathHelper.Lerp(Position.Y - 20, Position.Y + 20, (float)random.NextDouble()) // Y between this Y +-20
                        );
                    particle.Position = particlePosition - new Vector2(Bounds.Radius, Bounds.Radius);
                    Vector2 particleVelocity = (particlePosition - Position) * 3;
                    particle.Velocity = particleVelocity;
                    Vector2 particleAcceleration = 0.1f * new Vector2((float)-random.NextDouble(), (float)-random.NextDouble());
                    particle.Acceleration = particleAcceleration;
                    switch (curElement)
                    {
                        case Element.None:
                            particle.Color = Color.Transparent;
                            break;
                        case Element.Electric:
                            particle.Color = Color.White;
                            break;
                        case Element.Fire:
                            particle.Color = Color.Red;
                            break;
                        case Element.Ice:
                            particle.Color = Color.Blue;
                            break;
                        default:
                            particle.Color = Color.Transparent;
                            break;
                    }
                    particle.Scale = 1f;
                    particle.Life = 1.0f;
                };

                // Set the UpdateParticle method
                elementalOrbParticleSystem.UpdateParticle = (float deltaT, ref Particle particle) =>
                {
                    particle.Velocity += deltaT * particle.Acceleration;
                    particle.Position += deltaT * particle.Velocity;
                    particle.Scale -= deltaT;
                    particle.Life -= 2 * deltaT;
                };
            }

            return elementalOrbParticleSystem;
        }
    }
}
