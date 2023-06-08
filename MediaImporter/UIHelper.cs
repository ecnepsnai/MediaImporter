namespace MediaImporter
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using System;
    using System.Threading.Tasks;

    internal class UIHelper
    {
        private readonly UIElement element;

        public UIHelper(UIElement element)
        {
            this.element = element;
        }

        public async Task ShowMessageBox(string title, string message, string buttonLabel = "Dismiss")
        {
            ContentDialog dialog = new()
            {
                Title = title,
                Content = message,
                CloseButtonText = buttonLabel,
                XamlRoot = element.XamlRoot
            };
            await dialog.ShowAsync();
            return;
        }
    }
}
