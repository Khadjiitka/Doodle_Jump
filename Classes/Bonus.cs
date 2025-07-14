using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Doodle_Jump.Classes
{
    public class Bonus
    {
        public Physics physics;
        public Image sprite;
        public int type;
        public Bonus(PointF pos, int type)
        {
            switch (type)
            {
                case 1:
                    sprite = Properties.Resources.jetpack;
                    physics = new Physics(pos, new Size(30, 30));
                    break;
                case 2:
                    sprite = Properties.Resources.spring;
                    physics = new Physics(pos, new Size(15, 15));
                    break;
            }
            this.type = type;
        }
        public void DrawSprite(Graphics g)
        {
            g.DrawImage(sprite, physics.transform.position.X, physics.transform.position.Y, physics.transform.size.Width, physics.transform.size.Height);
        }
    }
}
