using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;

using Microsoft.Win32;
using System.Security.Cryptography;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Collections;



namespace RSA_ELGAMAL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class DemoRSAElGamal : Window
    {
        public DemoRSAElGamal()
        {
            InitializeComponent();
            rd_td.IsChecked = true;
            btMaHoa.IsEnabled = false;
            btGiaiMa.IsEnabled = false;
            //rd_tdRSA.IsChecked = true;
            //rd_tcRSA.IsChecked = false;
            //rsa_maHoaBanRoMoi.IsEnabled = false;            

        }
        #region code mã hóa elgamal
        private void rd_tc_Checked(object sender, RoutedEventArgs e)
        {
            So_A.IsEnabled = So_D.IsEnabled = So_K.IsEnabled = So_P.IsEnabled = So_X.IsEnabled = So_Y.IsEnabled = true;
            bt_taoKhoa.Content = "Tạo khóa tùy chọn";
            reset();
            bt_taoKhoa.IsEnabled = true;
            //So_A.Text = So_D.Text = so_K.Text = So_P.Text = So_X.Text = so_Y.Text = string.Empty;
        }

        private void rd_td_Checked(object sender, RoutedEventArgs e)
        {
            So_A.IsEnabled = So_D.IsEnabled = So_K.IsEnabled = So_P.IsEnabled = So_X.IsEnabled = So_Y.IsEnabled = false;
            reset();
            bt_taoKhoa.Content = "Tạo khóa ngẫu nhiên";
            bt_taoKhoa.IsEnabled = true;
            // So_A.Text = So_D.Text = so_K.Text = So_P.Text = So_X.Text = so_Y.Text = string.Empty;
        }

        private void bt_taoKhoamoi_Click(object sender, RoutedEventArgs e)
        {
            reset();
            // So_A.Text = So_D.Text = so_K.Text = So_P.Text = So_X.Text = so_Y.Text = string.Empty;
        }

        private void reset()
        {
            So_A.Text = So_D.Text = So_K.Text = So_P.Text = So_X.Text = So_Y.Text = string.Empty;
        }

        public int EsoP, EsoQ, E_So_G_A, EsoA, EsoX, EsoD, EsoK, EsoY;
        public int danhDau = 0;
        private int E_ChonSoNgauNhien()
        {
            Random rdE = new Random();
            return rdE.Next(1000, 3000);// tốc độ chậm nên chọn số bé
        }
        private bool E_kiemTraNguyenTo(int so_kt)
        {
            bool kiemtra = true;
            if (so_kt == 2 || so_kt == 3)
            {
                // kiemtra = true;
                return kiemtra;
            }
            else
            {
                if (so_kt == 1 || so_kt % 2 == 0 || so_kt % 3 == 0)
                {
                    kiemtra = false;
                }
                else
                {
                    for (int i = 5; i <= Math.Sqrt(so_kt); i = i + 6)
                        if (so_kt % i == 0 || so_kt % (i + 2) == 0)
                        {
                            kiemtra = false;
                            break;
                        }
                }
            }
            return kiemtra;
        }  //"Hàm kiểm tra nguyên tố"
        private bool E_kiemTraUocCuaSoP(int so_P, int so_Q)
        {
            bool kt_Okie = true;
            if ((so_P - 1) % so_Q == 0)
            {
                kt_Okie = true;
            }
            else
                kt_Okie = false;
            return kt_Okie;
        }
        private bool E_kiemTraPTSinh(int so_kt, int E_SoP_, int E_soQ_)// kiem tra phan tu sinh
        {
            bool ktOkie = true;
            int soMu = E_SoP_ - 1 / E_soQ_;
            int ketQuaKT = E_LuyThuaModulo_(so_kt, soMu, E_SoP_);

            if (ketQuaKT != 1)
            {
                ktOkie = true;
            }
            else
            {
                if (ketQuaKT == 1) ktOkie = false;
            }
            return ktOkie;
        }
        private bool nguyenToCungNhau(int ai, int bi)// "Hàm kiểm tra hai số nguyên tố cùng nhau"
        {
            bool ktx_;
            // giải thuật Euclid;
            int temp;
            while (bi != 0)
            {
                temp = ai % bi;
                ai = bi;
                bi = temp;
            }
            if (ai == 1) { ktx_ = true; }
            else ktx_ = false;
            return ktx_;
        }
        private string TapP_1(int soDauVao)
        {
            string ChuoiDauRa = string.Empty;
            for (int i = 1; i < soDauVao; i++)
            {
                if (nguyenToCungNhau(soDauVao, i) == true)
                { ChuoiDauRa += i.ToString() + "#"; }
            }
            return ChuoiDauRa;
        }
        // Find the all factors of Ø  {f1,f2,….,fn} – { 1 }
        private string Tap_Qi(int soDauvao) // tìm các số khi phân tích ra thừa số của số P
        {
            string ChuoiDauRa = string.Empty;
            int soix = 2;
            while (soDauvao != 1)
            {
                if (soDauvao % soix == 0)
                {
                    ChuoiDauRa += soix.ToString() + "#";
                    soDauvao = soDauvao / soix;
                }
                else soix++;
            }
            return ChuoiDauRa;
        }
        public int E_LuyThuaModulo_(int CoSo_, int SoMu_, int soModulo_)
        {

            //Sử dụng thuật toán "bình phương nhân"
            //Chuyển e sang hệ nhị phân
            int[] a = new int[100];
            int k = 0;
            do
            {
                a[k] = SoMu_ % 2;
                k++;
                SoMu_ = SoMu_ / 2;
            }
            while (SoMu_ != 0);
            //Quá trình lấy dư
            int kq = 1;

            for (int i = k - 1; i >= 0; i--)
            {
                kq = (kq * kq) % soModulo_;
                if (a[i] == 1)
                    kq = (kq * CoSo_) % soModulo_;
            }
            return kq;
        }

        //Ví dụ: x=  y2(y1a )-1 mod  p  = 133.(394^109)^-1 mod 569  =257
        private int E_tinhModulo_nghichdao(int SoNCNDn, int SoMdlm)
        {
            int kd = SoMdlm;
            int r = 1, q, y0 = 0, y1 = 1, y = 0;
            while (SoNCNDn != 0)
            {
                r = SoMdlm % SoNCNDn;
                if (r == 0)
                    break;
                else
                {
                    q = SoMdlm / SoNCNDn;
                    y = y0 - y1 * q;
                    SoMdlm = SoNCNDn;
                    SoNCNDn = r;
                    y0 = y1;
                    y1 = y;
                }
            }
            if (y >= 0)
                return y;
            else
            {
                y = kd + y;
                return y;
            }
        }
        private int E_TinhC1muxModP(int SoC1, int SomuX, int somDLP)
        {
            int kq_E_TinhC1muxModP = 1;
            for (int i = 0; i <= SomuX; i++)
            {
                kq_E_TinhC1muxModP = kq_E_TinhC1muxModP * E_tinhModulo_nghichdao(SoC1, somDLP);
            }
            return kq_E_TinhC1muxModP;
        }
        private void TaoKhoa_click()
        {
            EsoQ = E_So_G_A = EsoA = EsoX = EsoD = EsoK = 0;

            // chọn số nguyên tố ngẫu nhiên Q thỏa mãn Q là ước của P - 1;
            do
            {
                Random rdQ = new Random();
                EsoQ = rdQ.Next(2, EsoP - 1);
            }
            while (!E_kiemTraNguyenTo(EsoP) || !E_kiemTraUocCuaSoP(EsoP, EsoQ));
            // tìm số G để tìm số A (A là phần tử sinh): 
            do
            {
                Random rdE_So_G_A = new Random();
                E_So_G_A = rdE_So_G_A.Next(2, EsoP - 1);
            }
            while (!E_kiemTraPTSinh(E_So_G_A, EsoP, EsoQ));

            EsoA = E_LuyThuaModulo_(E_So_G_A, EsoP - 1 / EsoQ, EsoP); // phần tử sinh

            do
            {
                Random rdEsoX = new Random();
                EsoX = rdEsoX.Next(2, EsoP - 2);
            }
            while (EsoX == EsoQ || EsoX == E_So_G_A);
            // d= a^x mod P
            EsoD = E_LuyThuaModulo_(EsoA, EsoX, EsoP);// beta; d          
            do
            {
                Random rdEsoK = new Random();
                EsoK = rdEsoK.Next(2, EsoP - 2);
            }
            while (EsoK == EsoX || EsoK == EsoA || EsoK == EsoQ || EsoK == E_So_G_A || !nguyenToCungNhau(EsoK, EsoP - 1));
            // Tính Y = A^k mod p - Khóa công khai
            EsoY = E_LuyThuaModulo_(EsoA, EsoK, EsoP);
        }

        private void bt_taoKhoa_Click(object sender, RoutedEventArgs e)
        {


            if (rd_td.IsChecked == true && rd_tc.IsChecked == false)
            {
                // thực hiện thao tác tạo khóa ngẫu nhiên 
                reset();

                // chọn số nguyên tố ngẫu nhiên P
                EsoP = 0;
                do
                {
                    EsoP = E_ChonSoNgauNhien();
                }
                while (E_kiemTraNguyenTo(EsoP) == false);

                TaoKhoa_click();
                So_P.Text = EsoP.ToString();
                So_A.Text = EsoA.ToString();
                So_X.Text = EsoX.ToString();
                So_D.Text = EsoD.ToString();
                So_K.Text = EsoK.ToString();
                So_Y.Text = EsoY.ToString();
                bt_taoKhoa.Content = "Tạo khóa ngẫu nhiên mới";

            }
            else
            {
                if (rd_td.IsChecked == false && rd_tc.IsChecked == true)//(rd_tudongchon_.Checked == false && rd_tuychon_.Checked == true)
                {
                    // thực hiện thao tác tạo khóa tùy chọn 
                    if (So_P.Text == "")
                    {
                        MessageBox.Show("Phải nhập số P ", "Thông Báo ", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        EsoP = int.Parse(So_P.Text);
                        if (E_kiemTraNguyenTo(EsoP) == false)
                        {
                            MessageBox.Show("Phải chọn P là số nguyên tố ", "Thông Báo ", MessageBoxButton.OK, MessageBoxImage.Error);
                            So_P.Focus();
                        }
                        else
                            if (EsoP < 1000)
                            {
                                MessageBox.Show("Số P quá nhỏ! Nhập số khác ", "Thông Báo ", MessageBoxButton.OK, MessageBoxImage.Error);
                                So_P.Focus();
                            }
                            else
                            {
                                TaoKhoa_click();
                                So_P.Text = EsoP.ToString();
                                So_A.Text = EsoA.ToString();
                                So_X.Text = EsoX.ToString();
                                So_D.Text = EsoD.ToString();
                                So_K.Text = EsoK.ToString();
                                So_Y.Text = EsoY.ToString();

                                bt_taoKhoa.IsEnabled = false;
                                //bt_taokhoaTuychonMoi.Visible = true;
                            }
                    }
                }
            }
            danhDau = 1;
            btMaHoa.IsEnabled = true;
        }
        public string E_MaHoa(string ChuoiVao)
        {
            //Chuyen xau thanh ma Unicode         

            byte[] mhE_temp1 = Encoding.Unicode.GetBytes(ChuoiVao);
            string base64 = Convert.ToBase64String(mhE_temp1);

            // Chuyển xâu thành mã Unicode dạng số          
            int[] mh_temp2 = new int[base64.Length];
            for (int i = 0; i < base64.Length; i++)
            {
                mh_temp2[i] = (int)base64[i];
                //txtm1.Text += mh_temp2[i].ToString() + "#";
            }

            //txt_ChuoimaBanRo.Text = chuoi(mh_temp2);            
            //Mảng a chứa các kí tự sẽ  mã hóa
            int[] mh_temp3 = new int[mh_temp2.Length];
            // thực hiện mã hóa: z = (d^k * m ) mod p

            for (int i = 0; i < mh_temp2.Length; i++)
            {
                mh_temp3[i] = ((mh_temp2[i] % EsoP) * (E_LuyThuaModulo_(EsoD, EsoK, EsoP))) % EsoP;
                //txtm2.Text += mh_temp3[i].ToString()+"#";
            }
            //Chuyển sang kiểu kí tự trong bảng mã Unicode
            string str = "";
            for (int i = 0; i < mh_temp3.Length; i++)
            {
                str = str + (char)mh_temp3[i];
                // txtm3.Text = (char)mh_temp3[i] + "#";
            }
            byte[] E_data1 = Encoding.Unicode.GetBytes(str);
            string BanMaHoa = Convert.ToBase64String(E_data1);
            return BanMaHoa;

        }
        public string E_GiaiMa(string ChuoiVao)
        {
            //Chuyen xau thanh ma Unicode       
            string BanGiaiMa = "";

            byte[] Egm_temp1 = Convert.FromBase64String(ChuoiVao);
            string Egm_giaima = Encoding.Unicode.GetString(Egm_temp1);

            int[] Eb = new int[Egm_giaima.Length];
            for (int i = 0; i < Egm_giaima.Length; i++)
            {
                Eb[i] = (int)Egm_giaima[i];

            }
            //Giải mã
            //   m = ( r * z ) mod p =((r mod p) * (z mod p))mod p  with r = y^(p-1-x) mod p

            int[] Ec = new int[Eb.Length];
            int sor = E_LuyThuaModulo_(EsoY, (EsoP - (1 + EsoX)), EsoP);
            //txtm7.Text = sor.ToString();
            for (int i = 0; i < Ec.Length; i++)
            {
                Ec[i] = (Eb[i] * sor) % EsoP;// giải mã

            }
            string str = "";
            for (int i = 0; i < Ec.Length; i++)
            {
                str = str + (char)Ec[i];
            }
            byte[] data2 = Convert.FromBase64String(str);
            BanGiaiMa = Encoding.Unicode.GetString(data2);
            return BanGiaiMa;
        }

        private void btMaHoa_Click(object sender, RoutedEventArgs e)
        {
            if (danhDau != 1)
            {
                MessageBox.Show("Bạn chưa chọn khóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (txtBanRo.Text == string.Empty)
                {
                    MessageBox.Show("Bạn chưa nhập chuỗi cần mã hóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (danhDau == 1)
                    {
                        txt_maHoaBanRo.Text = string.Empty;

                        string Egm_chuoiMaHoa = E_MaHoa(txtBanRo.Text);
                        txt_maHoaBanRo.Text = Egm_chuoiMaHoa;
                        txt_banMaHoaNhanDuoc.Text = Egm_chuoiMaHoa;
                        danhDau = 2;
                        btMaHoa.IsEnabled = false;
                        btGiaiMa.IsEnabled = true;
                    }
                }
            }
        }
        private void btGiaiMa_Click(object sender, RoutedEventArgs e)
        {

            if (danhDau != 2)
            {
                MessageBox.Show("Bạn chưa chọn tệp giải mã!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (danhDau == 2)
            {
                txt_banGiaima.Text = string.Empty;
                txt_banGiaima.Text = E_GiaiMa(txt_banMaHoaNhanDuoc.Text);
            }
            danhDau = 1;
            btMaHoa.IsEnabled = false;
            btGiaiMa.IsEnabled = false;
        }

        private void btThoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btTaoBanRoMoi_Click(object sender, RoutedEventArgs e)
        {
            txtBanRo.Text = txt_maHoaBanRo.Text = txt_banMaHoaNhanDuoc.Text = txt_banGiaima.Text = string.Empty;
            btMaHoa.IsEnabled = true;
        }

        #endregion
        private void el_soP_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "." && IsNumber(e.Text) == false)
            {
                e.Handled = true;
            }
            else if (e.Text == ".")
            {
                if (((TextBox)sender).Text.IndexOf(e.Text) > -1)
                {
                    e.Handled = true;
                }
            }
        }

        private bool IsNumber(string Text_x)
        {
            int outPut;
            return int.TryParse(Text_x, out outPut);
        }
     }
}
