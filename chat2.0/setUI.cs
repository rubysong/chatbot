using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chat2._0
{
    public partial class setUI : Form
    {
        Font f; //Police d'interface actuelle
        chat c; //Fournir des méthodes publiques pour le formulaire principal
        string check; //ce que selectionne
        
        public setUI(Font f,chat c)
        {
            InitializeComponent();
            this.f = f;
            this.c = c;
        }
        private void setUI_Load(object sender, EventArgs e)
        {
            label1.Font = f;
            radioButton1.Font = f;
            radioButton2.Font = f;
            radioButton3.Font = f;
            button1.Font = f;
        }
        // les options actuelles
        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                check = radioButton1.Text;
            }
            else if (radioButton2.Checked)
            {
                check = radioButton2.Text;
            }
            else
            {
                check = radioButton3.Text;
            }
            c.SetUI(check);
            this.Visible = false;
        }
        //Changer
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                this.BackColor = Color.FromArgb(255, 217, 217, 217);
                c.SetUI(radioButton1.Text);
            }
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                this.BackColor = Color.FromArgb(255, 201, 216, 253);
                c.SetUI(radioButton2.Text);
            }
        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                this.BackColor = Color.FromArgb(255, 253, 201, 201);
                c.SetUI(radioButton3.Text);
            }
        }
        //Réinitialiser la position actuelle de la fenêtre
        private void setUI_VisibleChanged(object sender, EventArgs e)
        {
            this.Location = new Point(c.getLocation().X + c.getSize().X, c.getLocation().Y);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
