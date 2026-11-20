
using bookStore.Entities;

namespace bookStore.Logic
{
    internal class DiscountFabric(
        DiscountBuilder discountBuilder,
        FileCouponStore couponStore)
    {
        private readonly DiscountBuilder DiscountBuilder = discountBuilder;
        private readonly FileCouponStore CouponStore = couponStore;

        public bool TryAddCoupon(
            string name,
            out Coupon? replacedCoupon,
            out string message)
        {
            replacedCoupon = null;

            if (string.IsNullOrWhiteSpace(name))
            {
                message = "Название купона не может быть пустым.";
                return false;
            }

            Coupon? coupon = CouponStore.FindCoupon(name.Trim());

            if (coupon == null)
            {
                message = "Купон не найден.";
                return false;
            }

            if (!DiscountBuilder.AddCoupon(coupon, out replacedCoupon))
            {
                message = "Этот купон уже добавлен или выбран.";
                return false;
            }

            message = replacedCoupon == null ?
                $"Купон {coupon.Name} успешно добавлен."
                : $"Купон {replacedCoupon.Name} заменён купоном {coupon.Name}.";

            return true;
        }

        public List<string> GetCouponNames()
        {
            return DiscountBuilder.GetUsedCouponNames();
        }
    }
}
