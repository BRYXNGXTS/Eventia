using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using Eventia.ViewModels;
using Eventia.Models;

namespace Eventia.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            BindingContext = new EventosViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            string tabGuardado = Preferences.Default.Get("TabDeseado", "home");

            if (BindingContext is EventosViewModel viewModel)
            {
                if (tabGuardado == "home") { TituloSeccion.Text = "EVENTOS POPULARES"; viewModel.RecargarEventosCommand.Execute(null); }
                else if (tabGuardado == "favorites") { TituloSeccion.Text = "MIS FAVORITOS"; viewModel.FiltrarCommand.Execute("Favoritos"); }
                else if (tabGuardado == "pending") { TituloSeccion.Text = "PAGOS PENDIENTES"; viewModel.FiltrarCommand.Execute("Pendientes"); }
            }

            ActualizarEstadoBarra(tabGuardado);
            Preferences.Default.Set("TabDeseado", "home");

            ListaEventos.Opacity = 0;
            ListaEventos.TranslationY = 20;
            TituloSeccion.Opacity = 0;

            await Task.WhenAll(
                ListaEventos.FadeTo(1, 400, Easing.CubicOut),
                ListaEventos.TranslateTo(0, 0, 400, Easing.CubicOut),
                TituloSeccion.FadeTo(1, 400, Easing.CubicOut)
            );
        }

        // --- MAGIA VISUAL PARA LAS PÍLDORAS SUPERIORES ---
        private void OnFiltroClicked(object sender, EventArgs e)
        {
            Color bgInactivo = Color.FromArgb("#2A2A40");
            Color textInactivo = Color.FromArgb("#A0A0AA");
            Color bgActivo = Color.FromArgb("#5E5CE6");
            Color textActivo = Colors.White;

            // Apagamos todas primero
            BtnFiltroTodos.BackgroundColor = bgInactivo; BtnFiltroTodos.TextColor = textInactivo;
            BtnFiltroMusica.BackgroundColor = bgInactivo; BtnFiltroMusica.TextColor = textInactivo;
            BtnFiltroGratis.BackgroundColor = bgInactivo; BtnFiltroGratis.TextColor = textInactivo;
            BtnFiltroCercanos.BackgroundColor = bgInactivo; BtnFiltroCercanos.TextColor = textInactivo;
            BtnFiltroCortos.BackgroundColor = bgInactivo; BtnFiltroCortos.TextColor = textInactivo;

            // Encendemos solo el que el usuario tocó
            if (sender is Button botonTocado)
            {
                botonTocado.BackgroundColor = bgActivo;
                botonTocado.TextColor = textActivo;

                // Actualizamos el título según la píldora
                TituloSeccion.Text = $"FILTRO: {botonTocado.Text.ToUpper()}";
            }
        }

        private void RestaurarFiltrosPildoras()
        {
            if (BtnFiltroTodos == null) return;

            Color bgInactivo = Color.FromArgb("#2A2A40");
            Color textInactivo = Color.FromArgb("#A0A0AA");
            Color bgActivo = Color.FromArgb("#5E5CE6");
            Color textActivo = Colors.White;

            // Restaura visualmente a "Todos"
            BtnFiltroTodos.BackgroundColor = bgActivo; BtnFiltroTodos.TextColor = textActivo;
            BtnFiltroMusica.BackgroundColor = bgInactivo; BtnFiltroMusica.TextColor = textInactivo;
            BtnFiltroGratis.BackgroundColor = bgInactivo; BtnFiltroGratis.TextColor = textInactivo;
            BtnFiltroCercanos.BackgroundColor = bgInactivo; BtnFiltroCercanos.TextColor = textInactivo;
            BtnFiltroCortos.BackgroundColor = bgInactivo; BtnFiltroCortos.TextColor = textInactivo;
        }

        private async void OnEventoTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is Evento eventoSeleccionado)
            {
                var parametros = new Dictionary<string, object> { { "EventoInfo", eventoSeleccionado } };
                await Shell.Current.GoToAsync("detalle", parametros);
            }
        }

        private async void OnCambiarImagenTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is Evento eventoClickeado)
            {
                eventoClickeado.EsFavorito = !eventoClickeado.EsFavorito;

                if (sender is VisualElement estrellita)
                {
                    await estrellita.ScaleTo(1.4, 100, Easing.CubicIn);
                    await estrellita.ScaleTo(1.0, 150, Easing.SpringOut);
                }
            }
        }

        private async void OnCrearEventoClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//create");
        }

        private async void OnTabTapped(object sender, TappedEventArgs e)
        {
            var tabSeleccionado = e.Parameter?.ToString();

            if (sender is VisualElement elementoVisual)
            {
                await elementoVisual.ScaleTo(0.7, 100, Easing.CubicIn);
                await elementoVisual.ScaleTo(1.0, 150, Easing.SpringOut);
            }

            if (tabSeleccionado == "account")
            {
                await Shell.Current.GoToAsync("//account");
                return;
            }

            ActualizarEstadoBarra(tabSeleccionado);
            RestaurarFiltrosPildoras(); // Si usa la barra de abajo, reiniciamos las píldoras a "Todos"

            await Task.WhenAll(
                ListaEventos.FadeTo(0, 150, Easing.CubicIn),
                ListaEventos.TranslateTo(0, 15, 150, Easing.CubicIn),
                TituloSeccion.FadeTo(0, 150, Easing.CubicIn)
            );

            if (BindingContext is EventosViewModel viewModel)
            {
                if (tabSeleccionado == "home") { TituloSeccion.Text = "EVENTOS POPULARES"; viewModel.RecargarEventosCommand.Execute(null); }
                else if (tabSeleccionado == "pending") { TituloSeccion.Text = "PAGOS PENDIENTES"; viewModel.FiltrarCommand.Execute("Pendientes"); }
                else if (tabSeleccionado == "favorites") { TituloSeccion.Text = "MIS FAVORITOS"; viewModel.FiltrarCommand.Execute("Favoritos"); }
            }

            await Task.WhenAll(
                ListaEventos.FadeTo(1, 250, Easing.CubicOut),
                ListaEventos.TranslateTo(0, 0, 250, Easing.CubicOut),
                TituloSeccion.FadeTo(1, 250, Easing.CubicOut)
            );
        }

        private void ActualizarEstadoBarra(string tabActivo)
        {
            Color colorMorado = Color.FromArgb("#5E5CE6");
            Color colorGris = Color.FromArgb("#707080");

            IconoHome.Fill = colorGris; IconoHome.WidthRequest = 22; IconoHome.HeightRequest = 22; PuntoHome.IsVisible = false;
            IconoFav.Fill = colorGris; IconoFav.WidthRequest = 22; IconoFav.HeightRequest = 22; PuntoFav.IsVisible = false;
            IconoPend.Fill = colorGris; IconoPend.WidthRequest = 22; IconoPend.HeightRequest = 22; PuntoPend.IsVisible = false;

            if (tabActivo == "home") { IconoHome.Fill = colorMorado; IconoHome.WidthRequest = 24; IconoHome.HeightRequest = 24; PuntoHome.IsVisible = true; }
            else if (tabActivo == "favorites") { IconoFav.Fill = colorMorado; IconoFav.WidthRequest = 24; IconoFav.HeightRequest = 24; PuntoFav.IsVisible = true; }
            else if (tabActivo == "pending") { IconoPend.Fill = colorMorado; IconoPend.WidthRequest = 24; IconoPend.HeightRequest = 24; PuntoPend.IsVisible = true; }
        }
    }
}