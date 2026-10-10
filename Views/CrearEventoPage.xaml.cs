using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Devices.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eventia.Views
{
    public partial class CrearEventoPage : ContentPage
    {
        // Variable para atrapar el resultado del GPS y pasarlo a la tarjeta principal
        private string _distanciaCalculada = "Por definir";

        private List<string> _todosLosBarrios = new List<string>
        {
            "Marroquín", "Marruecos", "San Fernando", "Granada", "El Caney",
            "Valle del Lili", "Meléndez", "San Antonio", "Siloe", "Ciudad Jardín",
            "El Peñón", "San Judas", "La Flora", "Mengá", "Salomia", "Floralia"
        };

        public CrearEventoPage()
        {
            InitializeComponent();
        }

        private async void OnVolverClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//home");
        }

        private async void OnPublicarClicked(object sender, EventArgs e)
        {
            string rolUsuario = Preferences.Get("UsuarioActual_Rol", "Usuario");

            int capacidadEvento = 0;
            if (CapacidadPicker.SelectedItem != null)
            {
                int.TryParse(CapacidadPicker.SelectedItem.ToString(), out capacidadEvento);
            }
            else
            {
                await DisplayAlert("Dato faltante", "Por favor, selecciona el número de asistentes.", "OK");
                return;
            }

            if (rolUsuario == "Usuario" && capacidadEvento > 40)
            {
                await DisplayAlert("Límite excedido", "Las cuentas estándar solo pueden crear eventos de máximo 40 personas.", "Entendido");
                return;
            }

            string urlImagen = "https://images.unsplash.com/photo-1492684223066-81342ee5ff30?w=600&q=80";
            if (ContenedorImagenLugar.IsVisible)
            {
                string lugar = DireccionEntry.Text?.Trim().ToLower() ?? "";
                if (lugar.Contains("pascual") || lugar.Contains("estadio")) urlImagen = "https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=600&q=80";
                else if (lugar.Contains("universidad") || lugar.Contains("univalle")) urlImagen = "https://images.unsplash.com/photo-1562774053-701939374585?w=600&q=80";
                else urlImagen = "https://images.unsplash.com/photo-1449844908441-8829872d2607?w=600&q=80";
            }

            string ciudad = CiudadPicker.SelectedItem?.ToString() ?? "Cali";
            string barrio = BarrioEntry.Text?.Trim() ?? "";
            string nombreEvento = string.IsNullOrWhiteSpace(NombreEntry.Text) ? "Nuevo Evento" : NombreEntry.Text;

            // CORRECCIÓN LÍNEA 67: Usamos interpolación en lugar de ToString("formato")
            string fechaElegida = $"{FechaPicker.Date:MMM dd, yyyy}";

            string duracionElegida = DuracionPicker.SelectedItem?.ToString() ?? "No especificada";
            string categoriaElegida = CategoriaPicker.SelectedItem?.ToString() ?? "General";

            var eventoCreado = new Eventia.Models.Evento
            {
                Titulo = nombreEvento,
                Ubicacion = string.IsNullOrWhiteSpace(barrio) ? ciudad : $"{barrio}, {ciudad}",
                Fecha = fechaElegida,
                Categoria = categoriaElegida,
                Duracion = duracionElegida,
                Distancia = _distanciaCalculada, // Aquí pasamos lo que atrapó el GPS
                Precio = "Gratis",
                ImagenUrl = urlImagen
            };

            App.EventosGlobales.Add(eventoCreado);

            await DisplayAlert("Éxito", "El evento se ha publicado correctamente en la plataforma.", "OK");
            await Shell.Current.GoToAsync("//home");
        }

        private void OnBarrioTextChanged(object sender, TextChangedEventArgs e)
        {
            string busqueda = e.NewTextValue?.ToLower() ?? "";

            if (string.IsNullOrWhiteSpace(busqueda))
            {
                ContenedorSugerencias.IsVisible = false;
                return;
            }

            var coincidencias = _todosLosBarrios.Where(b => b.ToLower().Contains(busqueda)).ToList();

            if (coincidencias.Any())
            {
                ListaSugerencias.ItemsSource = coincidencias;
                ContenedorSugerencias.IsVisible = true;
            }
            else
            {
                ContenedorSugerencias.IsVisible = false;
            }
        }

        private void OnBarrioSeleccionado(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is string barrioSeleccionado)
            {
                BarrioEntry.TextChanged -= OnBarrioTextChanged;
                BarrioEntry.Text = barrioSeleccionado;

                ContenedorSugerencias.IsVisible = false;
                ListaSugerencias.SelectedItem = null;

                BarrioEntry.TextChanged += OnBarrioTextChanged;
            }
        }

        private async void OnLugarUnfocused(object sender, FocusEventArgs e)
        {
            string lugar = DireccionEntry.Text?.Trim().ToLower();
            string ciudad = CiudadPicker.SelectedItem?.ToString() ?? "";

            if (!string.IsNullOrWhiteSpace(lugar))
            {
                ContenedorImagenLugar.IsVisible = true;
                if (lugar.Contains("pascual") || lugar.Contains("estadio"))
                    ImagenLugar.Source = "https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=600&q=80";
                else if (lugar.Contains("universidad") || lugar.Contains("univalle"))
                    ImagenLugar.Source = "https://images.unsplash.com/photo-1562774053-701939374585?w=600&q=80";
                else
                    ImagenLugar.Source = "https://images.unsplash.com/photo-1449844908441-8829872d2607?w=600&q=80";

                try
                {
                    LabelDistancia.IsVisible = false;
                    CargandoDistancia.IsVisible = true;
                    CargandoDistancia.IsRunning = true;

                    var locationUsuario = await Geolocation.Default.GetLastKnownLocationAsync() ??
                                          await Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(5)));

                    if (locationUsuario != null)
                    {
                        string busquedaGPS = $"{lugar}, {ciudad}, Colombia";
                        var locationsLugar = await Geocoding.Default.GetLocationsAsync(busquedaGPS);
                        var locationDestino = locationsLugar?.FirstOrDefault();

                        if (locationDestino != null)
                        {
                            double distanciaKm = Location.CalculateDistance(locationUsuario, locationDestino, DistanceUnits.Kilometers);

                            // Guardamos la distancia exacta para la tarjeta
                            _distanciaCalculada = $"{distanciaKm:F1} km";
                            LabelDistancia.Text = $"El evento será a {_distanciaCalculada} de ti";
                        }
                        else
                        {
                            _distanciaCalculada = "Distancia desconocida";
                            LabelDistancia.Text = "Ubicación no encontrada en el mapa.";
                        }
                    }
                    else
                    {
                        _distanciaCalculada = "Sin acceso a GPS";
                        LabelDistancia.Text = "No pudimos acceder a tu GPS.";
                    }
                }
                catch (Exception)
                {
                    _distanciaCalculada = "GPS Inactivo";
                    LabelDistancia.Text = "Activa tu GPS para calcular la distancia.";
                }
                finally
                {
                    CargandoDistancia.IsRunning = false;
                    CargandoDistancia.IsVisible = false;
                    LabelDistancia.IsVisible = true;
                }
            }
            else
            {
                ContenedorImagenLugar.IsVisible = false;
                LabelDistancia.Text = "Escribe la dirección para calcular...";
            }
        }
    }
}