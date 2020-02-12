using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Converter
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string path;
            List<string> result = new List<string>();
            List<string> patterns = new List<string>() { //что заменить
                @"CREATE TABLE",
                @"\[|]",
                "\"bigint\"",
                "\"tinyint\"",
                "\"varchar\"",
                "N'",
                "INSERT"
            };
            List<string> replacements = new List<string>() { //на что заменить
                "CREATE TABLE IF NOT EXISTS",
                "\"",
                "bigint",
                "int",
                "varchar",
                "'",
                "INSERT INTO"


            };
            Regex regex = new Regex(@"(GO|USE|SET|ASC|ALTER|REFERENCES|EXEC)"); //пропуск генерации операторов SQL
            Regex regex3 = new Regex("\"\\w*\""); //изменение капитализации
            try
            {
                Console.WriteLine("Введите путь к файлу");
                path = Console.ReadLine();
                Console.WriteLine("Идет считывание и конвертация...");
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        if ((!regex.IsMatch(line)) && (line[0] != ' ') && (line[0] != '(') && (line[0] != ')')) //пропускаем все строки, содеражащие...
                        {
                            int counter = 0;
                            foreach (var item in patterns)
                            {
                                Regex regex2 = new Regex(item);
                                line = regex2.Replace(line, replacements[counter]);
                                counter++;
                            }
                            if (line.Split()[0] == "INSERT")
                            {
                                line = line + ";";
                            }
                            MatchCollection matches = regex3.Matches(line); //капитализация
                            if (matches.Count > 0)
                            {
                                foreach (Match match in matches)
                                {
                                    string replaceValue = match.Value;
                                    replaceValue = replaceValue.ToLower();
                                    line = line.Replace(match.Value, replaceValue);
                                }
                                    
                            }

                            result.Add(line);
                        }
                    }
                }
                Console.WriteLine("Введите путь, куда сохранить");
                string writePath = Console.ReadLine();
                Console.WriteLine("Идет запись в файл...");
                using (StreamWriter sw = new StreamWriter(writePath))
                {
                    foreach (var item in result)
                    {
                        await sw.WriteLineAsync(item);
                    }
                    Console.WriteLine("Конвертация выполнена");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }
    }
}
