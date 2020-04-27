/* Following Tutorial at 
 * https://www.codeproject.com/Articles/417272/How-to-perform-fading-transitions-on-Textures-in-X
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elemancy.Transitions
{
    #region Timer Finished Event Delegate
    internal delegate void TimerFinished();
    #endregion
    /// <summary>
    /// A Timer class that can automatically interpolate between two values over the specified time, or simply run for the specified time. Raises an event when complete.
    /// </summary>
    internal class InterpolationTimer
    {
        #region Private Members
        private TimeSpan _timeLimit;
        private TimeSpan _timeElapsed;
        private bool _isRunning = false;
        private float _deltaPerMillisecond;
        private float _currentValue;
        private float _minValue, _maxValue;
        private bool _interpolate;
        #endregion
        #region Internally-accessible Properties
        /// <summary>
        /// Gets the Time Limit of this timer. To Set the time limit, please use SetTimeLimit().
        /// </summary>
        internal TimeSpan TimeLimit
        {
            get { return _timeLimit; }
        }
        /// <summary>
        /// Gets the current time elapsed of this timer.
        /// </summary>
        internal TimeSpan TimeElapsed
        {
            get { return _timeElapsed; }
        }
        /// <summary>
        /// Returns true if the timer is currently running.
        /// </summary>
        internal bool IsRunning
        {
            get { return _isRunning; }
        }
        /// <summary>
        /// This event will be fired once the timer is complete.
        /// </summary>
        internal event TimerFinished OnTimerFinished;
        /// <summary>
        /// This value is calculated by the class.
        /// It is the delta value of CurrentValue, per millisecond, that the timer uses to interpolate based on gametime.
        /// </summary>
        internal float DeltaPerMillisecond
        {
            get
            {
                if (_interpolate)
                    return _deltaPerMillisecond;
                else
                    throw new NullReferenceException("You cannot retrieve DeltaPerMillisecond from a non-interpolated timer!");
            }
        }
        /// <summary>
        /// The current value of interpolation.
        /// </summary>
        internal float CurrentValue
        {
            get
            {
                if (_interpolate)
                    return _currentValue;
                else
                    throw new NullReferenceException("You cannot retrieve CurrentValue from a non-interpolated timer!");
            }
        }
        #endregion
        #region Constructors
        /// <summary>
        /// Creates a Timer that interpolates currentValue between minValue and maxValue over timeLimit.
        /// Raises an event when the timer is finished. Very useful for fading textures in/out, or moving along an axis at a steady speed over time.
        /// To interpolate backwards, simply input the higher value as the minValue.
        /// </summary>
        /// <param name="timeLimit">A TimeSpan variable containing the total time that the timer is to run before finishing.</param>
        /// <param name="minValue">The minimum value to interpolate from.</param>
        /// <param name="maxValue">The maximum value to interpolate to.</param>
        internal InterpolationTimer(TimeSpan timeLimit, float minValue, float maxValue)
        {
            _timeLimit = timeLimit;
            _timeElapsed = TimeSpan.Zero;
            // calculate the amount to interpolate by per millisecond of gametime.
            _deltaPerMillisecond = (maxValue - minValue) / (float)TimeLimit.TotalMilliseconds;
            _minValue = minValue;
            _maxValue = maxValue;
            _interpolate = true;
        }
        /// <summary>
        /// Creates a Timer that raises an event once the time limit has been reached.
        /// </summary>
        /// <param name="timeLimit">A TimeSpan variable containing the total time that the timer is to run before finishing.</param>
        internal InterpolationTimer(TimeSpan timeLimit)
        {
            _timeLimit = timeLimit;
            Stop();  // make sure it's not running
            _timeElapsed = TimeSpan.Zero;
            _interpolate = false;
        }
        #endregion
        #region Timer Control Methods
        /// <summary>
        /// Starts the timer.
        /// </summary>
        internal void Start()
        {
            _timeElapsed = TimeSpan.Zero;
            _isRunning = true;
            if (_interpolate)
            {
                // set the current value to the minimum
                _currentValue = _minValue;
            }
        }
        /// <summary>
        /// Stops the timer. Automatically called when the elapsed time reaches the time limit.
        /// </summary>
        internal void Stop()
        {
            _timeElapsed = TimeSpan.Zero;
            _isRunning = false;
        }
        /// <summary>
        /// Update the time limit for an interpolating timer. Will throw an exception if the timer is running.
        /// You must provide minValue and maxValue so the DeltaPerMillisecond can be recalculated based on the new time limit!
        /// </summary>
        /// <param name="newTimeLimit">The new time limit.</param>
        /// <param name="minValue">Minimum value for interpolation.</param>
        /// <param name="maxValue">Maximum value for interpolation.</param>
        internal void SetTimeLimit(TimeSpan newTimeLimit, float minValue, float maxValue)
        {
            if (!_interpolate)
            {
                SetTimeLimit(newTimeLimit);
                return;
            }
            if (IsRunning)
                throw new ApplicationException("You must stop the timer before changing the time limit!");
            // recalculate deltapermillisecond
            _deltaPerMillisecond = (maxValue - minValue) / (float)newTimeLimit.TotalMilliseconds;
            _timeLimit = newTimeLimit;
        }
        /// <summary>
        /// Update the time limit for a non-interpolating timer.
        /// NOTE: Will throw an exception if this IS an interpolating timer or if the timer is running.
        /// </summary>
        /// <param name="newTimeLimit"></param>
        internal void SetTimeLimit(TimeSpan newTimeLimit)
        {
            if (_interpolate)
                throw new NullReferenceException("You must re-specify the minimum and maximum values for an interpolation timer! See - SetTimeLimit(time, minValue, maxValue)");
            if (IsRunning)
                throw new ApplicationException("You must stop the timer before changing the time limit!");
            _timeLimit = newTimeLimit;
        }
        #endregion
        #region Update Method
        /// <summary>
        /// Call this from your game's update method. Or if you are using this class with my FadingTexture class, it will call this automatically for you.
        /// </summary>
        /// <param name="elapsedTime">The gameTime.ElapsedGameTime value (from your Update Method)</param>
        internal void Update(TimeSpan elapsedTime)
        {
            if (IsRunning)
            {
                // add elapsed time
                _timeElapsed += elapsedTime;
                // if we are interpolating between minValue and maxValue, do it here
                if (_interpolate)
                    _currentValue += DeltaPerMillisecond * (float)elapsedTime.TotalMilliseconds;
                // check if we have matched or exceeded the time limit, if so, 
                // stop the timer and raise the OnTimerFinished event if it is assigned to
                if (_timeElapsed >= _timeLimit)
                {
                    // make sure final value is what it should be (value is always *slightly* lower (like 0.0005 off for me!)
                    if (_currentValue != _maxValue)
                        _currentValue = _maxValue;
                    // stop the timer
                    Stop();
                    // trigger the event if it is assigned to
                    if (OnTimerFinished != null)
                        OnTimerFinished();
                }
            }
        }
        #endregion
    }
}
