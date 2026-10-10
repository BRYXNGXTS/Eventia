using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Threading.Tasks;

namespace Eventia.Views
{
    public partial class RegistroPage : ContentPage
    {
        public RegistroPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            this.Content.Opacity = 0;
            this.Content.TranslationY = 30;

            await Task.WhenAll(
                this.Content.FadeTo(1, 500, Easing.CubicOut),
                this.Content.TranslateTo(0, 0, 500, Easing.CubicOut)
            );
        }

        private async void OnRegistrarseClicked(object sender, EventArgs e)
        {
            await BtnRegistrar.ScaleTo(0.90, 100, Easing.CubicIn);
            await BtnRegistrar.ScaleTo(1.0, 150, Easing.SpringOut);

            string usuario = UsuarioEntry.Text?.Trim();
            string password = PasswordEntry.Text?.Trim();
            string nombre = NombreEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(nombre))
            {
                await DisplayAlert("Datos incompletos", "Por favor llena todos los campos.", "OK");
                return;
            }

            Preferences.Default.Set("UsuarioSistema", usuario);
            Preferences.Default.Set("PasswordSistema", password);

            Preferences.Default.Remove("UsuarioGuardado");
            SecureStorage.Default.Remove("PasswordGuardada");

            await DisplayAlert("¡Bienvenido!", $"Tu cuenta '{usuario}' ha sido creada con éxito. Ahora puedes iniciar sesión.", "Ir al Login");

            // CORRECCIÓN: Usamos la ruta absoluta del login en lugar de intentar "volver atrás"
            await Shell.Current.GoToAsync("//profile");
        }

        private async void OnVolverLoginTapped(object sender, EventArgs e)
        {
            // CORRECCIÓN: Saltamos directo al login
            await Shell.Current.GoToAsync("//profile");
        }
    }
}