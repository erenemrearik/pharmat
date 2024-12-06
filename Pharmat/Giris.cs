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
    public partial class Giris : Form
    {
        public Giris()
        {
            InitializeComponent();
            Bugun();
        }

        // Bugün tarihini ekrana yazdırmak için kullanılan metod
        private void Bugun()
        {
            string tarih = DateTime.Now.ToString("dd/MM/yyyy");
            label22.Text = tarih;
        }

        // Giriş yap butonuna tıklama olayını işleyen metod
        private void GirisYap_Butonu_Click(object sender, EventArgs e)
        {
            string baglanti = @"Data Source=YUNUS-EREN\SQLEXPRESS;Initial Catalog=PharmatDB;Integrated Security=True;Encrypt=True;TrustServerCertificate=true";
            string adSoyad = Ad_Soyad_Tb.Text;
            string sifre = Sifre_Tb.Text;

            // Ad ve soyadı ayırma işlemi
            string[] adSoyadArray = adSoyad.Split(' ');
            if (adSoyadArray.Length < 2)
            {
                MessageBox.Show("Lütfen ad ve soyadı arasında bir boşluk bırakarak girin.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                return;
            }
            string ad = adSoyadArray[0];
            string soyad = adSoyadArray[1];

            // Veritabanı bağlantısı oluşturma ve açma
            using (SqlConnection con = new SqlConnection(baglanti))
            {
                try
                {
                    con.Open();

                    // Kullanıcı adı ve şifreyi kontrol eden SQL sorgusu
                    string sorgu = "SELECT COUNT(1) FROM CalisanTbl WHERE CalisanAd = @ad AND CalisanSoyad = @soyad AND CalisanSifresi = @sifre";
                    SqlCommand cmd = new SqlCommand(sorgu, con);
                    cmd.Parameters.AddWithValue("@ad", ad);
                    cmd.Parameters.AddWithValue("@soyad", soyad);
                    cmd.Parameters.AddWithValue("@sifre", sifre);

                    // Sorgu sonucunu sayısal olarak alma
                    int sayac = Convert.ToInt32(cmd.ExecuteScalar());

                    if (sayac == 1)
                    {
                        // Giriş başarılıysa kullanıcıya mesaj gösterme ve çalışan paneline yönlendirme
                        MessageBox.Show("Giriş Başarılı! İyi çalışmalar " + ad + " " + soyad, "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                        ÇalışanPanel formCalisan = new ÇalışanPanel(adSoyad);
                        formCalisan.Show();
                        this.Hide();
                    }
                    else
                    {
                        // Giriş başarısızsa kullanıcıya hata mesajı gösterme
                        MessageBox.Show("Kullanıcı adı veya şifre hatalı.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
                    }
                }
                catch (Exception ex)
                {
                    // Herhangi bir hata oluşursa hata mesajı gösterme
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // Şifreyi göster/gizle checkbox'ının durumu değiştiğinde çalışan metod
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                Sifre_Tb.PasswordChar = '\0'; // Şifreyi göster
            }
            else
            {
                Sifre_Tb.PasswordChar = '*'; // Şifreyi gizle
            }
        }

        // Yönetici giriş butonuna tıklama olayını işleyen metod
        private void button1_Click(object sender, EventArgs e)
        {
            YöneticiGiriş formYonetici = new YöneticiGiriş();
            formYonetici.Show();
            this.Hide();
        }
    }
}
