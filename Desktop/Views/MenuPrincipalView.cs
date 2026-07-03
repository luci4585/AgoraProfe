using Desktop.Views;
using Microsoft.Extensions.Caching.Memory;

namespace Desktop
{
    public partial class MenuPrincipalView : Form
    {
        IMemoryCache _memoryCache;
        public MenuPrincipalView(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            InitializeComponent();
        }

        private void SubMenuSalirDelSistema_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SubMenuUsuarios_Click(object sender, EventArgs e)
        {
            var usuariosView = new UsuariosView(_memoryCache);
            usuariosView.MdiParent = this;
            usuariosView.Show();

        }

        private void subMenuCapacitaciones_Click(object sender, EventArgs e)
        {
            var capacitacionesView = new CapacitacionesView(_memoryCache);
            capacitacionesView.MdiParent = this;
            capacitacionesView.Show();
        }

        private void SubMenuTiposDeInscripciones_Click(object sender, EventArgs e)
        {
            var tipoInscripcionView = new TipoInscripcionView(_memoryCache);
            tipoInscripcionView.MdiParent = this;
            tipoInscripcionView.Show();
        }

        private void SubmenuInscripciones_Click(object sender, EventArgs e)
        {
            var inscripcionesView = new InscripcionesView(_memoryCache);
            inscripcionesView.MdiParent = this;
            inscripcionesView.Show();

        }

        private void SubmenuAcreditaciones_Click(object sender, EventArgs e)
        {
            var acreditacionesView = new AcreditacionesView(_memoryCache);
            acreditacionesView.MdiParent = this;
            acreditacionesView.Show();
        }
    }
}
