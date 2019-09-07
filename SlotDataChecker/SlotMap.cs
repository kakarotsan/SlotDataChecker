using CsvHelper.Configuration;

namespace SlotDataChecker
{
    public class SlotMap : ClassMap<SlotModel>
    {
        public SlotMap()
        {
            Map(m => m.Pulls).Name("Pulls");
            Map(m => m.SlotName).Name("Machine");
            Map(m => m.Date).Name("DateTime");
        }
    }
}
