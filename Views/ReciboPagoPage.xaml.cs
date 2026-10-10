using Microsoft.Maui.Controls;
using System;

namespace Eventia.Views
{
    [QueryProperty(nameof(EventoNombre), "EventoNombre")]
    [QueryProperty(nameof(Monto), "Monto")]
    [QueryProperty(nameof(MetodoPago), "MetodoPago")]
    [QueryProperty(nameof(EsPagoInmediato), "EsPagoInmediato")] // Recibimos la señal oculta
    public partial class ReciboPagoPage : ContentPage
    {
        private string _eventoNombre;
        public string EventoNombre
        {
            get => _eventoNombre;
            set { _eventoNombre = value; OnPropertyChanged(); }
        }

        private string _monto;
        public string Monto
        {
            get => _monto;
            set { _monto = value; OnPropertyChanged(); }
        }

        private string _metodoPago;
        public string MetodoPago
        {
            get => _metodoPago;
            set { _metodoPago = value; OnPropertyChanged(); }
        }

        // LÓGICA DINÁMICA DE ESTADO (Pagado vs Pendiente)
        private bool _esPagoInmediato = true;
        public bool EsPagoInmediato
        {
            get => _esPagoInmediato;
            set
            {
                _esPagoInmediato = value;
                OnPropertyChanged();
                // Le avisamos a la interfaz que actualice los colores y textos
                OnPropertyChanged(nameof(ColorTema));
                OnPropertyChanged(nameof(IconoTema));
                OnPropertyChanged(nameof(TituloTema));
                OnPropertyChanged(nameof(InstruccionQr));
            }
        }

        // Variables que cambian automáticamente según el estado
        public string ColorTema => EsPagoInmediato ? "#E10098" : "#FFC107"; // Magenta (Éxito) o Amarillo (Pendiente)
        public string IconoTema => EsPagoInmediato ? "✓" : "⏳";
        public string TituloTema => EsPagoInmediato ? "¡Movimiento exitoso!" : "¡Voucher Generado!";
        public string InstruccionQr => EsPagoInmediato ? "Escanea para verificar" : "Presenta este código al pagar";

        public string FechaActual { get; set; } = DateTime.Now.ToString("dd MMM yyyy, hh:mm tt");

        private string _referencia;
        public string Referencia
        {
            get
            {
                if (string.IsNullOrEmpty(_referencia))
                    _referencia = "M" + new Random().Next(100000, 999999).ToString();
                return _referencia;
            }
        }

        public string QrImageUrl => $"https://api.qrserver.com/v1/create-qr-code/?size=250x250&data=Eventia_Pago_{Referencia}";

        public ReciboPagoPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private async void OnListoClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//home");
        }
    }
}