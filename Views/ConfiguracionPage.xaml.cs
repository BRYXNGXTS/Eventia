using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace Eventia.Views
{
    public partial class ConfiguracionPage : ContentPage
    {
        public ConfiguracionPage()
        {
            InitializeComponent();
        }

        private async void OnVolverTapped(object sender, TappedEventArgs e)
        {
            if (sender is VisualElement elemento)
            {
                await elemento.ScaleTo(0.9, 100);
                await elemento.ScaleTo(1.0, 100);
            }
            await Shell.Current.GoToAsync("..");
        }

        private async void OnAccionTapped(object sender, EventArgs e)
        {
            if (sender is VisualElement elemento)
            {
                await elemento.ScaleTo(0.95, 100);
                await elemento.ScaleTo(1.0, 100);
            }
            await DisplayAlert("Próximamente", "Esta funcionalidad estará disponible en la versión final.", "OK");
        }
    }
}