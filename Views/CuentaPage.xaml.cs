using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Eventia.Models;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;

namespace Eventia.Views
{
    public class EventoHistorial
    {
        public string Titulo { get; set; }
        public string Fecha { get; set; }
        public string ImagenUrl { get; set; }
        public string Ubicacion { get; set; }
        public string Categoria { get; set; }
        public string Estado { get; set; }

        public Color ColorFondoEstado => Estado == "Pendiente" ? Color.FromArgb("#33FF9500") : Color.FromArgb("#3334C759");
        public Color ColorTextoEstado => Estado == "Pendiente" ? Color.FromArgb("#FF9500") : Color.FromArgb("#34C759");
    }

    public partial class CuentaPage : ContentPage
    {
        public ObservableCollection<EventoHistorial> MisEventos { get; set; }

        public int TotalPendientes { get; set; }
        public int TotalAsistidos { get; set; }
        public int TotalEventos { get; set; }

        public CuentaPage()
        {
            InitializeComponent();

            MisEventos = new ObservableCollection<EventoHistorial>
            {
                new EventoHistorial {
                    Titulo = "Superconcierto: Feid & Ryan Castro",
                    Fecha = "Nov 15, 2026",
                    Ubicacion = "Arena Cañaveralejo",
                    ImagenUrl = "https://images.unsplash.com/photo-1459749411175-04bf5292ceea?w=400&q=80",
                    Categoria = "Música",
                    Estado = "Pendiente"
                },
                new EventoHistorial {
                    Titulo = "Clásico: América vs Deportivo Cali",
                    Fecha = "Oct 24, 2026",
                    Ubicacion = "Estadio Pascual Guerrero",
                    ImagenUrl = "https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=400&q=80",
                    Categoria = "Deportes",
                    Estado = "Pendiente"
                },
                new EventoHistorial {
                    Titulo = "Feria de Desarrollo de Software",
                    Fecha = "Oct 10, 2026",
                    Ubicacion = "Universidad del Valle",
                    ImagenUrl = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=400&q=80",
                    Categoria = "Tech",
                    Estado = "Asistido"
                },
                new EventoHistorial {
                    Titulo = "Taller de UI/UX",
                    Fecha = "Sep 28, 2026",
                    Ubicacion = "Auditorio Central",
                    ImagenUrl = "https://images.unsplash.com/photo-1561070791-2526d30994b5?w=400&q=80",
                    Categoria = "Diseño",
                    Estado = "Asistido"
                }
            };

            TotalPendientes = MisEventos.Count(e => e.Estado == "Pendiente");
            TotalAsistidos = MisEventos.Count(e => e.Estado == "Asistido");
            TotalEventos = MisEventos.Count;

            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            this.Content.Opacity = 0;
            this.Content.TranslationY = 30;

            await Task.WhenAll(
                this.Content.FadeTo(1, 400, Easing.CubicOut),
                this.Content.TranslateTo(0, 0, 400, Easing.CubicOut)
            );
        }

        // --- NUEVO: Evento para la tuerquita de configuración ---
        private async void OnConfiguracionTapped(object sender, TappedEventArgs e)
        {
            if (sender is VisualElement elementoVisual)
            {
                await elementoVisual.ScaleTo(0.8, 100, Easing.CubicIn);
                await elementoVisual.ScaleTo(1.0, 150, Easing.SpringOut);
            }
            await Shell.Current.GoToAsync("configuracion");
        }

        private async void OnCambiarCuentaClicked(object sender, EventArgs e)
        {
            if (sender is VisualElement elementoVisual)
            {
                await elementoVisual.ScaleTo(0.95, 100, Easing.CubicIn);
                await elementoVisual.ScaleTo(1.0, 100, Easing.CubicOut);
            }

            bool confirmar = await DisplayAlert("Cerrar Sesión", "¿Deseas cerrar la sesión actual?", "Sí, salir", "Cancelar");
            if (confirmar) await Shell.Current.GoToAsync("//profile");
        }

        private async void OnCrearEventoClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//create");
        }

        private async void OnTabTapped(object sender, EventArgs e)
        {
            if (sender is Grid iconoSeleccionado && iconoSeleccionado.GestureRecognizers[0] is TapGestureRecognizer tap)
            {
                string destino = tap.CommandParameter.ToString();

                await iconoSeleccionado.ScaleTo(0.7, 100, Easing.CubicIn);
                await iconoSeleccionado.ScaleTo(1.0, 150, Easing.SpringOut);

                if (destino == "home" || destino == "favorites" || destino == "pending")
                {
                    Preferences.Default.Set("TabDeseado", destino);
                    await Shell.Current.GoToAsync("//home");
                }
            }
        }
    }
}