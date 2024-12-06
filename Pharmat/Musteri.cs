using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pharmat
{
    // Musteri sınıfı, müşterinin özelliklerini ve veritabanı işlemlerini tanımlar
    public class Musteri
    {
        public int MusteriID { get; set; } // Müşteri ID'si
        public string MusteriAd { get; set; } // Müşteri adı
        public string MusteriSoyad { get; set; } // Müşteri soyadı
        public string MusteriCinsiyet { get; set; } // Müşteri cinsiyeti
        public string MusteriTelefon { get; set; } // Müşteri telefon numarası
        public DateTime MusteriDTarihi { get; set; } // Müşteri doğum tarihi
        public string MusteriTC { get; set; } // Müşteri TC kimlik numarası

        // Veritabanı bağlantısı için SqlConnection nesnesi
        private static SqlConnection baglanti = new SqlConnection(@"Data Source=YUNUS-EREN\SQLEXPRESS;Initial Catalog=PharmatDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=true");

        // Müşteri ekleme metodu
        public void Ekle(List<Musteri> musteriler)
        {
            // Müşteri adı veya TC kimlik numarası boşsa uyarı göster
            if (string.IsNullOrEmpty(MusteriAd) || string.IsNullOrEmpty(MusteriTC))
            {
                MessageBox.Show("Lütfen boş alanları doldurunuz.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                return;
            }

            try
            {
                baglanti.Open(); // Veritabanı bağlantısını aç
                // Müşteri eklemek için SQL komutu
                SqlCommand komut = new SqlCommand("INSERT INTO MusteriTbl(MusteriAd, MusteriSoyad, MusteriCinsiyet, MusteriTelefon, MusteriDTarihi, MusteriTC) VALUES(@MA, @MS, @MC, @MT, @MDT, @MTC)", baglanti);
                // Parametreleri ekle
                komut.Parameters.AddWithValue("@MA", MusteriAd);
                komut.Parameters.AddWithValue("@MS", MusteriSoyad);
                komut.Parameters.AddWithValue("@MC", MusteriCinsiyet);
                komut.Parameters.AddWithValue("@MT", MusteriTelefon);
                komut.Parameters.AddWithValue("@MDT", MusteriDTarihi.Date);
                komut.Parameters.AddWithValue("@MTC", MusteriTC);

                int rowsAffected = komut.ExecuteNonQuery(); // SQL komutunu çalıştır
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Müşteri başarıyla eklendi.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                    musteriler.Add(this); // Müşteriyi listeye ekle
                }
                else
                {
                    MessageBox.Show("Müşteri eklenirken bir hata oluştu.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                }

                baglanti.Close(); // Veritabanı bağlantısını kapat
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Müşteri güncelleme metodu
        public void Guncelle(List<Musteri> musteriler)
        {
            // TC kimlik numarası boşsa uyarı göster
            if (string.IsNullOrWhiteSpace(MusteriTC))
            {
                MessageBox.Show("Lütfen bilgisi güncellenecek müşteriyi seçin.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                return;
            }

            // Gerekli alanlar boşsa uyarı göster
            if (string.IsNullOrEmpty(MusteriAd) || string.IsNullOrEmpty(MusteriSoyad) || string.IsNullOrEmpty(MusteriTC) || MusteriCinsiyet == null)
            {
                MessageBox.Show("Lütfen boş alanları doldurunuz.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                return;
            }

            try
            {
                baglanti.Open(); // Veritabanı bağlantısını aç
                // Müşteri güncellemek için SQL komutu
                SqlCommand komut = new SqlCommand("UPDATE MusteriTbl SET MusteriAd = @MA, MusteriSoyad = @MS, MusteriCinsiyet = @MC, MusteriTelefon = @MT WHERE MusteriID = @MID", baglanti);
                // Parametreleri ekle
                komut.Parameters.AddWithValue("@MID", MusteriID);
                komut.Parameters.AddWithValue("@MA", MusteriAd);
                komut.Parameters.AddWithValue("@MS", MusteriSoyad);
                komut.Parameters.AddWithValue("@MC", MusteriCinsiyet);
                komut.Parameters.AddWithValue("@MT", MusteriTelefon);
                komut.Parameters.AddWithValue("@MDT", MusteriDTarihi.Date);
                komut.Parameters.AddWithValue("@MTC", MusteriTC);

                int rowsAffected = komut.ExecuteNonQuery(); // SQL komutunu çalıştır
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Müşteri bilgileri başarıyla güncellendi.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                    // Müşteri listesinde güncellenen müşteriyi bul ve bilgilerini güncelle
                    var musteri = musteriler.FirstOrDefault(m => m.MusteriID == MusteriID);
                    if (musteri != null)
                    {
                        musteri.MusteriAd = MusteriAd;
                        musteri.MusteriSoyad = MusteriSoyad;
                        musteri.MusteriCinsiyet = MusteriCinsiyet;
                        musteri.MusteriTelefon = MusteriTelefon;
                        musteri.MusteriDTarihi = MusteriDTarihi;
                    }
                }
                else
                {
                    MessageBox.Show("Müşteri bilgileri güncellenemedi. Lütfen tekrar deneyin.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                }

                baglanti.Close(); // Veritabanı bağlantısını kapat
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Müşteri silme metodu
        public void Sil(List<Musteri> musteriler)
        {
            // Müşteri ID'si geçersizse uyarı göster
            if (MusteriID <= 0)
            {
                MessageBox.Show("Lütfen bilgisi silinecek müşteriyi seçin.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                return;
            }

            try
            {
                baglanti.Open(); // Veritabanı bağlantısını aç
                // Müşteri silmek için SQL komutu
                SqlCommand komut = new SqlCommand("DELETE FROM MusteriTbl WHERE MusteriID = @MID", baglanti);
                komut.Parameters.AddWithValue("@MID", MusteriID);

                int rowsAffected = komut.ExecuteNonQuery(); // SQL komutunu çalıştır
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Müşteri başarıyla silindi.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                    // Müşteri listesinde silinen müşteriyi bul ve listeden çıkar
                    var musteri = musteriler.FirstOrDefault(m => m.MusteriID == MusteriID);
                    if (musteri != null)
                    {
                        musteriler.Remove(musteri);
                    }
                }
                else
                {
                    MessageBox.Show("Müşteri silinemedi. Lütfen tekrar deneyin.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                }

                baglanti.Close(); // Veritabanı bağlantısını kapat
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if (baglanti.State == System.Data.ConnectionState.Open)
                {
                    baglanti.Close(); // Veritabanı bağlantısını kapat
                }
            }
        }

        // Yaş hesaplama metodu
        public static int YasHesapla(DateTime dogumTarihi)
        {
            int yas = DateTime.Now.Year - dogumTarihi.Year; // Yıl farkını hesapla
            if (DateTime.Now < dogumTarihi.AddYears(yas)) yas--; // Doğum günü henüz gelmediyse yaşı bir azalt
            return yas; 
        }
    }
}
