using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace Eventia.Models
{
    public class Asistente
    {
        public string Nombre { get; set; }
        public string AvatarUrl { get; set; }
    }

    public class Evento : INotifyPropertyChanged
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Titulo { get; set; }
        public string Fecha { get; set; }
        public string Ubicacion { get; set; }
        public string Categoria { get; set; }
        public string Precio { get; set; }
        public string Duracion { get; set; }
        public string Distancia { get; set; }

        public ObservableCollection<Asistente> Asistentes { get; set; } = new ObservableCollection<Asistente>();

        private string _imagenUrl;
        public string ImagenUrl { get => _imagenUrl; set { if (_imagenUrl != value) { _imagenUrl = value; OnPropertyChanged(); } } }

        private bool _estaInscrito;
        public bool EstaInscrito { get => _estaInscrito; set { if (_estaInscrito != value) { _estaInscrito = value; OnPropertyChanged(); } } }

        private bool _pagoPendiente;
        public bool PagoPendiente { get => _pagoPendiente; set { if (_pagoPendiente != value) { _pagoPendiente = value; OnPropertyChanged(); } } }

        private bool _esFavorito;
        public bool EsFavorito { get => _esFavorito; set { if (_esFavorito != value) { _esFavorito = value; OnPropertyChanged(); } } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}