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
using System.Text;

namespace PreciousBooty
{
    public class ObjectValuePair : IComparable
    {
        public GameObject obj;
        public float distance;


        public ObjectValuePair(GameObject obj, float distance)
        {
            this.obj = obj;
            this.distance =  distance;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            ObjectValuePair otherTemperature = obj as ObjectValuePair;
            if (otherTemperature != null)
                return this.distance.CompareTo(otherTemperature.distance);
            else
                return 1;
        }
    }
    public class PlayerManager
    {
        Game1 game;

        public int Points { get; set; }
        public int Deaths { get; set; }
        public Player player;
        public Camera camera;
        public int secondspassed = 0;
        int playTimeCounter = 0;
        public bool firstPerson = true;

        public bool hasPistol = false;
        int pistolDropCounter = 0;
        public bool canshoot = true;

        Camera defaultCamera;
        Model pistol;
        Model pistol_fire;
        ModelCollection pirate;
        ModelCollection pirate_pistol;
        ModelCollection navy;
        Matrix viewmodelMatrix = Matrix.CreateTranslation(new Vector3(2, -2, -8));

        public PlayerManager(Game1 game)
        {
            this.game = game;
            player = new Player(game, new Vector3(0,15,350), Vector3.Forward);
            camera = new FirstPersonCamera(game,player,Vector3.Up);
            pistol = game.Content.Load<Model>("Models/1851navy_view");
            pistol_fire = game.Content.Load<Model>("Models/1851navy_view_fire");
            navy = new ModelCollection(game, "Models/1851navy_view", 1, 1, 100);
            navy.AddAnimation("Cock", "Models/1851navy_view_cock", 1, 5, 100);

            pirate = new ModelCollection(game, "Models/pirate_idle", 1, 1, 150);
            pirate.AddAnimation("Walk", "Models/pirate_walk", 1, 4, 150);

            player.model = pirate;
            pirate_pistol = new ModelCollection(game, "Models/pirate_pistol_idle", 2, 1, 150);
            pirate_pistol.AddAnimation("Walk", "Models/pirate_pistol_walk", 2, 4, 150);
            Points = 0;
            Deaths = 0;
            defaultCamera = new Camera(game, Vector3.Zero, Vector3.Forward, Vector3.Up);
        }

        public void Update(GameTime gameTime)
        {
            player.Update(gameTime);
            camera.Update(gameTime);
            defaultCamera.Update(gameTime);

            playTimeCounter += gameTime.ElapsedGameTime.Milliseconds;
            if (playTimeCounter >= 1000)
            {
                playTimeCounter = 0;
                secondspassed += 1;
            }

            if (game.currentState.Buttons.LeftShoulder == ButtonState.Pressed && game.previousState.Buttons.LeftShoulder == ButtonState.Released)
            {
                camera = new ThirdPersonCamera(game, player.Position, player, Vector3.Up);
                firstPerson = false;
            }

            if (game.currentState.Buttons.RightShoulder == ButtonState.Pressed && game.previousState.Buttons.RightShoulder == ButtonState.Released)
            {
                camera = new FirstPersonCamera(game,player,Vector3.Up);
                firstPerson = true;
            }


            if (hasPistol)
            {
                player.model = pirate_pistol;
                if (!canshoot)
                {
                    pistolDropCounter += gameTime.ElapsedGameTime.Milliseconds;
                    if (pistolDropCounter > 500)
                    {
                        canshoot = true;
                        pistolDropCounter = 0;
                    }
                }
                if (game.currentState.Triggers.Right > 0 && game.previousState.Triggers.Right == 0 && canshoot)
                {
                    Fire();
                    canshoot = false;
                    navy.PlayOnce("Cock");
                }

            }
            else
            {
                player.model = pirate;
            }

            navy.Update(gameTime);
        }

        public void Draw()
        {
            player.Draw(camera);
            if (hasPistol)
            {
                if (camera is FirstPersonCamera)
                {
                    navy.Draw(viewmodelMatrix, defaultCamera);
                }
            }
        }

        void Fire()
        {
            List<ObjectValuePair> hits = new List<ObjectValuePair>();

            Ray bulletRay;

            if (camera is FirstPersonCamera)
            {
                bulletRay = new Ray(camera.Position, player.Direction);
                game.soundBank.PlayCue("flintlock_fire");
            }
            else
            {
                ThirdPersonCamera tcamera = (ThirdPersonCamera)camera;
                bulletRay = new Ray(tcamera.eyePosition, Vector3.Normalize(tcamera.eyePosition - camera.Position));
                game.soundBank.PlayCue("flintlock_fire_3");
            }

            foreach (GameObject obj in game.objectManager.objects)
            {

                if (bulletRay.Intersects(obj.Box) != null)
                {
                    float? distance = bulletRay.Intersects(obj.Box);
                    hits.Add(new ObjectValuePair(obj,(float)distance));
                }

            }

            foreach (Bomb b in game.objectManager.bombs)
            {

                if (bulletRay.Intersects(b.Box) != null && b.Alive)
                {
                    float? distance = bulletRay.Intersects(b.Box);
                    hits.Add(new ObjectValuePair(b, (float)distance));
                }

            }

            foreach (Snake s in game.objectManager.snakes)
            {

                if (bulletRay.Intersects(s.Box) != null && s.Alive)
                {
                    float? distance = bulletRay.Intersects(s.Box);
                    hits.Add(new ObjectValuePair(s, (float)distance));
                }

            }

            if (hits.Count > 0)
            {
                int damage = 20;
                while (damage > 0 && hits.Count > 0)
                {
                    GameObject obj = hits.Min().obj;
                    if (obj is Bomb || obj is Snake)
                    {
                        obj.Alive = false;
                        damage -= 2;
                        if (obj is Bomb)
                        {
                            game.soundBank.PlayCue("explosion");
                        }
                    }
                    else
                    {
                        damage -= 20;
                    }
                    hits.Remove(hits.Min());
                }
            }
        }

    }
}
