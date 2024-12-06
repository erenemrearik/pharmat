using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace Pharmat
{
    public partial class SatisYap : Form
    {
        // Toplam tutarı ve ilaç ID'sini saklayan değişkenler
        private decimal toplamTutar = 0;
        private int ilacId;

        // Yazdırma işlemleri için gereken nesneler
        private PrintDocument printDocument1 = new PrintDocument();
        private PrintPreviewDialog printPreviewDialog1 = new PrintPreviewDialog();

        // SatisYap sınıfının yapıcı metodu. Parametre olarak yönetici adını alır.
        public SatisYap(string yonetici)
        {
            InitializeComponent(); // Form bileşenlerini başlat.
            InitializeFaturaDGV(); // Fatura DataGridView bileşenini başlat.
            IlacSec_DGV.CellClick += new DataGridViewCellEventHandler(IlacSec_DGV_CellClick_1); // İlaç seçim DataGridView bileşenine tıklanma olayını atan.
            Bugun(); // Bugün tarihini alarak ilgili bileşenlere yazan metod.
            label2.Text = yonetici; // Yönetici adını ilgili etikete yaz.
            printDocument1.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage_1); // Yazdırma olayını atan.
        }


        // Fatura DataGridView'i başlatılıyor
        private void InitializeFaturaDGV()
        {
            Fatura_DGV.Columns.Clear();
            Fatura_DGV.Columns.Add("Column1", "Numara");
            Fatura_DGV.Columns.Add("Column2", "İlaç Adı");
            Fatura_DGV.Columns.Add("Column3", "Adet");
            Fatura_DGV.Columns.Add("Column4", "Fiyat - ₺");
            Fatura_DGV.Columns.Add("Column5", "Toplam");
        }

        // SQL bağlantısı
        private SqlConnection baglanti = new SqlConnection(@"Data Source=YUNUS-EREN\SQLEXPRESS;Initial Catalog=PharmatDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=true");

        // Bugünün tarihini alıp gösteren fonksiyon
        private void Bugun()
        {
            string tarih = DateTime.Now.ToString("dd/MM/yyyy");
            tarihLbl.Text = tarih;
        }

        // Formu temizleyen fonksiyon
        private void Temizle()
        {
            IlacAdi_Tb.Clear();
            IlacAdet_Tb.Clear();
            IlacFiyat_Tb.Clear();
            MusteriTC_Tb.Clear();
            M_ADS_Tb.Clear();

            Fatura_DGV.Rows.Clear();

            toplamTutar = 0;
            toplamTutar_Lbl.Text = "0 ₺";
        }

        // Form yüklendiğinde çalışan fonksiyon
        private void SatisYap_Load(object sender, EventArgs e)
        {
            this.ilaclarTblTableAdapter.Fill(this.pharmatDBDataSet1.IlaclarTbl);
        }

        // İlaç fiyatını veritabanından çeken fonksiyon
        private decimal IlacFiyatGetir(int ilacId)
        {
            decimal ilacFiyat = 0;
            try
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("SELECT IlacFiyat FROM IlaclarTbl WHERE IlacID = @IlacID", baglanti);
                komut.Parameters.AddWithValue("@IlacID", ilacId);

                SqlDataReader dr = komut.ExecuteReader();
                if (dr.Read())
                {
                    ilacFiyat = Convert.ToDecimal(dr["IlacFiyat"]);
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
            return ilacFiyat;
        }

        // İlaç stok bilgisini veritabanından çeken fonksiyon
        private int StokGetir(int ilacId)
        {
            int ilacStok = 0;
            try
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("SELECT IlacAdet FROM IlaclarTbl WHERE IlacID = @IlacID", baglanti);
                komut.Parameters.AddWithValue("@IlacID", ilacId);

                SqlDataReader dr = komut.ExecuteReader();
                if (dr.Read())
                {
                    ilacStok = Convert.ToInt32(dr["IlacAdet"]);
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
            return ilacStok;
        }

        // İlaç stokunu güncelleyen fonksiyon
        private void StokGuncelle(int ilacId, int yeniStok)
        {
            try
            {
                baglanti.Open();
                SqlCommand komut = new SqlCommand("UPDATE IlaclarTbl SET IlacAdet = @IlacAdet WHERE IlacID = @IlacID", baglanti);
                komut.Parameters.AddWithValue("@IlacAdet", yeniStok);
                komut.Parameters.AddWithValue("@IlacID", ilacId);

                komut.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }

        // Ekle butonuna tıklanınca çalışan fonksiyon
        private void Ekle_Butonu_Click(object sender, EventArgs e)
        {
            // Gerekli alanların doldurulup doldurulmadığını kontrol eden kısım
            if (string.IsNullOrEmpty(IlacAdi_Tb.Text) || string.IsNullOrEmpty(IlacAdet_Tb.Text))
            {
                MessageBox.Show("İlaç adı ve adet girilmelidir!", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
            }
            else
            {
                // Kullanıcıdan istenilen ilaç miktarı ve mevcut stok durumu
                int istenenAdet = Convert.ToInt32(IlacAdet_Tb.Text);
                int mevcutStok = StokGetir(ilacId);

                // Yeterli stok olup olmadığını kontrol eden kısım
                if (istenenAdet > mevcutStok)
                {
                    MessageBox.Show("Yeterli stok yok!", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                    return;
                }

                // İlaç fiyatını ve toplam tutarı hesaplayan kısım
                decimal fiyat = IlacFiyatGetir(ilacId);
                decimal toplam = istenenAdet * fiyat;

                // Bu kısım, DataGridView'deki hücre sayısını kontrol eder ve doğruysa yeni bir satır ekler.
                int cellCount = Fatura_DGV.Columns.Count;
                if (cellCount == 5)
                {
                    // Yeni bir DataGridViewRow oluşturulur ve hücreleri doldurulur.
                    DataGridViewRow newRow = new DataGridViewRow();
                    newRow.CreateCells(Fatura_DGV);
                    newRow.Cells[0].Value = Fatura_DGV.Rows.Count + 1;
                    newRow.Cells[1].Value = IlacAdi_Tb.Text;
                    newRow.Cells[2].Value = istenenAdet;
                    newRow.Cells[3].Value = fiyat;
                    newRow.Cells[4].Value = toplam;
                    Fatura_DGV.Rows.Add(newRow);
                    // Toplam tutarı günceller ve arayüzü yeniler.
                    toplamTutar += toplam; // toplamtutar += toplam; (decimal türünde)
                    toplamTutar_Lbl.Text = toplamTutar + " ₺";
                    StokGuncelle(ilacId, mevcutStok - istenenAdet);
                    IlacYukle();
                }
                else
                {
                    // Hücre sayısı beklendiği gibi değilse kullanıcıya bir uyarı gösterilir.
                    MessageBox.Show("Hücre sayısı girilen değerden farklı!", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                }
            }
        }

        // İlaç seçim veri tablosundaki bir hücreye tıklandığında tetiklenen olay.
        private void IlacSec_DGV_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Satır endeksini kontrol et.
            {
                DataGridViewRow row = IlacSec_DGV.Rows[e.RowIndex]; // Seçili satırı al.
                IlacAdi_Tb.Text = row.Cells[1].Value.ToString(); // İlaç adını ilgili metin kutusuna yaz.
                ilacId = Convert.ToInt32(row.Cells[0].Value); // İlaç ID'sini al ve integer'a dönüştür.
                IlacFiyatiniGetir(ilacId); // İlaç fiyatını getiren metod çağrısını yap.
            }
        }

        // Belirli bir ilacın fiyatını veritabanından alarak ilgili metin kutusuna yazan metod.
        private void IlacFiyatiniGetir(int ilacId)
        {
            try
            {
                baglanti.Open(); // Veritabanı bağlantısını aç.

                SqlCommand komut = new SqlCommand("SELECT IlacFiyat FROM IlaclarTbl WHERE IlacID = @IlacID", baglanti); // Fiyatı almak için SQL sorgusu hazırla.
                komut.Parameters.AddWithValue("@IlacID", ilacId); // Parametre ekleyerek SQL sorgusunu güvenli hale getir.

                SqlDataReader dr = komut.ExecuteReader(); // Sorguyu çalıştır ve sonucu SqlDataReader ile oku.

                if (dr.Read()) // Eğer bir sonuç varsa
                {
                    IlacFiyat_Tb.Text = dr["IlacFiyat"].ToString(); // İlaç fiyatını ilgili metin kutusuna yaz.
                }
                dr.Close(); // Okuyucuyu kapat.
            }
            catch (Exception ex) // Herhangi bir hata durumunda
            {
                MessageBox.Show(ex.Message); // Hata mesajını göster.
            }
            finally // Her durumda
            {
                baglanti.Close(); // Veritabanı bağlantısını kapat.
            }
        }

        // İlaçları yükleyen metod.
        private void IlacYukle()
        {
            this.ilaclarTblTableAdapter.Fill(this.pharmatDBDataSet1.IlaclarTbl); // IlaclarTbl tablosunu dolduran metod.
        }


        // Belirli bir müşterinin adını ve soyadını veritabanından alarak ilgili metin kutusuna yazan metod.
        private void MusteriBilgileriniGetir(string musteriTC)
        {
            string query = "SELECT MusteriAd, MusteriSoyad FROM MusteriTbl WHERE MusteriTC = @MTC"; // SQL sorgusunu hazırla.

            SqlCommand command = new SqlCommand(query, baglanti); // Bağlantı ve sorgu için SqlCommand nesnesi oluştur.
            command.Parameters.AddWithValue("@MTC", musteriTC); // Parametre ekleyerek SQL sorgusunu güvenli hale getir.

            try
            {
                baglanti.Open(); // Veritabanı bağlantısını aç.

                SqlDataReader reader = command.ExecuteReader(); // Sorguyu çalıştır ve sonucu SqlDataReader ile oku.

                if (reader.Read()) // Eğer bir sonuç varsa
                {
                    string ad = reader["MusteriAd"].ToString(); // MusteriAd sütunundaki değeri al.
                    string soyad = reader["MusteriSoyad"].ToString(); // MusteriSoyad sütunundaki değeri al.
                    M_ADS_Tb.Text = $"{ad} {soyad}"; // İlgili metin kutusuna ad ve soyadı yaz.
                }
                else // Eğer sonuç yoksa
                {
                    M_ADS_Tb.Text = "Müşteri Bulunamadı"; // İlgili metin kutusuna hata mesajını yaz.
                }

                reader.Close(); // Okuyucuyu kapat.
            }
            catch // Herhangi bir hata durumunda
            {
                // Hata yönetimi burada yapılabilir.
            }
            finally // Her durumda
            {
                baglanti.Close(); // Veritabanı bağlantısını kapat.
            }
        }


        // Müşteri TC alanında herhangi bir değişiklik olduğunda tetiklenen olay.
        private void MusteriTC_Tb_TextChanged_1(object sender, EventArgs e)
        {
            string musteriTC = MusteriTC_Tb.Text; // Müşteri TC bilgisini al.
            if (musteriTC.Length == 11) // TC uzunluğu 11 ise
            {
                MusteriBilgileriniGetir(musteriTC); // Müşteri bilgilerini getir.
            }
        }

        // Yazdırma butonuna tıklandığında tetiklenen olay.
        private void Yazdir_Butonu_Click(object sender, EventArgs e)
        {
            FaturaKaydet(); // Faturayı kaydet.
            printDocument1.DefaultPageSettings.PaperSize = new PaperSize("pprnm", 285, 600); // Sayfa boyutunu ayarla.
            printPreviewDialog1.Document = printDocument1; // Yazdırma önizleme belgesini belirle.
            if (printPreviewDialog1.ShowDialog() == DialogResult.OK) // Önizleme onaylandıysa
            {
                printDocument1.Print(); // Belgeyi yazdır.
            }
            Temizle(); // Temizleme işlemi yap.
        }

        // Geçerli bir çalışan ID almak için kullanılan yardımcı metod.
        private int GetValidCalisanId()
        {
            int calisanId = -1; // Başlangıçta geçersiz bir çalışan ID değeri atanır.

            try
            {
                SqlCommand komut = new SqlCommand("SELECT TOP 1 CalisanID FROM CalisanTbl", baglanti); // SQL sorgusu hazırla.
                SqlDataReader dr = komut.ExecuteReader(); // Sorguyu çalıştır ve sonucu oku.
                if (dr.Read()) // Eğer sonuç varsa
                {
                    calisanId = Convert.ToInt32(dr["CalisanID"]); // CalisanID değerini al.
                }
                dr.Close(); // Okuyucuyu kapat.
            }
            catch (Exception ex) // Hata durumunda
            {
                MessageBox.Show("Çalışan ID alınırken hata oluştu: " + ex.Message); // Kullanıcıya hata mesajı göster.
            }

            return calisanId; // CalisanID değerini geri döndür.
        }


        // Bu metod, fatura verilerini veritabanına kaydetmek için kullanılır.
        private void FaturaKaydet()
        {
            try
            {
                baglanti.Open(); // Bağlantı açılır.

                int calisanId = GetValidCalisanId(); // Geçerli bir çalışan ID alınır.
                if (calisanId == -1) // Geçerli bir ID bulunamazsa hata mesajı gösterilir.
                {
                    MessageBox.Show("Geçerli bir çalışan ID bulunamadı.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Metottan çıkılır.
                }

                // DataGridView içindeki satırlar tek tek işlenir.
                foreach (DataGridViewRow row in Fatura_DGV.Rows)
                {
                    if (row.IsNewRow) continue; // Yeni bir satır ise atlanır.

                    string musteriTC = MusteriTC_Tb.Text; // Müşteri TC alınır.
                    string musteriAd = M_ADS_Tb.Text.Split(' ')[0]; // Müşteri adı alınır.
                    string musteriSoyad = M_ADS_Tb.Text.Split(' ')[1]; // Müşteri soyadı alınır.
                    DateTime faturaTarih = DateTime.Now; // Fatura tarihi şu anki zaman olarak belirlenir.
                    float faturaMiktar = float.Parse(row.Cells[4].Value.ToString()); // Fatura miktarı alınır.

                    // Veritabanına fatura bilgilerini eklemek için SQL komutu hazırlanır.
                    SqlCommand komut = new SqlCommand("INSERT INTO SatisTbl (C_ID, M_ID, M_Ad, M_Soyad, FaturaTarih, FaturaMiktar) VALUES (@C_ID, @M_ID, @M_Ad, @M_Soyad, @FaturaTarih, @FaturaMiktar)", baglanti);
                    komut.Parameters.AddWithValue("@C_ID", calisanId); // Parametreler atanır.
                    komut.Parameters.AddWithValue("@M_ID", musteriTC);
                    komut.Parameters.AddWithValue("@M_Ad", musteriAd);
                    komut.Parameters.AddWithValue("@M_Soyad", musteriSoyad);
                    komut.Parameters.AddWithValue("@FaturaTarih", faturaTarih);
                    komut.Parameters.AddWithValue("@FaturaMiktar", faturaMiktar);

                    komut.ExecuteNonQuery(); // SQL komutu çalıştırılır.
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda kullanıcıya hata mesajı gösterilir.
                MessageBox.Show("Veri kaydedilemedi: " + ex.Message, "Pharmat - Sistem Mesajı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                baglanti.Close(); // Bağlantı kapatılır.
            }
        }


        // Bu metod, printDocument1 nesnesi tarafından tetiklenen PrintPage olayını işler.
        private void printDocument1_PrintPage_1(object sender, PrintPageEventArgs e)
        {
            int yPos = 10; // Başlangıç yüksekliği belirlenir.

            // Başlık ve alt başlıklar yazdırılır.
            e.Graphics.DrawString("PHARMAT", new Font("Montserrat", 12, FontStyle.Bold), Brushes.Black, new PointF(10, yPos));
            yPos += 40;
            e.Graphics.DrawString("Pharmacy Management System", new Font("Montserrat", 10, FontStyle.Bold), Brushes.Black, new PointF(10, yPos));
            yPos += 40;

            // Tarih ve müşteri bilgileri yazdırılır.
            e.Graphics.DrawString("Tarih: " + DateTime.Now.ToString("dd/MM/yyyy"), new Font("Montserrat", 8, FontStyle.Regular), Brushes.Black, new PointF(10, yPos));
            yPos += 20;
            e.Graphics.DrawString("Müşteri: " + M_ADS_Tb.Text, new Font("Montserrat", 8, FontStyle.Regular), Brushes.Black, new PointF(10, yPos));
            yPos += 20;

            // Tablonun başlığı yazdırılır.
            e.Graphics.DrawString("Numara    İlaç Adı    Adet    Fiyat    Toplam", new Font("Montserrat", 8, FontStyle.Bold), Brushes.Black, new PointF(10, yPos));
            yPos += 20;

            // DataGridView'deki satırlar tek tek yazdırılır.
            foreach (DataGridViewRow row in Fatura_DGV.Rows)
            {
                if (row.IsNewRow) continue; // Yeni bir satır ise atlanır.

                // Satırın değerleri alınır.
                string numara = row.Cells[0].Value.ToString();
                string ilacAdi = row.Cells[1].Value.ToString();
                string adet = row.Cells[2].Value.ToString();
                string fiyat = row.Cells[3].Value.ToString();
                string toplam = row.Cells[4].Value.ToString();

                // Satırın formatı oluşturulur ve yazdırılır.
                string line = $"{numara.PadRight(8)} {ilacAdi.PadRight(15)} {adet.PadRight(5)} {fiyat.PadRight(6)} {toplam.PadRight(8)}";
                e.Graphics.DrawString(line, new Font("Montserrat", 8, FontStyle.Regular), Brushes.Black, new PointF(10, yPos));
                yPos += 20;
            }

            // Toplam tutar yazdırılır.
            e.Graphics.DrawString("Toplam Tutar: " + toplamTutar.ToString("C"), new Font("Montserrat", 8, FontStyle.Bold), Brushes.Black, new PointF(10, yPos));
        }


        private void IlacAdet_Tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void label21_Click(object sender, EventArgs e) // Özet
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

        private void label20_Click(object sender, EventArgs e) // İlaçlar
        {
            İlaçlar formIlac = new İlaçlar(label2.Text);
            formIlac.Show();
            this.Hide();
        }

        private void label19_Click(object sender, EventArgs e) // Müşteriler
        {
            Müşteriler formMusteri = new Müşteriler(label2.Text);
            formMusteri.Show();
            this.Hide();
        }

        private void label18_Click(object sender, EventArgs e) // Üreticiler
        {
            Üreticiler formUretici = new Üreticiler(label2.Text);
            formUretici.Show();
            this.Hide();
        }

        private void label17_Click(object sender, EventArgs e) // Çıkış Yap
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
                SatisYap formSatis = new SatisYap(label2.Text);
                formSatis.Show();
                this.Hide();
            }
        }
    }
}