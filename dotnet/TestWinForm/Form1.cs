using OfdToPdf;

namespace TestWinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string[]? filenames = null;
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.DefaultExt = ".ofd";
            openFileDialog1.Multiselect = true;
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                filenames = openFileDialog1.FileNames;
                //_bytes = new byte[stream.Length];
                //stream.Read(_bytes, 0, (int) stream.Length);
                label1.Text = $"已经读取文件，共{filenames.Length} 个";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ConvertTo();
        }

        private async Task ConvertTo()
        {
            if (filenames == null || filenames.Length == 0)
            {
                MessageBox.Show("请先选择OFD文件");
                return;
            }

            var client = new OfdClient("AKIDmUQm21nn9y1ZEYDAUCiTA6N2rfc77rEpL8Y", "5cfEUFUhG1dtzS5Y4e5si1Sf81Q7shdF0vM4lGTu");
            foreach (var filename in filenames)
            {
                var fn = filename;
                var _bytes = File.ReadAllBytes(fn);
                var pdfResult = await client.ToPdfAsync(_bytes);
                CheckIfNotExists(Path.Combine(Environment.CurrentDirectory, $"pdf/"));
                var path = Path.Combine(Environment.CurrentDirectory, $"pdf/{Path.GetFileNameWithoutExtension(fn)}.pdf");
                var httpClient = new HttpClient();
                if (pdfResult != null)
                {
                    var bytes = await httpClient.GetByteArrayAsync(pdfResult.PdfUrl);
                    await File.WriteAllBytesAsync(path, bytes);
                    Invoke(() =>
                    {
                        label2.Text = $"转换成功，已经保存到{path}";
                    });

                }
                else
                {
                    Invoke(() =>
                    {
                        label2.Text = $"转换失败：{pdfResult.Code} {pdfResult.Message}";
                    });
                }
            }
       
        }

        private void CheckIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}