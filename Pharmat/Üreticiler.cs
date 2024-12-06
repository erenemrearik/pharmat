using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace Pharmat
{
    public partial class Üreticiler : Form
    {
        private List<Uretici> ureticiListesi; // Üretici listesi

        // Constructor
        public Üreticiler(string yonetici)
        {
            InitializeComponent();
            Ureticiler_DGV.CellClick += new DataGridViewCellEventHandler(Ureticiler_DGV_CellClick); // DataGridView hücre tıklama olayı
            Bugun(); // Bugünün tarihini ayarlar
            Yukle(); // Üretici listesini yükler
            label2.Text = yonetici; // Yönetici adını etikete yazar
        }

        // Üretici listesini yükler
        private void Yukle()
        {
            ureticiListesi = Uretici.GetUreticiler();
            Ureticiler_DGV.DataSource = ureticiListesi;
        }

        // Formdaki tüm giriş alanlarını temizler
        private void Temizle()
        {
            UreticiID_Tb.Text = string.Empty;
            UreticiAdi_Tb.Text = string.Empty;
            UreticiAdresi_Tb.Text = string.Empty;
            UreticiTelefon_Tb.Text = string.Empty;
            UreticiKTarihi_Dt.Value = DateTime.Now;
        }
        // Formu yenile butonu tıklama olayı
        private void Yenile_Buton_Click(object sender, EventArgs e)
        {
            Temizle(); // Formu temizler
            Yukle(); // Üretici listesini günceller
        }

        // Bugünün tarihini etikete yazar
        private void Bugun()
        {
            string tarih = DateTime.Now.ToString("dd/MM/yyyy");
            tarihLbl.Text = tarih;
        }

        // Yeni üretici ekleme butonu tıklama olayı
        private void Ekle_Butonu_Click(object sender, EventArgs e)
        {
            Uretici yeniUretici = new Uretici
            {
                UreticiAdi = UreticiAdi_Tb.Text,
                UreticiAdresi = UreticiAdresi_Tb.Text,
                UreticiTelefon = UreticiTelefon_Tb.Text,
                UreticiKTarihi = UreticiKTarihi_Dt.Value
            };
            yeniUretici.Ekle();
            Yukle(); // Üretici listesini günceller
            Temizle(); // Formu temizler
        }

        // Üretici güncelleme butonu tıklama olayı
        private void Guncelle_Butonu_Click(object sender, EventArgs e)
        {
            Uretici guncelUretici = ureticiListesi.FirstOrDefault(u => u.UreticiID == int.Parse(UreticiID_Tb.Text));
            if (guncelUretici != null)
            {
                guncelUretici.UreticiAdi = UreticiAdi_Tb.Text;
                guncelUretici.UreticiAdresi = UreticiAdresi_Tb.Text;
                guncelUretici.UreticiTelefon = UreticiTelefon_Tb.Text;
                guncelUretici.UreticiKTarihi = UreticiKTarihi_Dt.Value;
                guncelUretici.Guncelle();
                Yukle(); // Üretici listesini günceller
                Temizle(); // Formu temizler
            }
        }

        // Üretici silme butonu tıklama olayı
        private void Sil_Butonu_Click(object sender, EventArgs e)
        {
            Uretici silinecekUretici = ureticiListesi.FirstOrDefault(u => u.UreticiID == int.Parse(UreticiID_Tb.Text));
            if (silinecekUretici != null)
            {
                silinecekUretici.Sil();
                Yukle(); // Üretici listesini günceller
                Temizle(); // Formu temizler
            }
        }



        // Form yüklendiğinde yapılacak işlemler
        private void Üreticiler_Load(object sender, EventArgs e)
        {
            Yukle(); // Üretici listesini yükler
            araTur_Cb.Items.Add("Üretici adına göre"); // Arama türü seçenekleri ekler
            araTur_Cb.Items.Add("Üretici adresine göre");
            araTur_Cb.Items.Add("Sicil numarasına göre");
            araTur_Cb.SelectedIndex = 0; // Varsayılan seçeneği ayarlar
        }

        // Arama butonu tıklama olayı
        private void Ara_Butonu_Click(object sender, EventArgs e)
        {
            string baglantiAdresi = @"Data Source=YUNUS-EREN\SQLEXPRESS;Initial Catalog=PharmatDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=true";
            string aramaDegeri = Arama_Tb.Text;
            string sorgu = "";

            // Arama türüne göre sorgu oluşturur
            switch (araTur_Cb.SelectedIndex)
            {
                case 0: // Üretici adına göre
                    sorgu = "SELECT * FROM UreticiTbl WHERE UreticiAdi LIKE @aramaDegeri";
                    break;
                case 1: // Üretici adresine göre
                    sorgu = "SELECT * FROM UreticiTbl WHERE UreticiAdresi LIKE @aramaDegeri";
                    break;
                case 2: // Sicil numarasına göre
                    sorgu = "SELECT * FROM UreticiTbl WHERE UreticiID = @aramaDegeri";
                    break;
            }

            // Veritabanı bağlantısı ve arama sorgusu
            using (SqlConnection conn = new SqlConnection(baglantiAdresi))
            {
                using (SqlCommand cmd = new SqlCommand(sorgu, conn))
                {
                    if (araTur_Cb.SelectedIndex != 2)
                    {
                        cmd.Parameters.AddWithValue("@aramaDegeri", "%" + aramaDegeri + "%");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@aramaDegeri", aramaDegeri);
                    }

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    Ureticiler_DGV.DataSource = dt; // Arama sonuçlarını DataGridView'e yazar
                }
            }
        }

        // DataGridView hücre tıklama olayı
        private void Ureticiler_DGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = Ureticiler_DGV.Rows[e.RowIndex];
                UreticiID_Tb.Text = row.Cells[0].Value.ToString();
                UreticiAdi_Tb.Text = row.Cells[1].Value.ToString();
                UreticiAdresi_Tb.Text = row.Cells[2].Value.ToString();
                UreticiTelefon_Tb.Text = row.Cells[3].Value.ToString();
                UreticiKTarihi_Dt.Value = DateTime.Parse(row.Cells[4].Value.ToString());
            }
        }

        // Yönetici veya çalışan paneline geçiş yapar
        private void label21_Click(object sender, EventArgs e)
        {
            if (label2.Text == "yonetici")
            {
                Panel formYonetici = new Panel("yonetici");
                formYonetici.Show();
                this.Hide();
            }
            else
            {
                ÇalışanPanel formCalisanPanel = new ÇalışanPanel(label2.Text);
                formCalisanPanel.Show();
                this.Hide();
            }
        }

        // İlaçlar formuna geçiş yapar
        private void label20_Click(object sender, EventArgs e)
        {
            İlaçlar formIlac = new İlaçlar(label2.Text);
            formIlac.Show();
            this.Hide();
        }

        // Müşteriler formuna geçiş yapar
        private void label19_Click(object sender, EventArgs e)
        {
            Müşteriler formMusteri = new Müşteriler(label2.Text);
            formMusteri.Show();
            this.Hide();
        }

        // Satış Yap formuna geçiş yapar
        private void label16_Click(object sender, EventArgs e)
        {
            SatisYap formSatis = new SatisYap(label2.Text);
            formSatis.Show();
            this.Hide();
        }

        // Çıkış yapma işlemi
        private void label17_Click(object sender, EventArgs e)
        {
            DialogResult secim = MessageBox.Show("Çıkış yapmak istiyor musunuz?", "Pharmat - Sistem Mesajı", MessageBoxButtons.YesNo);
            if (secim == DialogResult.Yes)
            {
                Giris formGiris = new Giris();
                formGiris.Show();
                this.Hide();
            }
            else
            {
                Üreticiler formUretici = new Üreticiler(label2.Text);
                formUretici.Show();
                this.Hide();
            }
        }
    }
}
