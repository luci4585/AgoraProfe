using Microsoft.Extensions.Caching.Memory;
using Service.Models;
using Service.Services;
using System.Data;

namespace Desktop.Views
{
    public partial class TipoInscripcionView : Form
    {
        GenericService<TipoInscripcion> _tipoInscripcionService;
        TipoInscripcion _currentTipoInscripcion;
        List<TipoInscripcion>? _tiposInscripciones;

        public TipoInscripcionView(IMemoryCache memoryCache)
        {
            _tipoInscripcionService = new GenericService<TipoInscripcion>(memoryCache);
            InitializeComponent();
            _ = GetAllData();
            checkVerEliminados.CheckedChanged += DisplayHideControlsRestoreButton;
        }

        private void DisplayHideControlsRestoreButton(object? sender, EventArgs e)
        {
            BtnRestaurar.Visible=checkVerEliminados.Checked;
            TxtBuscar.Enabled = !checkVerEliminados.Checked;
            BtnModificar.Enabled = !checkVerEliminados.Checked;
            BtnEliminar.Enabled = !checkVerEliminados.Checked;
            BtnAgregar.Enabled = !checkVerEliminados.Checked;
            BtnBuscar.Enabled = !checkVerEliminados.Checked;
        }

        private async Task GetAllData()
        {
            if (checkVerEliminados.Checked)
                _tiposInscripciones = await _tipoInscripcionService.GetAllDeletedsAsync();
            else
                _tiposInscripciones = await _tipoInscripcionService.GetAllAsync();

            DataGrid.DataSource = _tiposInscripciones;
            DataGrid.Columns["Id"].Visible = false; // Ocultar la columna Id
            DataGrid.Columns["IsDeleted"].Visible = false; // Ocultar la columna Eliminado

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
                TipoInscripcion entitySelected = (TipoInscripcion)DataGrid.SelectedRows[0].DataBoundItem;
                var respuesta = MessageBox.Show($"¿Seguro que desea eliminar el Tipo de inscripción {entitySelected.Nombre}?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (respuesta == DialogResult.Yes)
                {
                    if (await _tipoInscripcionService.DeleteAsync(entitySelected.Id))
                    {
                        LabelStatusMessage.Text = $"Tipo inscripción {entitySelected.Nombre} eliminada correctamente";
                        TimerStatusBar.Start(); // Iniciar el temporizador para mostrar el mensaje en la barra de estado
                        await GetAllData();
                    }
                    else
                    {
                        MessageBox.Show("Error al eliminar el tipo de inscripción", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un tipo de inscripción a eliminar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            LimpiarControlesAgregarEditar();
            TabControl.SelectedTab = TabPageAgregarEditar;
        }

        private void LimpiarControlesAgregarEditar()
        {
            TxtNombre.Clear();           
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            TabControl.SelectedTab = TabPageLista;

        }

        private async void BtnGuardar_Click(object sender, EventArgs e)
        {
            TipoInscripcion entidadAGuardar = new TipoInscripcion
            {
                Id = _currentTipoInscripcion?.Id ?? 0,
                Nombre = TxtNombre.Text,                
            };
            bool response = false;
            if (_currentTipoInscripcion != null)
            {
                response = await _tipoInscripcionService.UpdateAsync(entidadAGuardar);
            }
            else
            {
                var nuevaEntidad = await _tipoInscripcionService.AddAsync(entidadAGuardar);
                response = nuevaEntidad != null;
            }
            if (response)
            {
                _currentTipoInscripcion = null; // Reset the modified entity after saving
                LabelStatusMessage.Text = $"Tipo de inscripción {entidadAGuardar.Nombre} guardada correctamente";
                TimerStatusBar.Start(); // Iniciar el temporizador para mostrar el mensaje en la barra de estado
                await GetAllData();
                LimpiarControlesAgregarEditar();
                TabControl.SelectedTab=TabPageLista;
            }
            else
            {
                MessageBox.Show("Error al guardar el tipo de inscripción", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnModificar_Click(object sender, EventArgs e)
        {
            //checheamos que haya una capacitación seleccionada
            if (DataGrid.RowCount > 0 && DataGrid.SelectedRows.Count > 0)
            {
                _currentTipoInscripcion = (TipoInscripcion)DataGrid.SelectedRows[0].DataBoundItem;
                TxtNombre.Text = _currentTipoInscripcion.Nombre;
                TabControl.SelectedTab = TabPageAgregarEditar;
            }
            else
            {
                MessageBox.Show("Debe seleccionar un tipo de inscripción a modificar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnBuscar_Click(object sender, EventArgs e)
        {
            DataGrid.DataSource = await _tipoInscripcionService.GetAllAsync(TxtBuscar.Text);
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
                TipoInscripcion entitySelected = (TipoInscripcion)DataGrid.SelectedRows[0].DataBoundItem;
                var respuesta = MessageBox.Show($"¿Seguro que desea recuperar el tipo de inscripción {entitySelected.Nombre}?", "Confirmar Restauración", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (respuesta == DialogResult.Yes)
                {
                    if (await _tipoInscripcionService.RestoreAsync(entitySelected.Id))
                    {
                        LabelStatusMessage.Text = $"Tipo de inscripción {entitySelected.Nombre} restaurada correctamente";
                        TimerStatusBar.Start(); // Iniciar el temporizador para mostrar el mensaje en la barra de estado
                        await GetAllData();
                    }
                    else
                    {
                        MessageBox.Show("Error al restaurar el tipo de inscripción", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un tipo de inscripción para restaurarla", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
