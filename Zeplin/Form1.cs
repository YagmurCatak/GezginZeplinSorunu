using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace Zeplin
{
    public partial class Form1 : Form
    {
        struct table
        {
            public double maliyet;
            public int onceki_sehir;
        };

        table[] maliyetDizisi = new table[81];
        List<int> visited = new List<int>();
        List<int> unvisited = new List<int>();
        double topmesafe = 0.0;
        double encokKar = 0.0;
        int[] longitudeForGraf = new int[81];
        int[] latitudeForGraf = new int[81];

        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            double[] edilenKar = new double[81];
            List<double> kisisayisiKar = new List<double>();
            List<string> yolCheck = new List<string>();

            double[] ucret = new double[81];
            List<double> kisiSayisiUcreti = new List<double>();
            List<string> yolVarYok = new List<string>();

            int retvalue = 0;
            for (int j = 0; j < 81; j++)
            {
                unvisited.Add(j);
                maliyetDizisi[j].maliyet = double.MaxValue;
                maliyetDizisi[j].onceki_sehir = -1;
            }

            if (radioButton1.Checked)
            {
                for (int i = 5; i <= 50; i++)
                {
                    SehirleriCizdir();
                    Double[,] komsulukMatrisi = komsulukgraf(i);
                    YollariCizdir(komsulukMatrisi);

                    double enkisayol = 0.0;
                    int baslangicSehri = Convert.ToInt32(txtbulSehir.Text);
                    int bitisSehri = Convert.ToInt32(txtGidilcekSehir.Text);


                    maliyetDizisi[baslangicSehri - 1].maliyet = 0;
                    retvalue = dijkstra(komsulukMatrisi, baslangicSehri - 1, bitisSehri - 1);
                    if (retvalue == -1)
                    {
                        yolCheck.Add("yolbulunamadi");
                        kisisayisiKar.Add(0);
                    }
                    else
                    {
                        listBox3.Items.Add(bitisSehri.ToString());
                        enkisayol = enkisayolYazdir(baslangicSehri - 1, bitisSehri - 1);
                        edilenKar[i] = sbtUcretFazlaKar(enkisayol, i);
                        yolCheck.Add("yolbulundu");
                        kisisayisiKar.Add(edilenKar[i]);
                    }
                }
                int index = 0;

                for (int count = 5; count <= 50; count++)
                {
                    if (encokKar < edilenKar[count])
                    {
                        encokKar = edilenKar[count];
                        index++;
                    }
                }

                for (int i = 5; i <= 50; i++)
                {
                    listBox1.Items.Add(i + "kisi icin " + yolCheck[i - 5]);
                    listBox2.Items.Add(i + "kisi icin kar miktari = " + kisisayisiKar[i - 5]);
                    label1.Text = index.ToString() + "kisi ile kar eder.";
                }

                
            }

            else
            {
                for (int kisisayisi = 10; kisisayisi <= 50; )
                {
                    SehirleriCizdir();
                    Double[,] komsulukMatrisi = komsulukgraf(kisisayisi);
                    YollariCizdir(komsulukMatrisi);

                    double enkisayol = 0.0;
                    int baslangicSehri = Convert.ToInt32(txtbulSehir.Text);
                    int bitisSehri = Convert.ToInt32(txtGidilcekSehir.Text);


                    maliyetDizisi[baslangicSehri - 1].maliyet = 0;
                    retvalue = dijkstra(komsulukMatrisi, baslangicSehri - 1, bitisSehri - 1);
                    if (retvalue == -1)
                    {
                        yolVarYok.Add("yolbulunamadi");
                        kisiSayisiUcreti.Add(0);
                    }
                    else
                    {
                        listBox3.Items.Add(bitisSehri.ToString());
                        enkisayol = enkisayolYazdir(baslangicSehri - 1, bitisSehri - 1);
                        ucret[kisisayisi] = yolcuSayisiKar(enkisayol, kisisayisi);
                        yolVarYok.Add("yolbulundu");
                        kisiSayisiUcreti.Add(ucret[kisisayisi]);
                    }
                    kisisayisi = kisisayisi + 10;
                }
                int index = 0;
                for (int i = 0; i <5; i++ )
                {
                    listBox1.Items.Add((index+10) + "kisi icin " + yolVarYok[i]);
                    listBox2.Items.Add((index+10) + "kisinin ödemesi gereken = " + kisiSayisiUcreti[i]);
                    index = index + 10;                 
                }
            }
        }

        private void YollariCizdir(Double[,] graf)
        {
            Graphics g = this.groupBox1.CreateGraphics();
            Pen pen = new Pen(Color.Blue);
            for (int i = 0; i < 81; i++)
            {
                for (int j = 0; j < 81; j++)
                {
                    if (graf[i, j] != 0)
                    {
                        if (graf[j, i] != 0)
                        {
                            pen = new Pen(Color.Red, 2);
                        }
                        else if (i < j)
                        {
                            pen = new Pen(Color.Green, 2);
                        }
                        else
                        {
                            pen = new Pen(Color.Blue, 2);
                        }

                        g.DrawLine(pen, longitudeForGraf[i] + 10, latitudeForGraf[i] + 10, longitudeForGraf[j] + 10, latitudeForGraf[j] + 10);
                    }
                }
            }
        }

        public String[] latlongBilgiErisim(int Plaka)
        {
            StreamReader srlongLat = new StreamReader("C:\\Users\\Yagmur\\Desktop\\ZeplinSorunu\\latlong.txt");

            //Satir satir okuma işlemini yaparak, bir dizide tutuyoruz. 
            String full = srlongLat.ReadToEnd();
            String[] rows = full.Split('\n');
            //ayırdığımız satır satır bilgileri virgüllere göre bölerek tek tek verileri çekmiş oldu
            String[] cols = rows[Plaka].Split(',');

            return cols;
        }

        public void SehirleriCizdir()
        {
            Graphics g = this.groupBox1.CreateGraphics();

            Pen pen = new Pen(Color.Red);
            Font drawFont = new Font("Arial", 8);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            double latCarpan = (42 - 35) / 750.00;
            double longCarpan = (46 - 25) / 1000.00;

            for (int i = 1; i < 82; i++)
            {
                String[] cols = latlongBilgiErisim(i);
                double latitude = Convert.ToDouble(cols[0]);
                double longitude = Convert.ToDouble(cols[1]);
                longitude = longitude / Math.Pow(10, 4);
                latitude = latitude / Math.Pow(10, 4);
                int lat = Convert.ToInt32((43 - latitude) / latCarpan);
                int log = Convert.ToInt32((longitude - 25) / longCarpan);

                longitudeForGraf[i - 1] = log;
                latitudeForGraf[i - 1] = lat;

                g.DrawEllipse(pen, log, lat, 20, 20);
                g.DrawString(i.ToString(), drawFont, drawBrush, log, lat);

            }
        }

        public double ikiSehirArasiMesafe(String[] ilkSehir, String[] ikinciSehir)
        {
            double long1, long2, lat1, lat2;
            double toplat, toplong;
            double mesafe, a, c;
            double R = 6373.0;

            long1 = Convert.ToDouble(ilkSehir[1]);
            lat1 = Convert.ToDouble(ilkSehir[0]);

            long2 = Convert.ToDouble(ikinciSehir[1]);
            lat2 = Convert.ToDouble(ikinciSehir[0]);

            //dereceyi radyana çevirme 
            double rlong1;
            rlong1 = long1 / Math.Pow(10, 4) * Math.PI / 180;

            double rlog2;
            rlog2 = long2 / Math.Pow(10, 4) * Math.PI / 180;

            double rlat1;
            rlat1 = lat1 / Math.Pow(10, 4) * Math.PI / 180;

            double rlat2;
            rlat2 = lat2 / Math.Pow(10, 4) * Math.PI / 180;

            //lat-long mesafe hesabi
            toplat = rlat2 - rlat1;
            toplong = rlog2 - rlong1;
            a = Math.Pow(Math.Sin(toplat / 2), 2) + Math.Cos(rlat1) * Math.Cos(rlat2) * Math.Pow(Math.Sin(toplong / 2), 2);
            c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            mesafe = R * c;

            return mesafe;
        }

        public double rakimHesabi(String[] ASehir, String[] BSehir)
        {
            //Rakim hesabi 
            double rakim_A, rakim_B, rakimfark, yeni_rakim;
            rakim_A = Convert.ToDouble(ASehir[3]);
            rakim_B = Convert.ToDouble(BSehir[3]);
            rakimfark = rakim_B - rakim_A;
            yeni_rakim = rakimfark + 50;

            return yeni_rakim;
        }

        public Boolean ucusEgimAcisiUcusKontrolu(double rakim, double mesafe, int yolcu_sayisi)
        {
            //egim hesabi 
            double radyan_egim, egimici, derece_egim;
            egimici = rakim / mesafe;
            radyan_egim = Math.Atan2(egimici, 1);
            derece_egim = Math.Round(radyan_egim * 180 / Math.PI);

            //yolcu sayisina göre aci(max50 kişi min5 kişi)
            int farkaci = 50 - yolcu_sayisi;
            int ucusacisi = 30 + farkaci; //30 = min derece 

            if (derece_egim <= ucusacisi && derece_egim >= 30 && derece_egim <= 75)
            {
                /*label5.Text = egim.ToString();
                label1.Text = "ucus olur";*/
                return true;
            }
            else
            {
                /*label5.Text = egim.ToString();
                label1.Text = "ucus olmaz";*/
                return false;
            }
        }

        public double[,] komsulukgraf(int yolcusayisi)
        {
            String[] sutun = new String[20];
            StreamWriter graflar = new StreamWriter("C:\\Users\\Yagmur\\Desktop\\ZeplinSorunu\\graf.txt");
            StreamWriter mesafedegerleri = new StreamWriter("C:\\Users\\Yagmur\\Desktop\\ZeplinSorunu\\ikisehirarasimesafe.txt");
            StreamReader srKomsuluk = new StreamReader("C:\\Users\\Yagmur\\Desktop\\ZeplinSorunu\\komsuluklar.txt");

            //Satir satir okuma işlemini yaparak, bir dizide tutuyoruz. 
            String komsuluktamami = srKomsuluk.ReadToEnd();
            String[] satir = komsuluktamami.Split('\n');

            //okunan komsuluk degerlerine göre dügüm hesaplanacak ve graf matrisinde tutulacak. 
            double[,] graf = new double[81, 81];

            //graf matrisine ilk deger olarak 0 degerini atıyoruz. 
            for (int i = 0; i < 81; i++)
            {
                for (int j = 0; j < 81; j++)
                {
                    graf[i, j] = 0;
                }
            }

            // okunan komsuluklara göre graf matrisimiz dolduruldu. 
            for (int i = 0; i < satir.Length; i++)
            {
                sutun = satir[i].Split(',');
                for (int j = 1; j < sutun.Length - 1; j++)
                {
                    String[] Asehri = latlongBilgiErisim(Convert.ToInt32(sutun[0]));
                    String[] Bsehri = latlongBilgiErisim(Convert.ToInt32(sutun[j]));
                    double mesafe = ikiSehirArasiMesafe(Asehri, Bsehri);
                    mesafedegerleri.Write(Asehri[2] + "-" + Bsehri[2] + "arasi mesafe=" + mesafe);
                    mesafedegerleri.WriteLine();
                    double rakim = rakimHesabi(Asehri, Bsehri);
                    Boolean ucuskontrol = ucusEgimAcisiUcusKontrolu(rakim, mesafe, yolcusayisi);

                    if (ucuskontrol == true)
                    {
                        graf[i, Convert.ToInt32(sutun[j]) - 1] = mesafe;
                        //label1.Text = graf[i, j].ToString();
                    }
                    else
                        graf[i, Convert.ToInt32(sutun[j]) - 1] = 0;
                }

            }

            for (int i = 0; i < 81; i++)
            {
                for (int j = 0; j < 81; j++)
                {
                    graflar.Write(graf[i, j]);
                    
                }
                graflar.WriteLine();
            }

            graflar.Close();
            mesafedegerleri.Close();
            return graf;
        }

        public int dijkstra(double[,] graf, int baslangicSehri, int bitisSehri)
        {
            double maliyet = 0;

            visited.Add(baslangicSehri);
            unvisited.Remove(baslangicSehri);

            for (int i = 0; i < 81; i++)
            {
                if (graf[baslangicSehri, i] != 0)
                {
                    if (!visited.Contains(i))
                    {
                        maliyet = maliyetDizisi[baslangicSehri].maliyet + graf[baslangicSehri, i];

                        if (maliyetDizisi[i].maliyet >= maliyet)
                        {
                            maliyetDizisi[i].maliyet = maliyet;
                            maliyetDizisi[i].onceki_sehir = baslangicSehri;
                        }
                    }
                }
            }

            if (baslangicSehri == bitisSehri)
            {
                // Console.Write("enkisa yol aramasi sonlandı. ");
                return 1;
            }
            else
            {
                int endusuk = endusukmaliyet();
                if (endusuk == -1)
                    return -1;
                dijkstra(graf, endusuk, bitisSehri);
            }

            return 1;
        }

        public int endusukmaliyet()
        {
            double enkucuk = double.MaxValue;
            int j = 0;
            int index = -1;

            for (j = 0; j < 81; j++)
            {
                if ((maliyetDizisi[j].maliyet < enkucuk) && (!visited.Contains(j)))
                {
                    enkucuk = maliyetDizisi[j].maliyet;
                    index = j;
                }
            }
            return index;
        }

        double enkisayolYazdir(int baslangic, int bitis)
        {

            if ((baslangic != bitis) && (maliyetDizisi[bitis].onceki_sehir != -1))
            {
                topmesafe = topmesafe + maliyetDizisi[bitis].maliyet;
                listBox3.Items.Add(maliyetDizisi[bitis].onceki_sehir + 1);
                enkisayolYazdir(baslangic, maliyetDizisi[bitis].onceki_sehir);

            }
            else
            {
                label6.Text = ("Top km = " + topmesafe);
            }

            return topmesafe;
        }

        public double sbtUcretFazlaKar(double gidilenKm, int kisisayisi)
        {
            double karMiktari = 0.0;
            int sbucret = 20;
            int gazparasi = 1000;
            //sbucret 20, benzin parasi 100km= 1000
            karMiktari = (sbucret * kisisayisi) - (gidilenKm * gazparasi / 100);
            return karMiktari;
        }

        public double yolcuSayisiKar(double gidilenKm, int kisisayisi)
        {
            int gazParasi = 1000;
            double ucret = 0.0;
            double kazanc = 0.0;
            kazanc = (gidilenKm * gazParasi / 100) + ((gidilenKm * gazParasi / 100) / 2);
            ucret = kazanc / kisisayisi;
            return ucret;
        }
    }
}
