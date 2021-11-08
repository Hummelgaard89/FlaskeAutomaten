using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace FlaskeAutomatenTake
{
    class Beverage
    {
        #region Beverage Constructor
        public string BeverageType { get; set; }
        public int BeverageID { get; set; }
        static Beverage[] producerbuf = new Beverage[5];
        static Beverage[] beerbuf = new Beverage[15];
        static Beverage[] sodabuf = new Beverage[15];
        public Beverage(string beveragetype, int beverageID)
        {
            BeverageType = beveragetype;

            BeverageID = beverageID;
        }
        #endregion

        #region Shared var's
        Random random = new Random();
        int beverageidcounter = 0;
        int b = 0;
        int s = 0;
        string[] beverageTypesArr = { "Beer", "Soda" };
        #endregion
        //The method that simulates the beverages turned in after use.
        public void CreateBeverages()
        {
            string tempbeveragetype = "";
            int tempbeverageid = 0;
            //As long as the producer array is not full, it will create new beverages.
            while (CheckIfFull(producerbuf) == false)
            {
                try
                {
                    //If the producer array is not full, it will try the enter, to make sure it's not occupied, if not it locks the array.
                    Monitor.Enter(producerbuf);
                    lock (producerbuf)
                    {
                        //Then it goes trough the array, find the empty indexes and creates random beverage (beers or sodas) and then adds the ID.
                        for (int i = 0; i < producerbuf.Length; i++)
                        {
                            if (producerbuf[i] == null)
                            {
                                tempbeveragetype = beverageTypesArr[random.Next(beverageTypesArr.Length)];
                                tempbeverageid = beverageidcounter;
                                producerbuf[i] = new Beverage(tempbeveragetype, tempbeverageid);
                                beverageidcounter++;
                            }
                            else if (producerbuf[i] != null)
                            {
                                i++;
                            }
                        }
                        #region Show what is produced
                        //This region is just to show what is being produced.
                        for (int i = 0; i < producerbuf.Length; i++)
                        {
                            if (producerbuf[i] != null)
                            {
                                Console.WriteLine(producerbuf[i].BeverageType + " with ID: \"" + producerbuf[i].BeverageID + "\" Has been made.");
                            }
                            else
                            {
                                break;
                            }
                        }
                        Console.WriteLine("\n\n");
                        #endregion
                        Monitor.PulseAll(producerbuf);
                    }
                }
                //When the prodicer array is filled again, it exits the producer array and unlocks it and then the thread sleeps for 1 sek.
                finally
                {
                    Monitor.Exit(producerbuf);
                    Thread.Sleep(2000);
                }
                Console.WriteLine("\n\n");
            }
        }
        //This method sort the beverages in two containers (arrays) once the producerbuf is full.
        //Then in enters and locks the producerbuf, beerbuf and sodabuf one at a time, starting with the producerbuf.
        //When all three are locked, the sorting begins.
        public void SortBeverages()
        {
            do
            {
                //First 
                if (CheckIfFull(producerbuf) == true)
                {
                    try
                    {
                        Monitor.Enter(producerbuf);
                        lock (producerbuf)
                        {
                            try
                            {
                                Monitor.Enter(beerbuf);
                                lock (beerbuf)
                                {
                                    try
                                    {
                                        Monitor.Enter(sodabuf);
                                        lock (sodabuf)
                                        {
                                            //Here it goes trough the producerbuf and determines if it's a beer og a soda.
                                            //Then moves it to that, by creating a "new beverage" with the same info and deleting it from the producerbuf.
                                            for (int i = 0; i < producerbuf.Length; i++)
                                            {
                                                
                                                if (producerbuf[i].BeverageType == "Beer")
                                                {
                                                    while (b < beerbuf.Length && beerbuf[b] != null) b++;
                                                    {
                                                        if (b < beerbuf.Length && CheckIfFull(beerbuf) == false)
                                                        {
                                                            beerbuf[b] = new Beverage(producerbuf[i].BeverageType, producerbuf[i].BeverageID);
                                                            producerbuf[i] = null;
                                                            b++;
                                                        }
                                                        else
                                                        {
                                                            Thread.Sleep(2000);
                                                        }
                                                    }
                                                }
                                                else if (producerbuf[i].BeverageType == "Soda")
                                                {
                                                    while (s < sodabuf.Length && sodabuf[s] != null) s++;
                                                    {
                                                        if (s < sodabuf.Length && CheckIfFull(sodabuf) == false)
                                                        {
                                                            sodabuf[s] = new Beverage(producerbuf[i].BeverageType, producerbuf[i].BeverageID);
                                                            producerbuf[i] = null;
                                                            s++;
                                                        }
                                                        else
                                                        {
                                                            Thread.Sleep(2000);
                                                        }
                                                    }
                                                }
                                            }
                                            #region Show beerbuf and sodabuf
                                            //This region is just to show what is in the beerbuf and sodabuf.
                                            Console.WriteLine("\nBeers in:");
                                            for (int i = 0; i < beerbuf.Length; i++)
                                            {
                                                if (beerbuf[i] != null)
                                                {
                                                    Console.WriteLine(beerbuf[i].BeverageType + beerbuf[i].BeverageID);
                                                }
                                            }

                                            Console.WriteLine("\nSodas in:");
                                            for (int i = 0; i < sodabuf.Length; i++)
                                            {
                                                if (sodabuf[i] != null)
                                                {
                                                    Console.WriteLine(sodabuf[i].BeverageType + sodabuf[i].BeverageID);
                                                }
                                            }
                                            #endregion
                                            Monitor.PulseAll(sodabuf);
                                            Monitor.PulseAll(beerbuf);
                                            Monitor.PulseAll(producerbuf);
                                        }//End lock sodabuf
                                    }
                                    finally //Sodabuf
                                    {
                                        Monitor.Exit(sodabuf);
                                    }
                                }//End lock beerbuf
                            }
                            finally //Beerbuf
                            {
                                Monitor.Exit(beerbuf);
                            }
                        }//End lock producerbuf
                    }
                    finally //Producerbuf
                    {
                        Monitor.Exit(producerbuf);
                        Thread.Sleep(2000);
                    }
                    //When it's done sorting, it exits and unlucks the arrays and the thread sleeps for 1 sek.
                }
            } while (true);

        }
        //This method "disposes" of the beers.
        public void GetBeers()
        {
            do
            {
                //First it checks if the beerbuf is full, when it is, it types out what beers have been picked up and then deletes them from the array.
                if (CheckIfFull(beerbuf) == true)
                {
                    try
                    {
                        Monitor.Enter(beerbuf);
                        lock (beerbuf)
                        {
                            Console.WriteLine("\nThese beers have just been picked up:");
                            for (int i = 0; i < beerbuf.Length; i++)
                            {
                                Console.WriteLine(beerbuf[i].BeverageType + beerbuf[i].BeverageID);
                                beerbuf[i] = null;
                            }
                            b = 0;
                            Monitor.PulseAll(beerbuf);
                        }
                    }
                    finally
                    {
                        Monitor.Exit(beerbuf);
                        Thread.Sleep(5000);
                    }
                }
            } while (true);
        }
        //This method "disposes" of the sodas.
        public void GetSodas()
        {
            do
            {
                //First it checks if the sodabuf is full, when it is, it types out what sodas have been picked up and then deletes them from the array.
                if (CheckIfFull(sodabuf) == true)
                {
                    try
                    {
                        Monitor.Enter(sodabuf);
                        lock (sodabuf)
                        {
                            Console.WriteLine("\nThese sodas have just been picked up:");
                            for (int i = 0; i < sodabuf.Length; i++)
                            {
                                Console.WriteLine(sodabuf[i].BeverageType + sodabuf[i].BeverageID);
                                sodabuf[i] = null;
                            }
                            s = 0;
                            Monitor.PulseAll(sodabuf);
                        }
                    }
                    finally
                    {
                        Monitor.Exit(sodabuf);
                        Thread.Sleep(5000);
                    }
                }
            } while (true);
        }
        //This methods checks if an array is full and then returns either true or false.
        public bool CheckIfFull(Beverage[] arrayToCheck)
        {
            bool b = false;

            for (int i = 0; i < arrayToCheck.Length; i++)
            {
                if (arrayToCheck[i] == null)
                {
                    b = false;
                    break;
                }
                else if (arrayToCheck != null)
                {
                    b = true;
                }
            }
            return b;
        }
    }
}
