using System;
using System.Collections.Generic;
using System.Text;
using System.IO;



namespace fileManager
{
    public class Tree
    {
        // Свойства класса:
        // Список строк для построения дерева каталогов в консоли
        public List<string> ListLines { get;}
        /// <summary>
        /// Массив файлов
        /// </summary>
        public string[] FilesArray { get; }
        /// <summary>
        /// DirectorysArray - Массив папок
        /// </summary>
        public string[] DirectorysArray { get; }
        /// <summary>
        /// NameCurentDirectory - имя рассматриваемой директории
        /// </summary>
        public string NameCurentDirectory { get; }

        /// <summary>
        /// CreationTime - время и дата создания каталога
        /// </summary>
        public DateTime CreationTime { get; }

        /// <summary>
        /// ParentCurentDirectory - сведения о родительской директории
        /// </summary>
        public DirectoryInfo ParentCurentDirectory { get; }
        

        /// <summary>
        /// Tree - метод конструктор для построения дерева каталогов 
        /// </summary>
        /// <param name="userPathText">Путь к конечному каталогу </param>
        public Tree (string userPathText)
        {
            // Перевести строку в массив        
            string[] userPathArray = userPathText.Split('\\');
            string parseDirectory = userPathArray[0];   // рассматриваемоя директория
            string spaseLine = ""; // маркировка отступов (пробелов)
            // для корневого каталога:
            spaseLine = GetSpaseLine(spaseLine, parseDirectory);
            // Создаем список строк для визуализации дерева
            List<string> listLines = new List<string>();
            // добавляем первую строку в список строк
            listLines.Add(new string (parseDirectory));

            // File.AppendAllText("user_Tree.txt", parseDirectory + Environment.NewLine);
            listLines.AddRange(GetfilesAndDir(userPathText, parseDirectory, 0, spaseLine)) ;
            
            // Присваиваем значение основному параметру
            ListLines = listLines;
            string[] filesArray = Directory.GetFiles(userPathText);            // получить файлы директории
            FilesArray = filesArray;
            string[] directorysArray = Directory.GetDirectories(userPathText); // получить каталоги директории  
            DirectorysArray = directorysArray;
            NameCurentDirectory = userPathArray[userPathArray.Length - 1];
            DateTime creationTime = Directory.GetCreationTime(userPathText);
            CreationTime = creationTime;
            DirectoryInfo parentCurentDirectory = Directory.GetParent(userPathText);
            ParentCurentDirectory = parentCurentDirectory;
        }

        // Метод: получить содержимое директории
        static List<string> GetfilesAndDir(string userPathText, string parseDirectory, int numberDirectory, string spaseLine)
        {
            // Создаем список строк для визуализации дерева
            List<string> listLines = new List<string>();
            // обработка содержимого корневого каталога:
            string nextDirectory;
            if (userPathText.Split('\\').Length > numberDirectory + 1)
                nextDirectory = userPathText.Split('\\')[numberDirectory + 1];
            else
                nextDirectory = "";
            string[] filesArray = Directory.GetFiles(parseDirectory + "\\");            // получить файлы директории
            string[] directorys = Directory.GetDirectories(parseDirectory + "\\"); // получить каталоги директории            
            // Соединить два массива
            string[] filesAndDir = new string[directorys.Length + filesArray.Length];
            directorys.CopyTo(filesAndDir, 0);
            filesArray.CopyTo(filesAndDir, directorys.Length);
            // записать имена файлов в список
            for (int i1 = 0; i1 < filesAndDir.Length; i1++)
            {
                string LinePath = filesAndDir[i1];
                string[] fileDirName = LinePath.Split('\\');
                string name = fileDirName[fileDirName.Length - 1]; // текущий файл или каталог
                if (name == nextDirectory)
                {
                    // добавляем новую строку в список строк
                    listLines.Add(new string(spaseLine + name));
                    // File.AppendAllText("user_Tree.txt", spaseLine + name + Environment.NewLine);
                    listLines.AddRange(GetfilesAndDir(userPathText, LinePath, numberDirectory + 1, GetSpaseLine(spaseLine, name)));
                }
                else
                {
                    if (i1 == 0)
                    {
                        // добавляем новую строку в список строк
                        listLines.Add(new string(spaseLine + name)); // + Environment.NewLine -убрал
                        // File.AppendAllText("user_Tree.txt", spaseLine + name + Environment.NewLine); // для первой итерации записываем имя корневого каталога
                    }

                    else
                    {
                        // добавляем новую строку в список строк
                        listLines.Add(new string(spaseLine + name));
                        // File.AppendAllText("user_Tree.txt", spaseLine + name + Environment.NewLine); // выводим последний элемент массива c отступами пробелами
                    }

                }
            }
            return listLines;
        }

        // Метод получение строки тире, для визуализации строки дерева
        static string GetSpaseLine(string spaseLine, string line)
        {
            string lineSpase = "";
            for (int i = 0; i < line.Length; i++)
            {
                if (i == line.Length - 1)
                    lineSpase = lineSpase + "|";
                else
                    lineSpase = lineSpase + "-";
            }
            return spaseLine + lineSpase;
        }











    }
}
