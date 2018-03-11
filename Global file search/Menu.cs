using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Global_file_search
{
    class ConsoleMenu
    {
        List<MenuItem> items = new List<MenuItem>();

        public void Execute()
        {
            int choice;
            PrintItems();
            do
            {
                choice = GetInt();
                if (choice <= 0 || choice > items.Count)
                {
                    Console.WriteLine("Invalid number =)");
                }
            } while (choice <= 0 || choice > items.Count);
            items[choice - 1].ItemAction.Invoke(); 
        }

        public ConsoleMenu AddItem(string actionName, Action action)
        {
            items.Add(new MenuItem(actionName, action));
            return this;
        }

        public void RemoveItem(string key)
        {
            items.Remove(items.Find((MenuItem mi) => { return mi.ItemName.ToUpper().Contains(key.ToUpper()); }));
        }

        private void PrintItems()
        {
            for (int i = 0; i < items.Count; i++)
            {
                Console.WriteLine("{0}. {1}", i+1, items[i].ItemName);
            }
        }

        private static int GetInt()
        {
            string str = Console.ReadLine();
            int res;
            try
            {
                res = int.Parse(str);
                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid value!");
                return GetInt();
            }
        }
        private struct MenuItem
        {
            public string ItemName { get; set; }
            public Action ItemAction { get; set; }

            public MenuItem(string name, Action action)
            {
                ItemName = name;
                ItemAction = action;
            }
        }


    }
}
