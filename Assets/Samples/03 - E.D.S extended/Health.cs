using System;
using Flux;

namespace Example03
{
    [Address] // An enum must be marked with the AddressAttribute to be selectable with a DynamicFlag in the Inspector
    public enum Health : byte // Addressable enums must be mapped onto byte to be correctly registered
    {
        Poor, // Character is not in condition to run
        Good // Character can run
    }
}