using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Globalization;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Eventia.Models;

namespace Eventia.ViewModels
{
    public class EventosViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Evento> EventosPopulares { get; set; }

        private string _textoBusqueda;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                if (_textoBusqueda != value)
                {
                    _textoBusqueda = value;
                    OnPropertyChanged();
                    EjecutarBusqueda();
                }
            }
        }

        public ICommand FiltrarCommand { get; }
        public ICommand RecargarEventosCommand { get; }

        public EventosViewModel()
        {
            EventosPopulares = new ObservableCollection<Evento>();

            FiltrarCommand = new Command<string>(EjecutarFiltro);
            RecargarEventosCommand = new Command(CargarEventosDesdeGlobal);

            CargarEventosDesdeGlobal();
        }

        private void CargarEventosDesdeGlobal()
        {
            if (App.EventosGlobales == null || App.EventosGlobales.Count == 0)
            {
                CrearDatosDePruebaIniciales();
            }

            EventosPopulares.Clear();

            foreach (var evt in App.EventosGlobales)
            {
                EventosPopulares.Add(evt);
            }
        }

        private void CrearDatosDePruebaIniciales()
        {
            App.EventosGlobales = new ObservableCollection<Evento>();

            var asistentesEjemplo = new ObservableCollection<Asistente>
            {
                new Asistente { Nombre = "Juan", AvatarUrl = "https://images.unsplash.com/photo-1599566150163-29194dcaad36?w=100&q=80" },
                new Asistente { Nombre = "María", AvatarUrl = "https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=100&q=80" }
            };

            App.EventosGlobales.Add(new Evento { Titulo = "Superconcierto: Feid & Ryan Castro", Fecha = "Nov 15, 2026", Ubicacion = "Arena Cañaveralejo", Duracion = "5 horas", Distancia = "a 12 km de ti", Categoria = "Reggaetón", Precio = "150.000 COP", ImagenUrl = "https://images.unsplash.com/photo-1459749411175-04bf5292ceea?w=800&q=80", Asistentes = asistentesEjemplo, PagoPendiente = true });
            App.EventosGlobales.Add(new Evento { Titulo = "Clásico: América vs Deportivo Cali", Fecha = "Oct 24, 2026", Ubicacion = "Estadio Pascual Guerrero", Duracion = "2 horas", Distancia = "a 5.2 km de ti", Categoria = "Deportes", Precio = "45.000 COP", ImagenUrl = "https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=800&q=80", Asistentes = asistentesEjemplo });
            App.EventosGlobales.Add(new Evento { Titulo = "Festival Rock al Parque", Fecha = "Ene 20, 2027", Ubicacion = "Parque de la Música", Duracion = "Todo el fin de semana", Distancia = "a 18 km de ti", Categoria = "Rock", Precio = "120.000 COP", ImagenUrl = "https://images.unsplash.com/photo-1498038432885-c6f3f1b912ee?w=800&q=80", Asistentes = asistentesEjemplo });
            App.EventosGlobales.Add(new Evento { Titulo = "Masterclass: Diseño UI/UX", Fecha = "Dic 12, 2026", Ubicacion = "Auditorio Central", Duracion = "3 horas", Distancia = "a 2 km de ti", Categoria = "Diseño", Precio = "80.000 COP", ImagenUrl = "https://images.unsplash.com/photo-1561070791-2526d30994b5?w=800&q=80", Asistentes = asistentesEjemplo });
            App.EventosGlobales.Add(new Evento { Titulo = "Feria de Startups Tecnológicas", Fecha = "Dic 05, 2026", Ubicacion = "Centro de Eventos", Duracion = "Todo el día", Distancia = "a 25 km de ti", Categoria = "Tecnología", Precio = "Gratis", ImagenUrl = "https://images.unsplash.com/photo-1555066931-4365d14bab8c?w=800&q=80", Asistentes = asistentesEjemplo });
        }

        private void EjecutarBusqueda()
        {
            if (string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                CargarEventosDesdeGlobal();
                return;
            }

            var textoLimpio = QuitarAcentos(TextoBusqueda);
            var terminosAFiltrar = new List<string> { textoLimpio };

            if (textoLimpio.Contains("musica")) { terminosAFiltrar.Add("rock"); terminosAFiltrar.Add("reggaeton"); terminosAFiltrar.Add("concierto"); }
            if (textoLimpio.Contains("clase") || textoLimpio.Contains("curso") || textoLimpio.Contains("aprender")) { terminosAFiltrar.Add("masterclass"); terminosAFiltrar.Add("taller"); }
            if (textoLimpio.Contains("tecnologia") || textoLimpio.Contains("tech") || textoLimpio.Contains("sistemas")) { terminosAFiltrar.Add("ui/ux"); terminosAFiltrar.Add("startup"); terminosAFiltrar.Add("diseño"); }
            if (textoLimpio.Contains("deporte") || textoLimpio.Contains("futbol")) { terminosAFiltrar.Add("clasico"); terminosAFiltrar.Add("estadio"); }

            var filtrados = App.EventosGlobales.Where(e =>
            {
                var titulo = QuitarAcentos(e.Titulo);
                var ubicacion = QuitarAcentos(e.Ubicacion);
                var categoria = QuitarAcentos(e.Categoria);

                return terminosAFiltrar.Any(termino =>
                    titulo.Contains(termino) ||
                    ubicacion.Contains(termino) ||
                    categoria.Contains(termino));
            }).ToList();

            RestaurarLista(filtrados);
        }

        private string QuitarAcentos(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;

            var textoNormalizado = texto.Normalize(NormalizationForm.FormD);
            var constructor = new StringBuilder();

            foreach (var caracter in textoNormalizado)
            {
                var categoriaUnicode = CharUnicodeInfo.GetUnicodeCategory(caracter);
                if (categoriaUnicode != UnicodeCategory.NonSpacingMark)
                {
                    constructor.Append(caracter);
                }
            }
            return constructor.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
        }

        private void EjecutarFiltro(string filtro)
        {
            TextoBusqueda = string.Empty;

            if (filtro == "Todos")
                CargarEventosDesdeGlobal();
            else if (filtro == "Música")
                RestaurarLista(App.EventosGlobales.Where(e => e.Categoria == "Rock" || e.Categoria == "Reggaetón" || e.Categoria == "Música").ToList());
            else if (filtro == "Gratis")
                RestaurarLista(App.EventosGlobales.Where(e => e.Precio != null && e.Precio.ToLower().Contains("gratis")).ToList());
            else if (filtro == "Cercanos")
                RestaurarLista(App.EventosGlobales.Where(e => e.Distancia != null && (e.Distancia.Contains("2 km") || e.Distancia.Contains("5.2 km"))).ToList());
            else if (filtro == "Cortos")
                RestaurarLista(App.EventosGlobales.Where(e => e.Duracion != null && (e.Duracion.Contains("2") || e.Duracion.Contains("3"))).ToList());
            else if (filtro == "Pendientes")
                RestaurarLista(App.EventosGlobales.Where(e => e.PagoPendiente).ToList());
            else if (filtro == "Favoritos")
                RestaurarLista(App.EventosGlobales.Where(e => e.EsFavorito).ToList());
        }

        private void RestaurarLista(List<Evento> listaFiltrada)
        {
            EventosPopulares.Clear();
            foreach (var evt in listaFiltrada)
            {
                EventosPopulares.Add(evt);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}