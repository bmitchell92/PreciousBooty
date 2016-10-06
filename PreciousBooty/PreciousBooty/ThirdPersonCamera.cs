using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace PreciousBooty
{
    /// <summary>
    /// This is a class that models a third person camera
    /// The camera tracks a player
    /// 
    /// @author Brett Mitchell
    ///
    /// </remarks>
    /// 
    public class ThirdPersonCamera : Camera
    {

        Player player;

        //the direction of the camera
        private Vector3 cameraDirection;

        public Vector3 eyePosition;

        float cameraDistance;

        Ray playerToCameraRay;

        float yOffset = 10;


        //current state of the game controller
        GamePadState currentState;


        //previous state of the game controller
        GamePadState previousState;


        //accessors and mutators
        public Matrix View
        {
            get
            {
                return view;
            }
            protected set
            {
                view = value;
            }
        }

        public Matrix Projection
        {
            get
            {
                return projection;
            }
            protected set
            {
                projection = value;
            }
        }

        public Vector3 Position
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

        public Player GamePlayer
        {
            get
            {
                return player;
            }
            set
            {
                player = value;
            }
        }

        public Vector3 Up
        {
            get
            {
                return up;
            }
            set
            {
                up = value;
            }
        }





        /// <summary>
        /// The constructor for the camera.  Takes a reference to the game, its starting position,
        /// its initial model to track, and the starting up direction for the camera
        /// </summary>
        /// <param name="game"></param>A callback to the game
        /// <param name="position"></param>The position of the camera in the world
        /// <param name="player"></param>The player that the camera is looking at
        /// <param name="up"></param>The up direction of the camera
        public ThirdPersonCamera(Game1 game //the game that the camera is in
        , Vector3 position //the position of the camera
        , Player player    //the player that the camera is looking at
        , Vector3 up       //the up position of the camera
        )
            : base(game,position,player.Position,up)     //calls the constructor for the parent class GameComponent
        {

            //the current state is set to the state of player one's controller
            currentState = GamePad.GetState(PlayerIndex.One);

            //view matrix is set
            View = Matrix.CreateLookAt(position, player.Position, up);

            //initializes variables with the parameters
            this.player = player;
            this.game = game;
            this.up = up;
            this.position = position;

            //the camera direction is calculated by subtracting the target point from the camera's position
            cameraDirection = player.Position - position;

            //the direction vector is set so that the length of the vector is equal to one
            cameraDirection.Normalize();


            //sets the projection of the camera
            Projection = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4,    // 45 degrees
                    (float)game.Window.ClientBounds.Width / (float)game.Window.ClientBounds.Height, //aspect ratio is equal to the width of the window divided by the height of the window
                    1, 3000);  // near & far clipping planes

            playerToCameraRay = new Ray(player.Position, Vector3.Forward);

        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //the current state is set to the state of player one's controller
            currentState = GamePad.GetState(PlayerIndex.One);

            //the up arrow key makes the camera look up
            if (game.currentState.ThumbSticks.Right.Y > 0)
            {
                yOffset -= 0.5f;
                if (yOffset < -8)
                {
                    yOffset = -8;
                }
            }

            //the down arrow key makes the camera look down
            if (currentState.ThumbSticks.Right.Y < 0)
            {
                yOffset += 0.5f;
                if (yOffset > 30)
                {
                    yOffset = 30;
                }
            }

            eyePosition = player.Position;
            eyePosition.Y = player.Position.Y + 10;

            position = eyePosition-(player.Direction2D * 30);
            position.Y = eyePosition.Y + yOffset;
            cameraDistance = FindCameraDistance();
            playerToCameraRay = new Ray(eyePosition, Vector3.Normalize(position - eyePosition));
            List<float> hits = new List<float>();

            foreach (GameObject obj in game.objectManager.objects)
            {
                
                if (playerToCameraRay.Intersects(obj.Box) != null)
                {
                    float? distance = playerToCameraRay.Intersects(obj.Box);
                    if (distance < cameraDistance)
                    {
                        hits.Add((float)distance);
                    }
                }
                
            }

            if (playerToCameraRay.Intersects(game.objectManager.groundBox) != null)
            {
                float? distance = playerToCameraRay.Intersects(game.objectManager.groundBox);
                if (distance < cameraDistance)
                {
                    hits.Add((float)distance);
                }
            }
            if (hits.Count > 0)
            {
                position = eyePosition + (playerToCameraRay.Direction * hits.Min());
                position.Y = eyePosition.Y + yOffset;
            }







            //the view is updated
            View = Matrix.CreateLookAt(position, eyePosition, up);


            //the previous state is set to the current state
            //the previous state will be previous the next time the game updates and the currentState is updated
            previousState = currentState;

            //updates the parent class
            base.Update(gameTime);
        }

        private float FindCameraDistance()
        {
            Vector3 vector = position - eyePosition;
            float distance = (float)Math.Sqrt(Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2));
            distance = (float)Math.Sqrt(Math.Pow(vector.Z, 2) + Math.Pow(distance, 2));
            return distance;
        }
    }
}