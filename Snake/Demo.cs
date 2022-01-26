using Game.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    public partial class Demo : Game.Game
    {
        #region Consutrctor
        public Demo()
        {
            InitializeComponent();
            Initialize();

            Start_Game();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Recursos graficos del juego
        /// </summary>
        public Elements.Resources Resources { get; set; }
        /// <summary>
        /// Grilla del tetris
        /// </summary>
        public Elements.Board Board { get; set; }
        #endregion

        #region Events
        private void btnLinkedIn_Click(object sender, EventArgs e)
        {
            base.Open_ZeroSoft_URL();
        }
        protected override bool IsInputKey(Keys keyData)
        {

            if (this.Board.Current_Direction == Elements.Directions.Right || this.Board.Current_Direction == Elements.Directions.Left)
            {
                if (keyData == Keys.Up)
                    this.Board.Change_Direction(Elements.Directions.Up);
                else if (keyData == Keys.Down)
                    this.Board.Change_Direction(Elements.Directions.Down);
            }

            if (this.Board.Current_Direction == Elements.Directions.Up || this.Board.Current_Direction == Elements.Directions.Down)
            {
                if (keyData == Keys.Right)
                    this.Board.Change_Direction(Elements.Directions.Right);
                else if (keyData == Keys.Left)
                    this.Board.Change_Direction(Elements.Directions.Left);
            }

            return base.IsInputKey(keyData);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Carga los recursos graficos del juego
        /// </summary>
        private void Initialize()
        {
            string directory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
            this.Resources = new Elements.Resources()
            {
                Block_Black = Load_Image($"{directory}/Block_Black.png"),
                Block_Blue = Load_Image($"{directory}/Block_Blue.png"),
                Grid = Load_Image($"{directory}/Grid.png"),
                Food = Load_Image($"{directory}/Food.png"),
            };
        }
        /// <summary>
        /// Inicia una nueva partida
        /// </summary>
        private void Start_Game()
        {
            Board = new Elements.Board(this.Resources);
            Board.GameOver += () =>
            {
                base.Pause_Game();
                MessageBox.Show("Game Over");
                Start_Game(); // reinicia el juego
            };

            base.Resume_Game();
        }
        #endregion

        #region Updates
        /// <summary>
        /// Metodo que donde se escribe la logica del juego
        /// </summary>
        /// <param name="gameTime">Informacion de tiempo de juego transcurrido</param>
        protected override void Update(GameTime gameTime)
        {
            Board.Update();
            lblScore.Text = Board.Get_Score().ToString();
        }
        #endregion

        #region Draw
        /// <summary>
        /// Dibuja la grilla
        /// </summary>
        /// <param name="drawHandler"></param>
        public override void Draw(DrawHandler drawHandler)
        {
            this.Board.Draw(drawHandler);
        }
        #endregion

    }
}
