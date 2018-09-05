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

        public Camera()
        {
            viewBounds = new Rectangle(Point.Zero, new Point(1920, 900));
            levelBounds = new Rectangle(0, 0, 0, 0);
            position = new Vector2(0, 0);
        }

        //get the Main Camera from everywhere
        public static Camera Main { get { if (mainCamera == null) { mainCamera = new Camera(); } return mainCamera; } }

        //move the Player or the Level
        public void move(Vector2 moveValue)
        {
            //check if the Player is going to move too far left or right and if, then cancel movement
            if (Player.Nyr.position.X + moveValue.X < levelBounds.X || Player.Nyr.position.X + Player.Nyr.width + moveValue.X > levelBounds.X + levelBounds.Width)
            {
                return;
            }

            //Move the Player when he's not in the center
            if ((int)Player.Nyr.position.X != viewBounds.Width / 2 + viewBounds.X)
            {
                Player.Nyr.move(moveValue);
                return;
            }

            //just move the player, if any borders are hit
            if (position.X + moveValue.X + viewBounds.X > levelBounds.X + levelBounds.Width - viewBounds.Width)
            {
                Player.Nyr.move(moveValue);
                return;
            }
            else if (position.X + moveValue.X < levelBounds.X)
            {
                Player.Nyr.move(moveValue);
                return;
            }
            else if (position.Y + moveValue.Y + viewBounds.Y > levelBounds.Y + levelBounds.Height)
            {
                Player.Nyr.move(moveValue);
                return;
            }
            else if (position.Y + moveValue.Y < levelBounds.Y)
            {
                Player.Nyr.move(moveValue);
                return;
            }

            //and if not, then move the Level
            position = new Vector2(position.X + moveValue.X, position.Y + moveValue.Y);
            Level.Current.moveGameObjects(moveValue);
        }
    }
}
