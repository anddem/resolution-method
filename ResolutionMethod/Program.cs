using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResolutionMethod
{
    class Program
    {
        //Структура
        //номер_дизъюнкта : дизъюнкт : из_каких_дизъюнктов_получен 
        static Dictionary<int, KeyValuePair<string, string>> historyOfUnion = new Dictionary<int, KeyValuePair<string, string>>();

        static bool HasDenial(string key, out int dictKey) //Проверка на существование противоположного дизъюнкта
        {
            dictKey = 1;
            bool hasDenial = false;

            foreach (var disunkt in historyOfUnion)
                if (disunkt.Value.Key == NegElem($"({key})") || $"({key})" == NegElem(disunkt.Value.Key))
                {
                    hasDenial = true;
                    dictKey = disunkt.Key;
                    break;
                }
                else if (disunkt.Value.Key == DenialOfDisunkt(key))
                {
                    hasDenial = true;
                    dictKey = disunkt.Key;
                }

            return hasDenial;
        }

        static bool CanBeUnited(string key, out int dictKey)
        {
            dictKey = 1;
            bool success = false;
            var keyhs = SplitDisunkt(DenialOfDisunkt(key));

            foreach (var disunkt in historyOfUnion)
            {
                var hs1 = SplitDisunkt(disunkt.Value.Key);

                var hs2  =  new SortedSet<string>(hs1.Intersect(keyhs));
                if (hs2.Count != 0)
                {
                    success = true;
                    dictKey = disunkt.Key;
                    break;
                }
            }

            return success;
        }

        static void Unite(int dictKeyLeft, int dictKeyRight)
        {
            var left = SplitDisunkt(DenialOfDisunkt(historyOfUnion[dictKeyLeft].Key));
            var right = SplitDisunkt(historyOfUnion[dictKeyRight].Key);

            right.SymmetricExceptWith(left);
            if (right.Count != 0)
            {
                SortedSet<string> dis = new SortedSet<string>();

                foreach (string val in right) if (left.Contains(val)) dis.Add(NegElem(val));
                    else dis.Add(val);

                string disunkt = GenerateDisunkt(dis);

                historyOfUnion.Add(historyOfUnion.Count + 1, new KeyValuePair<string, string>(disunkt, $"{dictKeyLeft}-{dictKeyRight}"));
            }
        }

        static string GenerateDisunkt(SortedSet<string> set)
        {
            string disunkt = set.ElementAt(0);
            if (set.Count > 1)
                for (int i = 1; i < set.Count; i++) disunkt += $"v{set.ElementAt(i)}";

            return disunkt;
        }

        static SortedSet<string> SplitDisunkt(string disunkt) => new SortedSet<string>(disunkt.Split(new char[] { 'v' }));

        static void FindEmptyDisunkt()
        {
            for (int i = 1; i <= historyOfUnion.Count; i++)
            {
                var row = historyOfUnion[i];

                if (row.Key == "[]") break;



                if (HasDenial(row.Key, out int key))
                {
                    historyOfUnion.Add(historyOfUnion.Count+1, new KeyValuePair<string, string>("[]", $"{i}-{key}"));
                    break;
                }

                if (CanBeUnited(row.Key, out key))
                    Unite(i, key);
            }
        }

        static void Main(string[] args)
        {
            InitDict();
            FindEmptyDisunkt();
            PrintResult();
        }

        static string[] InputDisunkts() => Console.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        static void InitDict() //Первичная инициализация словаря
        {
            Console.Write("Дизъюнкты: ");
            string[] input = InputDisunkts(); //Ввод дизъюнктов

            for (int i = 1; i <= input.Length; i++) //Добавление в словарь
            {
                historyOfUnion.Add(i, new KeyValuePair<string, string>(input[i-1], i.ToString()));
            }
        }

        static void PrintResult() //Печать результирующего массива
        {
            foreach (var note in historyOfUnion)
            {
                Console.Write($"{note.Key,2}) ");
                    Console.WriteLine($"{note.Value.Value,3}: {note.Value.Key}");
            }
        }

        static string NegElem(string elem) => elem[0] == '!' ? elem.Substring(1) : $"!{elem}";

        static string DenialOfDisunkt(string disunkt) //Отрицание элементов дизъюнкта
        {
            string[] buffer = disunkt.Split(new char[] { 'v' }, StringSplitOptions.RemoveEmptyEntries);

            string denial = NegElem(buffer[0]);

            for (int i = 1; i < buffer.Length; i++)
                denial += $"v{NegElem(buffer[i])}";

            return denial;
        }
    }
}
