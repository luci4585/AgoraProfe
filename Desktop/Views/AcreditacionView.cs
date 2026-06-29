using Desktop.ExtensionMethod;
using Microsoft.Extensions.Caching.Memory;
using Service.Models;
using Service.Services;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;

namespace Desktop.Views
{
    public partial class AcreditacionesView : Form
    {
        GenericService<Capacitacion> _capacitacionService;
        InscripcionService _inscripcionService;
        List<Inscripcion>? _inscripciones = new();

        public AcreditacionesView(IMemoryCache memoryCache)
        {
            _capacitacionService = new GenericService<Capacitacion>(memoryCache);
            _inscripcionService = new InscripcionService(memoryCache);
            InitializeComponent();
            _ = GetAllData();
        }

        private async Task GetAllData()
        {
            var stopwatch = Stopwatch.StartNew();
            var GetComboTask = GetComboCapacitaciones();
            await Task.WhenAll(GetComboTask);
            Debug.WriteLine($"Tiempo de carga de datos: {stopwatch.ElapsedMilliseconds} ms");
        }

        private async Task GetComboCapacitaciones()
        {
            //cargamos las capacitaciones en el combo
            var capacitaciones = await _capacitacionService.GetAllAsync();
            ComboCapacitaciones.DataSource = capacitaciones?.Where(c => c.InscripcionAbierta).ToList();
            ComboCapacitaciones.DisplayMember = "Nombre";
            ComboCapacitaciones.ValueMember = "Id";
        }

        private async void ComboCapacitaciones_SelectedIndexChanged(object sender, EventArgs e)
        {
            //controlamos que no sea null y haya una capacitación seleccionada
            if (ComboCapacitaciones.SelectedItem is Capacitacion selectedCapacitacion)
            {
                RefreshInscripciones(selectedCapacitacion);
            }
        }


        private async void RefreshInscripciones(Capacitacion selectedCapacitacion)
        {
            _inscripciones = selectedCapacitacion.Inscripciones.ToList();
            //ordeno las inscripciones por apellido y nombre
            GridInscripciones.DataSource = _inscripciones?.OrderBy(i => i?.Usuario?.Apellido).ThenBy(i => i?.Usuario?.Nombre).ToList();

            //ocultamos las columnas Id, UsuarioId, TipoInscripcionId,CapacitacionId, Capacitacion
            GridInscripciones.HideColumns("Id", "UsuarioId", "TipoInscripcionId", "CapacitacionId", "Capacitacion", "UsuarioCobroId", "IsDeleted", "UsuarioCobro", "Pagado");
            if (GridInscripciones.Columns.Contains("Importe"))
            {
                GridInscripciones.Columns["Importe"].DefaultCellStyle.Format = "C2";
                GridInscripciones.Columns["Importe"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            AgregarBotonAcreditarInscripcion();
            ActualizarTextoBotonesAccion();
        }

        private void AgregarBotonAcreditarInscripcion()
        {
            if (GridInscripciones.Columns["Acciones"] == null)
            {
                var buttonColumn = new DataGridViewButtonColumn
                {
                    Name = "Acciones",
                    HeaderText = "Acciones",
                    UseColumnTextForButtonValue = false // usaremos texto dinámico por celda
                };
                GridInscripciones.Columns.Add(buttonColumn);
                GridInscripciones.CellContentClick += AcreditarInscripcion();
            }
        }

        private void ActualizarTextoBotonesAccion()
        {
            if (GridInscripciones.Columns["Acciones"] == null) return;
            foreach (DataGridViewRow row in GridInscripciones.Rows)
            {
                if (row.DataBoundItem is Inscripcion ins)
                {
                    var cell = row.Cells["Acciones"] as DataGridViewButtonCell;
                    if (cell != null)
                    {
                        cell.Value = ins.Acreditado ? "Desacreditar inscripción" : "Acreditar inscripción";
                    }
                }
            }
        }

        private DataGridViewCellEventHandler AcreditarInscripcion()
        {
            // Toggle acreditación/desacreditación
            return async (sender, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex != GridInscripciones.Columns["Acciones"].Index)
                    return;

                var buttonCell = GridInscripciones.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewButtonCell;
                var selectedInscripcion = GridInscripciones.Rows[e.RowIndex].DataBoundItem as Inscripcion;
                if (selectedInscripcion == null)
                {
                    MessageBox.Show("Seleccione una inscripción válida.");
                    return;
                }

                bool nuevoEstado = !selectedInscripcion.Acreditado; // toggle
                bool estadoAnterior = selectedInscripcion.Acreditado;
                selectedInscripcion.Acreditado = nuevoEstado;

                try
                {
                    if (await _inscripcionService.UpdateAsync(selectedInscripcion))
                    {
                        // ajustar texto según nuevo estado
                        if (buttonCell != null)
                        {
                            buttonCell.Value = nuevoEstado ? "Desacreditar inscripción" : "Acreditar inscripción";
                        }
                    }
                    else
                    {
                        // revertir si fallo service
                        selectedInscripcion.Acreditado = estadoAnterior;
                        if (buttonCell != null)
                            buttonCell.Value = estadoAnterior ? "Desacreditar inscripción" : "Acreditar inscripción";
                        MessageBox.Show("No se pudo actualizar la inscripción.");
                    }
                }
                catch (Exception ex)
                {
                    selectedInscripcion.Acreditado = estadoAnterior;
                    if (buttonCell != null)
                        buttonCell.Value = estadoAnterior ? "Desacreditar inscripción" : "Acreditar inscripción";
                    MessageBox.Show($"Error al actualizar la inscripción: {ex.Message}");
                }

                GridInscripciones.Refresh();
            };
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            TxtBuscarInscripto_TextChanged(sender, e);
        }

        private async void TxtBuscarInscripto_TextChanged(object sender, EventArgs e)
        {
            GridInscripciones.DataSource = _inscripciones?
                .Where(i => i.Usuario!.Nombre!.Contains(TxtBuscarInscripto.Text,
                                                StringComparison.OrdinalIgnoreCase) ||
                            i.Usuario!.Apellido!.Contains(TxtBuscarInscripto.Text,
                                                StringComparison.OrdinalIgnoreCase))
                .OrderBy(i => i.Usuario!.Apellido)
                .ThenBy(i => i.Usuario!.Nombre)
                .ToList();
            ActualizarTextoBotonesAccion();
        }
    }
}
