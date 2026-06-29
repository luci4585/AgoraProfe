using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Utils
{
    public static class ApiEndpoints
    {
        public static string Capacitacion { get; set; } = "capacitaciones";
        public static string Usuario { get; set; } = "usuarios";
        public static string Inscripcion { get; set; } = "inscripciones";
        public static string TipoInscripcion { get; set; } = "tiposinscripciones";
        public static string TipoInscripcionCapacitacion { get; set; } = "tiposinscripcionescapacitaciones";
        public static string Login { get; set; } = "auth";
        
        public static string GetEndpoint(string name)
        {
            return name switch
            {
                nameof(Capacitacion) => Capacitacion,
                nameof(Usuario) => Usuario,
                nameof(Inscripcion) => Inscripcion,
                nameof(TipoInscripcion) => TipoInscripcion,
                nameof(TipoInscripcionCapacitacion) => TipoInscripcionCapacitacion,
                _ => throw new ArgumentException($"Endpoint '{name}' no está definido.")
            };
        }
    }
}
