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
    public class Snake: GameObject
    {
        int attackTime;
        int attackCounter;
        Model deadModel;

        public Snake(Game1 game, Vector3 position, string assetPath,string deadModelAssetPath, bool alive, float MinOffsetX, float MinOffsetY, float MinOffsetZ, float MaxOffsetX, float MaxOffsetY, float MaxOffsetZ)
            : base(game, position, assetPath, alive, MinOffsetX, MinOffsetY, MinOffsetZ, MaxOffsetX, MaxOffsetY, MaxOffsetZ)
        {
            attackCounter = 0;
            attackTime = 3000;
            deadModel = game.Content.Load<Model>(deadModelAssetPath);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Alive)
            {
                attackCounter += gameTime.ElapsedGameTime.Milliseconds;
                if (attackCounter > attackTime)
                {
                    attackCounter = attackTime;
                }
            }
            if (game.playerManager.player.box.Intersects(this.box) && Alive)
            {
                if(game.playerManager.player.topCollision(this.box))
                {
                    Alive = false;
                }
                else
                {
                    if (attackCounter >= attackTime)
                    {
                        game.playerManager.player.Health -= 20;
                        game.soundBank.PlayCue("snake");
                        attackCounter = 0;
                    }
                }
            }
        }

        public override void Draw(Camera camera)
        {
            if (Alive)
            {
                base.Draw(camera);
            }
            else
            {
                game.DrawModel(deadModel, Matrix.CreateTranslation(position), camera);
            }
        }
    }
}
