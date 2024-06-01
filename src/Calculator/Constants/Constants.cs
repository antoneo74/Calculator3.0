namespace Calculator3.Constants
{
    public static class Constants
    {
        public const int MAXRATE = 999;

        public const int MAXYEARS = 50;

        public const int MAXMONTHS = 600;

        public const int MAXDAYS = 18250;

        public const double MAXAMOUNT = 1e+9;

        public const double RATECBR = 16;

        public const double RATENDFL = 13;

        public enum TimeFrequency
        {
            Years = 0,
            Months = 1,
            Days = 2,
        }

        public enum PaymentFrequency
        {
            End = 0,
            Daily = 1,
            Weekly = 2,
            Monthly = 3,
            Quarterly = 4,
            HalfYearly = 5,
            Yearly = 6,
        }

        public static TimeFrequency TimeUnitParam { get; set; }
    }
}
