using Microsoft.Xna.Framework;

namespace Elemancy.Parallax
{
    public interface IScrollController
    {
        /// <summary>
        /// The current transform matrix to use
        /// </summary>
        Matrix Transform { get; }

        /// <summary>
        /// Updates the transformation matrix
        /// </summary>
        /// <param name="gameTime">The GameTime object</param>
        void Update(GameTime gameTime);
    }
}
