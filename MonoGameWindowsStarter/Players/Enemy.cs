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
    //created interface so we could easily lump all enemies together
    //And can be updated easily since they are all an "Enemy"
    public interface IEnemy
    {
        void Update(Player player, GameTime gametime);

        void LoadContent(ContentManager cm, string name);

        bool dead { get; set; }

        Vector2 Position { get; set; }
    }
}
