using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Elemancy
{
    public class Narrator
    {
        Game game;

        Dictionary<String, SoundEffect> Commentary = new Dictionary<string, SoundEffect>();

        List<SoundEffect> DeathQuips = new List<SoundEffect>();
        int curDeathQuipIndex;

        public Narrator(Game game)
        {
            this.game = game;
        }

        public void Initialize()
        {

        }

        public void LoadContent(ContentManager content)
        {
            Commentary.Add("A dragon in a dungeon", content.Load<SoundEffect>("A dragon in a dungeon"));
            Commentary.Add("All that work", content.Load<SoundEffect>("All that work"));
            Commentary.Add("Eww Thunder", content.Load<SoundEffect>("Eww Thunder"));
            Commentary.Add("Huh not a lot here", content.Load<SoundEffect>("Huh not a lot here"));
            Commentary.Add("Picked a less annoying", content.Load<SoundEffect>("Picked a less annoying"));
            Commentary.Add("Picked a more annoying", content.Load<SoundEffect>("Picked a more annoying"));
            Commentary.Add("Seems pretty basic", content.Load<SoundEffect>("Seems pretty basic"));

            DeathQuips.Add(content.Load<SoundEffect>("Are you sure"));
            //DeathQuips.Add(content.Load<SoundEffect>("C'mon now")); // Why isn't this one cooperating?
            DeathQuips.Add(content.Load<SoundEffect>("Take the controller back"));
            DeathQuips.Add(content.Load<SoundEffect>("That was pretty stupid of you"));
            DeathQuips.Add(content.Load<SoundEffect>("That was the most pro gamer move"));
            DeathQuips.Add(content.Load<SoundEffect>("This is almost torture to watch"));
        }

        public void playCommentary(String key)
        {
            SoundEffect result;
            if (Commentary.TryGetValue(key, out result))
                result.Play();
        }

        public void playDeathQuip()
        {
            DeathQuips[curDeathQuipIndex].Play();
            curDeathQuipIndex++;
            curDeathQuipIndex %= DeathQuips.Count;
        }
    }
}