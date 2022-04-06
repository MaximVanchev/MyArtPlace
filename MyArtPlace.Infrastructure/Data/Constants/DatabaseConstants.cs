using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyArtPlace.Infrastructure.Data.Constants
{
    public static class DatabaseConstants
    {
        public const int Guid_Max_Length = 100;
        public const int Currency_Iso_Length = 3;
        public const int Shop_Name_Max_Length = 50;
        public const int Product_Name_Max_Length = 11;
        public const int Category_Name_Max_Length = 20;
        public const int Name_Min_Length = 5;
        public const int Description_Max_Length = 200;
        public const int Location_Max_Length = 100;
        public const int ProfilePicture_Max_Length = 500;
        public const int Username_Max_Length = 50;
        public const int Products_Count_Max_Length = 10;
        public const int Products_Count_Min_Length = 1;
        public const double Price_Min_Range = 1.00;
        public const double Price_Max_Pange = 100000.00;
    }
}
