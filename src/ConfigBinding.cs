using System.Windows.Data;

namespace AutoBrowserChooser
{
    public class ConfigBinding : Binding
    {
        public ConfigBinding()
        {
            Initialize();
        }

        public ConfigBinding(string path)
            : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Source = Properties.Settings.Default;
            this.Mode = BindingMode.TwoWay;
        }
    }
}
