using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Valkyrie_Nyr
{
    class Camera
    {
        static Camera mainCamera;
        public Rectangle viewBounds;
        public Rectangle levelBounds;
        public Vector2 position;
        public int zoom;

        public Camera()
        {
            viewBounds = new Rectangle(Point.Zero, Game1.WindowSize);
            levelBounds = new Rectangle(0, 0, 0, 0);
            position = new Vector2(0, 0);
            zoom = 4;
        }

        //get the Main Camera from everywhere
        public static Camera Main { get { if (mainCamera == null) { mainCamera = new Camera(); } return mainCamera; } }

        //move the Player or the Level
        public void move(Vector2 moveValue)
        {
            if (moveValue == Vector2.Zero)
            {
                return;
            }

            //check if the Player is going to move out of the world and prevent that or exit game if dead
            if (Player.Nyr.position.X + moveValue.X + position.X < levelBounds.X || Player.Nyr.position.X + Player.Nyr.width + moveValue.X + position.X > levelBounds.X + levelBounds.Width)
            {
                return;
            }
            if (Player.Nyr.position.Y + moveValue.Y + position.Y < levelBounds.Y)
            {
                return;
            }
            if (Player.Nyr.position.Y + moveValue.Y + position.Y > levelBounds.Y + levelBounds.Height)
            {
                Player.Nyr.gameOver();
            }

            //is true, if the new position is bigger than the middle, while the old position is smaller, or otherwise. So the player must move to the middle
            bool PlayerIsMiddleX = (Player.Nyr.position.X < (viewBounds.X + viewBounds.Width) / 2f && Player.Nyr.position.X + moveValue.X > (viewBounds.X + viewBounds.Width) / 2f) || (Player.Nyr.position.X > (viewBounds.X + viewBounds.Width) / 2f && Player.Nyr.position.X + moveValue.X < (viewBounds.X + viewBounds.Width) / 2f) || Player.Nyr.position.X == (viewBounds.X + viewBounds.Width) / 2;
            bool PlayerIsMiddleY = (Player.Nyr.position.Y < (viewBounds.Y + viewBounds.Height) / 2f && Player.Nyr.position.Y + moveValue.Y > (viewBounds.Y + viewBounds.Height) / 2f) || (Player.Nyr.position.Y > (viewBounds.Y + viewBounds.Height) / 2f && Player.Nyr.position.Y + moveValue.Y < (viewBounds.Y + viewBounds.Height) / 2f) || Player.Nyr.position.Y == (viewBounds.Y + viewBounds.Height) / 2f;

            bool cameraAtMaxTop = position.Y + moveValue.Y <= levelBounds.Y;
            bool cameraAtMaxRight = position.X + moveValue.X + viewBounds.X + viewBounds.Width>= levelBounds.X + levelBounds.Width;
            bool cameraAtMaxBottom = position.Y + moveValue.Y + viewBounds.Y + viewBounds.Height >= levelBounds.Y + levelBounds.Height;
            bool cameraAtMaxLeft = position.X + moveValue.X <= levelBounds.X;

            //move just the player until its back in the middle
            //move the player, if any borders are hit. and if not, then move the Level

            //move x-axis
            if (PlayerIsMiddleX)
            {
                Player.Nyr.position.X = (viewBounds.X + viewBounds.Width) / 2;

                if (cameraAtMaxLeft || cameraAtMaxRight)
                {
                    Player.Nyr.move(new Vector2(moveValue.X, 0));
                }
                else
                {
                    position.X += moveValue.X;
                    Level.Current.moveGameObjects(new Vector2(moveValue.X, 0));
                }
            }
            else
            {
                Player.Nyr.move(new Vector2(moveValue.X, 0));
            }
            //move y-axis
            if (PlayerIsMiddleY)
            {
                Player.Nyr.position.Y = (viewBounds.Y + viewBounds.Height) / 2;

                if (cameraAtMaxTop || cameraAtMaxBottom)
                {
                    Player.Nyr.move(new Vector2(0, moveValue.Y));
                }
                else
                {
                    position.Y += moveValue.Y;
                    Level.Current.moveGameObjects(new Vector2(0, moveValue.Y));
                }
            }
            else
            {
                Player.Nyr.move(new Vector2(0, moveValue.Y));
            }
        }
    }
}