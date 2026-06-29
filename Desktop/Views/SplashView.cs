using Microsoft.Extensions.Caching.Memory;
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
    public partial class SplashView : Form
    {
        IMemoryCache _memoryCache;
        public SplashView(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            InitializeComponent();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            ProgressBar.Value += 2;
            if(ProgressBar.Value == 100)
            {
                Timer.Stop();
                this.Hide();
                var login = new LoginView(_memoryCache);
                login.ShowDialog();
                this.Close();

            }
        }
    }
}
