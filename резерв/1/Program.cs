using System;
using System.IO;
using System.Collections.Generic;
using System.Text;



namespace fileManager
{
    class Program
    {        
        static void Main(string[] args)
        {
            Console.SetWindowSize(180, 50);
            // Стартовая директория
            //  "C:\\"
            string startPath = "C:\\Users\\Евгений Заря\\Desktop\\GB обучение C#\\курсовой Файловый менеджер"; // необходимо сохранить в json файле
            // Предшествующая операция:
            string LastOperationInfo = "нет";
            string pathCurrent = startPath;
            string []pathCurrentArray = pathCurrent.Split('\\');
            // Основной цикл регенерации консоли
            while (true)
            {
                // регенирировать консоль (вывести обновленную информацию на консоль)            
                Tree treeForVisualisation = ConsoleRegen(pathCurrent, LastOperationInfo);
                Console.WriteLine("Введите команду:");
                #region Команды пользователя.
                // cd.. – перейти в папку на уровень выше
                // cd <каталог> – перейти во вложенный каталог, 
                // Mkdir – создать директорию
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
                    case "cd..":
                        var parentDirectory = Directory.GetParent(pathCurrent);                           
                        if(parentDirectory != null)                        
                        {
                            pathCurrent = parentDirectory.FullName;
                            LastOperationInfo = "cd.. - Переход на уровень вверх выполнен";
                        }                        
                        else                        
                        {                           
                            LastOperationInfo = "cd.. - Переход на уровень вверх не выполнен, отображается корневая директория";
                        }
                        break;
                    case "cd":
                        string nextDirectory = userAnsverArray[1]; // следующая директория в которую пользователь хочет перейти
                        // Проверяем, есть ли эта директория в текущей директории.
                        if (SearchingElementInStringArray(nextDirectory, treeForVisualisation.DirectorysArray))
                        {
                            pathCurrent = pathCurrent + nextDirectory + "\\";  
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        static Tree ConsoleRegen (string pathCurrent, string LastOperationInfo)
        {
            Tree treeForVisualisation = new Tree(pathCurrent);
            #region Линии оформления
            string horisontLineSingle = "-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------";
            string horisontLineDouble = "===================================================================================================================================================================================";
            #endregion
            Console.Clear();
            // Вывести первой строчкой полный путь текущей папки
            Console.WriteLine($"текущий каталог: {pathCurrent}");
            // Вывести горизонтальную черту
            Console.WriteLine(horisontLineSingle);
            // вывести дерево каталогов
            foreach (string element in treeForVisualisation.ListLines)
            {
                Console.WriteLine(element);
            }
            Console.WriteLine(horisontLineDouble);// двойной разделитель
            Console.WriteLine();
            //выводится информация:
            Console.WriteLine($"Имя директории: {treeForVisualisation.NameCurentDirectory} \t Количество папок: {treeForVisualisation.DirectorysArray.Length} \t Количество файлов: {treeForVisualisation.FilesArray.Length} \t {Environment.NewLine}Директория создана: {treeForVisualisation.CreationTime} \t");
            Console.WriteLine();
            Console.WriteLine(horisontLineDouble);
            Console.WriteLine($"Предыдущая операция: {LastOperationInfo}"); 
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











    }
}
