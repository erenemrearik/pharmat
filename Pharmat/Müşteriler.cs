using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pharmat
{
    public partial class Müşteriler : Form
    {
        private List<Musteri> _musteriler; // Müşteri listesini tutar

        // Formun yapıcı metodu
        public Müşteriler(string yonetici)
        {
            InitializeComponent();
            Musteriler_DGV.MultiSelect = false; // Çoklu seçim kapalı
            Musteriler_DGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Tam satır seçimi
            Musteriler_DGV.CellClick += Musteriler_DGV_CellClick; // Hücre tıklama olayı
            Bugun(); // Bugünkü tarihi göster
            _musteriler = new List<Musteri>(); // Müşteri listesi başlat
            Yukle(); // Müşteri verilerini yükle
            label10.Text = yonetici; // Yönetici adını göster
        }

        // Bugünkü tarihi gösteren metot
        private void Bugun()
        {
            string tarih = DateTime.Now.ToString("dd/MM/yyyy");
            tarihLbl.Text = tarih;
        }

        // Müşteri verilerini yükleyen metot
        private void Yukle()
        {
            _musteriler.Clear(); // Müşteri listesini temizle
            this.musteriTblTableAdapter1.Fill(this.pharmatDBDataSet15.MusteriTbl); // Veritabanından verileri çek
            foreach (DataRow row in this.pharmatDBDataSet15.MusteriTbl.Rows)
            {
                Musteri musteri = new Musteri
                {
                    MusteriID = Convert.ToInt32(row["MusteriID"]),
                    MusteriAd = row["MusteriAd"].ToString(),
                    MusteriSoyad = row["MusteriSoyad"].ToString(),
                    MusteriCinsiyet = row["MusteriCinsiyet"].ToString(),
                    MusteriTelefon = row["MusteriTelefon"].ToString(),
                    MusteriDTarihi = DateTime.Parse(row["MusteriDTarihi"].ToString()),
                    MusteriTC = row["MusteriTC"].ToString()
                };
                _musteriler.Add(musteri); // Listeye müşteri ekle
            }
            // DataGridView'e verileri yükle
            Musteriler_DGV.DataSource = _musteriler.Select(m => new
            {
                MusteriID = m.MusteriID,
                MusteriAd = m.MusteriAd,
                MusteriSoyad = m.MusteriSoyad,
                MusteriCinsiyet = m.MusteriCinsiyet,
                MusteriTelefon = m.MusteriTelefon,
                MusteriDTarihi = m.MusteriDTarihi,
                MusteriTC = m.MusteriTC
            }).ToList();
        }

        // Form elemanlarını temizleyen metot
        private void Temizle()
        {
            MusteriAdi_Tb.Clear();
            MusteriSoyadi_Tb.Clear();
            MusteriCinsiyet_Cb.SelectedIndex = -1;
            MusteriTelefon_Tb.Clear();
            MusteriDTarihi_Dt.Value = DateTime.Now;
            MusteriTC.Clear();
        }

        // Form yüklendiğinde çalışacak metot
        private void Müşteriler_Load(object sender, EventArgs e)
        {
            Yukle(); // Müşteri verilerini yükle
            araTur_Cb.Items.Add("Müşteri adına göre");
            araTur_Cb.Items.Add("Müşteri soyadına göre");
            araTur_Cb.Items.Add("Kimlik numarasına göre");
            araTur_Cb.SelectedIndex = 0; // Varsayılan seçim
        }

        // Arama butonuna tıklanınca çalışacak metot
        private void Ara_Butonu_Click(object sender, EventArgs e)
        {
            string baglantiAdresi = @"Data Source=YUNUS-EREN\SQLEXPRESS;Initial Catalog=PharmatDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=true";
            string aramaDegeri = Arama_Tb.Text;
            string sorgu = "";

            // Arama türüne göre sorgu oluştur
            switch (araTur_Cb.SelectedIndex)
            {
                case 0: // Müşteri adına göre
                    sorgu = "SELECT * FROM MusteriTbl WHERE MusteriAd LIKE @aramaDegeri";
                    break;
                case 1: // Müşteri soyadına göre
                    sorgu = "SELECT * FROM MusteriTbl WHERE MusteriSoyad LIKE @aramaDegeri";
                    break;
                case 2: // Kimlik numarasına göre
                    sorgu = "SELECT * FROM MusteriTbl WHERE MusteriTC = @aramaDegeri";
                    break;
            }

            // Veritabanı bağlantısı ve komutu
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
                    Musteriler_DGV.DataSource = dt; // DataGridView'e arama sonuçlarını yükle
                }
            }
        }

        // Ekle butonuna tıklanınca çalışacak metot
        private void Ekle_Butonu_Click(object sender, EventArgs e)
        {
            Musteri musteri = new Musteri
            {
                MusteriAd = MusteriAdi_Tb.Text,
                MusteriSoyad = MusteriSoyadi_Tb.Text,
                MusteriCinsiyet = MusteriCinsiyet_Cb.SelectedItem.ToString(),
                MusteriTelefon = MusteriTelefon_Tb.Text,
                MusteriDTarihi = MusteriDTarihi_Dt.Value,
                MusteriTC = MusteriTC.Text
            };

            musteri.Ekle(_musteriler); // Müşteri ekle
            Temizle(); // Formu temizle
            Yukle(); // Müşteri verilerini güncelle
        }

        // Güncelle butonuna tıklanınca çalışacak metot
        private void Guncelle_Butonu_Click(object sender, EventArgs e)
        {
            if (Musteriler_DGV.SelectedRows.Count > 0)
            {
                int selectedIndex = Musteriler_DGV.SelectedRows[0].Index;
                int musteriId = Convert.ToInt32(Musteriler_DGV.Rows[selectedIndex].Cells[0].Value);

                Musteri musteri = _musteriler.FirstOrDefault(m => m.MusteriID == musteriId);

                if (musteri != null)
                {
                    musteri.MusteriAd = MusteriAdi_Tb.Text;
                    musteri.MusteriSoyad = MusteriSoyadi_Tb.Text;
                    musteri.MusteriCinsiyet = MusteriCinsiyet_Cb.SelectedItem.ToString();
                    musteri.MusteriTelefon = MusteriTelefon_Tb.Text;
                    musteri.MusteriDTarihi = MusteriDTarihi_Dt.Value;
                    musteri.MusteriTC = MusteriTC.Text;

                    musteri.Guncelle(_musteriler); // Müşteri güncelle
                    Temizle(); // Formu temizle
                    Yukle(); // Müşteri verilerini güncelle
                }
                else
                {
                    MessageBox.Show("Seçili müşteri bulunamadı.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                }
            }
            else
            {
                MessageBox.Show("Lütfen güncellemek için bir müşteri seçin.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
            }
        }

        // Sil butonuna tıklanınca çalışacak metot
        private void Sil_Butonu_Click(object sender, EventArgs e)
        {
            if (Musteriler_DGV.SelectedRows.Count > 0)
            {
                int musteriId = Convert.ToInt32(Musteriler_DGV.SelectedRows[0].Cells[0].Value);
                Musteri selectedMusteri = _musteriler.FirstOrDefault(m => m.MusteriID == musteriId);

                if (selectedMusteri != null)
                {
                    selectedMusteri.Sil(_musteriler); // Müşteri sil
                    Temizle(); // Formu temizle
                    Yukle(); // Müşteri verilerini güncelle
                }
                else
                {
                    MessageBox.Show("Silinecek müşteri bulunamadı.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek için bir müşteri seçin.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
            }
        }

        // DataGridView hücre tıklama olayı
        private void Musteriler_DGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = Musteriler_DGV.Rows[e.RowIndex];

                int musteriId = Convert.ToInt32(row.Cells[0].Value);
                MusteriAdi_Tb.Text = row.Cells[1].Value.ToString();
                MusteriSoyadi_Tb.Text = row.Cells[2].Value.ToString();
                MusteriCinsiyet_Cb.SelectedItem = row.Cells[3].Value.ToString();
                MusteriTelefon_Tb.Text = row.Cells[4].Value.ToString();
                MusteriDTarihi_Dt.Value = DateTime.Parse(row.Cells[5].Value.ToString());
                MusteriTC.Text = row.Cells[6].Value.ToString();
            }
        }

        // Doğum tarihi değiştiğinde yaş hesaplama
        private void MusteriDTarihi_Dt_ValueChanged(object sender, EventArgs e)
        {
            Yas_Tb.Text = Musteri.YasHesapla(MusteriDTarihi_Dt.Value).ToString();
        }

        // Temizle butonuna tıklanınca çalışacak metot
        private void Temizle_Buton_Click(object sender, EventArgs e)
        {
            Temizle(); // Formu temizle
        }

        // Müşteri adı girerken sadece harfleri kabul et
        private void MusteriAdi_Tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsSeparator(e.KeyChar);
        }

        // Müşteri soyadı girerken sadece harfleri kabul et
        private void MusteriSoyadi_Tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsSeparator(e.KeyChar);
        }

        // Özet etiketine tıklanınca çalışacak metot
        private void label21_Click(object sender, EventArgs e)
        {
            if (label10.Text == "yonetici")
            {
                Panel formYonetici = new Panel("yonetici");
                formYonetici.Show();
                this.Hide();
            }
            else
            {
                ÇalışanPanel formCalisanPanel = new ÇalışanPanel(label10.Text);
                formCalisanPanel.Show();
                this.Hide();
            }
        }

        // İlaçlar etiketine tıklanınca çalışacak metot
        private void label7_Click(object sender, EventArgs e)
        {
            İlaçlar formIlac = new İlaçlar(label10.Text);
            formIlac.Show();
            this.Hide();
        }

        // Üreticiler etiketine tıklanınca çalışacak metot
        private void label18_Click(object sender, EventArgs e)
        {
            Üreticiler formUretici = new Üreticiler(label10.Text);
            formUretici.Show();
            this.Hide();
        }

        // Satış Yap etiketine tıklanınca çalışacak metot
        private void label16_Click(object sender, EventArgs e)
        {
            SatisYap formSatis = new SatisYap(label10.Text);
            formSatis.Show();
            this.Hide();
        }

        // Çıkış Yap etiketine tıklanınca çalışacak metot
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
                Müşteriler formMusteri = new Müşteriler(label10.Text);
                formMusteri.Show();
                this.Hide();
            }
        }
    }
}
