using System.Collections.Generic;
using System.Windows.Forms;
using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;

namespace Pharmat
{
    public partial class Çalışanlar : Form
    {
        private List<Calisan> calisanlar = new List<Calisan>();

        public Çalışanlar()
        {
            InitializeComponent();
            // DataGridView hücre tıklama olayını bağla
            Calisan_DGV.CellClick += new DataGridViewCellEventHandler(Calisan_DGV_CellClick);
            // Ekle butonu tıklama olayını bağla
            Ekle_Butonu.Click += new EventHandler(Ekle_Butonu_Click);
            // Güncelle butonu tıklama olayını bağla
            Guncelle_Butonu.Click += new EventHandler(Guncelle_Butonu_Click);
            // Sil butonu tıklama olayını bağla
            Sil_Butonu.Click += new EventHandler(Sil_Butonu_Click);
            // Temizle butonu tıklama olayını bağla
            Temizle_Buton.Click += new EventHandler(Temizle_Buton_Click);
            // DataGridView'de çoklu seçim yapılmasını engelle
            Calisan_DGV.MultiSelect = false;
            // DataGridView seçim modunu ayarla
            Calisan_DGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // Bugünün tarihini göster
            Bugun();
        }

        // Bugünün tarihini gösteren metod
        private void Bugun()
        {
            string tarih = DateTime.Now.ToString("dd/MM/yyyy");
            tarihLbl.Text = tarih;
        }

        // Formdaki alanları temizleyen metod
        private void Temizle()
        {
            CalisanAdi_Tb.Text = string.Empty;
            CalisanSoyadi_Tb.Text = string.Empty;
            CalisanCinsiyet_Cb.SelectedIndex = -1;
            CalisanTelefon_Tb.Text = string.Empty;
            CalisanDTarihi_Dt.Value = DateTime.Now;
            CalisanSifre_Tb.Text = string.Empty;
        }

        // Veritabanından çalışanları yükleyen metod
        private void Yukle()
        {
            calisanlar = Calisan.Yukle();
            Calisan_DGV.DataSource = null;
            Calisan_DGV.Rows.Clear();
            foreach (var calisan in calisanlar)
            {
                Calisan_DGV.Rows.Add(new object[]
                {
                    calisan.CalisanID,
                    calisan.CalisanAdi,
                    calisan.CalisanSoyadi,
                    calisan.CalisanCinsiyet,
                    calisan.CalisanTelefon,
                    calisan.CalisanDTarihi.ToString("dd/MM/yyyy"),
                    calisan.CalisanSifre
                });
            }
        }

        // Ekle butonuna tıklandığında çalışan ekleyen metod
        private void Ekle_Butonu_Click(object sender, EventArgs e)
        {
            // Şifre uzunluğunu kontrol et
            if (CalisanSifre_Tb.Text.Length < 12)
            {
                MessageBox.Show("12 karakterli bir şifre oluşturunuz.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                CalisanSifre_Tb.Text = string.Empty;
            }
            else
            {
                Calisan calisan = new Calisan
                {
                    CalisanAdi = CalisanAdi_Tb.Text,
                    CalisanSoyadi = CalisanSoyadi_Tb.Text,
                    CalisanCinsiyet = CalisanCinsiyet_Cb.SelectedItem?.ToString(),
                    CalisanTelefon = CalisanTelefon_Tb.Text,
                    CalisanDTarihi = CalisanDTarihi_Dt.Value.Date,
                    CalisanSifre = CalisanSifre_Tb.Text
                };

                try
                {
                    calisan.Ekle();
                    MessageBox.Show("Çalışan başarıyla eklendi.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                    Temizle();
                    Yukle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // Güncelle butonuna tıklandığında çalışan güncelleyen metod
        private void Guncelle_Butonu_Click(object sender, EventArgs e)
        {
            if (Calisan_DGV.SelectedRows.Count > 0)
            {
                int selectedIndex = Calisan_DGV.SelectedRows[0].Index;
                string selectedCalisanID = Calisan_DGV.Rows[selectedIndex].Cells[0].Value.ToString();

                Calisan selectedCalisan = calisanlar.FirstOrDefault(c => c.CalisanID == selectedCalisanID);

                if (selectedCalisan != null)
                {
                    selectedCalisan.CalisanAdi = CalisanAdi_Tb.Text;
                    selectedCalisan.CalisanSoyadi = CalisanSoyadi_Tb.Text;
                    selectedCalisan.CalisanCinsiyet = CalisanCinsiyet_Cb.SelectedItem.ToString();
                    selectedCalisan.CalisanTelefon = CalisanTelefon_Tb.Text;
                    selectedCalisan.CalisanDTarihi = CalisanDTarihi_Dt.Value.Date;
                    selectedCalisan.CalisanSifre = CalisanSifre_Tb.Text;

                    try
                    {
                        selectedCalisan.Guncelle();
                        MessageBox.Show("Çalışan bilgileri başarıyla güncellendi.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                        Temizle();
                        Yukle();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Seçili çalışan bulunamadı.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                }
            }
            else
            {
                MessageBox.Show("Lütfen güncellemek için bir çalışan seçin.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
            }
        }

        // Sil butonuna tıklandığında çalışan silen metod
        private void Sil_Butonu_Click(object sender, EventArgs e)
        {
            if (Calisan_DGV.SelectedRows.Count > 0)
            {
                int selectedIndex = Calisan_DGV.SelectedRows[0].Index;
                string selectedCalisanID = Calisan_DGV.Rows[selectedIndex].Cells[0].Value.ToString();
                Calisan selectedCalisan = calisanlar.FirstOrDefault(c => c.CalisanID == selectedCalisanID);

                if (selectedCalisan != null)
                {
                    try
                    {
                        selectedCalisan.Sil();
                        MessageBox.Show("Çalışan başarıyla silindi.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                        Temizle();
                        Yukle();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Seçili çalışan bulunamadı.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek için bir çalışan seçin.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
            }
        }

        // Doğum tarihi değiştiğinde yaş hesaplayan metod
        private void CalisanDTarihi_Dt_ValueChanged_1(object sender, EventArgs e)
        {
            Yas_Tb.Text = Calisan.YasHesapla(CalisanDTarihi_Dt.Value).ToString();
        }

        // Form yüklendiğinde çalışanları yükleyen metod
        private void Çalışanlar_Load(object sender, EventArgs e)
        {
            CalisanSifre_Tb.MaxLength = 12; // En fazla 12 karakter şifre girilmesini sağlıyor.
            Yukle();
            araTur_Cb.Items.Add("Adına göre");
            araTur_Cb.Items.Add("Soyadına göre");
            araTur_Cb.Items.Add("Cinsiyetine göre");
            araTur_Cb.Items.Add("Telefon numarasına göre");
            araTur_Cb.SelectedIndex = 0;
        }

        // Arama butonuna tıklandığında çalışan arayan metod
        private void Ara_Butonu_Click(object sender, EventArgs e)
        {
            string baglantiAdresi = @"Data Source=YUNUS-EREN\SQLEXPRESS;Initial Catalog=PharmatDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=true";
            string aramaDegeri = Arama_Tb.Text;
            string sorgu = "";

            switch (araTur_Cb.SelectedIndex)
            {
                case 0: // Adına göre
                    sorgu = "SELECT * FROM CalisanTbl WHERE CalisanAd LIKE @aramaDegeri";
                    break;
                case 1: // Soyadına göre
                    sorgu = "SELECT * FROM CalisanTbl WHERE CalisanSoyad LIKE @aramaDegeri";
                    break;
                case 2: // Cinsiyetine göre
                    sorgu = "SELECT * FROM CalisanTbl WHERE CalisanCinsiyet = @aramaDegeri";
                    break;
                case 3: // Telefon numarasına göre
                    sorgu = "SELECT * FROM CalisanTbl WHERE CalisanTelefon = @aramaDegeri";
                    break;
            }

            using (SqlConnection conn = new SqlConnection(baglantiAdresi))
            {
                using (SqlCommand cmd = new SqlCommand(sorgu, conn))
                {
                    if (araTur_Cb.SelectedIndex != 2 && araTur_Cb.SelectedIndex != 3)
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
                    Calisan_DGV.DataSource = dt;
                }
            }
        }

        // DataGridView hücre tıklama olayında çalışanın detaylarını form alanlarına dolduran metod
        private void Calisan_DGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = Calisan_DGV.Rows[e.RowIndex]; // Başlık satırına tıklanmasını önlemek için

                CalisanAdi_Tb.Text = row.Cells[1].Value.ToString();
                CalisanSoyadi_Tb.Text = row.Cells[2].Value.ToString();
                CalisanCinsiyet_Cb.SelectedItem = row.Cells[3].Value.ToString();
                CalisanTelefon_Tb.Text = row.Cells[4].Value.ToString();
                CalisanDTarihi_Dt.Value = DateTime.Parse(row.Cells[5].Value.ToString());
                CalisanSifre_Tb.Text = row.Cells[6].Value.ToString();
            }
        }

        // Temizle butonuna tıklandığında form alanlarını temizleyen metod
        private void Temizle_Buton_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        // DataGridView'de seçim değiştiğinde form alanlarını güncelleyen metod
        private void Calisan_DGV_SelectionChanged(object sender, EventArgs e)
        {
            if (Calisan_DGV.SelectedRows.Count > 0)
            {
                int selectedIndex = Calisan_DGV.SelectedRows[0].Index;
                DataGridViewRow selectedRow = Calisan_DGV.Rows[selectedIndex];

                CalisanAdi_Tb.Text = selectedRow.Cells[1].Value.ToString();
                CalisanSoyadi_Tb.Text = selectedRow.Cells[2].Value.ToString();
                CalisanCinsiyet_Cb.SelectedItem = selectedRow.Cells[3].Value.ToString();
                CalisanTelefon_Tb.Text = selectedRow.Cells[4].Value.ToString();
                CalisanDTarihi_Dt.Value = DateTime.Parse(selectedRow.Cells[5].Value.ToString());
                CalisanSifre_Tb.Text = selectedRow.Cells[6].Value.ToString();
            }
        }

        // Çalışan adı alanına sadece harf girişini sağlayan metod
        private void CalisanAdi_Tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsSeparator(e.KeyChar);
        }

        // Çalışan soyadı alanına sadece harf girişini sağlayan metod
        private void CalisanSoyadi_Tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsSeparator(e.KeyChar);
        }

        // Özet label'ına tıklandığında yöneticinin paneline geçiş yapan metod
        private void label10_Click(object sender, EventArgs e)
        {
            Panel formYoneticiPanel = new Panel("yonetici");
            formYoneticiPanel.Show();
            this.Hide();
        }

        // İlaçlar label'ına tıklandığında ilaçlar formuna geçiş yapan metod
        private void label21_Click(object sender, EventArgs e)
        {
            İlaçlar formIlac = new İlaçlar("yonetici");
            formIlac.Show();
            this.Hide();
        }

        // Müşteriler label'ına tıklandığında müşteriler formuna geçiş yapan metod
        private void label20_Click(object sender, EventArgs e)
        {
            Müşteriler formMusteri = new Müşteriler("yonetici");
            formMusteri.Show();
            this.Hide();
        }

        // Üreticiler label'ına tıklandığında üreticiler formuna geçiş yapan metod
        private void label18_Click(object sender, EventArgs e)
        {
            Üreticiler formUretici = new Üreticiler("yonetici");
            formUretici.Show();
            this.Hide();
        }

        // Satış Yap label'ına tıklandığında satış formuna geçiş yapan metod
        private void label16_Click(object sender, EventArgs e)
        {
            SatisYap formSatis = new SatisYap("yonetici");
            formSatis.Show();
            this.Hide();
        }

        // Çıkış Yap label'ına tıklandığında çıkış işlemini gerçekleştiren metod
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
                Çalışanlar formCalisan = new Çalışanlar();
                formCalisan.Show();
                this.Hide();
            }
        }
    }
}
