using System.Diagnostics.Eventing.Reader;

namespace PartnerTransactionAPI.Services
{
    public class DiscountService
    {
        public static (long totalDiscount, long finalAmount) CalculateDiscount(long totalAmount)
        {
            double baseDiscount = 0;

            // Base Discount ranges (continuous)
            if (totalAmount >= 20000 && totalAmount <= 50000) baseDiscount = 0.05;
            else if (totalAmount >= 50001 && totalAmount <= 80000) baseDiscount = 0.07;
            else if (totalAmount >= 80001 && totalAmount <= 120000) baseDiscount = 0.10;
            else if (totalAmount > 120000) baseDiscount = 0.15;

            double conditional = 0;

            // Conditional discounts
            decimal ringgitVal = ConvertToRinggit(totalAmount);
            if (totalAmount > 50000 && IsPrime(totalAmount)) conditional += 0.08;
            if (totalAmount > 90000 && IsLastDigitFive(ringgitVal)) conditional += 0.10;

            
            // Cap at 20%
            double totalDiscountRate = Math.Min(baseDiscount + conditional, 0.20);

            long discountAmount = (long)(totalAmount * totalDiscountRate);
            long finalAmount = totalAmount - discountAmount;

            return (discountAmount, finalAmount);
        }

        private static bool IsPrime(long amount)
        {
            if (amount <= 1) return false;
            if (amount == 2) return true;
            if (amount % 2 == 0) return false;

            for (long i = 3; i <= Math.Sqrt(amount); i += 2)
            {
                if (amount % i == 0) return false;
            }

            return true;
        }
        private static decimal ConvertToRinggit(long amountSen)
        {
            return amountSen / 100.0m;
        }

        private static bool IsLastDigitFive(decimal amountInRM)
        {
            int wholeRinggit = (int)Math.Floor(amountInRM);
            return wholeRinggit % 10 == 5;
        }
    }
}
