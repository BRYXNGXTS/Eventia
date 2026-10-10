using Microsoft.Maui.Controls;
using Eventia.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Graphics;

namespace Eventia.Views
{
    [QueryProperty(nameof(EventoSeleccionado), "EventoInfo")]
    public partial class DetalleEventoPage : ContentPage
    {
        private Evento _eventoSeleccionado;
        public Evento EventoSeleccionado
        {
            get => _eventoSeleccionado;
            set
            {
                _eventoSeleccionado = value;
                OnPropertyChanged();
                BindingContext = _eventoSeleccionado;
            }
        }

        private string _nombreMetodoElegido;

        public DetalleEventoPage()
        {
            InitializeComponent();
        }

        private async void OnVolverClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private void OnRegistrarseClicked(object sender, EventArgs e)
        {
            MenuPagosOverlay.IsVisible = true;
            OnVolverAListaTapped(null, null);
        }

        private void OnCerrarMenuPagosTapped(object sender, EventArgs e)
        {
            MenuPagosOverlay.IsVisible = false;
        }

        private void OnVolverAListaTapped(object sender, EventArgs e)
        {
            VistaListaMetodos.IsVisible = true;
            VistaFormularioTarjeta.IsVisible = false;
            VistaFormularioNequi.IsVisible = false;
            VistaFormularioEfectivo.IsVisible = false;

            if (NequiErrorLabel != null)
            {
                NequiErrorLabel.IsVisible = false;
                NequiPhoneBorder.Stroke = Color.FromArgb("#3D3D4A");
                NequiPhoneEntry.Text = string.Empty;
            }
        }

        private void OnMetodoPagoSeleccionado(object sender, TappedEventArgs e)
        {
            string parametro = e.Parameter?.ToString();
            VistaListaMetodos.IsVisible = false;

            if (parametro == "Tarjeta")
            {
                _nombreMetodoElegido = "Tarjeta de Crédito / Débito";
                VistaFormularioTarjeta.IsVisible = true;
            }
            else if (parametro == "Nequi")
            {
                _nombreMetodoElegido = "Nequi / DaviPlata";
                VistaFormularioNequi.IsVisible = true;
            }
            else if (parametro == "Transferencia")
            {
                _nombreMetodoElegido = "Transferencia Bancaria";
                VistaFormularioEfectivo.IsVisible = true;
            }
            else if (parametro == "Efectivo")
            {
                _nombreMetodoElegido = "Pago en Efectivo";
                VistaFormularioEfectivo.IsVisible = true;
            }
        }

        private void OnNequiPhoneTextChanged(object sender, TextChangedEventArgs e)
        {
            NequiErrorLabel.IsVisible = false;
            NequiPhoneBorder.Stroke = Color.FromArgb("#3D3D4A");
        }

        private async void OnConfirmarPagoClicked(object sender, EventArgs e)
        {
            if (_nombreMetodoElegido == "Nequi / DaviPlata")
            {
                string numeroSeguro = NequiPhoneEntry.Text ?? "";
                numeroSeguro = numeroSeguro.Replace(" ", "").Trim();

                if (numeroSeguro.Length < 10)
                {
                    NequiErrorLabel.IsVisible = true;
                    NequiPhoneBorder.Stroke = Colors.Red;
                    return;
                }
            }

            MenuPagosOverlay.IsVisible = false;

            bool esPagoInmediato = _nombreMetodoElegido == "Tarjeta de Crédito / Débito" || _nombreMetodoElegido == "Nequi / DaviPlata";

            if (esPagoInmediato)
            {
                EventoSeleccionado.Asistentes.Add(new Asistente
                {
                    Nombre = "sofia09",
                    AvatarUrl = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=400&q=80"
                });
                EventoSeleccionado.EstaInscrito = true;
            }
            else
            {
                // ¡NUEVA LÓGICA! Si es transferencia o efectivo, lo marcamos como PENDIENTE
                EventoSeleccionado.PagoPendiente = true;
            }

            var parametros = new Dictionary<string, object>
            {
                { "EventoNombre", EventoSeleccionado.Titulo },
                { "Monto", EventoSeleccionado.Precio },
                { "MetodoPago", _nombreMetodoElegido },
                { "EsPagoInmediato", esPagoInmediato }
            };

            await Shell.Current.GoToAsync("recibo", parametros);
        }
    }
}