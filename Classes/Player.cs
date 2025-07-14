 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doodle_Jump.Classes
{
    public class Player
    {
        public Physics physics;
        public Image sprite;

        public bool isUsingJetpackSprite = false;
        public Player()
        {
            sprite = Properties.Resources.man2;
            physics = new Physics(new PointF(100,350),new Size(40,40));
        }
       
        public void SetJetpackSprite()
        {
            sprite = Properties.Resources.man_jetpack;
            isUsingJetpackSprite = true;
        }

        public void ResetToDefaultSprite()
        {
            sprite = Properties.Resources.man2;
            isUsingJetpackSprite = false;
        }
        public void DrawSprite(Graphics g)
        {
            g.DrawImage(sprite, physics.transform.position.X, physics.transform.position.Y, physics.transform.size.Width, physics.transform.size.Height);
        }
    }
}
