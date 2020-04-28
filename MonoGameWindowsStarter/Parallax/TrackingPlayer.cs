using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elemancy.Transitions;

namespace Elemancy.Parallax
{
    /// <summary>
    /// Won't need to track enemies, cause when the player reaches enemy scrollling stops
    /// </summary>
    public class TrackingPlayer : IScrollController
    {
        /// <summary>
        /// The Player to track
        /// </summary>
        Player player;

        /// <summary>
        /// How much the parallax layer should scroll relative to the player position
        /// Should probably be a number between 0 and 1, corresponding to 0% to 100%.
        /// </summary>
        public float ScrollRatio { get; set; } = 1.0f;

        /// <summary>
        /// The offset between the scrolling layer and the player
        /// </summary>
        public float Offset = 50;

        /// <summary>
        /// Constructs a new PlayerTrackingScrollController
        /// </summary>
        /// <param name="player">The player to track</param>
        /// <param name="ratio">The scroll ratio for the controlled layer</param>
        public TrackingPlayer(Player player, float ratio)
        {
            this.player = player;
            this.ScrollRatio = ratio;
        }

        /// <summary>
        /// Used if the scrolling should stop
        /// </summary>
        private Matrix transform = Matrix.Identity;

        /// <summary>
        /// Gets the transformation matrix to use with the layer
        /// </summary>
        public Matrix Transform
        {
            get
            {
                float x = ScrollRatio * (Offset - player.Position.X);

                // Will need to tailor according to the levels.

               if (ScrollRatio == 0f)
                {
                    transform.M41 = -1735 + Offset;
                    return transform;
                }

                System.Diagnostics.Debug.WriteLine($"{x } Matrix");
                return Matrix.CreateTranslation(x, 0, 0);

            }
        }                       

        // <summary>
        /// Updates the controller (a no-op in this case)
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime) { }
    }
}
