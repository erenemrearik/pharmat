using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Pharmat
{
    public partial class İlaçlar : Form
    {
        // İlac sınıfından bir nesne oluşturuluyor.
        private Ilac ilac = new Ilac();

        // Constructor, formun yüklenmesi sırasında yapılacak işlemleri başlatır.
        public İlaçlar(string yonetici)
        {
            InitializeComponent();
            Ilaclar_DGV.CellClick += new DataGridViewCellEventHandler(Ilaclar_DGV_CellClick);
            Bugun();  // Bugünün tarihini gösterir.
            UreticiBagla();  // Üretici bilgilerini combobox'a bağlar.
            Yukle();  // İlaçları yükler ve DataGridView'de gösterir.
            Yonetici_Lbl.Text = yonetici;  // Yönetici bilgisi etikete atanır.
        }

        // İlaçları veritabanından çekip DataGridView'e yükler.
        private void Yukle()
        {
            List<Ilac> ilaclar = ilac.IlaclariGetir();
            Ilaclar_DGV.DataSource = ilaclar;
            Ilaclar_DGV.Refresh();
        }

        // Formdaki alanları temizler.
        private void Temizle()
        {
            IlacID_Tb.Text = string.Empty;
            IlacAdi_Tb.Text = string.Empty;
            IlacAdet_Tb.Text = string.Empty;
            IlacFiyat_Tb.Text = string.Empty;
            IlacSKT_Dt.Value = DateTime.Now;
            IlacTuru_Cb.SelectedIndex = 0;
            IlacUreticiID_Cb.SelectedIndex = 0;
        }

        // Üreticileri combobox'a bağlar.
        private void UreticiBagla()
        {
            List<Uretici> ureticiler = ilac.UreticileriGetir();
            IlacUreticiID_Cb.DisplayMember = "UreticiAdi";
            IlacUreticiID_Cb.ValueMember = "UreticiID";
            IlacUreticiID_Cb.DataSource = ureticiler;
        }

        // Seçilen ilacı günceller.
        private void Guncelle()
        {
            if (string.IsNullOrWhiteSpace(IlacID_Tb.Text))
            {
                MessageBox.Show("Lütfen bilgisi güncellenecek ilacı seçin.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
            }
            else if (string.IsNullOrWhiteSpace(IlacAdi_Tb.Text) || string.IsNullOrWhiteSpace(IlacAdet_Tb.Text) || string.IsNullOrWhiteSpace(IlacFiyat_Tb.Text) || IlacTuru_Cb.SelectedIndex == -1 || IlacUreticiID_Cb.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen boş alanları doldurunuz.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
            }
            else
            {
                // İlaç bilgilerini güncellemek için gerekli olan verileri alır ve ilac nesnesine atar.
                ilac.IlacID = int.Parse(IlacID_Tb.Text);
                ilac.IlacIsim = IlacAdi_Tb.Text;
                ilac.IlacAdet = int.Parse(IlacAdet_Tb.Text);
                ilac.IlacFiyat = int.Parse(IlacFiyat_Tb.Text);
                ilac.IlacSKT = IlacSKT_Dt.Value.Date;
                ilac.IlacTuru = IlacTuru_Cb.SelectedItem.ToString();
                ilac.IlacUreticiID = (int)IlacUreticiID_Cb.SelectedValue;
                ilac.Guncelle();  // Güncelleme işlemini yapar.
                Temizle();  // Form alanlarını temizler.
                Yukle();  // Güncellenmiş verileri tekrar yükler.
            }
        }

        // Yeni ilaç ekler.
        private void Ekle()
        {
            if (IlacAdi_Tb.Text == "" || IlacAdet_Tb.Text == "" || IlacFiyat_Tb.Text == "" || IlacTuru_Cb.SelectedIndex == -1 || IlacUreticiID_Cb.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen boş alanları doldurunuz.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
            }
            else
            {
                // İlaç bilgilerini ilac nesnesine atar ve ekleme işlemini yapar.
                ilac.IlacIsim = IlacAdi_Tb.Text;
                ilac.IlacAdet = int.Parse(IlacAdet_Tb.Text);
                ilac.IlacFiyat = int.Parse(IlacFiyat_Tb.Text);
                ilac.IlacSKT = IlacSKT_Dt.Value.Date;
                ilac.IlacTuru = IlacTuru_Cb.SelectedItem.ToString();
                ilac.IlacUreticiID = (int)IlacUreticiID_Cb.SelectedValue;
                ilac.Ekle();
                IlacID_Tb.Text = ilac.IlacID.ToString();  // Yeni eklenen ilacın ID'sini form alanına yazar.
                Temizle();  // Form alanlarını temizler.
                Yukle();  // Güncellenmiş verileri tekrar yükler.
            }
        }

        // Seçilen ilacı siler.
        private void Sil()
        {
            if (string.IsNullOrWhiteSpace(IlacID_Tb.Text))
            {
                MessageBox.Show("Lütfen bilgisi silinecek ilacı seçin.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
            }
            else
            {
                ilac.IlacID = int.Parse(IlacID_Tb.Text);
                ilac.Sil();  // Silme işlemini yapar.
                Temizle();  // Form alanlarını temizler.
                Yukle();  // Güncellenmiş verileri tekrar yükler.
            }
        }

        // Bugünün tarihini alır ve etikete yazar.
        private void Bugun()
        {
            string tarih = DateTime.Now.ToString("dd/MM/yyyy");
            tarihLbl.Text = tarih;
        }

        // Form yüklendiğinde yapılacak işlemler.
        private void İlaçlar_Load(object sender, EventArgs e)
        {
            Yukle();  // İlaçları yükler.
            araTur_Cb.Items.Add("İlaç adına göre");
            araTur_Cb.Items.Add("İlaç türüne göre");
            araTur_Cb.Items.Add("İlaç üreticisine göre");
            araTur_Cb.SelectedIndex = 0;
        }

        // Arama işlemi için butonun tıklanması.
        private void Ara_Butonu_Click(object sender, EventArgs e)
        {
            string baglantiAdresi = @"Data Source=YUNUS-EREN\SQLEXPRESS;Initial Catalog=PharmatDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=true";
            string aramaDegeri = Arama_Tb.Text;
            string sorgu = "";

            // Arama kriterine göre sorgu belirlenir.
            switch (araTur_Cb.SelectedIndex)
            {
                case 0: // İlaç adına göre
                    sorgu = "SELECT * FROM IlaclarTbl WHERE IlacIsim LIKE @aramaDegeri";
                    break;
                case 1: // İlaç türüne göre
                    sorgu = "SELECT * FROM IlaclarTbl WHERE IlacTuru LIKE @aramaDegeri";
                    break;
                case 2: // İlaç üreticisine göre
                    sorgu = "SELECT * FROM IlaclarTbl WHERE IlacUreticiID = @aramaDegeri";
                    break;
            }

            // Veritabanına bağlanarak arama işlemi yapılır.
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
                    Ilaclar_DGV.DataSource = dt;
                }
            }
        }

        // Yenile butonuna tıklanması.
        private void Yenile_Buton_Click(object sender, EventArgs e)
        {
            Temizle();  // Form alanlarını temizler.
            Yukle();  // İlaçları yeniden yükler.
        }

        // Ekle butonuna tıklanması.
        private void Ekle_Butonu_Click(object sender, EventArgs e)
        {
            Ekle();  // Yeni ilaç ekler.
        }

        // DataGridView'deki hücreye tıklanması.
        private void Ilaclar_DGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Seçilen hücredeki veriler form alanlarına aktarılır.
                DataGridViewRow row = Ilaclar_DGV.Rows[e.RowIndex];

                IlacID_Tb.Text = row.Cells[0].Value.ToString();
                IlacAdi_Tb.Text = row.Cells[1].Value.ToString();
                IlacAdet_Tb.Text = row.Cells[2].Value.ToString();
                IlacFiyat_Tb.Text = row.Cells[3].Value.ToString();
                IlacSKT_Dt.Value = DateTime.Parse(row.Cells[4].Value.ToString());
                IlacTuru_Cb.SelectedItem = row.Cells[5].Value.ToString();
                IlacUreticiID_Cb.SelectedValue = row.Cells[6].Value;
            }
        }

        // Güncelle butonuna tıklanması.
        private void Guncelle_Butonu_Click(object sender, EventArgs e)
        {
            Guncelle();  // İlaç günceller.
        }

        // Sil butonuna tıklanması.
        private void Sil_Butonu_Click(object sender, EventArgs e)
        {
            Sil();  // İlaç siler.
        }

        // İlaç adet textbox'ında sadece rakam girilmesine izin verir.
        private void IlacAdet_Tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        // İlaç fiyat textbox'ında sadece rakam girilmesine izin verir.
        private void IlacFiyat_Tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        // Özet etiketine tıklanması.
        private void label16_Click(object sender, EventArgs e) // Özet
        {
            if (Yonetici_Lbl.Text == "yonetici")
            {
                Panel formYonetici = new Panel("yonetici");
                formYonetici.Show();
                this.Hide();
            }
            else
            {
                ÇalışanPanel formCalisanPanel = new ÇalışanPanel(Yonetici_Lbl.Text);
                formCalisanPanel.Show();
                this.Hide();
            }
        }

        // Müşteriler etiketine tıklanması.
        private void label12_Click(object sender, EventArgs e) // Müşteriler
        {
            Müşteriler formMusteri = new Müşteriler(Yonetici_Lbl.Text);
            formMusteri.Show();
            this.Hide();
        }

        // Üreticiler etiketine tıklanması.
        private void label13_Click(object sender, EventArgs e) // Üreticiler
        {
            Üreticiler formUretici = new Üreticiler(Yonetici_Lbl.Text);
            formUretici.Show();
            this.Hide();
        }

        // Satış Yap etiketine tıklanması.
        private void label10_Click(object sender, EventArgs e) // Satış Yap
        {
            SatisYap formSatis = new SatisYap(Yonetici_Lbl.Text);
            formSatis.Show();
            this.Hide();
        }

        // Çıkış Yap etiketine tıklanması.
        private void label1_Click(object sender, EventArgs e) // Çıkış Yap
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
                İlaçlar formIlac = new İlaçlar(Yonetici_Lbl.Text);
                formIlac.Show();
                this.Hide();
            }
        }
    }
}
