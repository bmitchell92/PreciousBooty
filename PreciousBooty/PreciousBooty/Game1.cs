using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;

namespace PreciousBooty
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 

    [Serializable]
    public struct Score
    {
        string playerName;
        public int score;

        public Score(string name, int score)
        {
            playerName = name;
            this.score = score;
        }
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        public List<Score> scores;
        public enum GameState { Paused, Play, Menu, GameOver};
        public GameState currentGameState = GameState.Menu;

        public float maxHealth = 100;
        public float health = 100;

        public GUIManager guiManager;
        public PlayerManager playerManager;
        public ObjectManager objectManager;

        public GamePadState currentState;
        public GamePadState previousState;

        Model ground;
        Model skySphere;
        Model ocean;

        public Random rand;

        public AudioEngine engine;
        public SoundBank soundBank;
        public WaveBank waveBank;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Components.Add(new GamerServicesComponent(this));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize audio objects.
            engine = new AudioEngine("Content\\Sounds\\PreciousBootyAudio.xgs");
            soundBank = new SoundBank(engine, "Content\\Sounds\\Sound Bank.xsb");
            waveBank = new WaveBank(engine, "Content\\Sounds\\Wave Bank.xwb");

            // TODO: use this.Content to load your game content here
            rand = new Random(DateTime.Now.Millisecond);
            guiManager = new GUIManager(this);
            playerManager = new PlayerManager(this);
            objectManager = new ObjectManager(this);

            ground = Content.Load<Model>("Models/island");
            skySphere = Content.Load<Model>("Models/skySphere");
            ocean = Content.Load<Model>("Models/ocean");

            scores = new List<Score>();
            LoadScores();
            
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            SaveScores();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            currentState = GamePad.GetState(PlayerIndex.One);
            // TODO: Add your update logic here
            guiManager.Update(gameTime);

            if (currentState.Buttons.Start == ButtonState.Pressed && previousState.Buttons.Start == ButtonState.Released)
            {
                if (currentGameState == GameState.Play)
                {
                    currentGameState = GameState.Paused;
                }
                else if (currentGameState == GameState.Paused)
                {
                    currentGameState = GameState.Play;
                }
            }

            if (currentGameState == GameState.Play)
            {
                playerManager.Update(gameTime);
                objectManager.Update(gameTime);
            }

            previousState = currentState;
            base.Update(gameTime);
        }

        //red border = 0.9f theta
        //green border = 2.17f theta

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DodgerBlue);

            // TODO: Add your drawing code here
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            if (currentGameState == GameState.Play || currentGameState == GameState.Paused)
            {

                playerManager.Draw();

                objectManager.Draw(playerManager.camera);

                DrawModel(ground, Matrix.Identity, playerManager.camera);

                DrawModel(ocean, Matrix.Identity, playerManager.camera);


                DrawSkyBox(skySphere, playerManager.camera);
            }

            spriteBatch.Begin();

            guiManager.Draw();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawModel(Model model, Matrix world,Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {

                foreach (BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    be.Projection = camera.Projection;
                    be.View = camera.View;
                    be.World = world;
                }

                mesh.Draw();
            }
        }

        public void DrawSkyBox(Model model, Camera camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {

                foreach (BasicEffect be in mesh.Effects)
                {
                    be.Projection = camera.Projection;
                    be.View = camera.View;
                    be.World = Matrix.Identity;
                }

                mesh.Draw();
            }
        }

        protected void SaveScores()
        {
            SortScores();

            while (scores.Count > 10)
            {
                scores.RemoveAt(scores.Count - 1);
            }
            try
            {
                using(Stream stream = File.Open("scores.bin",FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, scores);
                }
            }
            catch (IOException)
            {
            }
        }

        protected void LoadScores()
        {
            try
            {
                using (Stream stream = File.Open("scores.bin", FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    scores = (List<Score>)bin.Deserialize(stream);
                }
            }
            catch (IOException)
            {
            }
        }

        public void SortScores()
        {
            for (int i = 0; i < scores.Count; i++)
            {
                int switchIndex = i;
                Score best = scores[i];

                for (int j = i; j < scores.Count; j++)
                {
                    if (scores[j].score > best.score)
                    {
                        best = scores[j];
                        switchIndex = j;
                    }
                }
                Score temp = scores[i];
                scores[i] = best;
                scores[switchIndex] = temp;
            }
        }

        public void Restart()
        {
            guiManager = new GUIManager(this);
            playerManager = new PlayerManager(this);
            objectManager = new ObjectManager(this);
            currentGameState = GameState.Play;
        }
    }
}
