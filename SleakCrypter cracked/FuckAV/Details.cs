using KeyAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuckAV
{
    class Details
    {
        public static api Auth = new api(
        name: "SleakCrypter",
        ownerid: "RCf16IYmvd",
        secret: Encoding.ASCII.GetString(Convert.FromBase64String("MjdmMzQ3MDcxNGFiOGFiOTc5YjZiM2YxM2Y5YmZlMTk4ZDA5MTcyZjZjMjM5MTUwNTZlMzM3YmRmNzQwOTM1YQ==")),
        version: "1.0"
        );
    }
}
