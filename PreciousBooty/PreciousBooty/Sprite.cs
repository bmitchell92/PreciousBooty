using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PreciousBooty
{
    class Sprite
    {
        Game1 game;
        public bool Alive;
        Texture2D skin;
        Vector2 position;
        float theta;
        Vector2 origin;
        Rectangle collisionBox;

        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public float Theta
        {
            get
            {
                return theta;
            }
            set
            {
                theta = value;
            }
        }

        public Sprite(Game1 game, string assetPath, Vector2 position, float initalTheta)
        {
            this.game = game;
            skin = game.Content.Load<Texture2D>(assetPath);
            this.position = position;
            theta = initalTheta;
            origin = new Vector2(skin.Width / 2, skin.Height / 2);
            collisionBox = new Rectangle((int)position.X, (int)position.Y, skin.Width, skin.Height);
            Alive = true;
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw()
        {
            if (Alive)
            {
                game.spriteBatch.Draw(skin, position,new Rectangle(0,0,skin.Width,skin.Height), Color.White, theta, origin, 1.0f, SpriteEffects.None, 0);
            }
        }


    }
}
