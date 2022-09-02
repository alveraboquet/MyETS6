namespace ClassControlsAndStyle.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для DialogMessage.xaml
    /// </summary>
    public partial class DialogMessage 
    {
        public DialogMessage(string message, string title)
        {
            InitializeComponent();
            this.Title = title;
            MessageBoxWpf.Text = message;
            this.Topmost = true;
            this.Tag = message;

            this.Show();
        }

        public DialogMessage()
        {
            InitializeComponent();
        }
    }
}
