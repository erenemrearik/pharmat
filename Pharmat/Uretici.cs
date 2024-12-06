using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace Pharmat
{
    public class Uretici
    {
        public int UreticiID { get; set; } // Üretici ID
        public string UreticiAdi { get; set; } // Üretici adı
        public string UreticiAdresi { get; set; } // Üretici adresi
        public string UreticiTelefon { get; set; } // Üretici telefonu
        public DateTime UreticiKTarihi { get; set; } // Üretici kayıt tarihi

        // Veritabanı bağlantısı
        private static SqlConnection baglanti = new SqlConnection(@"Data Source=YUNUS-EREN\SQLEXPRESS;Initial Catalog=PharmatDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=true");

        // Tüm üreticileri veritabanından getirir
        public static List<Uretici> GetUreticiler()
        {
            List<Uretici> ureticiler = new List<Uretici>();
            try
            {
                baglanti.Open(); // Veritabanı bağlantısını açar
                SqlCommand komut = new SqlCommand("SELECT * FROM UreticiTbl", baglanti);
                SqlDataReader reader = komut.ExecuteReader();
                while (reader.Read())
                {
                    Uretici uretici = new Uretici
                    {
                        UreticiID = (int)reader["UreticiID"],
                        UreticiAdi = reader["UreticiAdi"].ToString(),
                        UreticiAdresi = reader["UreticiAdresi"].ToString(),
                        UreticiTelefon = reader["UreticiTelefon"].ToString(),
                        UreticiKTarihi = (DateTime)reader["UreticiKTarihi"]
                    };
                    ureticiler.Add(uretici); // Üreticiyi listeye ekler
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message); // Hata mesajı gösterir
            }
            finally
            {
                baglanti.Close(); // Veritabanı bağlantısını kapatır
            }
            return ureticiler; // Üretici listesini döner
        }

        // Yeni üretici ekler
        public void Ekle()
        {
            try
            {
                baglanti.Open(); // Veritabanı bağlantısını açar
                SqlCommand komut = new SqlCommand("INSERT INTO UreticiTbl (UreticiAdi, UreticiAdresi, UreticiTelefon, UreticiKTarihi) OUTPUT INSERTED.UreticiID VALUES (@UA, @UAD, @UT, @UKT)", baglanti);
                komut.Parameters.AddWithValue("@UA", UreticiAdi);
                komut.Parameters.AddWithValue("@UAD", UreticiAdresi);
                komut.Parameters.AddWithValue("@UT", UreticiTelefon);
                komut.Parameters.AddWithValue("@UKT", UreticiKTarihi);

                UreticiID = (int)komut.ExecuteScalar(); // Yeni eklenen üreticinin ID'sini alır
                MessageBox.Show("Üretici başarıyla eklendi. Üretici ID: " + UreticiID, "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message); // Hata mesajı gösterir
            }
            finally
            {
                baglanti.Close(); // Veritabanı bağlantısını kapatır
            }
        }

        // Üreticiyi günceller
        public void Guncelle()
        {
            try
            {
                baglanti.Open(); // Veritabanı bağlantısını açar
                SqlCommand komut = new SqlCommand("UPDATE UreticiTbl SET UreticiAdi = @UA, UreticiAdresi = @UAD, UreticiTelefon = @UT, UreticiKTarihi = @UKT WHERE UreticiID = @UID", baglanti);
                komut.Parameters.AddWithValue("@UA", UreticiAdi);
                komut.Parameters.AddWithValue("@UAD", UreticiAdresi);
                komut.Parameters.AddWithValue("@UT", UreticiTelefon);
                komut.Parameters.AddWithValue("@UKT", UreticiKTarihi);
                komut.Parameters.AddWithValue("@UID", UreticiID);

                int rowsAffected = komut.ExecuteNonQuery(); // Etkilenen satır sayısını alır

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Üretici başarıyla güncellendi.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Üretici güncellenemedi. Lütfen tekrar deneyin.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message); // Hata mesajı gösterir
            }
            finally
            {
                baglanti.Close(); // Veritabanı bağlantısını kapatır
            }
        }

        // Üreticiyi siler
        public void Sil()
        {
            try
            {
                baglanti.Open(); // Veritabanı bağlantısını açar
                SqlCommand komut = new SqlCommand("DELETE FROM UreticiTbl WHERE UreticiID = @UID", baglanti);
                komut.Parameters.AddWithValue("@UID", UreticiID);

                int rowsAffected = komut.ExecuteNonQuery(); // Etkilenen satır sayısını alır

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Üretici başarıyla silindi.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Üretici silinemedi. Lütfen tekrar deneyin.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message); // Hata mesajı gösterir
            }
            finally
            {
                baglanti.Close(); // Veritabanı bağlantısını kapatır
            }
        }
    }
}
