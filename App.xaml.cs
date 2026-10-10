using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Eventia.Models;
using System.Collections.ObjectModel;

namespace Eventia
{
    public partial class App : Application
    {
        public static ObservableCollection<Evento> EventosGlobales { get; set; } = new ObservableCollection<Evento>();

        public App()
        {
            InitializeComponent();

            // Solo los creamos si la lista está vacía
            if (EventosGlobales.Count == 0)
            {
                // 1. GRATIS - Tecnología (En Univalle, perfecto para tu carrera)
                EventosGlobales.Add(new Evento
                {
                    Titulo = "Feria de Desarrollo de Software",
                    Ubicacion = "Universidad del Valle",
                    Fecha = "Oct 24, 2026",
                    Precio = "Gratis",
                    Categoria = "Tecnología",
                    Duracion = "Todo el día",
                    Distancia = "12.5 km",
                    ImagenUrl = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=600&q=80",
                    EsFavorito = true // Empezará con la estrellita dorada encendida
                });

                // 2. PAGO - Deportes (Clásico caleño)
                EventosGlobales.Add(new Evento
                {
                    Titulo = "Clásico: América vs Dep. Cali",
                    Ubicacion = "Estadio Pascual Guerrero",
                    Fecha = "Oct 30, 2026",
                    Precio = "45.000 COP",
                    Categoria = "Deportes",
                    Duracion = "2 horas",
                    Distancia = "5.2 km",
                    ImagenUrl = "https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=600&q=80"
                });

                // 3. PAGO - Música (Pendiente de pago)
                EventosGlobales.Add(new Evento
                {
                    Titulo = "Festival de Rock en Cali",
                    Ubicacion = "Estadio Pascual Guerrero",
                    Fecha = "Nov 15, 2026",
                    Precio = "120.000 COP",
                    Categoria = "Música",
                    Duracion = "Fin de semana",
                    Distancia = "5.2 km",
                    ImagenUrl = "https://images.unsplash.com/photo-1498038432885-c6f3f1b912ee?w=600&q=80",
                    PagoPendiente = true // Saldrá en la sección de pendientes
                });

                // 4. GRATIS - Educación
                EventosGlobales.Add(new Evento
                {
                    Titulo = "Taller de UI/UX",
                    Ubicacion = "Auditorio Central",
                    Fecha = "Dic 05, 2026",
                    Precio = "Gratis",
                    Categoria = "Educación",
                    Duracion = "1 a 3 horas",
                    Distancia = "8.1 km",
                    ImagenUrl = "https://images.unsplash.com/photo-1561070791-2526d30994b5?w=600&q=80"
                });

                // 5. PAGO - Cultura y Salsa
                EventosGlobales.Add(new Evento
                {
                    Titulo = "Espectáculo de Salsa",
                    Ubicacion = "Carpa Delirio",
                    Fecha = "Nov 20, 2026",
                    Precio = "180.000 COP",
                    Categoria = "Música",
                    Duracion = "4 horas",
                    Distancia = "15 km",
                    ImagenUrl = "https://images.unsplash.com/photo-1547153760-18fc86324498?w=600&q=80"
                });

                // 6. PAGO - Entretenimiento (Con otro pendiente)
                EventosGlobales.Add(new Evento
                {
                    Titulo = "Stand-up Comedy",
                    Ubicacion = "Arena Cañaveralejo",
                    Fecha = "Dic 10, 2026",
                    Precio = "60.000 COP",
                    Categoria = "Comedia",
                    Duracion = "2 horas",
                    Distancia = "10 km",
                    ImagenUrl = "https://images.unsplash.com/photo-1585699324551-f6c309eedeca?w=600&q=80",
                    PagoPendiente = true // Para que haya 2 tarjetas en "Pendientes"
                });
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}