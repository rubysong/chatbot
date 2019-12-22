using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace chat2._0
{
    public partial class login : Form
    {
        private Point mousePoint = new Point();//Pour la fenêtre de mouvement de la souris
        private Point formLocation;//Position de la fenêtre
        bool isConnect = false;//Déterminer s'il faut se connecter au serveur
        public chat beginChat;//Fenêtre d'interface principale
        public login()
        {
            InitializeComponent();
            dataProcessing.setlogin(this);//Fournit le fonctionnement des données ultérieures recevant et envoyant les résultats des commentaires d'opération
        }
        //Le formulaire s'initialise et tente de se connecter au serveur
        private void login_Load(object sender, EventArgs e)
        {
            //Image du bouton de chargement
            pictureBox2.Image = imageList1.Images[0];
            //============================
            label3.Parent = pictureBox2;
            label4.Parent = panel1;
            label3.Location = new Point(pictureBox2.Width / 2 - label3.Width / 2, pictureBox2.Height / 2 - label3.Height / 2);
            label4.Location = new Point(panel1.Width - label4.Width, panel1.Height / 2 - 2 - label4.Height / 2);
            if (!dataProcessing.beginWork("login"))
            {
                label7.Text = "La connexion au serveur a échoué......";
            }
        }
        public void setConnect(bool situation)
        {
            isConnect = situation;
        }
        public void setFooterSituation(string s)
        {
            label7.BeginInvoke(new Action(() =>
            {
                label7.Text = s;
            }));
        }
        public void showMessagebox(string s)
        {
            MessageBox.Show(s);
        }
        //==========Événement de clic de bouton==========

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (label3.Text == "Modifier")
            {
                dataProcessing.address(textBox1.Text.Trim(), int.Parse(textBox2.Text.Trim()));
                label7.Text = "Reconnexion au serveur......";
                isConnect = false;
                dataProcessing.beginWork("login");
                label5_Click(sender, e);
                MessageBox.Show("Modification réussie, veuillez vous connecter");
                return;
            }
            if (!isConnect)
            {
                MessageBox.Show("Non connecté au serveur et ne peut pas fonctionner");
                return;
            }
            //Le nom d'utilisateur et le mot de passe ne peuvent pas être vides
            if (textBox1.Text.Trim() == "" || textBox2.Text.Trim() == "")
            {
                label8.Text = "Veuillez saisir votre nom d'utilisateur et votre mot de passe!";
                return;
            }
            //Le nom d'utilisateur et le mot de passe ne peuvent pas contenir "$"
            //ce qui facilite les opérations lors de l'analyse des données
            if (textBox1.Text.Contains('$') || textBox1.Text.Contains('$'))
            {
                MessageBox.Show("Le nom d'utilisateur ou le mot de passe ne peut pas contenir'$'");
                return;
            }
            string[] data = new string[2];
            data[0] = textBox1.Text;//username
            //==============Appelez la propre fonction MD5 du système pour le chiffrement====================
            byte[] result = Encoding.Default.GetBytes(textBox2.Text);    //textBox2 est le mot de passe entré
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            data[1] = BitConverter.ToString(output).Replace("-", "");  //Sortie de texte crypté dans les data[1]
            //Message envoyé lors de l'enregistrement d'un compte
            if (label3.Text == "Signup")
            {
                if (!dataProcessing.sendData(4, data))
                {
                    MessageBox.Show("L'envoi a échoué.");
                    return;
                }
            }
            //Message envoyé lors de la connexion
            else if (!dataProcessing.sendData(0, data))
            {
                MessageBox.Show("L'envoi a échoué.");
                return;
            }
            //==========Recevoir les commentaires du serveur==========
            string[] receiveData = dataProcessing.receiveData();
            if (receiveData == null)
            {
                MessageBox.Show("Une erreur s'est produite lors de la réception des données.");
                return;
            }
            //Code reçu lors de la connexion
            if (receiveData[1] == "NOTEXISTNAME")
            {
                MessageBox.Show("Le nom d'utilisateur n'existe pas, veuillez d'abord vous inscrire");
                dataProcessing.beginWork("login");
                return;
            }
            else if (receiveData[1] == "WRONGPASSWORD")
            {
                MessageBox.Show("Mot de passe incorrect, veuillez ressaisir");
                dataProcessing.beginWork("login");
                return;
            }
            else if (receiveData[1] == "NAMEONLINE")
            {
                MessageBox.Show("Ce compte est déjà connecté, la connexion a échoué");
                return;
            }
            //Reussir
            else if (receiveData[1] == "SUCCESS")
            {
                this.Visible = false;
                beginChat = new chat(textBox1.Text.Trim());
                beginChat.Visible = true;
            }
            else if (receiveData[1] == "SAMENAME")
            {
                MessageBox.Show("Ce username existe déjà, veuillez entrer un autre username pour vous inscrire");
                dataProcessing.beginWork("login");
                return;
            }
            else if (receiveData[1] == "REGISTERSUCCESS")
            {
                MessageBox.Show("Enregistrement du compte réussi, veuillez vous connecter");
                label3.Text = "Login";
                label6.Text = "Signup";
                label5.Visible = true;
                label3.Location = new Point(pictureBox2.Width / 2 - label3.Width / 2, pictureBox2.Height / 2 - label3.Height / 2);
                dataProcessing.beginWork("login");
                return;
            }
        }
        //Cliquez sur le bouton principal
        private void label3_Click(object sender, EventArgs e)
        {
            pictureBox2_Click(sender, e);
        }
        //Bouton d'enregistrement dans le coin inférieur droit
        private void label6_Click(object sender, EventArgs e)
        {
            if (label6.Text == "Signup")
            {
                label3.Text = "Signup";
                label6.Text = "Back";
                label5.Visible = false;
            }
            else
            {
                label3.Text = "Login";
                label6.Text = "Signup";
                label5.Visible = true;
            }
            label3.Location = new Point(pictureBox2.Width / 2 - label3.Width / 2, pictureBox2.Height / 2 - label3.Height / 2);
        }
        //Bouton de modification du serveur dans le coin inférieur gauche
        private void label5_Click(object sender, EventArgs e)
        {
            if (label5.Text == "Modifier l'adresse du serveur")
            {
                label5.Text = "Back";
                label3.Text = "modifier";
                label1.Text = "Adresse：";
                label2.Text = "Port：";
                textBox1.Text = "127.0.0.1";
                textBox2.Text = "8081";
                label6.Visible = false;
            }
            else
            {
                label5.Text = "Modifier l'adresse du serveur";
                label3.Text = "Login";
                label1.Text = "Username:";
                label2.Text = "Password：";
                textBox1.Text = "";
                textBox2.Text = "";
                label6.Visible = true;
            }
            label3.Location = new Point(pictureBox2.Width / 2 - label3.Width / 2, pictureBox2.Height / 2 - label3.Height / 2);
        }
        //====================Gestion des événements(UI)=========================
        //Button de Login
        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2.Image = imageList1.Images[1];
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2.Image = imageList1.Images[0];
        }

        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox2.Image = imageList1.Images[2];
        }

        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox2.Image = imageList1.Images[1];
        }

        private void label3_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2_MouseEnter(sender, e);
        }

        private void label3_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox2_MouseDown(sender, e);
        }

        private void label3_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2_MouseLeave(sender, e);
        }

        private void label3_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox2_MouseUp(sender, e);
        }

        private void label4_MouseEnter(object sender, EventArgs e)
        {
            label4.ForeColor = Color.FromArgb(100, Color.Black);
        }

        private void label4_MouseLeave(object sender, EventArgs e)
        {
            label4.ForeColor = Color.FromArgb(100, Color.White);
        }

        private void label4_MouseDown(object sender, MouseEventArgs e)
        {
            label4.ForeColor = Color.FromArgb(100, 50, 90, 220);
        }

        private void label4_MouseUp(object sender, MouseEventArgs e)
        {
            label4.ForeColor = Color.FromArgb(100, 100, 180, 241);
        }

        private void label4_MouseClick(object sender, MouseEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            this.mousePoint = Control.MousePosition;
            this.formLocation = this.Location;
        }
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point p = new Point(formLocation.X + Control.MousePosition.X - mousePoint.X, formLocation.Y + Control.MousePosition.Y - mousePoint.Y);
                this.Location = p;
            }
        }
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            this.mousePoint.X = e.X;
            this.mousePoint.Y = e.Y;
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Top = Control.MousePosition.Y - mousePoint.Y;
                this.Left = Control.MousePosition.X - mousePoint.X;
            }
        }
        private void label6_MouseEnter(object sender, EventArgs e)
        {
            label6.ForeColor = Color.FromArgb(100, 100, 180, 241);
        }

        private void label6_MouseDown(object sender, MouseEventArgs e)
        {
            label6.ForeColor = Color.FromArgb(100, 50, 90, 220);
        }

        private void label6_MouseUp(object sender, MouseEventArgs e)
        {
            label6.ForeColor = Color.FromArgb(100, 100, 180, 241);
        }

        private void label6_MouseLeave(object sender, EventArgs e)
        {
            label6.ForeColor = Color.FromArgb(100, Color.White);
        }

        private void label5_MouseEnter(object sender, EventArgs e)
        {
            label5.ForeColor = Color.FromArgb(100, 100, 180, 241);
        }

        private void label5_MouseLeave(object sender, EventArgs e)
        {
            label5.ForeColor = Color.FromArgb(100, Color.White);
        }

        private void label5_MouseUp(object sender, MouseEventArgs e)
        {
            label5.ForeColor = Color.FromArgb(100, 100, 180, 241);
        }

        private void label5_MouseDown(object sender, MouseEventArgs e)
        {
            label5.ForeColor = Color.FromArgb(100, 50, 90, 220);
        }

        private void login_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                pictureBox2_Click(sender, e);
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                pictureBox2_Click(sender, e);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                textBox2.Focus();
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}