using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Project.Utilities
{
    public static class Password
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length">total size of password</param>
        /// <param name="lowercase">minimum amount of lowercase characters</param>
        /// <param name="uppercase">minimum amount of uppercase characters</param>
        /// <param name="numerics">minimum amount of numeric characters</param>
        /// <param name="special">minimum amount of special characters</param>
        /// <returns></returns>
        public static string GeneratePassword(int length, int lowercase, int uppercase, int numerics, int special)
        {
            string lowers = "abcdefghijkmnopqrstuvwxyz";
            string uppers = "ABCDEFGHJKLMNPRSTUVWXYZ";
            string number = "23456789";
            string specials = "!#%+:=?@_";
            string all = lowers + uppers + number;
            var rest = length - (lowercase + uppercase + numerics + special);
            rest = (rest < 0) ? 0 : rest;

            Random random = new Random();

            string generated = all[random.Next(all.Length - 1)].ToString();

            for (int i = 1; i <= lowercase; i++) generated = generated.Insert(random.Next(generated.Length),lowers[random.Next(lowers.Length - 1)].ToString());

            for (int i = 1; i <= uppercase; i++) generated = generated.Insert(random.Next(generated.Length),uppers[random.Next(uppers.Length - 1)].ToString());

            for (int i = 1; i <= numerics; i++) generated = generated.Insert(random.Next(generated.Length), number[random.Next(number.Length - 1)].ToString());

            for (int i = 1; i <= special; i++) generated = generated.Insert(random.Next(generated.Length), specials[random.Next(specials.Length - 1)].ToString());

            for (int i = 1; i <= rest; i++) generated = generated.Insert(random.Next(generated.Length), all[random.Next(all.Length - 1)].ToString());

            return generated;
        }
    }
}
