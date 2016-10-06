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
    public class GameObject
    {
        protected Game1 game;

        protected Vector3 position;

        protected BoundingBox box;

        public bool Alive { get; set; }

        protected Model model;

        protected float MinOffsetX;

        protected float MinOffsetY;

        protected float MinOffsetZ;

        protected float MaxOffsetX;

        protected float MaxOffsetY;

        protected float MaxOffsetZ;

        protected float rotation;

        protected Matrix world { set; get; }

        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                box = new BoundingBox(new Vector3(position.X - MinOffsetX, position.Y - MinOffsetY, position.Z - MinOffsetZ), new Vector3(position.X + MaxOffsetX, position.Y + MaxOffsetY, position.Z + MaxOffsetZ));
            }
        }

        public BoundingBox Box
        {
            get
            {
                return box;
            }

            set
            {
                box = value;
            }
        }

        public Model Model
        {
            get
            {
                return model;
            }
            set
            {
                model = value;
            }
        }

        public GameObject(Game1 game, Vector3 position, string assetPath, bool alive, float MinOffsetX, float MinOffsetY, float MinOffsetZ, float MaxOffsetX, float MaxOffsetY, float MaxOffsetZ)
        {
            this.game = game;
            this.position = position;
            model = game.Content.Load<Model>(assetPath);
            Alive = alive;
            box = new BoundingBox(new Vector3(position.X - MinOffsetX, position.Y - MinOffsetY, position.Z - MinOffsetZ), new Vector3(position.X + MaxOffsetX, position.Y + MaxOffsetY, position.Z + MaxOffsetZ));

            world = Matrix.CreateTranslation(position);

            this.MinOffsetX = MinOffsetX;
            this.MinOffsetY = MinOffsetY;
            this.MinOffsetZ = MinOffsetZ;
            this.MaxOffsetX = MaxOffsetX;
            this.MaxOffsetY = MaxOffsetY;
            this.MaxOffsetZ = MaxOffsetZ;

            rotation = 0;
        }

        public virtual void Update(GameTime gameTime)
        {
            box = new BoundingBox(new Vector3(position.X - MinOffsetX, position.Y - MinOffsetY, position.Z - MinOffsetZ), new Vector3(position.X + MaxOffsetX, position.Y + MaxOffsetY, position.Z + MaxOffsetZ));

            world = Matrix.CreateRotationY(rotation) * Matrix.CreateTranslation(position);
        }

        public virtual void Draw(Camera camera)
        {
            if (Alive)
            {
                game.DrawModel(model, Matrix.CreateTranslation(position), camera);
            }
        }

    }
}


