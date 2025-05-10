namespace AttendanceRecord.Models
{
    public class AttendanceRecord
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? StartRestTime { get; set; }
        public DateTime? EndRestTime { get; set; }
        public bool IsBusinessTrip { get; set; }
        public bool IsTripAll { get; set; }
    }
}
