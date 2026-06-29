using Desktop.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Desktop
{
        internal static class Program
        {
            public static IServiceProvider ServiceProvider { get; private set; }
            /// <summary>
            ///  The main entry point for the application.
            /// </summary>
            [STAThread]
            static void Main()
            {


                // Configurar el contenedor de dependencias
                var services = new ServiceCollection();
                ConfigureServices(services);
                ServiceProvider = services.BuildServiceProvider();

                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();
                Application.Run(new SplashView());
            }

            private static void ConfigureServices(IServiceCollection services)
            {
                services.AddMemoryCache(); // Agregar el servicio de caché en memoria
                services.AddScoped<LoginView>();
                services.AddScoped<MenuPrincipalView>();
                services.AddScoped<AcreditacionesView>();
                services.AddScoped<UsuariosView>();
                services.AddScoped<CapacitacionesView>();
                services.AddScoped<InscripcionesView>();
                services.AddScoped<TipoInscripcionView>();
                services.AddScoped<SplashView>();

            }
        }
}