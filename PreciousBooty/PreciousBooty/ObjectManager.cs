using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using System.Text;

namespace PreciousBooty
{
    public class ObjectManager
    {
        Game1 game;
        public BoundingBox groundBox;

        public GameObject[] objects;
        public Bomb[] bombs;
        public Snake[] snakes;
        public PowerUp[] powerups;
        public Treasure[] treasures;
        public int perfectScore = 0;

        public ObjectManager(Game1 game)
        {
            this.game = game;
            groundBox = new BoundingBox(new Vector3(-450, -20, -450), new Vector3(450, -10, 450));
            objects = new GameObject[100];
            treasures = new Treasure[80];
            bombs = new Bomb[15];
            snakes = new Snake[40];
            powerups = new PowerUp[27];

            for (int i = 0; i < 80; i++)
            {
                Vector3 pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);

                objects[i] = new GameObject(game, pos, "Models/palmtree", true, 4, 0, 4, 4, 35, 4);
                for (int j = 0; j < i; j++)
                {
                    while (objects[i].Box.Intersects(objects[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(200) - 100, 0, game.rand.Next(200) - 100);
                        objects[i].Position = pos;

                        j = 0;
                    }
                }
            }
            for (int i = 80; i < 100; i++)
            {
                Vector3 pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);

                objects[i] = new GameObject(game, pos, "Models/rock", true, 12, 0, 8, 12, 0.25f, 8);
                for (int j = 0; j < i; j++)
                {
                    while (objects[i].Box.Intersects(objects[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(200) - 100, 0, game.rand.Next(200) - 100);
                        objects[i].Position = pos;

                        j = 0;
                    }
                }
            }

            for (int i = 0; i < 15; i++)
            {
                Vector3 pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);

                int rand = game.rand.Next(80);
                    bombs[i] = new Bomb(game, pos, "Models/bomb", true, 6, 0, 6, 6, 4, 6);
                
                for (int j = 0; j < objects.Length; j++)
                {
                    while (bombs[i].Box.Intersects(objects[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);
                        bombs[i].Position = pos;

                        j = 0;
                    }
                }
            }

            for (int i = 0; i < 40; i++)
            {
                Vector3 pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);

                int rand = game.rand.Next(80);
                snakes[i] = new Snake(game, pos, "Models/snake", "Models/snake_dead", true, 4, 0, 4, 4, 0.1f, 4);

                for (int j = 0; j < objects.Length; j++)
                {
                    while (snakes[i].Box.Intersects(objects[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);
                        snakes[i].Position = pos;

                        j = 0;
                    }
                }
                for (int j = 0; j < bombs.Length; j++)
                {
                    while (snakes[i].Box.Intersects(bombs[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);
                        snakes[i].Position = pos;

                        j = 0;
                    }
                }
            }
    

            for (int i = 0; i < 80; i++)
            {
                Vector3 pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);

                int rand = game.rand.Next(80);
                if (rand > 75)
                {
                    treasures[i] = new Treasure(game, 50, pos, "Models/treasurechest", true, 4, 0, 8, 4, 8, 8,false);
                }
                else if (rand > 65 && rand <= 75)
                {
                    treasures[i] = new Treasure(game, 10, pos, "Models/goldchain", true, 4, 0, 4, 4, 8, 4,false);
                }
                else if (rand > 40 && rand <= 65)
                {
                    treasures[i] = new Treasure(game, 5, pos, "Models/diamond", true, 4, 0, 4, 4, 8, 4,true);
                }
                else
                {
                    treasures[i] = new Treasure(game, 1, pos, "Models/coin", true, 4, 0, 4, 4, 8, 4,true);
                }
                for (int j = 0; j < objects.Length; j++)
                {
                    while (treasures[i].Box.Intersects(objects[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);
                        treasures[i].Position = pos;

                        j = 0;
                    }
                }
                for (int j = 0; j < bombs.Length; j++)
                {
                    while (treasures[i].Box.Intersects(bombs[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);
                        treasures[i].Position = pos;

                        j = 0;
                    }
                }
                for (int j = 0; j < snakes.Length; j++)
                {
                    while (treasures[i].Box.Intersects(snakes[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);
                        treasures[i].Position = pos;

                        j = 0;
                    }
                }
                perfectScore += treasures[i].Points;
            }
            for (int i = 0; i < 10; i++)
            {
                Vector3 pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);

                powerups[i] = new Grog(game,pos,"Models/grog",true,6, 0, 6, 6, 4, 6,false);

                for (int j = 0; j < objects.Length; j++)
                {
                    while (powerups[i].Box.Intersects(objects[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);
                        powerups[i].Position = pos;

                        j = 0;
                    }
                }
                for (int j = 0; j < bombs.Length; j++)
                {
                    while (powerups[i].Box.Intersects(bombs[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);
                        powerups[i].Position = pos;

                        j = 0;
                    }
                }
                for (int j = 0; j < snakes.Length; j++)
                {
                    while (powerups[i].Box.Intersects(snakes[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);
                        powerups[i].Position = pos;

                        j = 0;
                    }
                }
                for (int j = 0; j < treasures.Length; j++)
                {
                    while (powerups[i].Box.Intersects(treasures[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);
                        powerups[i].Position = pos;

                        j = 0;
                    }
                }
            }

            for (int i = 10; i < 20; i++)
            {
                Vector3 pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);

                powerups[i] = new Adrenaline(game, pos, "Models/adrenaline", true, 6, 0, 6, 6, 4, 6,true);

                for (int j = 0; j < objects.Length; j++)
                {
                    while (powerups[i].Box.Intersects(objects[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);
                        powerups[i].Position = pos;

                        j = 0;
                    }
                }
                for (int j = 0; j < bombs.Length; j++)
                {
                    while (powerups[i].Box.Intersects(bombs[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);
                        powerups[i].Position = pos;

                        j = 0;
                    }
                }
                for (int j = 0; j < snakes.Length; j++)
                {
                    while (powerups[i].Box.Intersects(snakes[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);
                        powerups[i].Position = pos;

                        j = 0;
                    }
                }
                for (int j = 0; j < treasures.Length; j++)
                {
                    while (powerups[i].Box.Intersects(treasures[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);
                        powerups[i].Position = pos;

                        j = 0;
                    }
                }
            }

            for (int i = 20; i < 27; i++)
            {
                Vector3 pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);

                powerups[i] = new Pistol(game, pos, "Models/1851navy", true, 4, 0, 8, 4, 4, 8,true);

                for (int j = 0; j < objects.Length; j++)
                {
                    while (powerups[i].Box.Intersects(objects[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);
                        powerups[i].Position = pos;

                        j = 0;
                    }
                }
                for (int j = 0; j < bombs.Length; j++)
                {
                    while (powerups[i].Box.Intersects(bombs[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);
                        powerups[i].Position = pos;

                        j = 0;
                    }
                }
                for (int j = 0; j < snakes.Length; j++)
                {
                    while (powerups[i].Box.Intersects(snakes[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);
                        powerups[i].Position = pos;

                        j = 0;
                    }
                }
                for (int j = 0; j < treasures.Length; j++)
                {
                    while (powerups[i].Box.Intersects(treasures[j].Box))
                    {
                        pos = new Vector3(game.rand.Next(600) - 300, 0, game.rand.Next(600) - 300);
                        powerups[i].Position = pos;

                        j = 0;
                    }
                }
            }
        }


        public void Update(GameTime gameTime)
        {
            foreach (Treasure t in treasures)
            {
                t.Update(gameTime);
            }
            foreach (Bomb bomb in bombs)
            {
                bomb.Update(gameTime);
            }
            foreach (Snake snake in snakes)
            {
                snake.Update(gameTime);
            }
            foreach (PowerUp p in powerups)
            {
                p.Update(gameTime);
            }
        }

        public void Draw(Camera camera)
        {
            foreach(GameObject obj in objects)
            {
                obj.Draw(camera);
            }
            foreach (Bomb bomb in bombs)
            {
                bomb.Draw(camera);
            }
            foreach (Snake snake in snakes)
            {
                snake.Draw(camera);
            }
            foreach (Treasure t in treasures)
            {
                t.Draw(camera);
            }
            foreach (PowerUp p in powerups)
            {
                p.Draw(camera);
            }
        }

    }
}
