using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Drawing.Text;
using System.IO;

namespace chat2._0
{
    public partial class chat : Form
    {
        public Dictionary<string, string> chatBuffer;//Cache d'historique de chat lors du changement d'objets de chat
        private string UIfile = "Gray";//Spécifiez la couleur du thème
        private setUI setui;//Définir la couleur du thème
        private Point mousePoint = new Point();//Fournir un point de déplacement de formulaire
        private Point formLocation;//Position de la fenêtre
        private string userName;
        ImageList sendButton = null;
        public chat(string userName)
        {
            InitializeComponent();
            dataProcessing.setChat(this);
            this.userName = userName;
        }
        private void chat_Load(object sender, EventArgs e)
        {
            sendButton = GrayButton;
            chatBuffer = new Dictionary<string, string>();
            chatBuffer.Add("Chatroom","");            
            string path = ".\\myFont.ttf";
            Font f = null;
            Font chatFont = null;
            if (File.Exists(path))
            {
                PrivateFontCollection pfc = new PrivateFontCollection();
                pfc.AddFontFile(path);
                f = new Font(pfc.Families[0], 15);
                chatFont = new Font(pfc.Families[0], 13);
            }
            else
            {
                f = new Font("黑体", 15);
                chatFont = new Font("黑体", 13);
            }
            //font
            label10.Font = f;
            label6.Font = f;
            label1.Font = f;
            label3.Font = f;
            label3.ForeColor = Color.Red;
            label7.Font = f;
            listBox1.Font = f;
            setui = new setUI(f,this);
            label7.Text = "Utilisateur : " + userName;
            label10.Parent = pictureBox2;
            label6.Parent = panel2;
            label10.Location = new Point(pictureBox2.Width/2 - label10.Width/2,pictureBox2.Height/2 - label10.Height/2);
            label6.Location = new Point(panel2.Width / 2 - label6.Width / 2, panel2.Height / 2 - label6.Height / 2);
            listBox1.SelectedIndex = 0;
            if (!dataProcessing.beginWork("chat")) MessageBox.Show("La communication avec le serveur a échoué");
            //Obtenez une liste d'utilisateurs en ligne
            dataProcessing.sendData(3, null);
        }
        public string getUserName()
        {
            return this.userName;
        }
        public void showMessageBox(string s)
        { MessageBox.Show(s); }
        public void addText(string location,string s)
        {
            listBox1.BeginInvoke(new Action(() =>
                {
                    if (listBox1.SelectedItem.ToString() == location)
                    {
                        richTextBox1.BeginInvoke(new Action(() =>
                        {
                            richTextBox1.AppendText(s);
                            richTextBox1.AppendText("\n");
                        }));
                    }
                    else
                    {
                        chatBuffer[location] += s;
                        chatBuffer[location] += "\n";
                    }
                }));
        }
        //Ajouter une liste en ligne (connexion)
        public void addListBox(string s)
        {
            listBox1.BeginInvoke(new Action(() =>
            {
                if (s == this.userName)
                {
                    return;
                }
                listBox1.Items.Add(s);
                chatBuffer.Add(s, "");
            }));
        }
        //Supprimer la liste en ligne (déconnexion)
        public void delListBox(string s)
        {
            try
            {
                listBox1.BeginInvoke(new Action(() =>
                {
                    if (listBox1.SelectedItem.ToString() == s)
                    {
                        listBox1.SelectedIndex = 0;
                    }
                    for (int i = 0; i < listBox1.Items.Count; i++)
                    {
                        if (listBox1.Items[i].ToString() == s)
                        {
                            listBox1.Items.RemoveAt(i);
                            break;
                        }
                    }

                }));
                chatBuffer.Remove(s);
            }
            catch (Exception)
            {
                return;
            }
        }
        public void setBreakSituation()
        {
            label3.BeginInvoke(new Action(() =>
            {
                label3.Text = "La connexion au serveur a été interrompue, vérifiez le réseau et redémarrez le programme";
            }));
        }
        private void refreshButton()
        {
            switch (UIfile)
            {
                case "Gray":
                    sendButton = GrayButton;
                    break;
                case "Blue":
                    sendButton = BlueButton;
                    break;
                case "Pink":
                    sendButton = PinkButton;
                    break;
                default:
                    sendButton = GrayButton;
                    break;
            }
        }
        public Point getLocation()
        {
            return new Point(this.Location.X, this.Location.Y);
        }
        public Point getSize()//x:Width  y:Height
        {
            return new Point(this.Width, this.Height);
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if(richTextBox2.Text.Trim() == "")
            {
                MessageBox.Show("L'envoi de contenu ne peut pas être vide");
                return;
            }
            string[] data = new string[1];
            data[0] = richTextBox2.Text;
            if (listBox1.SelectedItem.ToString() == "Chatroom")
            {
                if (!dataProcessing.sendData(1, data))//Type de message 1: message envoyé à la salle de chat
                {
                    MessageBox.Show("Échec de l'envoi du message");
                    return;
                }
            }
            else
            {
                data = new string[2];
                data[0] = listBox1.SelectedItem.ToString();//receiver
                data[1] = richTextBox2.Text;
                if (!dataProcessing.sendData(2, data))//Type de message 2: chat privé
                {
                    MessageBox.Show("Échec de l'envoi du message");
                    return;
                }
            }
            richTextBox2.Text = "";
        }
        private void label10_Click(object sender, EventArgs e)
        {
            pictureBox2_Click(sender,e);
        }
        //==========Evenement boutton UI==========
        //Bouton de send
        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2.Image = sendButton.Images[1];
        }
        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2.Image = sendButton.Images[0];
        }
        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox2.Image = sendButton.Images[2];
        }
        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox2.Image = sendButton.Images[1];
        }
        private void label10_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox2_MouseDown(sender, e);
        }
        private void label10_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2_MouseEnter(sender, e);
        }
        private void label10_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2_MouseLeave(sender, e);
        }
        private void label10_MouseUp(object sender, MouseEventArgs e)
        {
            pictureBox2_MouseUp(sender, e);
        }
        //Annuler Enter le retour chariot
        private void richTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
            }
        }
        //Entrez Enter: envoyez un message
        private void richTextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                pictureBox2_Click(sender,e);
            }
        }
        //Définissez le message de la boîte de message à la fin du texte
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.Select(richTextBox1.TextLength, 0);
            richTextBox1.ScrollToCaret();
        }
        public void SetUI(string s)
        {
            switch (s)
            {
                case "Gris":
                    UIfile = "Gray";
                    this.BackColor = Color.FromArgb(255, 238, 238, 238);
                    panel1.BackColor = Color.FromArgb(255, 150, 150, 150);
                    panel2.BackColor = Color.FromArgb(255, 210, 210, 210);
                    panel3.BackColor = Color.FromArgb(255, 228, 228, 228);
                    richTextBox1.BackColor = Color.FromArgb(255, 217, 217, 217);
                    richTextBox2.BackColor = Color.FromArgb(255, 217, 217, 217);
                    listBox1.BackColor = Color.FromArgb(255, 150, 150, 150);
                    pictureBox2.Image = GrayButton.Images[0];
                    button1.BackColor = Color.FromArgb(255, 150, 150, 150);
                    break;
                case "Blue":
                    UIfile = "Blue";
                    this.BackColor = Color.FromArgb(255, 231, 238, 253);
                    panel1.BackColor = Color.FromArgb(255, 102, 147, 252);
                    panel2.BackColor = Color.FromArgb(255, 191, 209, 251);
                    panel3.BackColor = Color.FromArgb(255, 215, 227, 255);
                    richTextBox1.BackColor = Color.FromArgb(255, 201, 216, 253);
                    richTextBox2.BackColor = Color.FromArgb(255, 201, 216, 253);
                    listBox1.BackColor = Color.FromArgb(255, 102, 147, 252);
                    pictureBox2.Image = BlueButton.Images[0];
                    button1.BackColor = Color.FromArgb(255, 102, 147, 252);
                    break;
                case "Rose":
                    UIfile = "Pink";
                    this.BackColor = Color.FromArgb(255, 253, 231, 231);
                    panel1.BackColor = Color.FromArgb(255, 253, 102, 102);
                    panel2.BackColor = Color.FromArgb(255, 251, 191, 191);
                    panel3.BackColor = Color.FromArgb(255, 255, 215, 215);
                    richTextBox1.BackColor = Color.FromArgb(255, 253, 201, 201);
                    richTextBox2.BackColor = Color.FromArgb(255, 253, 201, 201);
                    listBox1.BackColor = Color.FromArgb(255, 253, 102, 102);
                    pictureBox2.Image = PinkButton.Images[0];
                    button1.BackColor = Color.FromArgb(255, 253, 102, 102);
                    break;
                default:
                    break; 
            }
            refreshButton();
        }//Définir la couleur
        private void label4_MouseEnter(object sender, EventArgs e)
        {
            label4.ForeColor = Color.FromArgb(100, 100, 100, 100);
        }
        private void label4_MouseLeave(object sender, EventArgs e)
        {
            label4.ForeColor = Color.White;
        }
        private void label4_MouseDown(object sender, MouseEventArgs e)
        {
            label4.ForeColor = Color.FromArgb(100, 60, 60, 60);
        }
        private void label4_MouseUp(object sender, MouseEventArgs e)
        {
            label4.ForeColor = Color.FromArgb(100, 100, 100, 100);
        }
        private void label4_Click(object sender, EventArgs e)
        {
            dataProcessing.sendData(404,null);
            Application.Exit();
        }
        private void label2_MouseEnter(object sender, EventArgs e)
        {
            label2.ForeColor = Color.FromArgb(100, 100, 100, 100);
        }
        private void label2_MouseLeave(object sender, EventArgs e)
        {
            label2.ForeColor = Color.White;
        }
        private void label2_MouseDown(object sender, MouseEventArgs e)
        {
            label2.ForeColor = Color.FromArgb(100, 60, 60, 60);
        }
        private void label2_MouseUp(object sender, MouseEventArgs e)
        {
            label2.ForeColor = Color.FromArgb(100, 100, 100, 100);
        }
        private void label2_Click(object sender, EventArgs e)
        {
            if (setui.Visible)
            {
                setui.Visible = false;
            }
            else
            {
                setui.Visible = true;
            }
        }
        private void chat_FormClosed(object sender, FormClosedEventArgs e)
        {
            dataProcessing.sendData(404, null);
        }
        //Lorsque le choix du partenaire de chat change
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string history = "\n--------------------------History message--------------------------\n";
            if (label6.Text == listBox1.SelectedItem.ToString())
            {
                return;
            }
            if (richTextBox1.Text.Contains(history))
            {
                richTextBox1.Text = richTextBox1.Text.Replace(history, "");
            }

            chatBuffer[label6.Text] = richTextBox1.Text + history;
            
            label6.Text = listBox1.SelectedItem.ToString();
            richTextBox1.Text = chatBuffer[label6.Text];
            chatBuffer[label6.Text] = "";
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
        private void label6_TextChanged(object sender, EventArgs e)
        {
            label6.Location = new Point(panel2.Width / 2 - label6.Width / 2, panel2.Height / 2 - label6.Height / 2);
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Ajoute Chatroom
            AjouteChatroom ajouteChatroom = new AjouteChatroom();
            ajouteChatroom.ShowDialog();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}
