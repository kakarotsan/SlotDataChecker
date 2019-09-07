using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SlotDataChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            // Arguments            
            DateTime startDate = new DateTime(2019, 2, 1);
            DateTime endDate = new DateTime(2019, 2, 6);

            // Read and sort data
            List<SlotModel> slotRecords = ReadDataFromCSV("SlotsDataSmall.csv");
            var sortedSlotRecords = slotRecords.OrderBy(x => x.SlotName).ThenBy(x => x.Date).ToList();

            List<SlotModel> missingData = new List<SlotModel>();
            List<SlotModel> duplicateData = new List<SlotModel>();

            FindDuplicateAndMissingData(startDate, endDate, sortedSlotRecords, missingData, duplicateData);
            OutputErrorData(sortedSlotRecords, missingData, duplicateData);
        }

        private static void FindDuplicateAndMissingData(
            DateTime startDate, 
            DateTime endDate,
            List<SlotModel> sortedSlotRecords,
            List<SlotModel> missingData,
            List<SlotModel> duplicateData)
        {
            int i = 0;
            string currentMachine = "";

            while (i < sortedSlotRecords.Count - 1)
            {
                if (currentMachine == sortedSlotRecords.Last().SlotName)
                {
                    break;
                }

                if (currentMachine == "")
                {
                    currentMachine = sortedSlotRecords[0].SlotName;
                } else
                {
                    //Get the next slot machine name to iterate over
                    while(i < sortedSlotRecords.Count - 1 && currentMachine == sortedSlotRecords[i].SlotName)
                    {                       
                        i++;
                    }
                    currentMachine = sortedSlotRecords[i].SlotName;                    
                }
                
                DateTime currentDateTime = startDate;

                while (currentDateTime < endDate)
                {
                    // Check for duplicate data
                    if (i != 0 && 
                        i < sortedSlotRecords.Count && 
                        sortedSlotRecords[i].Date == sortedSlotRecords[i - 1].Date)
                    {
                        duplicateData.Add(new SlotModel()
                        {
                            SlotName = currentMachine,
                            Date = currentDateTime
                        });
                        i++;
                        continue;
                    }

                    // Check for missing data
                    if (i < sortedSlotRecords.Count && 
                        (sortedSlotRecords[i].Date != currentDateTime || 
                        currentMachine != sortedSlotRecords[i].SlotName))
                    {
                        missingData.Add(new SlotModel()
                        {
                            SlotName = currentMachine,
                            Date = currentDateTime
                        });
                        currentDateTime = currentDateTime.AddHours(1);
                        continue;
                    }

                    currentDateTime = currentDateTime.AddHours(1);
                    if (i != sortedSlotRecords.Count - 1)
                    {
                        i++;
                    }
                }
            }
        }

        private static void OutputErrorData(List<SlotModel> sortedSlotRecords, List<SlotModel> missingData, List<SlotModel> duplicateData)
        {
            Console.WriteLine("Missing Data:");
            foreach (var r in missingData)
            {
                Console.WriteLine($"{r.SlotName} {r.Date}");
            }
            Console.WriteLine();

            Console.WriteLine("Duplicate Data:");
            foreach (var r in duplicateData)
            {
                Console.WriteLine($"{r.SlotName} {r.Date}");
            }
            Console.WriteLine();

            //Console.WriteLine("All Data:");
            //foreach (var r in sortedSlotRecords)
            //{
            //    Console.WriteLine($"{r.SlotName} {r.Date}");
            //}

            Console.ReadKey();
        }

        private static List<SlotModel> ReadDataFromCSV(string filepath)
        {
            List<SlotModel> slotRecords = new List<SlotModel>();

            using (var reader = new StreamReader(filepath))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.RegisterClassMap<SlotMap>();
                var records = csv.GetRecords<SlotModel>();
                foreach (var r in records)
                {
                    slotRecords.Add(r);
                }
            }

            return slotRecords;
        }

    }
}
