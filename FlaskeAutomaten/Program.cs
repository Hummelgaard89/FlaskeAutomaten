using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace FlaskeAutomatenTake
{
    class Program
    {
        static void Main(string[] args)
        {
            Beverage beverage = new Beverage("tempname", 0);

            //The thread that simulates turning in there used beers and sodas.
            Thread producebeverage = new Thread(beverage.CreateBeverages);
            producebeverage.Start();

            //The thread that sorts the beverages turned in.
            Thread sortbeverages = new Thread(beverage.SortBeverages);
            sortbeverages.Start();

            //The thread that disposes of the beers once the container(array) is full.
            Thread getbeers = new Thread(beverage.GetBeers);
            getbeers.Start();

            //The thread that disposes of the sodas once the container(array) is full.
            Thread getsodas = new Thread(beverage.GetSodas);
            getsodas.Start();
        }
    }
}
