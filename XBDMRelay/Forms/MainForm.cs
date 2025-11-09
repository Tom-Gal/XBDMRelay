using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using XBDMRelay.Parsers;

namespace XBDMRelay.Forms
{
    public partial class MainForm : Form
    {
        private readonly XBDMParser XBDMParser = new XBDMParser();
        private UdpClient? udpListener;
        private TcpListener? tcpListener;
        private CancellationTokenSource? cts;

        private IPAddress? LocalIP;
        private string? XboxIP;
        private int XboxPort;

        public MainForm()
        {
            InitializeComponent();
        }

        private void RichTextBoxOutput_SelectionChanged(object sender, EventArgs e)
        {
            int index = RichTextBoxOutput.SelectionStart;
            int line = RichTextBoxOutput.GetLineFromCharIndex(index);
            int firstCharIndex = RichTextBoxOutput.GetFirstCharIndexOfCurrentLine();
            int column = index - firstCharIndex;

            StatusLabelLnCol.Text = $"Ln: {line + 1}, Cl: {column + 1}";
        }

        private async void ButtonToggleListener_Click(object sender, EventArgs e)
        {
            if (cts == null)
            {
                try
                {
                    cts = new CancellationTokenSource();
                    CancellationToken token = cts.Token;

                    ButtonToggleListener.Text = "Stop Listening";

                    LocalIP = GetLocalSubnetIP();
                    TextBoxProxyIP.Text = LocalIP.ToString();
                    AppendLog("Using local IP: " + LocalIP);

                    XboxIP = TextBoxXboxIPAddress.Text.Trim();
                    XboxPort = int.Parse(TextBoxXboxPort.Text);

                    AppendLog("Starting UDP and TCP listeners...");

                    var udpTask = Task.Run(() => StartUdpDiscovery(token), token);
                    var tcpTask = Task.Run(() => StartTcpProxy(token), token);

                    await Task.WhenAll(udpTask, tcpTask);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    StopProxy();
                }
            }
            else
            {
                StopProxy();
            }
        }

        private void StopProxy()
        {
            AppendLog("Stopping proxy...");

            cts?.Cancel();

            try
            {
                udpListener?.Close();
                tcpListener?.Stop();
            }
            catch (Exception ex)
            {
                AppendLog("Error while stopping: " + ex.Message);
            }

            cts?.Dispose();
            cts = null;

            ButtonToggleListener.Text = "Start Proxy";
            StatusLabelCurrentStatus.Text = "Waiting...";
            AppendLog("Proxy stopped.");
        }

        private void StartUdpDiscovery(CancellationToken token)
        {
            using (udpListener = new UdpClient(XboxPort))
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                AppendLog($"UDP Discovery listener started on port {XboxPort}...");

                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        if (udpListener.Available > 0)
                        {
                            byte[] data = udpListener.Receive(ref remoteEP);
                            AppendLog($"UDP Broadcast received from {remoteEP.Address}");

                            string response = $"MachineName=ProxyXbox;ConsoleType=DEVKIT;IP={LocalIP};Port={XboxPort};SerialNumber=PX123456;XDKVersion=2.0";
                            byte[] respBytes = Encoding.ASCII.GetBytes(response);

                            udpListener.Send(respBytes, respBytes.Length, remoteEP);
                            AppendLog($"Sent discovery response to {remoteEP.Address}");
                        }
                        else
                        {
                            Thread.Sleep(50); // Yes I know it's bad practice 
                        }
                    }
                }
                catch (ObjectDisposedException) { } 
                catch (Exception ex)
                {
                    AppendLog("UDP error: " + ex.Message);
                }
            }
        }

        private void StartTcpProxy(CancellationToken token)
        {
            tcpListener = new TcpListener(IPAddress.Any, XboxPort);
            tcpListener.Start();
            StatusLabelCurrentStatus.Text = $"Proxy listening on {LocalIP}:{XboxPort}...";
            AppendLog($"TCP Proxy listening on {LocalIP}:{XboxPort}...");

            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (tcpListener.Pending())
                    {
                        TcpClient client = tcpListener.AcceptTcpClient();
                        Task.Run(() => HandleClientAsync(client, token), token);
                    }
                    else
                    {
                        Thread.Sleep(50);
                    }
                }
            }
            catch (SocketException) { } // occurs on Stop
            catch (Exception ex)
            {
                AppendLog("TCP error: " + ex.Message);
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            using (client)
            {
                try
                {
                    using TcpClient xboxClient = new TcpClient();
                    await xboxClient.ConnectAsync(XboxIP!, XboxPort, token);

                    using NetworkStream clientStream = client.GetStream();
                    using NetworkStream xboxStream = xboxClient.GetStream();

                    var task1 = ForwardTrafficAsync(clientStream, xboxStream, "PC -> Xbox", token);
                    var task2 = ForwardTrafficAsync(xboxStream, clientStream, "Xbox -> PC", token);

                    await Task.WhenAny(task1, task2); // finish when one sidecloses
                }
                catch (Exception ex)
                {
                    AppendLog("Error handling client: " + ex.Message);
                }
            }
        }

        private async Task ForwardTrafficAsync(NetworkStream from, NetworkStream to, string direction, CancellationToken token)
        {
            byte[] buffer = new byte[8192];
            StringBuilder sb = new StringBuilder();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    int bytesRead = await from.ReadAsync(buffer.AsMemory(0, buffer.Length), token);
                    if (bytesRead == 0) break;

                    await to.WriteAsync(buffer.AsMemory(0, bytesRead), token);
                    await to.FlushAsync(token);

                    string asciiData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    sb.Append(asciiData);

                    string[] lines = sb.ToString().Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                    for (int i = 0; i < lines.Length - 1; i++)
                    {
                        string line = lines[i].Trim();
                        if (!string.IsNullOrEmpty(line))
                        {
                            string parsed = XBDMParser.ParseXboxCommand(line, direction);
                            AppendLog(parsed);
                        }
                    }

                    sb.Clear();
                    sb.Append(lines.Last());
                }

                if (sb.Length > 0)
                {
                    string remaining = sb.ToString().Trim();
                    if (!string.IsNullOrEmpty(remaining))
                        XBDMParser.ParseXboxCommand(remaining, direction);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                AppendLog($"Traffic forward error ({direction}): {ex.Message}");
            }
        }

        private IPAddress GetLocalSubnetIP()
        {
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != OperationalStatus.Up) continue;

                var ipProps = ni.GetIPProperties();
                foreach (var ip in ipProps.UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork &&
                        !IPAddress.IsLoopback(ip.Address))
                    {
                        return ip.Address;
                    }
                }
            }
            throw new Exception("No valid local IPv4 address found.");
        }

        private void AppendLog(string message)
        {
            if (RichTextBoxOutput.InvokeRequired)
            {
                RichTextBoxOutput.Invoke(() => AppendLog(message));
                return;
            }

            RichTextBoxOutput.AppendText(message + Environment.NewLine);
            RichTextBoxOutput.SelectionStart = RichTextBoxOutput.Text.Length;
            RichTextBoxOutput.ScrollToCaret();
        }

        private void exitApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopProxy();
            Application.Exit();
        }
    }
}
