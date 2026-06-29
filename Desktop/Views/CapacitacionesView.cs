using Desktop.ExtensionMethod;
using Microsoft.Extensions.Caching.Memory;
using Service.Interfaces;
using Service.Models;
using Service.Services;
using System.Data;

namespace Desktop.Views
{
    public partial class CapacitacionesView : Form
    {
        GenericService<Capacitacion> _capacitacionService;
        GenericService<TipoInscripcion> _tipoInscripcionService;
        Capacitacion? _currentCapacitacion;
        List<Capacitacion>? _capacitaciones;

        public CapacitacionesView(IMemoryCache memoryCache)
        {
            _capacitacionService = new GenericService<Capacitacion>(memoryCache);
            _tipoInscripcionService = new GenericService<TipoInscripcion>(memoryCache);
            InitializeComponent();
            _ = GetAllData();
            checkVerEliminados.CheckedChanged += DisplayHideControlsRestoreButton;
        }

        private void DisplayHideControlsRestoreButton(object? sender, EventArgs e)
        {
            BtnRestaurar.Visible = checkVerEliminados.Checked;
            TxtBuscar.Enabled = !checkVerEliminados.Checked;
            BtnModificar.Enabled = !checkVerEliminados.Checked;
            BtnEliminar.Enabled = !checkVerEliminados.Checked;
            BtnAgregar.Enabled = !checkVerEliminados.Checked;
            BtnBuscar.Enabled = !checkVerEliminados.Checked;
        }

        private async Task GetAllData()
        {
            if (checkVerEliminados.Checked)
                _capacitaciones = await _capacitacionService.GetAllDeletedsAsync();
            else
                _capacitaciones = await _capacitacionService.GetAllAsync();

            DataGrid.DataSource = _capacitaciones;
            DataGrid.Columns["Id"].Visible = false; // Ocultar la columna Pais
            DataGrid.Columns["IsDeleted"].Visible = false; // Ocultar la columna Eliminado
            await GetComboTiposDeInscripciones();

        }

        private async Task GetComboTiposDeInscripciones()
        {
            ComboTiposInscripciones.DataSource = await _tipoInscripcionService.GetAllAsync();
            ComboTiposInscripciones.DisplayMember = "Nombre";
            ComboTiposInscripciones.ValueMember = "Id";
        }

        private void GridPeliculas_SelectionChanged_1(object sender, EventArgs e)
        {
            if (DataGrid.RowCount > 0 && DataGrid.SelectedRows.Count > 0)
            {
                //Capacitacion _curr = (Pelicula)GridPeliculas.SelectedRows[0].DataBoundItem;
                //FilmPicture.ImageLocation = peliSeleccionada.portada;
            }
        }

        private async void BtnEliminar_Click_1(object sender, EventArgs e)
        {
            //checheamos que haya peliculas seleccionadas
            if (DataGrid.RowCount > 0 && DataGrid.SelectedRows.Count > 0)
            {
                Capacitacion entitySelected = (Capacitacion)DataGrid.SelectedRows[0].DataBoundItem;
                var respuesta = MessageBox.Show($"¿Seguro que desea eliminar la capacitación {entitySelected.Nombre}?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (respuesta == DialogResult.Yes)
                {
                    if (await _capacitacionService.DeleteAsync(entitySelected.Id))
                    {
                        LabelStatusMessage.Text = $"Capacitación {entitySelected.Nombre} eliminada correctamente";
                        TimerStatusBar.Start(); // Iniciar el temporizador para mostrar el mensaje en la barra de estado
                        await GetAllData();
                    }
                    else
                    {
                        MessageBox.Show("Error al eliminar la capacitación", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar una capacitación para eliminarla", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            LimpiarControlesAgregarEditar();
            _currentCapacitacion = new Capacitacion();
            TabControl.SelectedTab = TabPageAgregarEditar;
        }

        private void LimpiarControlesAgregarEditar()
        {
            TxtNombre.Clear();
            TxtPonente.Clear();
            DateTimeFechaHora.Value = DateTime.Now;
            checkInscripcionAbierta.Checked = false;
            NumericCupo.Value = 0;
            TxtDetalle.Clear();
            GridTiposDeInscripciones.DataSource = null;

        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            TabControl.SelectedTab = TabPageLista;

        }

        private async void BtnGuardar_Click(object sender, EventArgs e)
        {

            _currentCapacitacion.Nombre = TxtNombre.Text;
            _currentCapacitacion.Detalle = TxtDetalle.Text;
            _currentCapacitacion.Ponente = TxtPonente.Text;
            _currentCapacitacion.FechaHora = DateTimeFechaHora.Value;
            _currentCapacitacion.Cupo = (int)NumericCupo.Value;
            _currentCapacitacion.InscripcionAbierta = checkInscripcionAbierta.Checked;

            bool successfull = false;
            try
            {
                if (_currentCapacitacion.Id == 0)//capacitación nueva
                {
                    var nuevacapacitacion = await _capacitacionService.AddAsync(_currentCapacitacion);
                    successfull = nuevacapacitacion != null;
                }

                if (_currentCapacitacion.Id > 0) //modificando capacitación existente
                {
                    successfull = await _capacitacionService.UpdateAsync(_currentCapacitacion);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la capacitación: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (successfull)
            {
                LabelStatusMessage.Text = $"Capacitación {_currentCapacitacion.Nombre} guardada correctamente";
                TimerStatusBar.Start(); // Iniciar el temporizador para mostrar el mensaje en la barra de estado
                await GetAllData();
                LimpiarControlesAgregarEditar();
                TabControl.SelectedTab = TabPageLista;
                _currentCapacitacion = null; // Reset the modified movie after saving

            }
            else
            {
                MessageBox.Show("Error al guardar la capacitación", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnModificar_Click(object sender, EventArgs e)
        {
            //checheamos que haya una capacitación seleccionada
            if (DataGrid.RowCount > 0 && DataGrid.SelectedRows.Count > 0)
            {
                _currentCapacitacion = (Capacitacion)DataGrid.SelectedRows[0].DataBoundItem;
                TxtNombre.Text = _currentCapacitacion.Nombre;
                TxtDetalle.Text = _currentCapacitacion.Detalle;
                TxtPonente.Text = _currentCapacitacion.Ponente;
                DateTimeFechaHora.Value = _currentCapacitacion.FechaHora;
                NumericCupo.Value = _currentCapacitacion.Cupo;
                checkInscripcionAbierta.Checked = _currentCapacitacion.InscripcionAbierta;
                GridTiposDeInscripciones.DataSource = _currentCapacitacion.TiposDeInscripciones;
                GridTiposDeInscripciones.HideColumns("Id", "CapacitacionId", "Capacitacion", "TipoInscripcionId", "IsDeleted");
                //mostramos la columna costo como moneda con 2 decimales
                GridTiposDeInscripciones.Columns["Costo"].DefaultCellStyle.Format = "C2";


                TabControl.SelectedTab = TabPageAgregarEditar;
            }
            else
            {
                MessageBox.Show("Debe seleccionar una capacitación para modificarla", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnBuscar_Click(object sender, EventArgs e)
        {
            DataGrid.DataSource = await _capacitacionService.GetAllAsync(TxtBuscar.Text);
        }

        private void TxtBuscar_TextChanged(object sender, EventArgs e)
        {
            //BtnBuscar.PerformClick();
        }

        private void TimerStatusBar_Tick(object sender, EventArgs e)
        {
            LabelStatusMessage.Text = string.Empty;
            TimerStatusBar.Stop(); // Detener el temporizador después de mostrar el mensaje
        }

        private async void checkVerEliminados_CheckedChanged(object sender, EventArgs e)
        {
            await GetAllData();
        }

        private async void BtnRestaurar_Click(object sender, EventArgs e)
        {
            if (!checkVerEliminados.Checked) return;
            //checheamos que haya peliculas seleccionadas
            if (DataGrid.RowCount > 0 && DataGrid.SelectedRows.Count > 0)
            {
                Capacitacion entitySelected = (Capacitacion)DataGrid.SelectedRows[0].DataBoundItem;
                var respuesta = MessageBox.Show($"¿Seguro que recuper la capacitación {entitySelected.Nombre}?", "Confirmar Restauración", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (respuesta == DialogResult.Yes)
                {
                    if (await _capacitacionService.RestoreAsync(entitySelected.Id))
                    {
                        LabelStatusMessage.Text = $"Capacitación {entitySelected.Nombre} restaurada correctamente";
                        TimerStatusBar.Start(); // Iniciar el temporizador para mostrar el mensaje en la barra de estado
                        await GetAllData();
                    }
                    else
                    {
                        MessageBox.Show("Error al restaurar la capacitación", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar una capacitación para restaurarla", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAniadir_Click(object sender, EventArgs e)
        {
            var tipoInscripcionCapacitacion = new TipoInscripcionCapacitacion
            {
                TipoInscripcionId = (int)ComboTiposInscripciones.SelectedValue,
                TipoInscripcion = (TipoInscripcion)ComboTiposInscripciones.SelectedItem,
                CapacitacionId = _currentCapacitacion?.Id ?? 0,                
                Costo = numericCosto.Value
            };
            _currentCapacitacion?.TiposDeInscripciones?.Add(tipoInscripcionCapacitacion);
            GridTiposDeInscripciones.DataSource = _currentCapacitacion?.TiposDeInscripciones?.ToList();
            GridTiposDeInscripciones.HideColumns("Id", "CapacitacionId", "Capacitacion", "TipoInscripcionId", "IsDeleted");
            GridTiposDeInscripciones.Columns["Costo"].DefaultCellStyle.Format = "C2";

        }

        private void BtnQuitar_Click(object sender, EventArgs e)
        {
            var tipoInscripcionCapacitacionSeleccionado = (TipoInscripcionCapacitacion)GridTiposDeInscripciones.SelectedRows[0].DataBoundItem;
            _currentCapacitacion?.TiposDeInscripciones.Remove(tipoInscripcionCapacitacionSeleccionado);
            GridTiposDeInscripciones.DataSource = _currentCapacitacion?.TiposDeInscripciones.ToList();
        }
    }
}
