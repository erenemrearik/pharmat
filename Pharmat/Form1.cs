using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Pharmat
{
    public partial class Panel : Form
    {
        public Panel(string yonetici)
        {
            InitializeComponent();
            Bugun(); // Bugünün tarihini ekrana yazdırma
            IlacSayac(); // İlaç sayısını ekrana yazdırma
            MusteriSayac(); // Müşteri sayısını ekrana yazdırma
            CalisanSayac(); // Çalışan sayısını ekrana yazdırma
            UreticiSayac(); // Üretici sayısını ekrana yazdırma
            B_MusteriSayac(); // Bugünkü müşteri sayısını ekrana yazdırma
            timer1.Start(); // Saat zamanlayıcısını başlatma
            Yonetici_Lbl.Text = yonetici; // Yöneticinin adını etikete yazdırma
        }

        // Bugünün tarihini ekrana yazdıran metod
        private void Bugun()
        {
            string tarih = DateTime.Now.ToString("dd/MM/yyyy");
            label22.Text = tarih;

            // Eğer bugün 14 Mayıs ise, eczacılar gününü kutlama mesajı göster
            if (DateTime.Now.Day == 14 && DateTime.Now.Month == 5)
            {
                MessageBox.Show("Eczacılar günü kutlu olsun!", "Pharmat - Sistem Mesajı");
            }
        }

        SqlConnection baglanti = new SqlConnection(@"Data Source=YUNUS-EREN\SQLEXPRESS;Initial Catalog=PharmatDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=true");

        // İlaç sayısını ekrana yazdıran metod
        private void IlacSayac()
        {
            string sayac = "SELECT COUNT(*) FROM IlaclarTbl";

            try
            {
                using (SqlConnection connection = new SqlConnection(baglanti.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sayac, connection);
                    connection.Open();
                    int ilacSayac = (int)command.ExecuteScalar();
                    IlacSayac_Tb.Text = ilacSayac.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Müşteri sayısını ekrana yazdıran metod
        private void MusteriSayac()
        {
            string sayac = "SELECT COUNT(*) FROM MusteriTbl";

            try
            {
                using (SqlConnection connection = new SqlConnection(baglanti.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sayac, connection);
                    connection.Open();
                    int musteriSayac = (int)command.ExecuteScalar();
                    MusteriSayac_Tb.Text = musteriSayac.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Çalışan sayısını ekrana yazdıran metod
        private void CalisanSayac()
        {
            string sayac = "SELECT COUNT(*) FROM CalisanTbl";

            try
            {
                using (SqlConnection connection = new SqlConnection(baglanti.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sayac, connection);
                    connection.Open();
                    int calisanSayac = (int)command.ExecuteScalar();
                    CalisanSayac_Tb.Text = calisanSayac.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Üretici sayısını ekrana yazdıran metod
        private void UreticiSayac()
        {
            string sayac = "SELECT COUNT(*) FROM UreticiTbl";
            try
            {
                using (SqlConnection connection = new SqlConnection(baglanti.ConnectionString))
                {
                    SqlCommand command = new SqlCommand(sayac, connection);
                    connection.Open();
                    int ureticiSayac = (int)command.ExecuteScalar();
                    UreticiSayac_Lbl.Text = ureticiSayac.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Bugünkü müşteri sayısını ekrana yazdıran metod
        private void B_MusteriSayac()
        {
            DateTime bugun = DateTime.Today;
            string sorgu = "SELECT COUNT(DISTINCT M_ID) FROM SatisTbl WHERE CONVERT(date, FaturaTarih) = @Bugun";

            try
            {
                using (SqlConnection connection = new SqlConnection(baglanti.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(sorgu, connection))
                    {
                        command.Parameters.AddWithValue("@Bugun", bugun);
                        connection.Open();
                        int musteriSayisi = (int)command.ExecuteScalar();
                        GunlukMusteri_Lbl.Text = musteriSayisi.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Form yüklendiğinde çalışan metod
        private void Panel_Load(object sender, EventArgs e)
        {
            DateTime bugun = DateTime.Today;
            string sorgu = "SELECT * FROM SatisTbl WHERE CONVERT(date, FaturaTarih) = @Bugun";

            try
            {
                using (SqlConnection connection = new SqlConnection(baglanti.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(sorgu, connection))
                    {
                        command.Parameters.AddWithValue("@Bugun", bugun);
                        connection.Open();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(this.pharmatDBDataSet6.SatisTbl);
                    }
                }

                // Bugünkü satış geçmişini veri ızgarasına bağlama
                this.GunlukGecmis_DGV.DataSource = this.pharmatDBDataSet6.SatisTbl;
                this.GunlukGecmis_DGV.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            // Bugünkü toplam tutarı hesaplama
            string toplamTutar = "SELECT SUM(FaturaMiktar) FROM SatisTbl WHERE CONVERT(date, FaturaTarih) = @Bugun";

            try
            {
                using (SqlConnection connection = new SqlConnection(baglanti.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(toplamTutar, connection))
                    {
                        command.Parameters.AddWithValue("@Bugun", bugun);
                        connection.Open();
                        object sonuc = command.ExecuteScalar();
                        if (sonuc != DBNull.Value)
                        {
                            double toplamSatis = Convert.ToDouble(sonuc);
                            GunlukSatis_Lbl.Text = toplamSatis.ToString() + " ₺";
                        }
                        else
                        {
                            GunlukSatis_Lbl.Text = "0";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Zamanlayıcı tick olayı, her tikte saati etikete yazdırır
        private void timer1_Tick(object sender, EventArgs e)
        {
            SaatLbl.Text = DateTime.Now.ToString("HH:mm");
        }

        // İlaçlar etiketi tıklama olayı
        private void label21_Click(object sender, EventArgs e)
        {
            string yonetici = Yonetici_Lbl.Text;
            İlaçlar formIlac = new İlaçlar("yonetici");
            formIlac.Show();
            this.Hide();
        }

        // Müşteriler etiketi tıklama olayı
        private void label20_Click(object sender, EventArgs e)
        {
            string yonetici = Yonetici_Lbl.Text;
            Müşteriler formMusteri = new Müşteriler("yonetici");
            formMusteri.Show();
            this.Hide();
        }

        // Çalışanlar etiketi tıklama olayı
        private void label19_Click(object sender, EventArgs e)
        {
            Çalışanlar formCalisan = new Çalışanlar();
            formCalisan.Show();
            this.Hide();
        }

        // Üreticiler etiketi tıklama olayı
        private void label18_Click(object sender, EventArgs e)
        {
            string yonetici = Yonetici_Lbl.Text;
            Üreticiler formUretici = new Üreticiler("yonetici");
            formUretici.Show();
            this.Hide();
        }

        // Satış Yap etiketi tıklama olayı
        private void label16_Click(object sender, EventArgs e)
        {
            string yonetici = Yonetici_Lbl.Text;
            SatisYap formUretici = new SatisYap("yonetici");
            formUretici.Show();
            this.Hide();
        }

        // Çıkış Yap etiketi tıklama olayı
        private void label17_Click(object sender, EventArgs e)
        {
            DialogResult secim = MessageBox.Show("Çıkış yapmak istiyor musunuz?", "Pharmat - Sistem Mesajı", MessageBoxButtons.YesNo);
            if (secim == DialogResult.Yes)
            {
                // Kullanıcı çıkış yapmak isterse giriş formuna yönlendirme
                Giris formGiris = new Giris();
                formGiris.Show();
                this.Hide();
            }
            else
            {
                // Kullanıcı çıkış yapmak istemezse mevcut paneli yeniden yükleme
                Panel formYoneticiPanel = new Panel(Yonetici_Lbl.Text);
                formYoneticiPanel.Show();
                this.Hide();
            }
        }
    }
}
