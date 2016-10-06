using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;


namespace PreciousBooty
{
    public class Bomb: GameObject
    {
        public Bomb(Game1 game, Vector3 position, string assetPath, bool alive, float MinOffsetX, float MinOffsetY, float MinOffsetZ, float MaxOffsetX, float MaxOffsetY, float MaxOffsetZ)
            : base(game, position, assetPath, alive, MinOffsetX, MinOffsetY, MinOffsetZ, MaxOffsetX, MaxOffsetY, MaxOffsetZ)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (game.playerManager.player.box.Intersects(this.box) && Alive)
            {
                game.playerManager.player.Health -= 100;
                game.soundBank.PlayCue("explosion");
                Alive = false;
            }
        }
    }
}
