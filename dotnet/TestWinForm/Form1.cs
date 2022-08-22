using OfdToPdf;

namespace TestWinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private byte[]? _bytes = null;
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = ".ofd";
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                using var stream = openFileDialog1.OpenFile();
                _bytes = new byte[stream.Length];
                stream.Read(_bytes, 0, (int) stream.Length);
                label1.Text = $"�Ѿ���ȡ�ļ�����{_bytes.Length}�ֽ�";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ConvertTo();
        }

        private async Task ConvertTo()
        {
            if (_bytes == null)
            {
                MessageBox.Show("����ѡ��OFD�ļ�");
                return;
            }

            var client = new OfdClient("", "");
            var pdfResult = await client.ToPdfAsync(_bytes);
            var path = Path.Combine(Environment.CurrentDirectory, $"{DateTime.Now.Ticks}.pdf");
            var httpClient = new HttpClient();
            if (pdfResult != null)
            {
                var bytes = await httpClient.GetByteArrayAsync(pdfResult.PdfUrl);
                await File.WriteAllBytesAsync(path, bytes);
                Invoke(() =>
                {
                    label2.Text = $"ת���ɹ����Ѿ����浽{path}";
                });

            }
            else
            {
                Invoke(() =>
                {
                    label2.Text = $"ת��ʧ�ܣ�{pdfResult.Code} {pdfResult.Message}";
                });
            }
        }
    }
}