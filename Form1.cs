using Doodle_Jump.Classes;

namespace Doodle_Jump
{
    public partial class Form1 : Form
    {
        Player player;
        System.Windows.Forms.Timer timer1;
        private bool isGameOver = false;


        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true; // Убирает мерцание
            Init();
            timer1 = new System.Windows.Forms.Timer();
            timer1.Interval = 15;
            timer1.Tick += new EventHandler(Update);
            timer1.Start();
            this.KeyDown += new KeyEventHandler(OnKeyboardPressed);
            this.KeyUp += new KeyEventHandler(OnKeyboardUp);
            this.BackgroundImage = Properties.Resources.back;
            this.Height = 600;
            this.Width = 330;
            this.Paint += new PaintEventHandler(OnRepaint);// перерисовка 
        }
        private void GameOver()
        {
            isGameOver = true;
            timer1.Stop(); // Останавливаем игру
            Invalidate();     // Перерисовываем экран (вызовет Paint)
        //    Init();// запуск и инициализация платформ и игрока
        }

        public void Init()
        {
            PlatformController.platforms = new System.Collections.Generic.List<Platform>();
            PlatformController.AddPlatforms(new System.Drawing.PointF(100, 400));
            PlatformController.startPlatformPosY = 400;
            PlatformController.score = 0;
            PlatformController.GenerateStartSequence();
            PlatformController.bullets.Clear();
            PlatformController.bonuses.Clear();
            PlatformController.enemies.Clear();
            player = new Player();
        }
        private void OnKeyboardUp(object sender,KeyEventArgs e)
        {
            player.physics.dx = 0;
            player.sprite = Properties.Resources.man2;
            switch (e.KeyCode.ToString())
            {
                case "Space":
                    PlatformController.CreateBullet(new PointF(player.physics.transform.position.X + player.physics.transform.size.Width / 2, player.physics.transform.position.Y));
                    break;
            }
        }
        private void OnKeyboardPressed(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode.ToString())
            {
                case "Right":
                    player.physics.dx = 6;
                    break;
                case "Left":
                    player.physics.dx = -6;
                    break;
                case "Space":
                    player.sprite = Properties.Resources.man_shooting;
                  //  PlatformController.CreateBullet(new PointF(player.physics.transform.position.X + player.physics.transform.size.Width / 2, player.physics.transform.position.Y));
                    break;
            }
            if (isGameOver && e.KeyCode == Keys.Enter)
            {
                Init();
                isGameOver = false;
                timer1.Start();
            }
        }
        private void Update(object sender, EventArgs e)
        {
            this.Text = "DoodleJump: Score - " + PlatformController.score;
            player.physics.ApplyPhysics();


            if ((player.physics.transform.position.Y >= PlatformController.platforms[0].transform.position.Y + 200) || //поражение (упал)
                player.physics.StandartCollidePlayerWithObjects(true,false)) //поражение (монстр) 
            {
                GameOver();
                return;     // прекратить выполнение Update, чтобы не сработала физика и отрисовка

            }

            // Столкновение с бонусами
            player.physics.StandartCollidePlayerWithObjects(false,true);
            // Если jetpack включён, но ещё не сменён спрайт — сменить
            if (player.physics.isJetpackActive && !player.isUsingJetpackSprite)
            {
                player.SetJetpackSprite();
            }

            // Если игрок приземлился на платформу — отключить jetpack и вернуть спрайт
            if (player.physics.StandartCollidePlayerWithObjects(true, false))
            {
                if (player.physics.isJetpackActive || player.isUsingJetpackSprite)
                {
                    player.physics.isJetpackActive = false;
                    player.ResetToDefaultSprite();
                }
            }
            // Движение и удаление пуль
            if (PlatformController.bullets.Count > 0)
            {
                for (int i = 0; i < PlatformController.bullets.Count; i++)
                {
                    if (Math.Abs(PlatformController.bullets[i].physics.transform.position.Y - player.physics.transform.position.Y) > 500) // удаление пуль, когда они далеко
                    {
                        PlatformController.RemoveBullet(i);
                        continue;
                    }
                    PlatformController.bullets[i].MoveUp();
                }
            }

            // Проверка столкновений пуль с врагами
            if (PlatformController.enemies.Count > 0)
            {
                for (int i = 0; i < PlatformController.enemies.Count; i++) // удаление врага при столкновении
                {
                    if (PlatformController.enemies[i].physics.StandartCollide())
                    {
                        PlatformController.RemoveEnemy(i);
                        break;
                    }
                }
            }
            
            FollowPlayer();
            Invalidate();// Перерисовка

             // Телепорт по горизонтали
            if (player.physics.transform.position.X > this.ClientSize.Width)
            {
                player.physics.transform.position.X = -player.physics.transform.size.Width;
            }
            else if (player.physics.transform.position.X + player.physics.transform.size.Width < 0)
            {
                player.physics.transform.position.X = this.ClientSize.Width;
            }

        }
        public void FollowPlayer() //камера
        {
            int offset = 400 - (int)player.physics.transform.position.Y; // смещение камеры ближе к низу
            player.physics.transform.position.Y += offset;
            for ( int i = 0;i < PlatformController.platforms.Count;i++)
            {
                var platform = PlatformController.platforms[i];
                platform.transform.position.Y += offset;//offset — это смещение
            }
            for (int i = 0; i < PlatformController.bullets.Count; i++)
            {
                var bullet = PlatformController.bullets[i];
                bullet.physics.transform.position.Y += offset;
            }
            for (int i = 0; i < PlatformController.enemies.Count; i++)
            {
                var enemy = PlatformController.enemies[i];
                enemy.physics.transform.position.Y += offset;
            }
            for (int i = 0; i < PlatformController.bonuses.Count; i++)
            {
                var bonus = PlatformController.bonuses[i];
                bonus.physics.transform.position.Y += offset;
            }
        }
        private void OnRepaint(object sender, PaintEventArgs e) // перерисовка
        {
            Graphics g = e.Graphics;
            if (PlatformController.platforms.Count > 0)
            {
                for (int i = 0; i < PlatformController.platforms.Count; i++)
                {
                    PlatformController.platforms[i].DrawSprite(g);
                }
            }
            if (PlatformController.bullets.Count > 0)
            {
                for (int i = 0; i < PlatformController.bullets.Count; i++)
                {
                    PlatformController.bullets[i].DrawSprite(g);
                }
            }
            if (PlatformController.enemies.Count > 0)
            {
                for (int i = 0; i < PlatformController.enemies.Count; i++)
                {
                    PlatformController.enemies[i].DrawSprite(g);
                }
            }
            if (PlatformController.bonuses.Count > 0)
            {
                for (int i = 0; i < PlatformController.bonuses.Count; i++)
                {
                    PlatformController.bonuses[i].DrawSprite(g);
                }
            }
            player.DrawSprite(g);
            if (isGameOver)
            {
                // Основная надпись "GAME OVER"
                using (Font font = new Font("Arial", 36, FontStyle.Bold))
                using (SolidBrush brush = new SolidBrush(Color.Red))
                {
                    string gameOverText = "GAME OVER";
                    SizeF textSize = g.MeasureString(gameOverText, font);
                    float x = (this.ClientSize.Width - textSize.Width) / 2;
                    float y = (this.ClientSize.Height - textSize.Height) / 2;
                    g.DrawString(gameOverText, font, brush, x, y);

                    // Надпись "Press Enter to restart" под ней
                    using (Font smallFont = new Font("Arial", 14, FontStyle.Regular))
                    using (SolidBrush smallBrush = new SolidBrush(Color.Black))
                    {
                        string infoText = "Press Enter to restart";
                        SizeF infoSize = g.MeasureString(infoText, smallFont);
                        float infoX = (this.ClientSize.Width - infoSize.Width) / 2;
                        float infoY = y + textSize.Height + 10; // немного ниже основной надписи
                        g.DrawString(infoText, smallFont, smallBrush, infoX, infoY);
                    }
                }
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Init(); // запуск и инициализация платформ и игрока
        }
    }
}
