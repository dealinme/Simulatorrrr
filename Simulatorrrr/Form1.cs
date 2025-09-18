using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simulatorrrr
{
    public partial class Form1 : Form
    {
        private List<Proceso> procesos = new List<Proceso>();
        private List<Proceso> procesosFinalizados = new List<Proceso>();
        private int ciclos = 0;
        private Timer timer = new Timer();
        private Random random = new Random();

        private DataGridView dgvProcesos = new DataGridView();
        private DataGridView dgvFinalizados = new DataGridView();
        private TextBox txtNombre = new TextBox();
        private ComboBox cbVelocidad = new ComboBox();
        private Button btnAgregar = new Button();
        private Button btnIniciar = new Button();
        private Button btnPausar = new Button();
        private Button btnReiniciar = new Button();
        private Button btnEjemplo = new Button();
        private Label lblCiclos = new Label();

        public Form1()
        {
            InicializarComponentesGraficos();
            InicializarFormulario();
        }

        private void InicializarComponentesGraficos()
        {
            this.Text = "Simulator2.0 planificación de baja prioridad";
            this.Size = new Size(900, 700);
            this.BackColor = Color.OldLace;

            dgvProcesos.Location = new Point(20, 20);
            dgvProcesos.Size = new Size(840, 250);
            dgvProcesos.Columns.Add("Proceso", "Proceso");
            dgvProcesos.Columns.Add("Dimensiones", "Dimensiones (s)");
            dgvProcesos.Columns.Add("Prioridad", "Prioridad");
            dgvProcesos.Columns.Add("Estado", "Estado");
            dgvProcesos.Columns.Add("Rafaga", "Ráfaga de CPU");
            dgvProcesos.Columns.Add("Quantum", "Quantum (Q)");
            dgvProcesos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvFinalizados.Location = new Point(20, 290);
            dgvFinalizados.Size = new Size(840, 150);
            dgvFinalizados.Columns.Add("NombreF", "Nombre");
            dgvFinalizados.Columns.Add("PrioridadF", "Prioridad");
            dgvFinalizados.Columns.Add("TiempoTotalF", "Ciclos totales");
            dgvFinalizados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            Label lblNombre = new Label() { Text = "Proceso:", Location = new Point(20, 460) };
            txtNombre.Location = new Point(90, 455);

            Label lblVelocidad = new Label() { Text = "Velocidad:", Location = new Point(250, 460) };
            cbVelocidad.Location = new Point(330, 455);

            btnAgregar.Text = "Agregar";
            btnAgregar.Location = new Point(470, 455);
            btnAgregar.Click += btnAgregar_Click;

            btnEjemplo.Text = "Cargar Lista";
            btnEjemplo.Location = new Point(560, 455);
            btnEjemplo.Click += btnEjemplo_Click;

            btnIniciar.Text = "Iniciar";
            btnIniciar.Location = new Point(470, 490);
            btnIniciar.Click += btnIniciar_Click;

            btnPausar.Text = "Pausar";
            btnPausar.Location = new Point(560, 490);
            btnPausar.Click += btnPausar_Click;

            btnReiniciar.Text = "Reiniciar";
            btnReiniciar.Location = new Point(650, 455);
            btnReiniciar.Click += btnReiniciar_Click;

            lblCiclos.Text = "0 Ciclos";
            lblCiclos.Location = new Point(20, 520);
            lblCiclos.Font = new Font("Arial", 10, FontStyle.Bold);

            this.Controls.AddRange(new Control[]
            {
                dgvProcesos, dgvFinalizados,
                lblNombre, txtNombre,
                lblVelocidad, cbVelocidad,
                btnAgregar, btnEjemplo, btnIniciar, btnPausar, btnReiniciar,
                lblCiclos
            });
        }

        private void InicializarFormulario()
        {
            cbVelocidad.Items.AddRange(new object[] { "1X", "2X", "4X", "10X" });
            cbVelocidad.SelectedIndex = 2;

            timer.Interval = 10000;
            timer.Tick += new EventHandler(CicloDePlanificacion);
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            if (string.IsNullOrEmpty(nombre)) return;

            int prioridad = random.Next(1, 6);
            int dimensiones = random.Next(1, 7);
            int quantum = 1;
            procesos.Add(new Proceso(nombre, prioridad, dimensiones, quantum));
            ActualizarTabla();
        }

        private void btnEjemplo_Click(object sender, EventArgs e)
        {
            procesos.Clear();
            procesos.Add(new Proceso("P1", 6, random.Next(1, 7), 1));
            procesos.Add(new Proceso("P2", 1, random.Next(1, 7), 1));
            procesos.Add(new Proceso("P3", 2, random.Next(1, 7), 1));
            procesos.Add(new Proceso("P4", 2, random.Next(1, 7), 1));
            procesos.Add(new Proceso("P5", 3, random.Next(1, 7), 1));
            procesos.Add(new Proceso("P6", 4, random.Next(1, 7), 1));
            procesos.Add(new Proceso("P7", 6, random.Next(1, 7), 1));
            procesos.Add(new Proceso("P8", 4, random.Next(1, 7), 1));
            procesos.Add(new Proceso("P9", 7, random.Next(1, 7), 1));
            procesos.Add(new Proceso("P10", 5, random.Next(1, 7), 1));
            ActualizarTabla();
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            AjustarIntervalo(cbVelocidad.SelectedItem.ToString());
            timer.Start();
        }

        private void btnPausar_Click(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void btnReiniciar_Click(object sender, EventArgs e)
        {
            timer.Stop();
            ciclos = 0;
            procesos.Clear();
            procesosFinalizados.Clear();
            lblCiclos.Text = "0 Ciclos";
            dgvProcesos.Rows.Clear();
            dgvFinalizados.Rows.Clear();
        }

        private void AjustarIntervalo(string velocidad)
        {
            switch (velocidad)
            {
                case "1X": timer.Interval = 10000; break;
                case "2X": timer.Interval = 5000; break;
                case "4X": timer.Interval = 2500; break;
                case "10X": timer.Interval = 1000; break;
                default: timer.Interval = 2500; break;
            }
        }

        private void CicloDePlanificacion(object sender, EventArgs e)
        {
            ciclos++;
            lblCiclos.Text = $"{ciclos} Ciclos";

            var procesoAEjecutar = procesos
                .Where(p => p.Estado != "Finalizado")
                .OrderByDescending(p => p.Prioridad)
                .FirstOrDefault();

            if (procesoAEjecutar != null)
            {
                procesoAEjecutar.Estado = "En ejecución";
                procesoAEjecutar.Rafaga--;
                procesoAEjecutar.CiclosEnEspera = 0;

                if (procesoAEjecutar.Rafaga <= 0)
                {
                    procesoAEjecutar.Estado = "Finalizado";
                    procesosFinalizados.Add(procesoAEjecutar);
                }
            }

            foreach (var p in procesos)
            {
                if (p != procesoAEjecutar && p.Estado != "Finalizado")
                {
                    p.CiclosEnEspera++;
                    p.Estado = p.CiclosEnEspera >= 10 && p.Prioridad >= 4 ? "Inanición" : "En espera";
                }
            }

            ActualizarTabla();
            ActualizarColector();
        }

        private void ActualizarTabla()
        {
            dgvProcesos.Rows.Clear();
            foreach (var p in procesos)
            {
                int index = dgvProcesos.Rows.Add(p.Nombre, p.Dimensiones, p.Prioridad, p.Estado, p.Rafaga, p.Quantum);
                DataGridViewRow row = dgvProcesos.Rows[index];
                switch (p.Estado)
                {
                    case "En ejecución": row.DefaultCellStyle.BackColor = Color.LightGreen; break;
                    case "Finalizado": row.DefaultCellStyle.BackColor = Color.LightCoral; break;
                    case "Inanición": row.DefaultCellStyle.BackColor = Color.LightYellow; break;
                    default: row.DefaultCellStyle.BackColor = Color.LightGray; break;
                }
            }
        }

        private void ActualizarColector()
        {
            dgvFinalizados.Rows.Clear();
            foreach (var pf in procesosFinalizados)
            {
                dgvFinalizados.Rows.Add(pf.Nombre, pf.Prioridad, ciclos);
            }
        }
    }

    public class Proceso
    {
        public string Nombre { get; set; }
        public int Prioridad { get; set; }
        public int Rafaga { get; set; }
        public int Dimensiones { get; set; }
        public int Quantum { get; set; }
        public int CiclosEnEspera { get; set; } = 0;
        public string Estado { get; set; } = "Nuevo";

        public Proceso(string nombre, int prioridad, int dimensiones, int quantum)
        {
            Nombre = nombre;
            Prioridad = prioridad;
            Dimensiones = dimensiones;
            Rafaga = dimensiones;
            Quantum = quantum;
        }
    }
}