using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;

namespace InfopoleProject
{
    public class Program
    {
        public static TreeNode root;    //корень дерева
        public static int id = 0;   //старторвый id
        public static string filePath;  //полный путь файла с данными
        public static int topRequestsCount; //число
        public static List<RequestObject> requestObjects= new List<RequestObject>();
        static void Main(string[] args)
        {
            if (args.Length == 0)
                return;
            filePath = args[0];
            root = new TreeNode(null, id++, "Infopole");
            topRequestsCount = getCount(new ConsoleReader());
            ReadData();
            BuildTree();
            root.GetTopRequests(topRequestsCount);
            SerializeToJSON();
            Console.ReadLine();
        }
        public static void SerializeToJSON() //сериализация в JSON
        {
            try
            {
                Console.WriteLine("Сериализация в json...");
                File.WriteAllText("tree.json", JsonConvert.SerializeObject(root));
                Console.WriteLine("Выполнено.");
            }
            catch(JsonSerializationException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static int getCount(IConsoleReader reader) //ввод количества реквестов в топе
        {
            int count = 0;
            Console.WriteLine("Введите число, ограничивающее топ запросов");
            if (!int.TryParse(reader.ReadLine(), out count)||count<=0)
            {
                    Console.WriteLine("Введено некорректное число, взято число по умолчанию (5)");
                    count = 5;
            }
            return count;
        }
        public static void ReadData() //чтение даннх 
        {
            Console.WriteLine("Чтение данных...");
            using (TextFieldParser tfp = new TextFieldParser(filePath, Encoding.Default))
            {
                tfp.TextFieldType = FieldType.Delimited;
                tfp.SetDelimiters(";");
                tfp.ReadFields();
                while (!tfp.EndOfData)
                {
                    requestObjects.Add(new RequestObject(tfp.ReadFields()));
                }
            }
        }

        public static void BuildTree() //построение дерева
        {
            TreeNode current;
            requestObjects.ForEach(
                request =>
                {
                    current = AddLeafToTree(root, request.Group);
                    current.AddRequestToNode(request.Request, request.PhraseFrequency, request.AccurateFrequency);
                });
        }

        public static TreeNode AddLeafToTree(TreeNode node,string str) //добавление узлов к дереву
        {
            //имена узлов
            string[] nodes = str.Split('.');
            TreeNode current = node;
            TreeNode child;
            foreach(var element in nodes)
            {
                child = current.Children.Find(x => x.ShortName == element);
                if (child == null)
                {
                    child = new TreeNode(current, id++, element);
                }
                current = child;
            }
            //возвращение добавленного листа(последнего узла)
            return current;
        }
    }
}

