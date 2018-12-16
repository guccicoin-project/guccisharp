using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PaymentExample
{
    using System.Diagnostics;
    using System.Windows.Threading;

    using FontAwesome.WPF;

    using GucciSharp;

    using QRCoder;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Interface gucciInterface;

        private string paymentId/* = "A43DF8A0C4D50E2C7912B70B60C9F9975618C4C659126A82812BB6ADE7460B1D"*/;

        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            
            this.gucciInterface = new Interface("<YOURKEYHERE>");
            this.paymentId = Interface.CreatePaymentId();

            this.timer = new DispatcherTimer();
            this.timer.Tick += Timer_Tick;
            this.timer.Interval = new TimeSpan(0, 0, 10);

            this.CheckConnection();
            this.UpdateUi();

            this.timer.Start();
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("Checking for payment...");

            var transactions = await this.gucciInterface.GetTransactionsAsync(this.paymentId);
            var address = await this.gucciInterface.AddressAsync();

            if (transactions.Count > 0)
            {
                Debug.WriteLine($"Found {transactions.Count} payments!");
                // only get transactions where the transaction is inbound and to our address
                if (transactions.Where(x => x.inbound && x.address == address).Sum(x => x.amount) >= 10.0f)
                {
                    Debug.WriteLine($"Correct amount, transaction complete.");
                    this.TransactionComplete();
                    this.timer.Stop();
                }
            }
        }

        private void TransactionComplete()
        {
            this.statusIcon.Spin = false;
            this.statusIcon.Icon = FontAwesomeIcon.Check;
            this.statusText.Text = "Transaction Complete!";
        }

        private async void CheckConnection()
        {
            var good = await this.gucciInterface.CheckAsync();
            if (good)
            {
                return;
            }
            MessageBox.Show("Failed to connect to the API");
            Application.Current.Shutdown();
        }

        private async void UpdateUi()
        {
            var address = await this.gucciInterface.AddressAsync();

            // Update wallet address
            this.addressBox.Text = address;

            // Update payment ID
            this.paymentIdBox.Text = paymentId;

            // Update QR code
            var generator = new QRCodeGenerator();
            var data = generator.CreateQrCode(
                $"guccicoin:{address}?amount=10&id={this.paymentId}",
                QRCodeGenerator.ECCLevel.Q);
            var qr = new QRCode(data);
            var bitmap = qr.GetGraphic(5);
            var bitmapImage = BitmapConverter.ToBitmapImage(bitmap);
            this.qrImage.Source = bitmapImage;
        }
    }
}
