
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Othello_grafico
{
    public partial class Form1 : Form
    {
        Button[,] botones = new Button[8, 8];
        char[,] tablero = new char[8, 8];

        int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

        char turno = 'B';

        string jugador1 = "";
        string jugador2 = "";

        int puntajeJugador1 = 0;
        int puntajeJugador2 = 0;

        Label lblPuntajes = new Label();

        public Form1()
        {
            InitializeComponent();

            LoginJugadores();

            CrearTablero();
            InicializarJuego();
            CrearLabelPuntajes();
            ActualizarPuntajes();

            this.Text = "Turno: " + jugador1;
        }

        // LOGIN SIN MICROSOFT.VISUALBASIC
        void LoginJugadores()
        {
            jugador1 = "Jugador 1";
            jugador2 = "Jugador 2";

            string nombre1 = Prompt.ShowDialog(
                "Ingrese el nombre del Jugador 1 (Negras)",
                "Login"
            );

            string nombre2 = Prompt.ShowDialog(
                "Ingrese el nombre del Jugador 2 (Blancas)",
                "Login"
            );

            if (nombre1 != "")
            {
                jugador1 = nombre1;
            }

            if (nombre2 != "")
            {
                jugador2 = nombre2;
            }
        }

        void CrearLabelPuntajes()
        {
            lblPuntajes.AutoSize = true;
            lblPuntajes.Top = 500;
            lblPuntajes.Left = 10;
            lblPuntajes.Font = new Font("Arial", 12, FontStyle.Bold);

            this.Controls.Add(lblPuntajes);

            this.Height = 600;
            this.Width = 520;
        }

        void ActualizarPuntajes()
        {
            int negras = 0;
            int blancas = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (tablero[i, j] == 'B')
                    {
                        negras++;
                    }
                    else if (tablero[i, j] == 'W')
                    {
                        blancas++;
                    }
                }
            }

            puntajeJugador1 = negras;
            puntajeJugador2 = blancas;

            lblPuntajes.Text =
                jugador1 + " (Negras): " + puntajeJugador1 +
                "    |    " +
                jugador2 + " (Blancas): " + puntajeJugador2;
        }

        void CrearTablero()
        {
            int size = 60;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Button btn = new Button();

                    btn.Width = size;
                    btn.Height = size;

                    btn.Left = j * size;
                    btn.Top = i * size;

                    btn.BackColor = Color.ForestGreen;

                    btn.FlatStyle = FlatStyle.Flat;

                    btn.Tag = new Point(i, j);

                    btn.Click += ClickCasilla;

                    this.Controls.Add(btn);

                    botones[i, j] = btn;
                }
            }
        }

        void InicializarJuego()
        {
            tablero[3, 3] = 'W';
            tablero[3, 4] = 'B';
            tablero[4, 3] = 'B';
            tablero[4, 4] = 'W';

            botones[3, 3].BackColor = Color.White;
            botones[3, 4].BackColor = Color.Black;
            botones[4, 3].BackColor = Color.Black;
            botones[4, 4].BackColor = Color.White;
        }

        void ClickCasilla(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            Point p = (Point)btn.Tag;

            int fila = p.X;
            int columna = p.Y;

            if (tablero[fila, columna] != '\0' ||
                !MovimientoValido(fila, columna))
            {
                return;
            }

            tablero[fila, columna] = turno;

            if (turno == 'B')
            {
                btn.BackColor = Color.Black;
            }
            else
            {
                btn.BackColor = Color.White;
            }

            VoltearFichas(fila, columna);

            ActualizarPuntajes();

            if (turno == 'B')
            {
                turno = 'W';
                this.Text = "Turno: " + jugador2;
            }
            else
            {
                turno = 'B';
                this.Text = "Turno: " + jugador1;
            }

            if (!HayMovimientos())
            {
                MessageBox.Show("No hay movimientos posibles. Turno perdido.");

                if (turno == 'B')
                {
                    turno = 'W';
                    this.Text = "Turno: " + jugador2;
                }
                else
                {
                    turno = 'B';
                    this.Text = "Turno: " + jugador1;
                }

                if (!HayMovimientos())
                {
                    VerificarGanadorFinal();
                }
            }

            VerificarGanador();
        }

        bool MovimientoValido(int fila, int columna)
        {
            char enemigo;

            if (turno == 'B')
            {
                enemigo = 'W';
            }
            else
            {
                enemigo = 'B';
            }

            for (int dir = 0; dir < 8; dir++)
            {
                int x = fila + dx[dir];
                int y = columna + dy[dir];

                bool hayEnemigo = false;

                while (x >= 0 && x < 8 && y >= 0 && y < 8)
                {
                    if (tablero[x, y] == enemigo)
                    {
                        hayEnemigo = true;
                    }
                    else if (tablero[x, y] == turno)
                    {
                        if (hayEnemigo)
                        {
                            return true;
                        }

                        break;
                    }
                    else
                    {
                        break;
                    }

                    x += dx[dir];
                    y += dy[dir];
                }
            }

            return false;
        }

        bool HayMovimientos()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (tablero[i, j] == '\0')
                    {
                        if (MovimientoValido(i, j))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        void VoltearFichas(int fila, int columna)
        {
            char enemigo;

            if (turno == 'B')
            {
                enemigo = 'W';
            }
            else
            {
                enemigo = 'B';
            }

            for (int dir = 0; dir < 8; dir++)
            {
                int x = fila + dx[dir];
                int y = columna + dy[dir];

                bool hayEnemigo = false;

                while (x >= 0 && x < 8 && y >= 0 && y < 8)
                {
                    if (tablero[x, y] == enemigo)
                    {
                        hayEnemigo = true;
                    }
                    else if (tablero[x, y] == turno)
                    {
                        if (hayEnemigo)
                        {
                            int flipX = fila + dx[dir];
                            int flipY = columna + dy[dir];

                            while (flipX != x || flipY != y)
                            {
                                tablero[flipX, flipY] = turno;

                                if (turno == 'B')
                                {
                                    botones[flipX, flipY].BackColor = Color.Black;
                                }
                                else
                                {
                                    botones[flipX, flipY].BackColor = Color.White;
                                }

                                flipX += dx[dir];
                                flipY += dy[dir];
                            }
                        }

                        break;
                    }
                    else
                    {
                        break;
                    }

                    x += dx[dir];
                    y += dy[dir];
                }
            }
        }

        void GuardarPuntajes(string ganador)
        {
            string ruta = "puntajes.txt";

            string texto =
                "Ganador: " + ganador +
                " | " +
                jugador1 + ": " + puntajeJugador1 +
                " - " +
                jugador2 + ": " + puntajeJugador2 +
                " | Fecha: " + DateTime.Now.ToString();

            File.AppendAllText(ruta, texto + Environment.NewLine);
        }

        void VerificarGanador()
        {
            int vacias = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (tablero[i, j] == '\0')
                    {
                        vacias++;
                    }
                }
            }

            if (vacias == 0)
            {
                VerificarGanadorFinal();
            }
        }

        void VerificarGanadorFinal()
        {
            int negras = 0;
            int blancas = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (tablero[i, j] == 'B')
                    {
                        negras++;
                    }
                    else if (tablero[i, j] == 'W')
                    {
                        blancas++;
                    }
                }
            }

            puntajeJugador1 = negras;
            puntajeJugador2 = blancas;

            string mensaje = "";
            string ganador = "";

            if (negras > blancas)
            {
                ganador = jugador1;
                mensaje = "Ganó " + jugador1;
            }
            else if (blancas > negras)
            {
                ganador = jugador2;
                mensaje = "Ganó " + jugador2;
            }
            else
            {
                ganador = "Empate";
                mensaje = "Empate";
            }

            mensaje += "\n\n" + jugador1 + ": " + negras;
            mensaje += "\n" + jugador2 + ": " + blancas;

            GuardarPuntajes(ganador);

            MessageBox.Show(mensaje);

            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

    // CLASE PARA PEDIR NOMBRES
    public static class Prompt
    {
        public static string ShowDialog(string texto, string titulo)
        {
            Form prompt = new Form()
            {
                Width = 300,
                Height = 150,
                Text = titulo,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label lblTexto = new Label()
            {
                Left = 10,
                Top = 10,
                Width = 260,
                Text = texto
            };

            TextBox txtInput = new TextBox()
            {
                Left = 10,
                Top = 40,
                Width = 260
            };

            Button btnOk = new Button()
            {
                Text = "Aceptar",
                Left = 100,
                Width = 80,
                Top = 70,
                DialogResult = DialogResult.OK
            };

            prompt.Controls.Add(lblTexto);
            prompt.Controls.Add(txtInput);
            prompt.Controls.Add(btnOk);

            prompt.AcceptButton = btnOk;

            return prompt.ShowDialog() == DialogResult.OK
                ? txtInput.Text
                : "";
        }
    }
}