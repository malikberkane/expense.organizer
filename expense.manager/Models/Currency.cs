namespace expense.manager.Models
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
}