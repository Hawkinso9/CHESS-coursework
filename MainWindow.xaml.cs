using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CHESS_coursework
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChessGame c;
        public MainWindow()
        {
            c = new ChessGame();
            InitializeComponent();
        }
        private void b1_Click(object sender, RoutedEventArgs e)
        {
            c.doSomething();
            updateGUI();

        }

        public void updateGUI()
        {
            int x = c.getBoard().getWin();

            T1.Text = x.ToString();

        }
    }

}