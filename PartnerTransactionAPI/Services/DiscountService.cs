using System.Diagnostics.Eventing.Reader;

namespace PartnerTransactionAPI.Services
{
    public class DiscountService
    {
        public static (long totaLDiscount, long finalAmount) CalculateDiscount(long totalAmount)
        {
            double baseDiscount = 0;

            if (totalAmount >= 20000 && totalAmount <= 50000) baseDiscount = 0.05;
            else if (totalAmount >= 50100 && totalAmount <= 80000) baseDiscount = 0.07;
            else if (totalAmount >= 80100 && totalAmount <= 120000) baseDiscount = 0.10;
            else if (totalAmount > 120000) baseDiscount = 0.15;

            double conditional = 0;

            if (totalAmount > 50000 && IsPrime(totalAmount)) conditional += 0.08;
            if (totalAmount > 90000 && totalAmount % 10 == 5) conditional += 0.10;

            double totalDiscountRate = Math.Min(baseDiscount + conditional, 0.20);
            long discountAmount = (long)(totalAmount * totalDiscountRate);
            long finalAmount = totalAmount - discountAmount;

            return (discountAmount, finalAmount);
        }

        private static bool IsPrime(double amount) {
            if (amount <= 1) return false;
            if (amount == 2) return true;
            for(long i = 2; i <= Math.Sqrt(amount); i++)
            {
                if (amount % i == 0) return false;
            }
            return true;
        }
    }
}
