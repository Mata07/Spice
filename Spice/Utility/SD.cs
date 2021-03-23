using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Utility
{
    // static details - for constants
    public static class SD
    {
        public const string DefaultFoodImage = "default_food.png";

        //roles - created here to not use magic strings
        public const string ManagerUser = "Manager";
        public const string KitchenUser = "Kitchen";
        public const string FrontDeskUser = "FrontDesk";
        public const string CustomerEndUser = "Customer";
    }
}
