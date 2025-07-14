using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doodle_Jump.Classes
{
    public class Platform
    {
        Image sprite;
        public Transform transform;
        public int sizeX;
        public int sizeY;
        public bool isTouchedByPlayer;

        public Platform(PointF pos) 
        {
            sprite = Properties.Resources.platform;
            sizeX = 60;
            sizeY = 12;
            transform = new Transform(pos, new Size(sizeX, sizeY));
            isTouchedByPlayer = false;
        }
        public void DrawSprite(Graphics g) //отрисовка картинки
        {
            g.DrawImage(sprite, transform.position.X, transform.position.Y, transform.size.Width, transform.size.Height);
        }
    }
}
