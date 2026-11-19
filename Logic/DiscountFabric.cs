using bookStore.Entities;

namespace bookStore.Logic
{
    internal class DiscountFabric(DiscountBuilder discountBuilder, FileCouponStore couponStore)
    {
        private readonly DiscountBuilder DiscountBuilder = discountBuilder;
        private readonly FileCouponStore CouponStore = couponStore;

        public bool TryAddCoupon(
            string name,
            out Coupon? replacedCoupon)
        {
            replacedCoupon = null;

            if (string.IsNullOrWhiteSpace(name))
                return false;

            Coupon? coupon = CouponStore.FindCoupon(name.Trim());

            if (coupon == null)
                return false;

            if (CouponStore.amountCoupons.Contains(coupon) || CouponStore.percentCoupons.Contains(coupon))
                return false;

            return DiscountBuilder.AddCoupon(
                coupon,
                out replacedCoupon);
        }

        public List<string> GetCouponNames()
        {
            return DiscountBuilder.GetUsedCouponNames();
        }
    }
}