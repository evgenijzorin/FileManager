using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System.Text.Json;


namespace fileManager
{
    class Program
    {
        // Коллекция глобальных переменных программы
        public static class MyGlobals
        {                                      
            // Настройки пэйджинга
            public static int maxNumberLinesInPage = 40; // Максимальное количестро строк на странице
            public static int numberLineInPage = 0; // номер первой выводимой строк на странице
            public static int numberPage = 1; // номер первой выводимой строк на странице
            public static string LastOperationInfo = "нет";
        }
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;            
            Console.SetWindowSize(180, 55);
            #region Создание json файла для сохранения параметров приложения
            string pathCurrent;
            //string pathVisual;
            var appFileManagerOptions = new AppFileManagerOptions();
            if (File.Exists("fileManagerSetings.json"))
            {// Стартовая директория - извлечение стартовой директории из json файла
                var appConfiguraftion = new ConfigurationBuilder().AddJsonFile("fileManagerSetings.json").Build();
                pathCurrent = appConfiguraftion.GetSection("Path").Value;
                appFileManagerOptions.Path = pathCurrent;
                //pathVisual = appConfiguraftion.GetSection("pathVisual").Value; ; // Путь для визуализации дерева 
                //appFileManagerOptions.pathVisual = pathVisual;
            }
            else
            {                
                appFileManagerOptions.Path = "C:";
                pathCurrent = "C:";
                //appFileManagerOptions.pathVisual = "C:\\";
                //pathVisual = "C:\\"; // Путь для визуализации дерева (отличается от pathCurrent тем, что не содержит пустых каталогов в конце)
                string json = JsonSerializer.Serialize(appFileManagerOptions);
                File.WriteAllText("fileManagerSetings.json", json);
            }
            #endregion
            // Предшествующая операция:            
            MyGlobals.LastOperationInfo = "нет";            
            string []pathCurrentArray = pathCurrent.Split('\\');
            // Основной цикл регенерации консоли
            while (true)
            {
                // регенирировать консоль (вывести обновленную информацию на консоль)
                Tree treeForVisualisation = ConsoleRegen(pathCurrent);              
                Console.WriteLine("Введите команду:");

                #region Команды пользователя.
                // cd.. – перейти в папку на уровень выше
                // cd <каталог> – перейти во вложенный каталог, 
                // md – создать директорию
                // cp <файл который копируем> <файл в который копируем> - копирование файлов
                // rm <файл, директория которую удаляем> – удалить файл или директорию (рекурсивное удаление –r)
                // info <файл, директория> - получение информации о файле либо директории, находящемся в текущем каталоге
                // > - следующая страница дерева
                // < - Предыдущая страница дерева
                // quit - Завершить работу и выйти
                #endregion
                string userAnsver = Console.ReadLine();
                string[] userAnsverArray = userAnsver.Split(' ');
                string userCommand = userAnsverArray[0];
                switch (userCommand)
                {
                    case "cp":// копировать файл или директорию
                        copyDirOrFile(userAnsverArray);
                        break;
                    case "rm":
                        // Определить название удаляемой директории
                        string delDirName = "";
                        for (int i = 1; i < userAnsverArray.Length; i++)
                        {
                            if (!(i == 1))
                                delDirName = delDirName + " " + userAnsverArray[i];
                            else
                                delDirName = userAnsverArray[i];
                        }
                        // Определение введенного пути пользователем 

                        if (Directory.Exists(pathCurrent + "\\" + delDirName)) // проверка существования директории
                        {
                            Directory.Delete(pathCurrent + "\\" + delDirName, true);
                            MyGlobals.LastOperationInfo = $"rm {delDirName} - директория удалена";
                        }
                        else if (Directory.Exists(delDirName)) // если введен глобальный путь
                        {
                            Directory.Delete(delDirName, true);
                            MyGlobals.LastOperationInfo = $"rm {delDirName} - директория удалена";
                        }
                        else if (File.Exists(delDirName)) // если введен глобальный путь
                        {
                            File.Delete(delDirName);
                            MyGlobals.LastOperationInfo = $"rm {delDirName} - файл удалён";
                        }
                        else if (File.Exists(pathCurrent + "\\" + delDirName)) // если введен глобальный путь
                        {
                            File.Delete(pathCurrent + "\\" + delDirName);
                            MyGlobals.LastOperationInfo = $"rm {pathCurrent + "\\" + delDirName} - файл удалён";
                        }
                        else
                        {
                            MyGlobals.LastOperationInfo = $"rm {delDirName} - директория не удалена. Директория не обнаружена";
                        }
                        break;
                    case "md": // создать директорию
                        // Определить название создаваемой директории
                        string newDirName = "";
                        for (int i = 1; i < userAnsverArray.Length; i++)
                        {
                            if (!(i==1))                            
                                newDirName = newDirName + " " + userAnsverArray[i];
                            else
                                newDirName = userAnsverArray[i];
                        }
                        if (!Directory.Exists(pathCurrent + "\\" + newDirName)) // проверка существования директории
                        {
                            Directory.CreateDirectory(pathCurrent + "\\" + newDirName);
                            MyGlobals.LastOperationInfo = $"md {newDirName} - директория создана";
                        }
                        else
                        {
                            MyGlobals.LastOperationInfo = $"md {newDirName} - директория не создана. Директория с таким именем существует";
                        }
                        break;
                    case "cd..":
                        // var parentDirectory = Directory.GetParent(pathCurrent);
                        var parentDirectory = treeForVisualisation.ParentCurentDirectory;                        
                        if (parentDirectory != null)                        
                        {                                                       
                            pathCurrent = parentDirectory.FullName;
                            // Нужно убрать из родительской директории в конце "//"                               
                            string[] pathCurrentArray_1 = pathCurrent.Split('\\');
                            string pathCurrent_1 = "";
                            for (int i = 0; i < pathCurrentArray_1.Length; i++)
                            {
                                if (i != 0)
                                {
                                    if (pathCurrentArray_1[i] != "")
                                        pathCurrent_1 = pathCurrent_1 + "\\" + pathCurrentArray_1[i];
                                }
                                else
                                    pathCurrent_1 = pathCurrent_1 + pathCurrentArray_1[i];
                            }
                            pathCurrent = pathCurrent_1;
                            MyGlobals.LastOperationInfo = "cd.. - Переход на уровень вверх выполнен";
                        }                        
                        else                        
                        {                           
                            MyGlobals.LastOperationInfo = "cd.. - Переход на уровень вверх не выполнен, отображается корневая директория";
                        }
                        break;
                    case "cd":
                        // следующая директория в которую пользователь хочет перейти                     
                        // Удалить первый элемент из массива (команду cp)
                        string nextDirectory = "";
                        for (int i = 1; i < userAnsverArray.Length; i++)
                        {
                            if (!(i == 1))
                                nextDirectory = nextDirectory + " " + userAnsverArray[i];
                            else
                                nextDirectory = userAnsverArray[i];
                        }
                        if (SearchingElementInStringArray(pathCurrent + "\\" + nextDirectory, treeForVisualisation.DirectorysArray))
                        {
                            pathCurrent = pathCurrent + "\\" + nextDirectory;
                            MyGlobals.LastOperationInfo = $"cd {nextDirectory} - Переход в директорию выполнен.";
                        }
                        else
                        {
                            MyGlobals.LastOperationInfo = $"cd {nextDirectory} - Переход в директорию НЕ ВЫПОЛНЕН. Директория не найдена!";
                        }
                        //MyGlobals.numberLineInPage = 0; // обнуляем страницу
                        break;                        
                    case ">":// переход по визуализации вперед
                        MyGlobals.numberPage = MyGlobals.numberPage + 1;
                        break;
                    case "<":// переход по визуализации назад
                        if (MyGlobals.numberPage > 1)
                            MyGlobals.numberPage = MyGlobals.numberPage - 1;
                        else
                            MyGlobals.numberPage = 1;
                        break;
                    case "quit":
                        // Сохранить настройки пользователя
                        EndAppFileManager(pathCurrent);
                        return;                        
                    default:
                        break;
                }
            }
        }
        static Tree ConsoleRegen (string pathCurrent)
        {
            Tree treeForVisualisation = new Tree(pathCurrent);
            #region Линии оформления и цвета
            string horisontLineSingle = "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------";
            string horisontLineDouble = "===================================================================================================================================================================================";
            // цвета выделения:
            ConsoleColor currentBackground = Console.BackgroundColor;
            ConsoleColor settingsBackground = ConsoleColor.Red;
            #endregion
            Console.Clear();
            // Вывести первой строчкой полный путь текущей папки
            Console.WriteLine($"текущий каталог: {pathCurrent}");
            // Вывести горизонтальную черту
            Console.WriteLine(horisontLineSingle);
            // вывести дерево каталогов
            ConsoleWriteList(treeForVisualisation);
            Console.WriteLine(horisontLineDouble);// двойной разделитель
            Console.WriteLine();
            //выводится информация:
            Console.WriteLine($"Имя директории: {treeForVisualisation.NameCurentDirectory} \t Количество папок: {treeForVisualisation.DirectorysArray.Length} \t Количество файлов: {treeForVisualisation.FilesArray.Length} \t {Environment.NewLine}Директория создана: {treeForVisualisation.CreationTime} \t");            
            Console.WriteLine();
            Console.WriteLine(horisontLineDouble);
            Console.WriteLine($"Предыдущая операция: {MyGlobals.LastOperationInfo}"); 
            Console.WriteLine(horisontLineDouble);// двойной разделитель
            return treeForVisualisation;
        }

        /// <summary>
        /// SearchingElementInArray - Проверка эквивалентности строки элементу строкового массива
        /// </summary>
        /// <param name="Element"> элемент, который ищем в массиве </param>
        /// <param name="Array"> массив </param>
        /// <returns></returns>
        static bool SearchingElementInStringArray(string element, string[] Array)
        {
            for (int i = 0; i < Array.Length; i++ )
            {
                if (String.Compare(Array[i], element) == 0)
                    return true;
            }            
            return false;
        }

        static void EndAppFileManager (string path)
        {
            // Сохранить настройки в json файле
            var appFileManagerOptions = new AppFileManagerOptions();
            appFileManagerOptions.Path = path;
            string json = JsonSerializer.Serialize(appFileManagerOptions);
            File.WriteAllText("fileManagerSetings.json", json);
        }
        static void ConsoleWriteList(Tree treeForVisualisation)
        {
            // Подбор страницы для пейджинга
            if (
                (MyGlobals.numberPage < 1)
                ||
                // Условие чтобы все поместилось на странице
                ((1 + treeForVisualisation.ListLines.Count / MyGlobals.maxNumberLinesInPage) <  MyGlobals.numberPage))
                MyGlobals.numberPage = 1;
            int i;
            for (i = (MyGlobals.numberPage - 1) * MyGlobals.maxNumberLinesInPage ;
                i < MyGlobals.maxNumberLinesInPage * MyGlobals.numberPage; 
                i++ )
            {
                if (i < treeForVisualisation.ListLines.Count)                
                    Console.WriteLine(treeForVisualisation.ListLines[i]);
                else  
                    Console.WriteLine();
            }
            MyGlobals.numberLineInPage = i;            
        }
        static void copyDirOrFile(string[] userAnsverArray)
        {
            // Удалить первый элемент из массива (команду cp)
            string pathString = "";
            for (int i = 1; i < userAnsverArray.Length; i++)
            {
                if (!(i == 1))
                    pathString = pathString + " " + userAnsverArray[i];
                else
                    pathString = userAnsverArray[i];
            }
            // Определить массив путей 
            string[] pathArray = pathString.Split('\'');
            // Удалить из массива пустые строки
            string path1 = "";
            string path2 = "";
            foreach(string str in pathArray)
            {
                if (path1 == "")  
                {
                    if (!(str == ""))
                        path1 = str;
                }
                else
                {
                    if (!(str == ""))
                        path2 = str;
                }
            }
            if ((path1 == "") || (path2 == ""))
            {
                MyGlobals.LastOperationInfo = $"Операция копирования не выполнена. Не удалость определить пути копирования. Проверьте правильность ввода данных";
                return;
            }            
            // Проверка существования копируемого пути файла
            // Проверка существования пути в который копируем файл
            if (File.Exists(path1))
            {                  
                try                  
                {
                    File.Copy(path1, path2);
                    }
                    catch 
                    {
                    //здесь обрабатывай ошибки                    
                    MyGlobals.LastOperationInfo = $"Операция копирования из {path1} в {path2} не выполнена. Проверьте правильность ввода данных";
                    return;   
                }
                MyGlobals.LastOperationInfo = $" cp - Операция копирования из {path1} в {path2} выполнена.";
                return;
            }
            else if (Directory.Exists(path1))
            {
                CopyPasteDir(path1, path2, false);                
                return;
            }

            else
            { 
                MyGlobals.LastOperationInfo = $"Операция копирования из {path1} в {path2} не выполнена. Проверьте правильность ввода данных";
                return;
            }           
        }

        // Метод копирования файлов или папок с указанием пути копирования
        static void CopyPasteDir (string path1, string path2, bool remove_id)
        {
            //Создать идентичную структуру папок
            if (!(Directory.Exists(path2))) 
                Directory.CreateDirectory(path2);
            foreach (string dirPath in Directory.GetDirectories(path1, "*", SearchOption.AllDirectories))
            {
                try
                {
                    Directory.CreateDirectory(dirPath.Replace(path1, path2));
                    MyGlobals.LastOperationInfo = $" cp - Операция копирования из {path1} в {path2} выполнена.";
                }
                catch (Exception e)
                {
                    //здесь обрабатывай ошибки
                    MyGlobals.LastOperationInfo = $"Операция копирования из {path1} в {path2} не выполнена. Проверьте правильность ввода данных и права доступа [{e}]";
                    return;
                }
            }
            //Копировать все файлы и перезаписать файлы с идентичным именем
            foreach (string newPath in Directory.GetFiles(path1, "*.*",
                SearchOption.AllDirectories))
            {
                try
                {
                    File.Copy(newPath, newPath.Replace(path1, path2), true);
                    MyGlobals.LastOperationInfo = $" cp - Операция копирования из {path1} в {path2} выполнена.";
                }
                catch (Exception e)
                {
                    //здесь обрабатывай ошибки
                    MyGlobals.LastOperationInfo = $"Операция копирования из {path1} в {path2} не выполнена. Проверьте правильность ввода данных и права доступа [{e}]";
                    return;
                }
            }
            if (remove_id)
            {
                try
                {
                    Directory.Delete(path1, true);
                }
                catch (Exception e)
                {
                    MyGlobals.LastOperationInfo = $"Операция перемещения из {path1} в {path2} не выполнена. Проверьте правильность ввода данных и права доступа [{e}]";
                    return;
                }

            }


        }

    }
}
