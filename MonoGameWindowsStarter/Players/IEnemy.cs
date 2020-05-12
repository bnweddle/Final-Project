using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Elemancy.Parallax;
using Elemancy.Transitions;

namespace Elemancy
{
    //created interface so we could easily lump all enemies together
    //And can be updated easily since they are all an "Enemy"
    public interface IEnemy
    {
        void Update(Player player, GameTime gametime);

        void LoadContent(ContentManager content);

        void SetUpEnemy(GameState level);

        void UpdateHealth(int damage);

        void RestoreHealth(int enemy);

        bool Dead { get; set; }

        bool Hit { get; set; }

        bool IsActive { get; set; }

        int Health { get; set; }

        BoundingRectangle Bounds { get; set; }

    }
}
