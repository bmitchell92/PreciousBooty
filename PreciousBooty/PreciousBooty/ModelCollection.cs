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
    struct Animation
    {
        public Model model;
        public int meshCount;
        public int frames;
        public int frameRate;

        public Animation(Model model, int meshCount, int frames, int frameRate)
        {
            this.model = model;
            this.meshCount = meshCount;
            this.frames = frames;
            this.frameRate = frameRate;
        }
    }

    public class ModelCollection
    {
        Game1 game;

        string currentAnimationName;
        Dictionary<string,Animation> animations;

        public string CurrentAnimationName 
        {
            get
            {
                return currentAnimationName;
            }
        }

        Animation currentAnimation;

        int FrameTime = 0;
        int Frame = 0;

        bool playOnce = false;

        public ModelCollection(Game1 game, string idleAssetPath, int idleMeshCount, int idleFrames, int idleFrameRate)
        {
            this.game = game;
            animations = new Dictionary<string, Animation>();

            animations.Add("Idle", new Animation(game.Content.Load<Model>(idleAssetPath), idleMeshCount, idleFrames, idleFrameRate));
            PlayLoop("Idle");
            currentAnimationName = "Idle";
        }

        public void AddAnimation(string name, string assetPath,int meshCount, int frames, int frameRate)
        {
            animations.Add(name, new Animation(game.Content.Load<Model>(assetPath), meshCount, frames, frameRate));
        }

        public void Update(GameTime gameTime)
        {
            FrameTime += gameTime.ElapsedGameTime.Milliseconds;
            if (FrameTime >= currentAnimation.frameRate)
            {
                FrameTime -= currentAnimation.frameRate;
                Frame += 1;
                if (Frame >= currentAnimation.frames)
                {
                    if (playOnce)
                    {
                        PlayLoop("Idle");
                    }
                    else
                    {
                        Frame = 0;
                        FrameTime = 0;
                    }
                }
            }
        }

        public void PlayOnce(string name)
        {
            currentAnimation = animations[name];
            Frame = 0;
            FrameTime = 0;
            playOnce = true;
            currentAnimationName = name;
        }

        public void PlayLoop(string name)
        {
            currentAnimation = animations[name];
            Frame = 0;
            FrameTime = 0;
            playOnce = false;
            currentAnimationName = name;
        }

        public void Draw(Matrix world, Camera camera)
        {
            
            Matrix[] transforms = new Matrix[currentAnimation.model.Bones.Count];
            currentAnimation.model.CopyAbsoluteBoneTransformsTo(transforms);

            for (int i = (Frame*currentAnimation.meshCount); i < (Frame*currentAnimation.meshCount)+currentAnimation.meshCount; i++ )
            {
                if (i >= currentAnimation.model.Meshes.Count)
                {
                    break;
                }
                foreach (BasicEffect effect in currentAnimation.model.Meshes[i].Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = world;
                    effect.Projection = camera.Projection;
                    effect.View = camera.View;

                }
                currentAnimation.model.Meshes[i].Draw();
            }
 
        }
    }
}
