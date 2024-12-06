using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pharmat
{
    public partial class ÇalışanPanel : Form
    {
        public ÇalışanPanel(string adSoyad)
        {
            InitializeComponent();
            // Çalışan adı ve soyadını etikete yazdırma
            CalisanAdiSoyad_Lbl.Text = adSoyad;
            timer1.Start();
            B_MusteriSayac();
            Bugun();
        }

        // Bugün tarihini etikete yazdıran metod
        private void Bugun()
        {
            string tarih = DateTime.Now.ToString("dd/MM/yyyy");
            label22.Text = tarih;
        }

        // Zamanlayıcı tick olayı, her tikte saati etikete yazdırır
        private void timer1_Tick(object sender, EventArgs e)
        {
            SaatLbl.Text = DateTime.Now.ToString("HH:mm");
        }

        SqlConnection baglanti = new SqlConnection(@"Data Source=YUNUS-EREN\SQLEXPRESS;Initial Catalog=PharmatDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=true");

        // Bugünkü müşteri sayısını hesaplayan metod
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
                // Hata mesajı gösterme
                MessageBox.Show(ex.Message);
            }
        }

        // Form yüklendiğinde çalışan metod
        private void ÇalışanPanel_Load_1(object sender, EventArgs e)
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

                // Bugünkü satış geçmişini veri bağlama
                this.GunlukGecmis_DGV.DataSource = this.pharmatDBDataSet6.SatisTbl;
                this.GunlukGecmis_DGV.Refresh();
            }
            catch (Exception ex)
            {
                // Hata mesajı gösterme
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
                // Hata mesajı gösterme
                MessageBox.Show(ex.Message);
            }
        }

        // İlaçlar etiketi tıklama olayı
        private void label20_Click(object sender, EventArgs e)
        {
            string adSoyad = CalisanAdiSoyad_Lbl.Text;
            İlaçlar formIlac = new İlaçlar(adSoyad);
            formIlac.Show();
            this.Hide();
        }

        // Müşteriler etiketi tıklama olayı
        private void label19_Click(object sender, EventArgs e)
        {
            string adSoyad = CalisanAdiSoyad_Lbl.Text;
            Müşteriler formMusteri = new Müşteriler(adSoyad);
            formMusteri.Show();
            this.Hide();
        }

        // Üreticiler etiketi tıklama olayı
        private void label18_Click(object sender, EventArgs e)
        {
            string adSoyad = CalisanAdiSoyad_Lbl.Text;
            Üreticiler formUretici = new Üreticiler(adSoyad);
            formUretici.Show();
            this.Hide();
        }

        // Satış Yap etiketi tıklama olayı
        private void label16_Click(object sender, EventArgs e)
        {
            string adSoyad = CalisanAdiSoyad_Lbl.Text;
            SatisYap formSatis = new SatisYap(adSoyad);
            formSatis.Show();
            this.Hide();
        }

        // Çıkış Yap etiketi tıklama olayı
        private void label17_Click(object sender, EventArgs e)
        {
            string adSoyad = CalisanAdiSoyad_Lbl.Text;
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
                ÇalışanPanel formCalisanPanel = new ÇalışanPanel(adSoyad);
                formCalisanPanel.Show();
                this.Hide();
            }
        }
    }
}
