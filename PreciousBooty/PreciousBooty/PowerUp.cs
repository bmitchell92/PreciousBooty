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
    public class PowerUp : GameObject
    {
        bool rotating;

            public PowerUp(Game1 game, Vector3 position, string assetPath, bool alive, float MinOffsetX, float MinOffsetY, float MinOffsetZ, float MaxOffsetX, float MaxOffsetY, float MaxOffsetZ,bool rotating)
            : base(game, position, assetPath, alive, MinOffsetX, MinOffsetY, MinOffsetZ, MaxOffsetX, MaxOffsetY, MaxOffsetZ)
        {
            this.rotating = rotating;
        }

            public override void Update(GameTime gameTime)
            {
                if (rotating)
                {
                    rotation += 0.1f;
                }

                base.Update(gameTime);
            }

            public override void Draw(Camera camera)
            {
                if (Alive)
                {
                    game.DrawModel(model, world, camera);
                }
            }
    }
}
