using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Doodle_Jump.Classes
{
    public class Physics
    {
        public Transform transform;// размер и позиция
        float gravity;
        float a;// ускорение
        public float dx;// для движения впрво-влево
        public bool useBonus = false;

        public bool isJetpackActive = false;

        public Physics(PointF position, Size size)
        {
            transform = new Transform(position,size);
            gravity = 0;
            a = 0.4f;
            dx = 0;
        }
        public void ApplyPhysics()
        {
            CalculatePhysics();
        }
        public void CalculatePhysics()//лево право прыжок
        {
            if(dx!=0)
            {
                transform.position.X += dx;
            }
            if (transform.position.Y < 700)
            {
                transform .position.Y += gravity;
                gravity += a;

                if (gravity > -25 && useBonus)
                {
                    PlatformController.GenerateRandomPlatform();// платформы генерировались после использования бонусов
                    PlatformController.startPlatformPosY = -200;
                    PlatformController.GenerateStartSequence(); // чтобы генерировалось все поле, а не поштучно
                    PlatformController.startPlatformPosY = 0; // платформы были ближе друг к другу
                    useBonus = false;
                }

                Collide();
            }
        }
        public bool StandartCollide() // коллизия (столкновение) пули с врагами
        {
            for(int i = 0; i < PlatformController.bullets.Count; i++)
            {
                var bullet = PlatformController.bullets[i];
                PointF delta = new PointF();
                delta.X = (transform.position.X + transform.size.Width / 2) - (bullet.physics.transform.position.X + bullet.physics.transform.size.Width / 2);
                delta.Y = (transform.position.Y + transform.size.Height / 2) - (bullet.physics.transform.position.Y + bullet.physics.transform.size.Height / 2);
                if (Math.Abs(delta.X) <= transform.size.Width /2 + bullet.physics.transform.size.Width / 2)
                {
                    PlatformController.RemoveBullet(i);// удаляем пулю при столкновении ее с врагом
                    return true;
                }
            }
            return false;
        }
        public bool StandartCollidePlayerWithObjects(bool forMonsters, bool forBonuses) // коллизия (столкновение) игрока с врагами
        {
            if (forMonsters)
            {
                for (int i = 0; i < PlatformController.enemies.Count; i++)
                {
                    var enemy = PlatformController.enemies[i];
                    PointF delta = new PointF();
                    delta.X = (transform.position.X + transform.size.Width / 2) - (enemy.physics.transform.position.X + enemy.physics.transform.size.Width / 2);
                    delta.Y = (transform.position.Y + transform.size.Height / 2) - (enemy.physics.transform.position.Y + enemy.physics.transform.size.Height / 2);
                    if (Math.Abs(delta.X) <= transform.size.Width / 2 + enemy.physics.transform.size.Width / 2)
                    {
                        if (Math.Abs(delta.Y) <= transform.size.Height / 2 + enemy.physics.transform.size.Height / 2)
                        {
                            if (!useBonus)
                                return true;
                        }
                    }
                }
            }
                if (forBonuses)
                {
                    for (int i = 0; i < PlatformController.bonuses.Count; i++)
                    {
                        var bonus = PlatformController.bonuses[i];
                        PointF delta = new PointF();
                        delta.X = (transform.position.X + transform.size.Width / 2) - (bonus.physics.transform.position.X + bonus.physics.transform.size.Width / 2);
                        delta.Y = (transform.position.Y + transform.size.Height / 2) - (bonus.physics.transform.position.Y + bonus.physics.transform.size.Height / 2);
                        if (Math.Abs(delta.X) <= transform.size.Width / 2 + bonus.physics.transform.size.Width / 2)
                        {
                            if (Math.Abs(delta.Y) <= transform.size.Height / 2 + bonus.physics.transform.size.Height / 2)
                            {
                                if (bonus.type == 1 && !useBonus)
                                {
                                    useBonus = true;
                                    AddForce(-60);
                                    useBonus = true;
                                    isJetpackActive = true; 
                            }
                                if (bonus.type == 2 && !useBonus)
                                {
                                    useBonus = true;
                                    AddForce(-30);
                                }

                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        
            public void Collide()// проверка на столкновение
           {
            for ( int i = 0; i < PlatformController.platforms.Count; i++ )
            {
                var platform = PlatformController.platforms[i];
                if (transform.position.X + transform.size.Width/2  >=  platform.transform.position.X  && 
                    transform.position.X + transform.size.Width/2  <=  platform.transform.position.X + platform.transform.size.Width) 
                {
                    if (transform.position.Y + transform.size.Height  >=  platform.transform.position.Y &&
                        transform.position.Y + transform.size.Height  <= platform.transform.position.Y + platform.transform.size.Height)
                    {
                        if ( gravity > 0 ) // персонаж прилетел сверху
                        {
                            AddForce();
                            if (!platform.isTouchedByPlayer) //если игрок еще не касался платформы
                            {
                                PlatformController.score += 20; // добавили очки
                                PlatformController.GenerateRandomPlatform();// создаем новую платформу
                                platform.isTouchedByPlayer = true;
                            }
                        }
                    }
                }
            }
        }
        public void AddForce(int force = -10) //прыжок
        {
            gravity = force;

        }
    }
}
