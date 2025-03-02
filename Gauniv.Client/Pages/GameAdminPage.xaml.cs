using Gauniv.Client.ViewModels;
using Microsoft.Maui.Controls;

namespace Gauniv.Client.Pages
{
    public partial class GameAdminPage : ContentPage
    {
        public GameAdminPage()
        {
            InitializeComponent();
            BindingContext = new GameAdminViewModel(); // 🔥 Lier la page au ViewModel
        }
    }
}
