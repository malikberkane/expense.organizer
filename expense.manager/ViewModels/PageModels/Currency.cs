using System.Collections.Generic;

namespace expense.manager.ViewModels.PageModels
{
    public class Currency
    {
        public string symbol { get; set; }
        public string name { get; set; }
        public string cc { get; set; }




        public override string ToString()
        {
            return $"{name} {symbol}";
        }


    }


    public class Rootobject
    {
        public List<Currency> currencies { get; set; }
    }
}