using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Eventia.Views
{
    public partial class ProfilePage : ContentPage
    {
        // 1. Quitamos el valor fijo para que acepte a cualquier usuario nuevo
        private string _adminUsuario;
        private string _adminPassword;
        private List<string> _codigosRecuperacion;

        private bool _animandoLogo = false;

        private string _usuarioGuardado = string.Empty;
        private string _passwordGuardada = string.Empty;

        public ProfilePage()
        {
            InitializeComponent();

            string codigosGuardados = Preferences.Default.Get("CodigosRecuperacion", "748291,394850,102938,564738,938475");
            _codigosRecuperacion = new List<string>(codigosGuardados.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // 2. Cargamos el usuario y contraseña del sistema CADA VEZ que la pantalla aparece.
            // Así, si vienes de registrarte, el login ya sabe quién eres.
            _adminUsuario = Preferences.Default.Get("UsuarioSistema", "sofia09");
            _adminPassword = Preferences.Default.Get("PasswordSistema", "SofiMaturana20");

            // Precargamos los datos en la memoria rápida para la ventanita de autocompletado
            _usuarioGuardado = Preferences.Default.Get("UsuarioGuardado", string.Empty);
            _passwordGuardada = await SecureStorage.Default.GetAsync("PasswordGuardada");

            LabelEmailSugerido.Text = string.IsNullOrEmpty(_usuarioGuardado) ? "" : _usuarioGuardado;
            PopupAutocompletar.IsVisible = false;

            _animandoLogo = true;
            AnimarLogoContinuamente();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _animandoLogo = false;
        }

        private async void AnimarLogoContinuamente()
        {
            while (_animandoLogo)
            {
                await ContenedorLogo.TranslateTo(0, -12, 1500, Easing.SinInOut);
                await ContenedorLogo.TranslateTo(0, 0, 1500, Easing.SinInOut);
            }
        }

        private void OnEmailTextChanged(object sender, TextChangedEventArgs e)
        {
            string textoEscrito = e.NewTextValue?.ToLower() ?? "";

            if (!string.IsNullOrEmpty(_usuarioGuardado) &&
                !string.IsNullOrEmpty(_passwordGuardada) &&
                !string.IsNullOrEmpty(textoEscrito) &&
                _usuarioGuardado.ToLower().StartsWith(textoEscrito) &&
                textoEscrito != _usuarioGuardado.ToLower())
            {
                PopupAutocompletar.IsVisible = true;
            }
            else
            {
                PopupAutocompletar.IsVisible = false;
            }
        }

        private async void OnSugerenciaTapped(object sender, TappedEventArgs e)
        {
            await PopupAutocompletar.ScaleTo(0.95, 50, Easing.CubicIn);
            await PopupAutocompletar.ScaleTo(1.0, 50, Easing.SpringOut);

            EmailEntry.Text = _usuarioGuardado;
            PasswordEntry.Text = _passwordGuardada;

            PopupAutocompletar.IsVisible = false;
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await BtnIngresar.ScaleTo(0.90, 100, Easing.CubicIn);
            await BtnIngresar.ScaleTo(1.0, 150, Easing.SpringOut);

            string usuario = EmailEntry.Text?.Trim();
            string password = PasswordEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Por favor ingresa tu usuario y contraseña.", "OK");
                return;
            }

            if (usuario.ToLower() == _adminUsuario.ToLower() && password == _adminPassword)
            {
                string usuarioGuardadoActual = Preferences.Default.Get("UsuarioGuardado", string.Empty);
                if (usuarioGuardadoActual.ToLower() != usuario.ToLower())
                {
                    bool guardar = await DisplayAlert("Guardar sesión", "¿Deseas guardar tu inicio de sesión para autocompletarlo la próxima vez?", "Sí, guardar", "No, gracias");
                    if (guardar)
                    {
                        Preferences.Default.Set("UsuarioGuardado", usuario);
                        await SecureStorage.Default.SetAsync("PasswordGuardada", password);

                        _usuarioGuardado = usuario;
                        _passwordGuardada = password;
                        LabelEmailSugerido.Text = usuario;
                    }
                }

                EmailEntry.Text = string.Empty;
                PasswordEntry.Text = string.Empty;

                await Shell.Current.GoToAsync("//home");
            }
            else
            {
                await DisplayAlert("Acceso denegado", "El usuario o la contraseña son incorrectos.", "Intentar de nuevo");
            }
        }

        private async void OnOlvidoPasswordTapped(object sender, EventArgs e)
        {
            string codigoIngresado = await DisplayPromptAsync(
                "Recuperar Contraseña",
                "Ingresa uno de los códigos numéricos de 6 dígitos que te proporcionamos al crear tu cuenta:",
                "Verificar",
                "Cancelar",
                "Ej. 123456",
                maxLength: 6,
                keyboard: Keyboard.Numeric);

            if (string.IsNullOrWhiteSpace(codigoIngresado))
                return;

            if (_codigosRecuperacion.Contains(codigoIngresado.Trim()))
            {
                string nuevaContraseña = await DisplayPromptAsync("Nueva Contraseña",
                                                                  "Código aceptado. Ingresa tu nueva contraseña:",
                                                                  "Guardar",
                                                                  "Cancelar",
                                                                  "Nueva contraseña");

                if (!string.IsNullOrWhiteSpace(nuevaContraseña))
                {
                    _adminPassword = nuevaContraseña.Trim();
                    Preferences.Default.Set("PasswordSistema", _adminPassword);

                    _codigosRecuperacion.Remove(codigoIngresado.Trim());
                    Preferences.Default.Set("CodigosRecuperacion", string.Join(",", _codigosRecuperacion));

                    Preferences.Default.Remove("UsuarioGuardado");
                    SecureStorage.Default.Remove("PasswordGuardada");

                    _usuarioGuardado = "";
                    _passwordGuardada = "";
                    PopupAutocompletar.IsVisible = false;

                    await DisplayAlert("Éxito", $"Contraseña actualizada. Te quedan {_codigosRecuperacion.Count} códigos de recuperación disponibles.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "El código ingresado es incorrecto o ya fue utilizado.", "OK");
            }
        }

        private async void OnRegistroTapped(object sender, EventArgs e)
        {
            // AQUÍ ESTÁ EL CAMBIO: Las dos barras "//" aseguran un viaje perfecto a la nueva pantalla
            await Shell.Current.GoToAsync("//registro");
        }
    }
}