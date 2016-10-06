using System;
using System.Collections.Generic;
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
    /// This is a class that models a first person camera.
    /// The camera follows the position and diection of a player character
    /// 
    /// @author Brett Mitchell
    ///
    /// </summary>
    /// <remarks>
    /// 
    /// ----Known Bugs----
    /// The Camera will sometimes rotate slower if the player looks too far up and down
    /// 
    /// </remarks>
    /// 
    public class FirstPersonCamera : Camera
    {
        
        //the direction of the camera
        private Vector3 cameraDirection;

        Player player;

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



        /// <summary>
        /// The constructor for the camera.  Takes a reference to the game, its starting position,
        /// its initial target vector, and the starting up direction for the camera
        /// </summary>
        /// <param name="game"></param>A callback to the game
        /// <param name="position"></param>The position of the camera in the world
        /// <param name="target"></param>The point that the camera is looking at
        /// <param name="up"></param>The up direction of the camera
        public FirstPersonCamera(Game1 game //the game that the camera is in
        ,Player player
        , Vector3 up       //the up position of the camera
        ) : base(game,player.Position,player.Position + player.Direction,up)     //calls the constructor for the parent class GameComponent
        {
            this.player = player;
            cameraDirection = player.Direction;

            //the direction vector is set so that the length of the vector is equal to one
            cameraDirection.Normalize();

        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            cameraDirection = player.Direction;

            position = player.Position;
            position.Y = player.Position.Y + 10;

            //the view is updated
            View = Matrix.CreateLookAt(position, position + cameraDirection, up);

            //updates the parent class
            base.Update(gameTime);
        }

    }
}