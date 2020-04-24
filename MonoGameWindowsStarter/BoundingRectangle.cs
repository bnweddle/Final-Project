using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elemancy
{
    public struct BoundingRectangle
    {
        public float X;

        public float Y;

        public float Width;

        public float Height;

        /// <summary>
        /// Gets this bounding rectangle
        /// </summary>
        public BoundingRectangle Bounds => this;

        //Constructor
        public BoundingRectangle(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Constructs a new bounding rectangle.  This class is similar to a rectangle,
        /// but uses floats
        /// </summary>
        /// <param name="position">The upper-left corner's position</param>
        /// <param name="width">The width of the rectangle</param>
        /// <param name="height">The height of the rectangle</param>
        public BoundingRectangle(Vector2 position, float width, float height)
        {
            X = position.X;
            Y = position.Y;
            Width = width;
            Height = height;
        }

        public bool CollidesWith(BoundingRectangle other)
        {
            return !(this.X > other.X + other.Width
                || this.X + this.Width < other.X
                || this.Y > other.Y + other.Height
                || this.Y + this.Height < other.Y);

        }

        public static implicit operator Rectangle(BoundingRectangle BR)
        {
            return new Rectangle((int)BR.X, (int)BR.Y, (int)BR.Width, (int)BR.Height);
        }

    }
}
