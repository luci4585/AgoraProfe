using Firebase.Auth;
using Firebase.Auth.Providers;
using Microsoft.Extensions.Caching.Memory;
using Service.Interfaces;
using Service.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Desktop.Views
{
    public partial class LoginView : Form
    {
        private readonly IMemoryCache? _memoryCache;
        IUsuarioService _usuarioService;
        int loginFailsCount = 0;

        public LoginView(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _usuarioService= new UsuarioService(_memoryCache);
            InitializeComponent();

        }


        private async void BtnIniciarSesion_Click(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                var login = await _usuarioService.LoginInSystem(TxtEmail.Text, TxtPassword.Text);
                if (login)
                {
                    this.Hide();
                    var menuPrincipalView = new MenuPrincipalView();
                    menuPrincipalView.ShowDialog();
                    this.Close();
                }
            }
            catch (FirebaseAuthException ex)
            {
                MessageBox.Show("Error al iniciar sesión. Verifique sus credenciales.");
                loginFailsCount++;
                if (loginFailsCount >= 3)
                {
                    MessageBox.Show("Ha excedido el número máximo de intentos fallidos. La aplicación se cerrará.");
                    this.Close();
                }
                this.Enabled = true;
            }
        }

        private void CheckVerContraseña_CheckedChanged(object sender, EventArgs e)
        {
            TxtPassword.PasswordChar = CheckVerContraseña.Checked ? '\0' : '*';

        }

        private void TxtEmail_KeyPress(object sender, KeyPressEventArgs e)
        {
            //si presiono enter, seleccioni el siguiente campo
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true; // Evita que se agregue un salto de línea
                TxtPassword.Focus(); // Mueve el foco al campo de contraseña
            }
        }

        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            //si se presiona enter, se intenta iniciar sesión
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true; // Evita que se agregue un salto de línea
                BtnIniciarSesion.PerformClick(); // Simula el clic en el botón de iniciar sesión
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
