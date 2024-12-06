using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Pharmat
{
    // Üreticiler sınıfı, üretici bilgilerini tutar.
    public class Ureticiler
    {
        public int UreticiID { get; set; }
        public string UreticiAdi { get; set; }
    }

    // İlaç sınıfı, ilaç bilgilerini tutar ve veritabanı işlemlerini gerçekleştirir.
    public class Ilac
    {
        public int IlacID { get; set; }
        public string IlacIsim { get; set; }
        public int IlacAdet { get; set; }
        public int IlacFiyat { get; set; }
        public DateTime IlacSKT { get; set; }
        public string IlacTuru { get; set; }
        public int IlacUreticiID { get; set; }

        // Veritabanı bağlantısı için gerekli bağlantı dizesi.
        private SqlConnection baglanti = new SqlConnection(@"Data Source=YUNUS-EREN\SQLEXPRESS;Initial Catalog=PharmatDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=true");

        // Veritabanından üretici bilgilerini getirir.
        public List<Uretici> UreticileriGetir()
        {
            List<Uretici> ureticiler = new List<Uretici>();
            try
            {
                baglanti.Open();  // Veritabanı bağlantısını açar.
                using (SqlCommand komut = new SqlCommand("SELECT UreticiID, UreticiAdi FROM UreticiTbl", baglanti))
                {
                    using (SqlDataReader reader = komut.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Her bir üretici bilgisini listeye ekler.
                            ureticiler.Add(new Uretici
                            {
                                UreticiID = reader.GetInt32(reader.GetOrdinal("UreticiID")),
                                UreticiAdi = reader.GetString(reader.GetOrdinal("UreticiAdi"))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);  // Hata durumunda mesaj gösterir.
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();  // Veritabanı bağlantısını kapatır.
                }
            }
            return ureticiler;
        }

        // Veritabanından ilaç bilgilerini getirir.
        public List<Ilac> IlaclariGetir()
        {
            List<Ilac> ilaclar = new List<Ilac>();
            try
            {
                baglanti.Open();  // Veritabanı bağlantısını açar.
                using (SqlCommand komut = new SqlCommand("SELECT IlacID, IlacIsim, IlacAdet, IlacFiyat, IlacSKT, IlacTuru, IlacUreticiID FROM IlaclarTbl", baglanti))
                {
                    using (SqlDataReader reader = komut.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Her bir ilaç bilgisini listeye ekler.
                            ilaclar.Add(new Ilac
                            {
                                IlacID = reader.GetInt32(0),
                                IlacIsim = reader.GetString(1),
                                IlacAdet = reader.GetInt32(2),
                                IlacFiyat = reader.GetInt32(3),
                                IlacSKT = reader.GetDateTime(4),
                                IlacTuru = reader.GetString(5),
                                IlacUreticiID = reader.GetInt32(6)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);  // Hata durumunda mesaj gösterir.
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();  // Veritabanı bağlantısını kapatır.
                }
            }
            return ilaclar;
        }

        // Yeni bir ilaç ekler.
        public void Ekle()
        {
            try
            {
                baglanti.Open();  // Veritabanı bağlantısını açar.
                using (SqlCommand komut = new SqlCommand("INSERT INTO IlaclarTbl (IlacIsim, IlacAdet, IlacFiyat, IlacSKT, IlacTuru, IlacUreticiID) OUTPUT INSERTED.IlacID VALUES (@IA, @IAD, @IF, @ISKT, @IT, @IUID)", baglanti))
                {
                    // Parametreleri komuta ekler.
                    komut.Parameters.AddWithValue("@IA", IlacIsim);
                    komut.Parameters.AddWithValue("@IAD", IlacAdet);
                    komut.Parameters.AddWithValue("@IF", IlacFiyat);
                    komut.Parameters.AddWithValue("@ISKT", IlacSKT);
                    komut.Parameters.AddWithValue("@IT", IlacTuru);
                    komut.Parameters.AddWithValue("@IUID", IlacUreticiID);

                    IlacID = (int)komut.ExecuteScalar();  // Yeni eklenen ilacın ID'sini alır.
                }

                MessageBox.Show("İlaç başarıyla eklendi. İlaç ID: " + IlacID, "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);  // Başarı mesajı gösterir.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);  // Hata durumunda mesaj gösterir.
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();  // Veritabanı bağlantısını kapatır.
                }
            }
        }

        // Var olan bir ilacı günceller.
        public void Guncelle()
        {
            try
            {
                baglanti.Open();  // Veritabanı bağlantısını açar.
                using (SqlCommand komut = new SqlCommand("UPDATE IlaclarTbl SET IlacIsim = @IA, IlacAdet = @IAD, IlacFiyat = @IF, IlacSKT = @ISKT, IlacTuru = @IT, IlacUreticiID = @IUID WHERE IlacID = @IID", baglanti))
                {
                    // Parametreleri komuta ekler.
                    komut.Parameters.AddWithValue("@IA", IlacIsim);
                    komut.Parameters.AddWithValue("@IAD", IlacAdet);
                    komut.Parameters.AddWithValue("@IF", IlacFiyat);
                    komut.Parameters.AddWithValue("@ISKT", IlacSKT);
                    komut.Parameters.AddWithValue("@IT", IlacTuru);
                    komut.Parameters.AddWithValue("@IUID", IlacUreticiID);
                    komut.Parameters.AddWithValue("@IID", IlacID);

                    int rowsAffected = komut.ExecuteNonQuery();  // Etkilenen satır sayısını alır.

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("İlaç başarıyla güncellendi.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);  // Başarı mesajı gösterir.
                    }
                    else
                    {
                        MessageBox.Show("İlaç güncellenemedi. Lütfen tekrar deneyin.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);  // Başarısızlık mesajı gösterir.
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);  // Hata durumunda mesaj gösterir.
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();  // Veritabanı bağlantısını kapatır.
                }
            }
        }

        // Var olan bir ilacı siler.
        public void Sil()
        {
            try
            {
                baglanti.Open();  // Veritabanı bağlantısını açar.
                using (SqlCommand komut = new SqlCommand("DELETE FROM IlaclarTbl WHERE IlacID = @IID", baglanti))
                {
                    // Parametreyi komuta ekler.
                    komut.Parameters.AddWithValue("@IID", IlacID);

                    int rowsAffected = komut.ExecuteNonQuery();  // Etkilenen satır sayısını alır.

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("İlaç başarıyla silindi.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);  // Başarı mesajı gösterir.
                    }
                    else
                    {
                        MessageBox.Show("İlaç silinemedi. Lütfen tekrar deneyin.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);  // Başarısızlık mesajı gösterir.
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);  // Hata durumunda mesaj gösterir.
            }
            finally
            {
                if (baglanti.State == ConnectionState.Open)
                {
                    baglanti.Close();  // Veritabanı bağlantısını kapatır.
                }
            }
        }
    }
}
