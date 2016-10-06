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
    public class GUIManager
    {
        Game1 game;

        Texture2D barTexture;
        Texture2D fillingTexture;
        Texture2D deadTexture;
        Texture2D pausedTexture;
        Texture2D menuTexture;
        Texture2D gameOverTexture;

        Texture2D[] pages;
        Texture2D currentTexture;

        Rectangle barRectangle;
        Rectangle fillingRectangle;
        Rectangle backGroundRect;
        bool showingInstructions = false;
        int page = 1;
        int finalscore = 0;


        Sprite adrenalineBar;
        Sprite needle;
        Sprite cursor;
        Sprite crosshair;

        SpriteFont font;

        public int menuPosition = 1;

        public GUIManager(Game1 game)
        {
            this.game = game;

            font = game.Content.Load<SpriteFont>("Fonts/Arial");
            pages = new Texture2D[3];

            for (int i= 1; i <= 3; i++)
            {            
                pages[i-1] = game.Content.Load<Texture2D>("Sprites/Instructions"+i.ToString());
            }
            currentTexture = pages[0];

            barTexture = game.Content.Load<Texture2D>("Sprites/HealthBar");
            fillingTexture = game.Content.Load<Texture2D>("Sprites/barFilling");
            deadTexture = game.Content.Load<Texture2D>("Sprites/dead");
            pausedTexture = game.Content.Load<Texture2D>("Sprites/pause");
            menuTexture = game.Content.Load<Texture2D>("Sprites/mainmenu");
            gameOverTexture = game.Content.Load<Texture2D>("Sprites/gameOver");
            cursor = new Sprite(game, "Sprites/cursor", new Vector2(170, 170), 0);
            crosshair = new Sprite(game, "Sprites/crosshair", new Vector2(game.Window.ClientBounds.Width/2,
                game.Window.ClientBounds.Height/2), 0);

            barRectangle = new Rectangle(310, 35, 150, 50);
            fillingRectangle = new Rectangle(barRectangle.X + 5, barRectangle.Y + 5, 140, 40);
            backGroundRect = new Rectangle(0, 0, game.Window.ClientBounds.Width, game.Window.ClientBounds.Height);

            adrenalineBar = new Sprite(game, "Sprites/AdrenalineBar", new Vector2(100, 60), 0);
            needle = new Sprite(game, "Sprites/needle", new Vector2(100, 80), 0);

        }

        public void Update(GameTime gameTime)
        {

            barRectangle = new Rectangle(310, 35, (int)(150 * (game.playerManager.player.MaxHealth / 100)), 50);
            fillingRectangle = new Rectangle(barRectangle.X + 5, barRectangle.Y + 5, (int)((barRectangle.Width - 10) * (game.playerManager.player.Health / game.playerManager.player.MaxHealth)), 40);
            needle.Theta = (game.playerManager.player.Adrenaline/100) * MathHelper.Pi;
            cursor.Update(gameTime);
            crosshair.Update(gameTime);
            if (game.currentGameState == Game1.GameState.Paused || game.currentGameState == Game1.GameState.Menu)
            {
                if (!showingInstructions)
                {
                    if (game.currentState.ThumbSticks.Left.Y < 0 && game.previousState.ThumbSticks.Left.Y >= 0)
                    {
                        menuPosition += 1;
                    }
                    if (game.currentState.ThumbSticks.Left.Y > 0 && game.previousState.ThumbSticks.Left.Y <= 0)
                    {
                        menuPosition -= 1;
                    }
                }

                if (menuPosition > 3)
                {
                    menuPosition = 1;
                }
                if (menuPosition < 1)
                {
                    menuPosition = 3;
                }
            }

            if (game.currentGameState == Game1.GameState.Paused)
            {
                if (menuPosition == 1)
                {
                    cursor.Position = new Vector2(170, 170);

                    if (game.currentState.Buttons.A == ButtonState.Pressed && game.previousState.Buttons.A == ButtonState.Released)
                    {
                        game.currentGameState = Game1.GameState.Play;
                    }
                }
                else if (menuPosition == 2)
                {
                    cursor.Position = new Vector2(170, 210);

                    if (game.currentState.Buttons.A == ButtonState.Pressed && game.previousState.Buttons.A == ButtonState.Released)
                    {
                        game.Restart();
                    }
                }
                else if (menuPosition == 3)
                {
                    cursor.Position = new Vector2(170, 250);
                    if (game.currentState.Buttons.A == ButtonState.Pressed && game.previousState.Buttons.A == ButtonState.Released)
                    {
                        game.Exit();
                    }
                }
            }
            else if (game.currentGameState == Game1.GameState.Menu)
            {
                if (menuPosition == 1)
                {
                    cursor.Position = new Vector2(90, 210);

                    if (game.currentState.Buttons.A == ButtonState.Pressed && game.previousState.Buttons.A == ButtonState.Released)
                    {
                        game.Restart();
                        game.currentGameState = Game1.GameState.Play;
                    }
                }
                else if (menuPosition == 2)
                {
                    cursor.Position = new Vector2(90, 245);

                    if (game.currentState.Buttons.A == ButtonState.Pressed && game.previousState.Buttons.A == ButtonState.Released)
                    {
                        if (!showingInstructions)
                        {
                            showingInstructions = true;
                            page = 0;
                            currentTexture = pages[0];
                        }
                        else
                        {
                            if (page == pages.Length-1)
                            {
                                showingInstructions = false;
                            }
                            else
                            {
                                page += 1;
                                currentTexture = pages[page];
                            }
                        }
                    }
                }
                else if (menuPosition == 3)
                {
                    cursor.Position = new Vector2(90, 280);
                    if (game.currentState.Buttons.A == ButtonState.Pressed && game.previousState.Buttons.A == ButtonState.Released)
                    {
                        game.Exit();
                    }
                }
            }

            if (game.playerManager.secondspassed >= 120 && game.currentGameState == Game1.GameState.Play)
            {
                game.currentGameState = Game1.GameState.GameOver;
                finalscore = CalculateFinalScore();
                
            }
            if (game.currentGameState == Game1.GameState.GameOver && game.currentState.Buttons.A == ButtonState.Pressed && game.previousState.Buttons.A == ButtonState.Released)
            {
                game.scores.Add(new Score("player", finalscore));
                game.SortScores();
                game.currentGameState = Game1.GameState.Menu;
            }
        }

        public void Draw()
        {
            adrenalineBar.Draw();

            needle.Draw();

            game.spriteBatch.Draw(barTexture, barRectangle, Color.White);
            game.spriteBatch.Draw(fillingTexture, fillingRectangle, Color.White);

            game.spriteBatch.DrawString(font, "Energy: ", new Vector2(200,40), Color.Black);
            game.spriteBatch.DrawString(font, "Score: "+ game.playerManager.Points, new Vector2(580, 40), Color.Yellow);
            game.spriteBatch.DrawString(font, "Perfect Score: " + game.objectManager.perfectScore, new Vector2(500, 100), Color.Yellow);
            game.spriteBatch.DrawString(font, "Time Left: " + (120 - game.playerManager.secondspassed), new Vector2(500, 150), Color.Yellow);

            if (game.playerManager.hasPistol && game.playerManager.canshoot)
            {
                crosshair.Draw();
            }

            if (!game.playerManager.player.Alive)
            {
                game.spriteBatch.Draw(deadTexture, backGroundRect, Color.White);
            }
            if (game.currentGameState == Game1.GameState.Paused)
            {
                game.spriteBatch.Draw(pausedTexture, backGroundRect, Color.White);
                cursor.Draw();
            }

            if (game.currentGameState == Game1.GameState.GameOver)
            {
                game.spriteBatch.Draw(gameOverTexture, backGroundRect, Color.White);
                DrawCalculations();
            }

            if (game.currentGameState == Game1.GameState.Menu)
            {
                game.spriteBatch.Draw(menuTexture, backGroundRect, Color.White);
                cursor.Draw();
                if (game.scores.Count > 0)
                {
                    game.spriteBatch.DrawString(font, "High Score: " + game.scores[0].score.ToString(), new Vector2(40, 440), Color.Red);
                }
                if (showingInstructions)
                {
                    game.spriteBatch.Draw(currentTexture, backGroundRect, Color.White);
                }
            }
        }

        private int CalculateFinalScore()
        {
            float fscore = (float)game.playerManager.Points / (float)game.objectManager.perfectScore;
            fscore *= 500;
            if (game.playerManager.Deaths < 1)
            {
                fscore += 200;
            }
            else
            {
                fscore = fscore / game.playerManager.Deaths;
            }

            fscore += game.playerManager.player.Adrenaline;
            fscore += game.playerManager.player.Health;

            return (int)fscore;

        }

        private void DrawCalculations()
        {
            game.spriteBatch.DrawString(font, "Game Score: " + game.playerManager.Points, new Vector2(20, 40), Color.Red);
            game.spriteBatch.DrawString(font, "Perfect Score: " + game.objectManager.perfectScore, new Vector2(20, 90), Color.Red);
            game.spriteBatch.DrawString(font, "Deaths: " + game.playerManager.Deaths, new Vector2(20, 140), Color.Red);
            if (game.playerManager.Deaths < 1)
            {
                game.spriteBatch.DrawString(font, "No Death Bonus + 200!", new Vector2(20, 190), Color.Red);
            }
            game.spriteBatch.DrawString(font, "Adrenaline Left: " + game.playerManager.player.Adrenaline, new Vector2(20, 240), Color.Red);
            game.spriteBatch.DrawString(font, "Energy Left: " + game.playerManager.player.Health, new Vector2(20, 290), Color.Red);
            game.spriteBatch.DrawString(font, "FinalScore: " + finalscore, new Vector2(20, 340), Color.Red);

            game.spriteBatch.DrawString(font, "High Scores", new Vector2(400, 40), Color.Red);
            for (int i = 0; i < 10; i++)
            {
                if (i < game.scores.Count)
                {
                    game.spriteBatch.DrawString(font, (i+1).ToString() + ". " + game.scores[i].score.ToString(), new Vector2(400, 70 + (i * 40)), Color.Red);
                }
                else
                {
                    game.spriteBatch.DrawString(font,(i+1).ToString() + ".", new Vector2(400, 70+(i*40)), Color.Red);
                }
            }
        }
    }
}
