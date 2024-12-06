using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pharmat
{
    public partial class YöneticiGiriş : Form
    {
        public YöneticiGiriş()
        {
            InitializeComponent();
            Bugun();
        }
        private void Bugun()
        {
            string tarih = DateTime.Now.ToString("dd/MM/yyyy");
            label22.Text = tarih;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                Sifre_Tb.PasswordChar = '\0';
            }
            else
            {
                Sifre_Tb.PasswordChar = '*';
            }
        }

        private void GirisYap_Butonu_Click(object sender, EventArgs e) // Yönetici Kullanıcı Adı: yonetici Şifre: yonetici123
        {
            string yonetici = YKullaniciAdi_Tb.Text;

            // Yönetici kullanıcı adı ve şifresinin doğru olup olmadığını kontrol etme
            if (YKullaniciAdi_Tb.Text == "yonetici" && Sifre_Tb.Text == "yonetici123")
            {
                // Eğer kullanıcı adı ve şifre doğruysa yönetici paneline yönlendirme
                Panel formYonetici = new Panel(yonetici);
                formYonetici.Show();
                this.Hide();
            }
            else
            {
                // Eğer kullanıcı adı veya şifre hatalıysa hata mesajı gösterme
                MessageBox.Show("Kullanıcı adı veya şifre hatalı.", "Pharmat - Sistem Mesajı", MessageBoxButtons.OK);
            }
        }

        private void button1_Click(object sender, EventArgs e) // Çalışan Girişi
        {
            // Çalışan giriş formunu açma
            Giris formCalisanGiris = new Giris();
            formCalisanGiris.Show();
            this.Hide();
        }

    }
}
