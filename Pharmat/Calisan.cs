using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Pharmat
{
    public class Calisan
    {
        // Calisan sınıfının özellikleri
        public string CalisanID { get; set; }
        public string CalisanAdi { get; set; }
        public string CalisanSoyadi { get; set; }
        public string CalisanCinsiyet { get; set; }
        public string CalisanTelefon { get; set; }
        public DateTime CalisanDTarihi { get; set; }
        public string CalisanSifre { get; set; }

        // SQL bağlantı dizesi
        private static SqlConnection baglanti = new SqlConnection(@"Data Source=YUNUS-EREN\SQLEXPRESS;Initial Catalog=PharmatDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=true");

        // Yeni bir çalışanı veritabanına eklemek için kullanılan metod
        public void Ekle()
        {
            // Gerekli alanların kontrolü
            if (string.IsNullOrEmpty(CalisanAdi) || string.IsNullOrEmpty(CalisanSifre))
            {
                throw new ArgumentException("Lütfen boş alanları doldurunuz.");
            }
            else
            {
                try
                {
                    // Bağlantıyı aç
                    baglanti.Open();

                    // Yeni bir çalışan eklemek için SQL komutu
                    SqlCommand komut = new SqlCommand("INSERT INTO CalisanTbl(CalisanAd, CalisanSoyad, CalisanCinsiyet, CalisanTelefon, CalisanDTarihi, CalisanSifresi) VALUES(@CA, @CS, @CC, @CT, @CDT, @CSifre)", baglanti);
                    // SQL komutuna parametre ekle
                    komut.Parameters.AddWithValue("@CA", CalisanAdi);
                    komut.Parameters.AddWithValue("@CS", CalisanSoyadi);
                    komut.Parameters.AddWithValue("@CC", CalisanCinsiyet);
                    komut.Parameters.AddWithValue("@CT", CalisanTelefon);
                    komut.Parameters.AddWithValue("@CDT", CalisanDTarihi);
                    komut.Parameters.AddWithValue("@CSifre", CalisanSifre);

                    // Komutu çalıştır
                    int rowsAffected = komut.ExecuteNonQuery();
                    // Eklemenin başarılı olup olmadığını kontrol et
                    if (rowsAffected <= 0)
                    {
                        throw new Exception("Çalışan eklenirken bir hata oluştu.");
                    }
                }
                finally
                {
                    // Bağlantıyı kapat
                    baglanti.Close();
                }
            }
        }

        // Varolan bir çalışanı güncellemek için kullanılan metod
        public void Guncelle()
        {
            // Gerekli alanların kontrolü
            if (string.IsNullOrEmpty(CalisanID) || string.IsNullOrEmpty(CalisanAdi) || string.IsNullOrEmpty(CalisanSoyadi) || string.IsNullOrEmpty(CalisanTelefon) || string.IsNullOrEmpty(CalisanSifre) || string.IsNullOrEmpty(CalisanCinsiyet))
            {
                throw new ArgumentException("Lütfen boş alanları doldurunuz.");
            }
            else
            {
                try
                {
                    // Bağlantıyı aç
                    baglanti.Open();

                    // Varolan bir çalışanı güncellemek için SQL komutu
                    SqlCommand komut = new SqlCommand("UPDATE CalisanTbl SET CalisanAd = @CA, CalisanSoyad = @CS, CalisanCinsiyet = @CC, CalisanTelefon = @CT, CalisanDTarihi = @CDT, CalisanSifresi = @CSifre WHERE CalisanID = @CID", baglanti);
                    // SQL komutuna parametre ekle
                    komut.Parameters.AddWithValue("@CID", CalisanID);
                    komut.Parameters.AddWithValue("@CA", CalisanAdi);
                    komut.Parameters.AddWithValue("@CS", CalisanSoyadi);
                    komut.Parameters.AddWithValue("@CC", CalisanCinsiyet);
                    komut.Parameters.AddWithValue("@CT", CalisanTelefon);
                    komut.Parameters.AddWithValue("@CDT", CalisanDTarihi);
                    komut.Parameters.AddWithValue("@CSifre", CalisanSifre);

                    // Komutu çalıştır
                    int rowsAffected = komut.ExecuteNonQuery();
                    // Güncellemenin başarılı olup olmadığını kontrol et
                    if (rowsAffected <= 0)
                    {
                        throw new Exception("Çalışan bilgileri güncellenemedi. Lütfen tekrar deneyin.");
                    }
                }
                finally
                {
                    // Bağlantıyı kapat
                    baglanti.Close();
                }
            }
        }

        // Bir çalışanı veritabanından silmek için kullanılan metod
        public void Sil()
        {
            // Gerekli alanların kontrolü
            if (string.IsNullOrEmpty(CalisanID))
            {
                throw new ArgumentException("Lütfen silinecek çalışanın ID'sini girin.");
            }
            else
            {
                try
                {
                    // Bağlantıyı aç
                    baglanti.Open();

                    // Bir çalışanı silmek için SQL komutu
                    SqlCommand komut = new SqlCommand("DELETE FROM CalisanTbl WHERE CalisanID = @CID", baglanti);
                    // SQL komutuna parametre ekle
                    komut.Parameters.AddWithValue("@CID", CalisanID);

                    // Komutu çalıştır
                    int rowsAffected = komut.ExecuteNonQuery();
                    // Silmenin başarılı olup olmadığını kontrol et
                    if (rowsAffected <= 0)
                    {
                        throw new Exception("Çalışan silinemedi. Lütfen tekrar deneyin.");
                    }
                }
                finally
                {
                    // Bağlantıyı kapat
                    baglanti.Close();
                }
            }
        }

        // Doğum tarihine göre yaş hesaplayan metod
        public static int YasHesapla(DateTime dogumTarihi)
        {
            int yas = DateTime.Now.Year - dogumTarihi.Year;
            if (DateTime.Now < dogumTarihi.AddYears(yas)) yas--;
            return yas;
        }

        // Veritabanından tüm çalışanları yükleyen metod
        public static List<Calisan> Yukle()
        {
            List<Calisan> calisanlar = new List<Calisan>();

            try
            {
                // Bağlantıyı aç
                baglanti.Open();

                // Tüm çalışanları seçmek için SQL komutu
                SqlCommand komut = new SqlCommand("SELECT * FROM CalisanTbl", baglanti);
                // Komutu çalıştır ve veri okuyucuyu al
                SqlDataReader reader = komut.ExecuteReader();

                // Tüm satırları oku ve Calisan nesneleri oluştur
                while (reader.Read())
                {
                    Calisan calisan = new Calisan
                    {
                        CalisanID = reader["CalisanID"].ToString(),
                        CalisanAdi = reader["CalisanAd"].ToString(),
                        CalisanSoyadi = reader["CalisanSoyad"].ToString(),
                        CalisanCinsiyet = reader["CalisanCinsiyet"].ToString(),
                        CalisanTelefon = reader["CalisanTelefon"].ToString(),
                        CalisanDTarihi = DateTime.Parse(reader["CalisanDTarihi"].ToString()),
                        CalisanSifre = reader["CalisanSifresi"].ToString()
                    };
                    // Calisan nesnesini listeye ekle
                    calisanlar.Add(calisan);
                }
                reader.Close();
            }
            finally
            {
                // Bağlantıyı kapat
                baglanti.Close();
            }

            return calisanlar;
        }
    }
}
