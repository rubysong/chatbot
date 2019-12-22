using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

//Traitement des données transmises (y compris l'envoi et la réception)
namespace chat2._0
{
    static class dataProcessing
    {
        //Connecter le socket du serveur
        private static Socket server;
        //Adresse du serveur
        private static IPAddress IP = IPAddress.Parse("127.0.0.1");//Adresse du serveur
        private static int port = 8081;//Numéro de port du serveur
        private static chat myChat = null;
        private static login myLogin = null;
        //Initialiser le membre statique lorsque la fenêtre est initialisée
        public static void setChat(chat s)
        {myChat = s;}
        public static void setlogin(login l)
        {myLogin = l;}
        //Modifier l'adresse du serveur et le numéro de port
        public static void address(string ip,int por)
        {
            IP = IPAddress.Parse(ip);
            port = por;
        }
        //Envoyer des données (contrairement à sendData sur le serveur)
        public static bool sendData(int num, string[] data)
        {
            if (server == null) return false;
            string sendData = "";

            switch (num)
            {
                //Vérifiez le mot de passe du compte lors de la connexion:
                case 0:
                    sendData = num.ToString() + "$" + 
                          data[0] + "$" + data[1] + "$";
                    break;
                //Envoyer un message public
                case 1:
                    sendData = num.ToString() + "$" + myChat.getUserName() +"$" + data[0].Length + "$" + data[0] + "$";
                    break;
                //chat prive
                //data[0]:receiver    data[1]:message
                case 2://Format: Type de données 2 $ expéditeur $ destinataire $ Longueur du message $ Contenu du message $
                    sendData = num.ToString() + "$" + myChat.getUserName() +
                        "$" + data[0] + "$" + data[1].Length.ToString() + "$" + data[1] + "$";
                    break;
                //Obtenez une liste d'utilisateurs en ligne
                case 3://Format: Type de données 3 $ Code ("GETONLINE": Obtenir la liste des utilisateurs en ligne)
                    sendData = num.ToString() + "$";
                    break;
                case 4:
                    sendData = num.ToString() + "$" + data[0] + "$" + data[1] + "$";
                    break;
                case 5:
                    myChat.addText("Chatroom",data[1]+"Deja Login");
                    myChat.addListBox(data[1]);
                    break;
                case 404:
                    sendData = "404$";
                    break;
                default:
                    return false;
            }
            try
            {
                server.Send(UTF8Encoding.UTF8.GetBytes(sendData));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        //Traitement des données reçues
        public static string[] receiveData()
        {
            string[] data = null;
            if (server == null) return data;
            
            byte[] receiveByte = new byte[1024];
            try
            {
                server.Receive(receiveByte);
            }
            catch (Exception)
            {
                data = null;
                return data;
            }
            string receiveString = UTF8Encoding.UTF8.GetString(receiveByte);
            data = receiveString.Split('$');
            //Sélectionnez le type de message correspondant à traiter
            switch (data[0])
            {
                case "0":
                    //Renvoie directement les messages segmentés
                    break;
                case "1"://1$sender$textLength$text$
                    string sender = data[1];
                    int textLength = int.Parse(data[2]);
                    string text = receiveString.Substring(receiveString.IndexOf('$', data[0].Length + data[1].Length + 2) + 1, textLength);
                    string result = sender+"["+DateTime.Now.ToString()+"]:\n"+text;
                    myChat.addText("Chatroom",result);
                    break;
                //Chat privé
                case "2":
                    result = data[1]+"["+DateTime.Now.ToString()+"]:\n"+receiveString.Substring(data[0].Length + data[1].Length + data[2].Length + data[3].Length + 4, int.Parse(data[3]));
                    if (data[1] == myChat.getUserName())
                    {
                        myChat.addText(data[2], result);
                    }
                    else
                    {
                        myChat.addText(data[1], result);
                    }
                    
                    break;
                case "3":
                    for (int i = 1; i < data.Length-1; i++)
                    {
                        myChat.addListBox(data[i]);
                    }
                    break;
                case "5":
                    myChat.addListBox(data[1]);
                    break;
                case "6":
                    myChat.delListBox(data[1]);
                    break;
                case "404":
                    sendData(404, null);
                    myChat.showMessageBox("Vous avez été forcé hors ligne");
                    data = null;
                    break;
                default:
                    break;
            }
            return data;
        }
        // commence à travailler
        // login: créer un nouveau thread d'arrière-plan pour se connecter au serveur et modifier l'état de l'interface
        // chat: créer un nouveau thread d'arrière-plan et un nouveau serveur pour transmettre les données
        public static bool beginWork(string choice)
        {
            if (myChat == null && myLogin == null) return false;
            Thread thread = null;
            switch (choice)
            {
                case "login"://login
                    thread = new Thread(connectServer);
                    break;
                case "chat"://chat
                    thread = new Thread(Receive);
                    break;
                default:
                    return false;
            }
            thread.IsBackground = true;
            thread.Start();
            return true;
        }
        //Connectez-vous au serveur et modifiez l'état de l'interface
        private static void connectServer()
        {
            if (myLogin == null) return;
            try
            {
                server = new Socket(SocketType.Stream, ProtocolType.Tcp);
                server.Connect(IP, port);
            }
            catch (SocketException)
            {
                myLogin.setConnect(false);
                myLogin.setFooterSituation("Impossible de se connecter au serveur.");
                return;
            }
            myLogin.setConnect(true);
            myLogin.setFooterSituation("Connecté au serveur, veuillez continuer.");
        }
        //Le serveur transmet des données. 
        //Lorsque la valeur de retour de receiveData est nulle, il sortira de la boucle et cessera de recevoir des données.
        private static void Receive()
        {
            if (myChat == null) return;
            while (true)
            {
                string[] data = dataProcessing.receiveData();
                if (data == null)
                {
                    myChat.setBreakSituation();
                    return;//404
                }
            }
        }
    }
}
