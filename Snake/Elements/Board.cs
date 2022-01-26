using Game.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake.Elements
{
    public class Board : Sprite
    {
        #region Objects
        private Point _tailLocation;
        private bool _blockMove;

        /// <summary>
        /// Evento que da aviso al formulario que finalizo el juego
        /// </summary>
        public Action GameOver;
        #endregion

        #region Constructor
        public Board(Elements.Resources resources) : base(null, Point.Empty)
        {
            this.Resources = resources;
            Cell_Size = new Size(20, 20);
            Grid_Size = new Size(30, 20);

            Locations = new List<Point>();
            for (int x = 0; x < Grid_Size.Width; x++)
                for (int y = 0; y < Grid_Size.Height; y++)
                    Locations.Add(new Point(x, y));

            Snake = new List<Point>() { Point.Empty };

            Current_Direction = Directions.Right;
            Add_Food();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Recursos graficos del juego
        /// </summary>
        private Elements.Resources Resources { get; set; }
        /// <summary>
        /// Tamaño en celdas de la grilla
        /// </summary>
        private Size Grid_Size { get; set; }
        /// <summary>
        /// Tamaño en pixeles de cada celda
        /// </summary>
        private Size Cell_Size { get; set; }
        /// <summary>
        /// Coordenadas de la grilla en la cual se encuentra la serpiente
        /// </summary>
        private List<Point> Snake { get; set; }
        /// <summary>
        /// Direccion hacia donde se desplaza la serpiente
        /// </summary>
        public Directions Current_Direction { get; private set; }
        /// <summary>
        /// Coordenada donde se encuentra la comida
        /// </summary>
        private Point Food_Location { get; set; }
        /// <summary>
        /// Coordenadas de cada celda el tablero
        /// </summary>
        private List<Point> Locations { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Actualiza el juego en cada Frame
        /// </summary>
        public void Update()
        {
            Move_Snake();
            Eat_Food();

            Check_Colition();
        }
        /// <summary>
        /// Cambia la direccion hacia donde se desplaza la serpiente
        /// </summary>
        /// <param name="direction"></param>
        public void Change_Direction(Directions direction)
        {
            if (!_blockMove)
            {
                this.Current_Direction = direction;
                _blockMove = true; // bloquea el movimiento hasta que se desplace a la proxima celda
                // es necesario el bloqueo para evitar varios cambios de direccion antes que la serpiente cambie de celda
            }
        }
        /// <summary>
        /// Obtiene la puntuacion
        /// </summary>
        /// <returns></returns>
        public int Get_Score()
        {
            return this.Snake.Count() - 1; // la puntuacion es igual al largo de la serpiente (sin la cabeza)
        }

        /// <summary>
        /// Desplaza la serpiente
        /// </summary>
        private void Move_Snake()
        {
            var last = Snake.Last();
            var first = Snake.First();
            Snake.Remove(last); // remueve el ultimo segmeento de la serpiente (la cola)

            var _direction =
                Current_Direction == Directions.Right ? new Point(1, 0) :
                Current_Direction == Directions.Left ? new Point(-1, 0) :
                Current_Direction == Directions.Up ? new Point(0, -1) :
                Current_Direction == Directions.Down ? new Point(0, 1) :
                Point.Empty;
            
            // agrega un nuevo segmento a la serpiente segun la direccion en la que se desplace
            var _new = new Point(first.X + _direction.X, first.Y + _direction.Y);
            Snake.Insert(0, _new);
            
            _tailLocation = last; // guarda la posicion que tenia la cola de la serpiente antes de moverse
            _blockMove = false;
        }
        private void Eat_Food()
        {
            // valida si la cabeza de la serpiente posee la misma ubicacion que la comida
            if (this.Snake.First() == Food_Location)
            {
                // agrega un nuevo segmento a la serpiente
                // el nuevo segmento se agrega en la ultima posicion que tuvo la cola de la serpiente
                this.Snake.Add(_tailLocation);
                Add_Food();
            }
        }
        /// <summary>
        /// Agrega comida en el escenario
        /// </summary>
        private void Add_Food()
        {
            // obtiene todas las coordenadas libres del escenario, es decir, todas en las que no se encuentra la serpiente 
            var freeCells = Locations.Where(pos => !Snake.Contains(pos)).ToList(); // celdas libres

            // utiliza un valor aleatorio para obtener la nueva coordenada de la comida
            Random rand = new Random();
            int index = rand.Next(0, freeCells.Count - 1); 
            Food_Location = freeCells[index];
        }
        /// <summary>
        /// valida si la cabeza de la serpiente coliciono
        /// </summary>
        private void Check_Colition()
        {
            var head = Snake.First(); // cabeza de la serpiente    
            bool _collition =
                head.X < 0 ||                   // limite izquierda
                head.X >= Grid_Size.Width ||    // limite derecha
                head.Y < 0 ||                   // limite superior 
                head.Y >= Grid_Size.Height ||   // limite inferior
                Snake.Skip(1).Contains(head);   // cuerpo de la serpiente

            // si se detecta colicion finaliza el juego
            if (_collition)
                GameOver();
        }
        #endregion

        #region Draw
        /// <summary>
        /// Dibuja la grilla
        /// </summary>
        /// <param name="drawHandler"></param>
        public override void Draw(DrawHandler drawHandler)
        {
            // dibuja la grilla de fondo
            Locations.ForEach(pos =>
            {
                Point _position = new Point(pos.X * Cell_Size.Width, pos.Y * Cell_Size.Height);
                drawHandler.Draw(this.Resources.Grid, _position);
            });

            // recorre cada segmento del cuerpo de la serpiente
            for (int i = 0; i < Snake.Count; i++)
            {
                var location = Snake[i];
                Point _position = new Point(location.X * Cell_Size.Width, location.Y * Cell_Size.Height);

                var image = i == 0 ? this.Resources.Block_Blue : this.Resources.Block_Black; // la cabeza la dibuja azul
                drawHandler.Draw(image, _position);
            }

            // dibuja la comida
            Point _food_position = new Point(Food_Location.X * Cell_Size.Width, Food_Location.Y * Cell_Size.Height);
            drawHandler.Draw(this.Resources.Food, _food_position);
        }
        #endregion
    }

    public enum Directions
    {
        Left,
        Right,
        Up,
        Down
    }
}
