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
    public class Player
    {
        Game1 game;

        public bool Alive { get; set; }

        bool canjump = true;

        bool canDoubleJump = true;

        public ModelCollection model;

        Vector3 position;

        Vector3 previousPosition;

        Vector3 direction;

        public float Adrenaline { get; set; }

        public float Health { get; set; }

        public float MaxHealth { get; set; }

        //current state of the game controller
        GamePadState currentState;

        //previous state of the game controller
        GamePadState previousState;

        //the collision box for the player
        public BoundingBox box;

        //the movement speed of the player
        private float walkingSpeed;

        //the base movement speed of the player
        private float baseWalkingSpeed;

        //the movement speed of the player while running
        private float runningSpeed;

        //the rotation speed of the player
        private float turningSpeed;

        //the 2D direction of the player to only move on two axes
        //this is the direction the player goes in so it does not fly
        private Vector3 direction2D;

        float DeltaY;

        Matrix world;

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

        public Vector3 Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
            }
        }

        public Vector3 Direction2D
        {
            get
            {
                return direction2D;
            }
            set
            {
                direction2D = value;
            }
        }

        public Player(Game1 game, Vector3 position, Vector3 direction)
        {
            this.game = game;

            model = new ModelCollection(game, "Models/pirate", 1, 1, 150);

            this.position = position;

            this.direction = direction;

            direction.Normalize();

            //the direction 2d is the same thing as the direction except that it does not point up
            direction2D = new Vector3(direction.X, 0, direction.Z);
            //the direction vector is set so that the length of the vector is equal to one
            direction2D.Normalize();

            //the current state is set to the state of player one's controller
            currentState = GamePad.GetState(PlayerIndex.One);

            //initializes the player's bounding box
            box = new BoundingBox(new Vector3(position.X - 3, position.Y - 10, position.Z - 3), new Vector3(position.X + 3, position.Y + 10, position.Z + 3));

            world = Matrix.Identity;

            baseWalkingSpeed = 1.0f;

            runningSpeed = baseWalkingSpeed * 1.5f;

            walkingSpeed = baseWalkingSpeed;

            turningSpeed = 5.0f;

            DeltaY = 0;

            Adrenaline = 100;

            Health = 100;

            MaxHealth = 100;

            Alive = true;

        }

        public void Update(GameTime gameTime)
        {
            //the current state is set to the state of player one's controller
            currentState = GamePad.GetState(PlayerIndex.One);

            if (Health <= 0)
            {
                if (Alive)
                {
                    game.playerManager.Deaths += 1;
                }
                Alive = false;
            }
            else
            {
                Alive = true;
            }

            if (Alive)
            {

                model.Update(gameTime);

                if (Health > MaxHealth)
                {
                    Health = MaxHealth;
                }

                    position += direction2D * walkingSpeed * currentState.ThumbSticks.Left.Y;

                    position -= Vector3.Cross(Vector3.Up, direction2D) * walkingSpeed * currentState.ThumbSticks.Left.X;

                    if (currentState.ThumbSticks.Left.Y == 0 && currentState.ThumbSticks.Left.Y == 0)
                    {
                        if (model.CurrentAnimationName == "Walk")
                        {
                            model.PlayLoop("Idle");
                        }
                    }
                    else
                    {
                        if (model.CurrentAnimationName == "Idle")
                        {
                            model.PlayLoop("Walk");
                        }
                    }

                //pressing the left thumbstick is to run and use adrenaline
                if (currentState.Buttons.LeftStick == ButtonState.Pressed)
                {
                    if (Adrenaline > 0)
                    {
                        Adrenaline -= 0.2f;
                        Health -= 0.05f;
                        walkingSpeed = runningSpeed;
                    }
                }
                else
                {
                    if (Adrenaline >= 66)
                    {
                        walkingSpeed = baseWalkingSpeed;
                    }
                    else if (Adrenaline >= 33 && Adrenaline < 66)
                    {
                        walkingSpeed = baseWalkingSpeed * .75f;
                    }
                    else
                    {
                        walkingSpeed = baseWalkingSpeed * .5f;
                    }
                }

                //simple simulated gravity
                DeltaY -= .2f;

                if (currentState.Buttons.A == ButtonState.Pressed && previousState.Buttons.A == ButtonState.Released)
                {
                    if (canjump)
                    {
                        DeltaY = 3f;
                        canjump = false;
                        Health -= 0.5f;
                    }
                    else
                    {
                        if (canDoubleJump)
                        {
                            DeltaY = 3f;
                            canDoubleJump = false;
                            Health -= 1.5f;
                        }
                    }
                }

                position.Y += DeltaY;

                //updates the player's bounding box
                box = new BoundingBox(new Vector3(position.X - 3, position.Y - 10, position.Z - 3), new Vector3(position.X + 3, position.Y + 10, position.Z + 3));

                //collision code for the floor
                //uses the same logic as with the cube
                if (box.Intersects(game.objectManager.groundBox))
                {
                    if (topCollision(game.objectManager.groundBox))
                    {
                        position.Y = previousPosition.Y;
                        position.Y = game.objectManager.groundBox.Max.Y + 10;
                        DeltaY = 0;
                        canjump = true;
                        canDoubleJump = true;
                    }
                    else if (bottomCollision(game.objectManager.groundBox))
                    {
                        position = previousPosition;
                    }
                    else
                    {
                        position.X = previousPosition.X;
                        position.Z = previousPosition.Z;
                    }
                }

                foreach (GameObject obj in game.objectManager.objects)
                {
                    if (box.Intersects(obj.Box))
                    {
                        if (topCollision(obj.Box))
                        {
                            position.Y = previousPosition.Y;
                            position.Y = obj.Box.Max.Y + 10;
                            DeltaY = 0;
                            canjump = true;
                            canDoubleJump = true;
                        }
                        else if (bottomCollision(obj.Box))
                        {
                            position = previousPosition;
                        }
                        else
                        {
                            position.X = previousPosition.X;
                            position.Z = previousPosition.Z;
                        }
                    }
                }
                if (position.X > 450)
                {
                    position.X = 450;
                }
                if (position.X < -450)
                {
                    position.X = -450;
                }
                if (position.Z > 450)
                {
                    position.Z = 450;
                }
                if (position.Z < -450)
                {
                    position.Z = -450;
                }


                //updates the previous position
                previousPosition = position;

                //the right arrow key turns the camera on its y axis to the right
                if (currentState.ThumbSticks.Right.X != 0)
                {
                    direction = Vector3.Transform(direction,
                    Matrix.CreateFromAxisAngle(Vector3.Up, (currentState.ThumbSticks.Right.X * -MathHelper.PiOver4 / 150) * turningSpeed));
                    world *= Matrix.CreateRotationY((currentState.ThumbSticks.Right.X * -MathHelper.PiOver4 / 150) * turningSpeed);
                }

                if (game.playerManager.firstPerson)
                {
                    if (currentState.ThumbSticks.Right.Y != 0)
                    {
                        if (direction.Y > -0.9f && direction.Y < 0.9f)
                        {
                            direction = Vector3.Transform(direction,
                            Matrix.CreateFromAxisAngle(Vector3.Cross(Vector3.Up, direction), (currentState.ThumbSticks.Right.Y * -MathHelper.PiOver4 / 150) * turningSpeed));
                        }
                        else if (direction.Y <= -0.9f)
                        {
                            direction.Y = -0.89f;
                            direction.Normalize();
                        }
                        else
                        {
                            direction.Y = 0.89f;
                            direction.Normalize();
                        }
                    }
                }

                world.Translation = position;

                //the direction 2d is the same thing as the direction except that it does not point up
                direction2D = new Vector3(direction.X, 0, direction.Z);

                //the direction vector is set so that the length of the vector is equal to one
                direction2D.Normalize();
            }
            else
            {
                //respawn
                if (currentState.Buttons.Back == ButtonState.Pressed && previousState.Buttons.Back == ButtonState.Released)
                {
                    Health = 100;
                    MaxHealth = 100;
                    position = new Vector3(0, 15, 350);
                }
            }
            //the previous state is set to the current state
            //the previous state will be previous the next time the game updates and the currentState is updated
            previousState = currentState;
        }

        public void Draw(Camera camera)
        {
            if(camera is FirstPersonCamera)
            {
                FirstPersonCamera fpsCamera = (FirstPersonCamera)camera;

                if (fpsCamera.GamePlayer == this)
                {
                    return;
                }
            }

            model.Draw(world, camera);
        }

        /// <summary>
        /// This method checks if the camera intersects with a bounding box from the top
        /// </summary>
        /// <param name="otherBox"></param>The box to be checked for a collision from the top
        /// <returns></returns>
        public bool topCollision(BoundingBox otherBox)
        {
            //if the boxes don't collide, it is false
            if (box.Intersects(otherBox))
            {
                //returns true if the camera box's min is less than the other box's max
                //and the min of the camera's box was previously more than the max of the other box
                if (box.Min.Y < otherBox.Max.Y && ((previousPosition.Y - 5) > otherBox.Max.Y))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }





        /// <summary>
        /// This method checks if the camera intersects with a bounding box from the bottom
        /// </summary>
        /// <param name="otherBox"></param>The box to be checked for a collision from the bottom
        /// <returns></returns>
        private bool bottomCollision(BoundingBox otherBox)
        {
            //if the boxes don't collide, it is false
            if (box.Intersects(otherBox))
            {
                //returns true if the camera box's max is more than the other box's min
                //and the max of the camera's box was previously less than the min of the other box
                if (box.Max.Y > otherBox.Min.Y && ((previousPosition.Y + 5) < otherBox.Min.Y))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
