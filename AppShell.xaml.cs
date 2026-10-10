using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Eventia.Views;
using Font = Microsoft.Maui.Font;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Maui.Controls;
using System;

namespace Eventia
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registramos las rutas dinámicas
            Routing.RegisterRoute("detalle", typeof(DetalleEventoPage));
            Routing.RegisterRoute("recibo", typeof(ReciboPagoPage));

            // NUEVO: Ruta para la página de configuración
            Routing.RegisterRoute("configuracion", typeof(ConfiguracionPage));
        }

        public static async Task DisplaySnackbarAsync(string message)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Color.FromArgb("#FF3300"),
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.Yellow,
                CornerRadius = new CornerRadius(0),
                Font = Font.SystemFontOfSize(18),
                ActionButtonFont = Font.SystemFontOfSize(14)
            };
            var snackbar = Snackbar.Make(message, visualOptions: snackbarOptions);
            await snackbar.Show(cancellationTokenSource.Token);
        }

        public static async Task DisplayToastAsync(string message)
        {
            if (OperatingSystem.IsWindows()) return;
            var toast = Toast.Make(message, textSize: 18);
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await toast.Show(cts.Token);
        }
    }
}