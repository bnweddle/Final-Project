/* Following Tutorial at 
 * https://www.codeproject.com/Articles/417272/How-to-perform-fading-transitions-on-Textures-in-X
 * */
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elemancy.Transitions
{
    #region Sequence Complete Event Delegate
    internal delegate void FadeSequenceComplete();
    #endregion
    /// <summary>
    /// A class that utilises the InterpolationTimer class to fade a texture in and out.
    /// </summary>
    internal class FadingTextures : DrawableGameComponent
    {
        #region Private Members
        /// <summary>
        /// Used only by this class, hence inside the scope of the class.
        /// Keeps track of the current fade state.
        /// </summary>
        enum FadeState { InitialDelay, FadingIn, Opaque, FadingOut, Complete };
        private Texture2D _splashTexture;
        private Elemancy.Game _game;
        private Vector2 _position;
        private TimeSpan _fadeInTime, _fadeOutTime, _opaqueTime, _initialDelayTime;
        private FadeState _currentState;
        private float _opacity;
        #endregion
        #region Internally-accessible Properties & Members
        // timers
        internal InterpolationTimer FadeInTimer, OpaqueTimer, FadeOutTimer, InitialDelayTimer;
        /// <summary>
        /// This event will fire once the entire sequence (initial delay + fade in + opaque + fade out) is completed.
        /// If you wish, you can use this event to call Reset() which sets the sequence to fire again at a later time.
        /// </summary>
        internal event FadeSequenceComplete OnFadeSequenceCompleted;
        /// <summary>
        /// Gets the sum duration of the entire sequence.
        /// </summary>
        internal TimeSpan TotalDuration
        {
            get
            {
                return _fadeInTime + _fadeOutTime + _opaqueTime + _initialDelayTime;
            }
        }
        #endregion
        #region Constructors
        /// <summary>
        /// Creates a FadingTexture sequence WITHOUT an initial delay (eg, fade in will begin immediately).
        /// </summary>
        /// <param name="game">Obvious.</param>
        /// <param name="position">The *top-left* coordinates of the texture.</param>
        /// <param name="texture">The texture to fade in and out.</param>
        /// <param name="fadeInTime">How long you want the fade in transition to last.</param>
        /// <param name="opaqueTime">How long you want the texture to be fully opaque.</param>
        /// <param name="fadeOutTime">How long you want the fade out transition to last.</param>
        internal FadingTextures(Elemancy.Game game, Vector2 position, Texture2D texture, TimeSpan fadeInTime, TimeSpan opaqueTime, TimeSpan fadeOutTime)
            : base(game)
        {
            _game = game;
            // add to game components
            if (!_game.Components.Contains(this))
                _game.Components.Add(this);
            // make sure this draws on top of everything else
            DrawOrder = 30000;
            // set members according to constructor params
            _splashTexture = texture;
            _fadeInTime = fadeInTime;
            _opaqueTime = opaqueTime;
            _fadeOutTime = fadeOutTime;
            _initialDelayTime = TimeSpan.Zero;
            _currentState = FadeState.FadingIn;
            _position = position;
            // call initialize, since the game may not call it if it instantiated at the wrong time (I still don't get how that works...)
            Initialize();
        }
        /// <summary>
        /// Creates a FadingTexture sequence WITH an initial delay (eg, fade in will begin *AFTER* the initial delay has passed).
        /// </summary>
        /// <param name="game">Obvious.</param>
        /// <param name="position">The *top-left* coordinates of the texture.</param>
        /// <param name="texture">The texture to fade in and out.</param>
        /// <param name="initialDelayTime">How long you want the class to wait before starting the fade in transition.</param>
        /// <param name="fadeInTime">How long you want the fade in transition to last.</param>
        /// <param name="opaqueTime">How long you want the texture to be fully opaque.</param>
        /// <param name="fadeOutTime">How long you want the fade out transition to last.</param>
        internal FadingTextures(Elemancy.Game game, Vector2 position, Texture2D texture, TimeSpan initialDelayTime, TimeSpan fadeInTime, TimeSpan opaqueTime, TimeSpan fadeOutTime)
            : base(game)
        {
            _game = game;
            // add to game components
            if (!_game.Components.Contains(this))
                _game.Components.Add(this);
            // make sure this draws on top of everything else
            DrawOrder = 30000;
            // set members according to constructor params
            _splashTexture = texture;
            _fadeInTime = fadeInTime;
            _opaqueTime = opaqueTime;
            _fadeOutTime = fadeOutTime;
            _initialDelayTime = initialDelayTime;
            _currentState = FadeState.InitialDelay;
            _position = position;
            // call initialize, since the game may not call it if it instantiated at the wrong time (I still don't get how that works...)
            Initialize();
        }
        #endregion
        #region Overridden XNA Methods (Initialize, Update & Draw)
        public override void Initialize()
        {
            base.Initialize();
            // if we have an initial delay, create the initial delay timer.
            if (_currentState == FadeState.InitialDelay)
            {
                InitialDelayTimer = new InterpolationTimer(_initialDelayTime);
                InitialDelayTimer.OnTimerFinished += new TimerFinished(InitialDelayTimer_OnTimerFinished);
            }
            // set up timers and their events
            FadeInTimer = new InterpolationTimer(_fadeInTime, 0.0f, 1.0f);
            FadeInTimer.OnTimerFinished += new TimerFinished(FadeInTimer_OnTimerFinished);
            OpaqueTimer = new InterpolationTimer(_opaqueTime);
            OpaqueTimer.OnTimerFinished += new TimerFinished(OpaqueTimer_OnTimerFinished);
            FadeOutTimer = new InterpolationTimer(_fadeOutTime, 1.0f, 0.0f);
            FadeOutTimer.OnTimerFinished += new TimerFinished(FadeOutTimer_OnTimerFinished);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // respond appropriately to the current state
            switch (_currentState)
            {
                case FadeState.InitialDelay:
                    if (!InitialDelayTimer.IsRunning)
                        InitialDelayTimer.Start();
                    InitialDelayTimer.Update(gameTime.ElapsedGameTime);
                    _opacity = 0.0f; // going to be fully transparent at this stage
                    break;
                case FadeState.FadingIn:
                    if (!FadeInTimer.IsRunning)
                        FadeInTimer.Start();
                    FadeInTimer.Update(gameTime.ElapsedGameTime);
                    _opacity = FadeInTimer.CurrentValue;
                    break;
                case FadeState.Opaque:
                    if (!OpaqueTimer.IsRunning)
                        OpaqueTimer.Start();
                    OpaqueTimer.Update(gameTime.ElapsedGameTime);
                    _opacity = 1.0f;
                    break;
                case FadeState.FadingOut:
                    if (!FadeOutTimer.IsRunning)
                        FadeOutTimer.Start();
                    FadeOutTimer.Update(gameTime.ElapsedGameTime);
                    _opacity = FadeOutTimer.CurrentValue;
                    break;
            }
            // prevent calls to Draw() unless the texture is visible
            if (_opacity > 0.0f)
                Visible = true;
            else
                Visible = false;
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            _game.spriteBatch.Begin();
            _game.spriteBatch.Draw(_splashTexture, _position, Color.White * _opacity);
            _game.spriteBatch.End();
        }
        #endregion
        #region Custom Methods
        /// <summary>
        /// Resets the current sequence's InitialDelayTimer to delay and begins anew.
        /// </summary>
        /// <param name="delay">The time to wait before starting the fade sequence again.</param>
        internal void Reset(TimeSpan delay)
        {
            if (_currentState != FadeState.Complete)
                throw new Exception("Please wait until the sequence is complete before calling Reset!");
            _initialDelayTime = delay;
            // make sure InitialDelayTimer is not null (which would be the case if no initial delay was specified in the constructor)
            if (InitialDelayTimer == null)
            {
                InitialDelayTimer = new InterpolationTimer(delay);
                InitialDelayTimer.OnTimerFinished += new TimerFinished(InitialDelayTimer_OnTimerFinished);
            }
            else
                InitialDelayTimer.SetTimeLimit(delay);
            _currentState = FadeState.InitialDelay;
        }
        #endregion
        #region InterpolationTimer Events
        void FadeOutTimer_OnTimerFinished()
        {
            _currentState = FadeState.Complete;
            // fire timer complete event if it is assigned to
            if (OnFadeSequenceCompleted != null)
                OnFadeSequenceCompleted();
        }
        void OpaqueTimer_OnTimerFinished()
        {
            _currentState = FadeState.FadingOut;
        }
        void FadeInTimer_OnTimerFinished()
        {
            _currentState = FadeState.Opaque;
        }
        void InitialDelayTimer_OnTimerFinished()
        {
            _currentState = FadeState.FadingIn;
        }
        #endregion
    }
}
