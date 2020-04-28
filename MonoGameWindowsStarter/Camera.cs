using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elemancy
{
    /// <summary>
    /// Using the camera for creating Zoom capability to satisfy the 
    /// Sprite.Begin and Matrices requirement
    /// </summary>
    public class Camera
    {
        Vector2 Position;
        Matrix matrix;
        Vector2 Origin;

        public Matrix Matrix
        {
            get { return matrix; }
        }

        public void Update(Player player, Viewport view)
        {
            Origin = new Vector2(view.Width / 2, view.Height / 2);
            //Zooms to the position of the mouse
            Position.X = player.Position.X;
            Position.Y = player.Position.Y;

            matrix = Matrix.CreateTranslation(new Vector3(-Position, 0.0f)) *
               Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
               Matrix.CreateTranslation(new Vector3(Origin, 0.0f)); //Add Origin
        }
    }
}
