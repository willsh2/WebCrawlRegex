using System.Text.RegularExpressions;

namespace WebCrawlRegex
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36";
        bool islem = false;
        Thread a;
        Thread b;

        void scraper(int kalan, List<string> siteler)
        {

            for (int i = 0; i < siteler.Count(); i++)
            {
                if (islem == false)
                {
                    return;
                }
                else if (i % 2 == kalan)
                {
                    try
                    {
                        using (HttpClient httpClient = new HttpClient())
                        {
                            httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
                            HttpResponseMessage kaynak = httpClient.GetAsync(siteler[i]).Result;

                            if (kaynak.IsSuccessStatusCode)
                            {
                                MatchCollection veriler = new Regex(@textBox1.Text).Matches(kaynak.Content.ReadAsStringAsync().Result);
                                List<string> yveriler = new List<string>();
                                
                                foreach (Match veri in veriler)
                                {
                                    if (!yveriler.Contains(veri.Value))
                                    {
                                        yveriler.Add(veri.Value);
                                    }
                                }

                                foreach (string veri in yveriler)
                                {
                                    listBox1.Items.Add(veri);
                                }

                                listBox2.Items.Add($"{siteler[i]} sitesi üzerinden {veriler.Count()} veri alýndý.");
                            }
                            else
                            {
                                listBox2.Items.Add($"{siteler[i]} sitesi {kaynak.StatusCode} kodunu döndürdü.");
                            }
                        }
                    }
                    catch
                    {
                        listBox2.Items.Add($"{siteler[i]} sitesine ulaþýlamýyor.");
                    }
                }
            }

            button1.Text = "Baþlat";
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] veriler = File.ReadAllLines(openFileDialog1.FileName);
                listBox3.Items.AddRange(veriler);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Baþlat")
            {
                if (listBox3.Items.Count == 0)
                {
                    listBox2.Items.Add("Site listesi girilmediði için iþlem baþlatýlamýyor.");
                    return;
                }

                List<string> siteler = new List<string>();

                foreach (string site in listBox3.Items)
                {
                    siteler.Add(site);
                }

                islem = true;

                a = new Thread(() => scraper(0, siteler));
                b = new Thread(() => scraper(1, siteler));

                a.Start();
                b.Start();

                button1.Text = "Durdur";
            }
            else
            {
                islem = false;
                button1.Text = "Baþlat";
                listBox2.Items.Add("Program durduruldu.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter dosya = new StreamWriter(saveFileDialog1.FileName))
                {
                    foreach (var veri in listBox1.Items)
                    {
                        dosya.WriteLine(veri.ToString());
                    }
                }

                listBox2.Items.Add($"{listBox1.Items.Count} veri {Path.GetFileName(saveFileDialog1.FileName)} dosyasýna kayýt edildi.");
            }
        }
    }
}